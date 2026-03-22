/**
 * populate-grades.js
 *
 * Populates realistic test grade data for all course sections at
 * Lewetik Elementary School (ID 163) in the fedsis tenant.
 *
 * Usage:
 *   node scripts/populate-grades.js
 *   DRY_RUN=1 node scripts/populate-grades.js
 *
 * Requires: scripts/.token file with a valid JWT.
 */

const fs = require('fs');
const path = require('path');
const https = require('https');
const http = require('http');

// ---------------------------------------------------------------------------
// Configuration
// ---------------------------------------------------------------------------

const DRY_RUN = process.env.DRY_RUN === '1';
const TENANT_ID = '1e93c7bf-0fae-42bb-9e09-a1cedc8c0355';
const SCHOOL_ID = 163;
const ACADEMIC_YEAR = 2025;
const GRADE_SCALE_ID = 2;

// Quarters: Q1=5, Q2=6, Q3=7, Q4=8
const QUARTERS = [5, 6, 7, 8];
const QUARTER_NAMES = { 5: 'Q1', 6: 'Q2', 7: 'Q3', 8: 'Q4' };

// PDOE Grade Scale — ordered from highest to lowest breakoff
const GRADE_SCALE = [
  { grade: 'A+', breakoff: 97, gradeId: 15, gp: 4 },
  { grade: 'A',  breakoff: 94, gradeId: 16, gp: 4 },
  { grade: 'A-', breakoff: 90, gradeId: 17, gp: 4 },
  { grade: 'B+', breakoff: 87, gradeId: 18, gp: 3 },
  { grade: 'B',  breakoff: 84, gradeId: 19, gp: 3 },
  { grade: 'B-', breakoff: 80, gradeId: 20, gp: 3 },
  { grade: 'C+', breakoff: 77, gradeId: 21, gp: 2 },
  { grade: 'C',  breakoff: 74, gradeId: 22, gp: 2 },
  { grade: 'C-', breakoff: 70, gradeId: 23, gp: 2 },
  { grade: 'D+', breakoff: 67, gradeId: 24, gp: 1 },
  { grade: 'D',  breakoff: 64, gradeId: 25, gp: 1 },
  { grade: 'D-', breakoff: 60, gradeId: 26, gp: 1 },
  { grade: 'F',  breakoff: 50, gradeId: 27, gp: 0 },
  { grade: 'Inc', breakoff: 0, gradeId: 28, gp: 0 },
];

// ---------------------------------------------------------------------------
// Token & auth helpers
// ---------------------------------------------------------------------------

const tokenPath = path.join(__dirname, '.token');
if (!fs.existsSync(tokenPath)) {
  console.error('ERROR: Token file not found at', tokenPath);
  process.exit(1);
}
const TOKEN = fs.readFileSync(tokenPath, 'utf8').trim();

function parseJwt(token) {
  const payload = token.split('.')[1];
  const json = Buffer.from(payload, 'base64').toString('utf8');
  return JSON.parse(json);
}

const jwtPayload = parseJwt(TOKEN);
const uniqueName = jwtPayload.unique_name || '';
// unique_name format: "fedsisDisplayName|..." — strip tenant prefix
const displayName = uniqueName.split('|')[0].replace(/^fedsis/, '');
const emailAddress = jwtPayload.email || jwtPayload.emailaddress || uniqueName.split('|')[1] || 'ghachey@purltek.com';

console.log(`Parsed JWT — display name: "${displayName}", email: "${emailAddress}"`);

// Common fields for every API request
const commonFields = {
  _tenantName: 'fedsis',
  _userName: displayName,
  _token: TOKEN,
  _academicYear: ACADEMIC_YEAR,
};

// ---------------------------------------------------------------------------
// HTTP helper
// ---------------------------------------------------------------------------

const API_BASE = 'https://localhost:5001';

function apiPost(path, body) {
  return new Promise((resolve, reject) => {
    const url = new URL(`${API_BASE}/fedsis/${path}`);
    const data = JSON.stringify(body);
    const req = https.request({
      hostname: url.hostname,
      port: url.port,
      path: url.pathname,
      method: 'POST',
      headers: { 'Content-Type': 'application/json', 'Content-Length': Buffer.byteLength(data) },
      rejectUnauthorized: false
    }, (res) => {
      let body = '';
      res.on('data', chunk => body += chunk);
      res.on('end', () => {
        try { resolve(JSON.parse(body)); }
        catch { reject(new Error(`JSON parse error on ${path}: ${body.substring(0, 200)}`)); }
      });
    });
    req.on('error', reject);
    req.write(data);
    req.end();
  });
}

function sleep(ms) {
  return new Promise((r) => setTimeout(r, ms));
}

// ---------------------------------------------------------------------------
// Grade generation
// ---------------------------------------------------------------------------

/**
 * Simple seeded PRNG (mulberry32). Produces values in [0, 1).
 */
function mulberry32(seed) {
  let s = seed | 0;
  return function () {
    s = (s + 0x6D2B79F5) | 0;
    let t = Math.imul(s ^ (s >>> 15), 1 | s);
    t = (t + Math.imul(t ^ (t >>> 7), 61 | t)) ^ t;
    return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
  };
}

/**
 * Hash an integer to get a stable seed.
 */
function hashInt(n) {
  let h = n | 0;
  h = ((h >> 16) ^ h) * 0x45d9f3b;
  h = ((h >> 16) ^ h) * 0x45d9f3b;
  h = (h >> 16) ^ h;
  return h;
}

/**
 * Given a percent (50-100), return the matching grade entry.
 */
function gradeFromPercent(pct) {
  for (const g of GRADE_SCALE) {
    if (pct >= g.breakoff) return g;
  }
  // Below 50 — Inc (shouldn't happen with our generation, but just in case)
  return GRADE_SCALE[GRADE_SCALE.length - 1];
}

/**
 * Generate a percent mark for a student in a specific context.
 *
 * @param {number} studentId
 * @param {number} courseId
 * @param {number} quarterId
 * @param {boolean} isExam
 * @returns {number} percent mark (50–100, with one decimal)
 */
function generatePercent(studentId, courseId, quarterId, isExam) {
  // Student ability: consistent baseline per student
  const abilityRng = mulberry32(hashInt(studentId));
  const ability = abilityRng(); // 0..1

  // Map ability to a target mean:
  //   ability 0.0 → mean ~55 (weak student)
  //   ability 1.0 → mean ~95 (strong student)
  // This gives roughly: 10% F, 15% D, 30% C, 30% B, 15% A
  const mean = 55 + ability * 40; // 55..95

  // Subject variance (deterministic per student+course)
  const subjectRng = mulberry32(hashInt(studentId * 9973 + courseId * 7919));
  const subjectShift = (subjectRng() - 0.5) * 12; // -6..+6

  // Quarter variance
  const qtrRng = mulberry32(hashInt(studentId * 6271 + courseId * 3847 + quarterId * 1031));
  const qtrShift = (qtrRng() - 0.5) * 6; // -3..+3

  // Exam slight offset
  const examShift = isExam ? ((qtrRng() - 0.5) * 4) : 0;

  let pct = mean + subjectShift + qtrShift + examShift;
  // Clamp to 50–100 and round to 1 decimal
  pct = Math.max(50, Math.min(100, pct));
  pct = Math.round(pct * 10) / 10;
  return pct;
}

// ---------------------------------------------------------------------------
// API calls
// ---------------------------------------------------------------------------

async function getAllCourses() {
  console.log('Fetching all courses...');
  const res = await apiPost('CourseManager/getAllCourseList', {
    ...commonFields,
    tenantId: TENANT_ID,
    schoolId: SCHOOL_ID,
    academicYear: ACADEMIC_YEAR,
  });
  if (res._failure) {
    console.error(`  API error: ${res._message}`);
    console.error('  Token may have expired — get a fresh one from session storage');
    process.exit(1);
  }
  const list = res.courseViewModelList || [];
  console.log(`  Found ${list.length} courses`);
  if (list.length === 0) {
    console.log('  Response keys:', Object.keys(res).join(', '));
    console.log('  courseCount:', res.courseCount);
  }
  return list;
}

async function getCourseSections(courseId) {
  const res = await apiPost('CourseManager/getAllCourseSection', {
    ...commonFields,
    tenantId: TENANT_ID,
    schoolId: SCHOOL_ID,
    academicYear: ACADEMIC_YEAR,
    courseId,
  });
  return res.getCourseSectionForView || [];
}

async function getStudentsForSection(courseSectionId, courseId, calendarId) {
  const res = await apiPost('StudentSchedule/getStudentListByCourseSection', {
    ...commonFields,
    tenantId: TENANT_ID,
    schoolId: SCHOOL_ID,
    courseSectionIds: [courseSectionId],
    pageNumber: 1,
    pageSize: 200,
    _pageSize: 200,
  });

  if (res._failure) return [];
  return res.scheduleStudentForView || [];
}

async function checkExistingGrades(courseSectionId, courseId, calendarId, markingPeriodId, isExamGrade) {
  const res = await apiPost('InputFinalGrade/getAllStudentFinalGradeList', {
    ...commonFields,
    tenantId: TENANT_ID,
    schoolId: SCHOOL_ID,
    academicYear: ACADEMIC_YEAR,
    courseSectionId,
    courseId,
    calendarId,
    markingPeriodId,
    isExamGrade,
  });
  if (res._failure) return false; // "No Record Found" means no existing grades
  const existing = res.studentFinalGradeList || [];
  return existing.length > 0;
}

async function submitGrades(courseSectionId, courseId, calendarId, markingPeriodId, isExamGrade, creditHours, studentGradeList) {
  const body = {
    ...commonFields,
    tenantId: TENANT_ID,
    schoolId: SCHOOL_ID,
    academicYear: ACADEMIC_YEAR,
    courseSectionId,
    courseId,
    calendarId,
    markingPeriodId,
    isExamGrade,
    isPercent: true,
    creditHours,
    createdOrUpdatedBy: emailAddress,
    studentFinalGradeList: studentGradeList,
  };

  if (DRY_RUN) {
    console.log(`    [DRY RUN] Would submit ${studentGradeList.length} grades for markingPeriodId=${markingPeriodId}, isExam=${isExamGrade}`);
    return { _failure: false };
  }

  const res = await apiPost('InputFinalGrade/addUpdateStudentFinalGrade', body);
  return res;
}

// ---------------------------------------------------------------------------
// Main
// ---------------------------------------------------------------------------

async function main() {
  if (DRY_RUN) {
    console.log('=== DRY RUN MODE — no grades will be submitted ===\n');
  }

  let totalSectionsGraded = 0;
  let totalGradeRecords = 0;
  let totalErrors = 0;
  const errors = [];

  // 1. Fetch all courses
  const courseViewList = await getAllCourses();

  // 2. For each course, get sections
  for (const cv of courseViewList) {
    const courseId = cv.course.courseId;
    const courseTitle = cv.course.courseTitle;

    let sections;
    try {
      sections = await getCourseSections(courseId);
    } catch (err) {
      console.error(`  ERROR fetching sections for course ${courseId} (${courseTitle}): ${err.message}`);
      errors.push(`Course ${courseId}: ${err.message}`);
      totalErrors++;
      continue;
    }

    if (!sections.length) continue;

    for (const sectionView of sections) {
      const section = sectionView.courseSection;
      const courseSectionId = section.courseSectionId;
      const calendarId = section.calendarId;
      const creditHours = section.creditHours || 1.0;
      const studentCount = sectionView.totalStudentSchedule || 0;

      if (studentCount === 0) {
        continue; // Skip sections with no students
      }

      console.log(`\nGrading: [${courseTitle}] section ${courseSectionId} (${studentCount} students, ${creditHours} credits)`);

      // 3. Get enrolled students
      let students;
      try {
        students = await getStudentsForSection(courseSectionId, courseId, calendarId);
      } catch (err) {
        console.error(`  ERROR fetching students for section ${courseSectionId}: ${err.message}`);
        errors.push(`Section ${courseSectionId}: ${err.message}`);
        totalErrors++;
        continue;
      }

      if (!students.length) {
        console.log('  No students returned, skipping');
        continue;
      }

      console.log(`  Enrolled students: ${students.length}`);

      // 4. For each quarter, submit regular + exam grades
      let sectionOk = true;
      for (const qtrId of QUARTERS) {
        for (const isExam of [false, true]) {
          const markingPeriodId = isExam ? `2_${qtrId}_E` : `2_${qtrId}`;
          const label = `${QUARTER_NAMES[qtrId]} ${isExam ? 'Exam' : 'Grade'}`;

          // Check if grades already exist for this section/quarter/exam combo
          try {
            const exists = await checkExistingGrades(courseSectionId, courseId, calendarId, markingPeriodId, isExam);
            if (exists) {
              console.log(`  ${label}: grades already exist, skipping`);
              continue;
            }
          } catch (err) {
            // If check fails, proceed with submission anyway
            console.log(`  ${label}: could not check existing (${err.message}), will submit`);
          }

          // Build grade list for all students
          const studentGradeList = students.map((s) => {
            const studentId = s.studentId;
            const pct = generatePercent(studentId, courseId, qtrId, isExam);
            const gradeEntry = gradeFromPercent(pct);
            const isFailing = gradeEntry.gp === 0 && gradeEntry.grade !== 'Inc';

            return {
              studentId,
              gradeId: gradeEntry.gradeId,
              gradeScaleId: GRADE_SCALE_ID,
              percentMarks: pct,
              gradeObtained: gradeEntry.grade,
              creditAttempted: creditHours,
              creditEarned: isFailing ? 0 : creditHours,
            };
          });

          try {
            const res = await submitGrades(
              courseSectionId, courseId, calendarId, markingPeriodId,
              isExam, creditHours, studentGradeList
            );

            if (res._failure) {
              console.error(`  ERROR submitting ${label}: ${res._message || 'unknown failure'}`);
              errors.push(`Section ${courseSectionId} ${label}: ${res._message || 'API failure'}`);
              totalErrors++;
              sectionOk = false;
            } else {
              console.log(`  ${label}: ${studentGradeList.length} grades submitted`);
              totalGradeRecords += studentGradeList.length;
            }
          } catch (err) {
            console.error(`  ERROR submitting ${label}: ${err.message}`);
            errors.push(`Section ${courseSectionId} ${label}: ${err.message}`);
            totalErrors++;
            sectionOk = false;
          }

          // Small delay to avoid hammering the API
          if (!DRY_RUN) await sleep(100);
        }
      }

      if (sectionOk) totalSectionsGraded++;
    }
  }

  // Summary
  console.log('\n' + '='.repeat(60));
  console.log('SUMMARY');
  console.log('='.repeat(60));
  console.log(`Sections graded successfully: ${totalSectionsGraded}`);
  console.log(`Total grade records submitted: ${totalGradeRecords}`);
  console.log(`Errors: ${totalErrors}`);
  if (errors.length) {
    console.log('\nError details:');
    errors.forEach((e, i) => console.log(`  ${i + 1}. ${e}`));
  }
  if (DRY_RUN) {
    console.log('\n(Dry run — no actual API calls were made)');
  }
}

main().catch((err) => {
  console.error('Fatal error:', err);
  process.exit(1);
});
