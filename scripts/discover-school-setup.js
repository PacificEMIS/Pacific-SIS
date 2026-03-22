/**
 * Discovery script: queries a school's setup to understand what marking periods,
 * grade scales, course sections, and student enrollments exist.
 *
 * Usage: TOKEN=<jwt> node scripts/discover-school-setup.js
 * Requires: API running at https://localhost:5001
 *
 * Get token: log in via UI, then DevTools > Application > Session Storage > token
 * Optional env vars: TENANT, SCHOOL_ID, ACADEMIC_YEAR
 */

const https = require('https');

const API_BASE = 'https://localhost:5001';
const TENANT = process.env.TENANT || 'fedsis';
const fs = require('fs');
const TOKEN = process.env.TOKEN || (function() {
  try { return fs.readFileSync(__dirname + '/.token', 'utf8').trim(); } catch { return null; }
})();

if (!TOKEN) {
  console.error('Usage: TOKEN=<jwt> node scripts/discover-school-setup.js');
  console.error('Or create scripts/.token file with the JWT');
  console.error('Get token from browser: DevTools > Application > Session Storage > token');
  process.exit(1);
}

// Decode JWT to extract tenantId and display name
const payload = JSON.parse(Buffer.from(TOKEN.split('.')[1], 'base64').toString());
const parts = payload.unique_name.split('|');
const EMAIL = parts[1];
const TENANT_ID = parts[2];
// _userName must be the display name (not email) — token validation uses tenantName + displayName
const USER_NAME = parts[0].replace(new RegExp('^' + TENANT), '');

const schoolId = parseInt(process.env.SCHOOL_ID || '163');  // default: Lewetik Elementary
const academicYear = parseInt(process.env.ACADEMIC_YEAR || '2025');

console.log(`Tenant: ${TENANT}, TenantId: ${TENANT_ID}`);
console.log(`User: ${EMAIL}, DisplayName: ${USER_NAME}`);
console.log(`School ID: ${schoolId}, Academic Year: ${academicYear}`);

const common = { _tenantName: TENANT, _token: TOKEN, _userName: USER_NAME, _academicYear: academicYear };

// HTTP helper — ignores self-signed cert
function apiPost(path, body) {
  return new Promise((resolve, reject) => {
    const url = new URL(`${API_BASE}/${TENANT}/${path}`);
    const data = JSON.stringify(body);
    const options = {
      hostname: url.hostname,
      port: url.port,
      path: url.pathname,
      method: 'POST',
      headers: { 'Content-Type': 'application/json', 'Content-Length': Buffer.byteLength(data) },
      rejectUnauthorized: false
    };
    const req = https.request(options, (res) => {
      let body = '';
      res.on('data', chunk => body += chunk);
      res.on('end', () => {
        try { resolve(JSON.parse(body)); }
        catch { resolve(body); }
      });
    });
    req.on('error', reject);
    req.write(data);
    req.end();
  });
}

async function main() {
  console.log('\n=== DISCOVERING SCHOOL SETUP ===\n');

  // Step 1: Marking periods
  console.log('--- Marking Periods ---');
  const mpRes = await apiPost('StaffPortalGradebook/populateFinalGrading', {
    ...common,
    schoolId,
    tenantId: TENANT_ID,
    academicYear,
    isConfiguration: false
  });

  if (mpRes._failure) {
    console.error('Failed to get marking periods:', mpRes._message);
    process.exit(1);
  }

  if (mpRes.schoolYears) {
    console.log(`School Year: ${mpRes.schoolYears.title} (ID: ${mpRes.schoolYears.markingPeriodId})`);
    console.log(`  DoesGrades: ${mpRes.schoolYears.doesGrades}, DoesExam: ${mpRes.schoolYears.doesExam}`);
  }

  const semesters = mpRes.semesters || [];
  if (semesters.length > 0) {
    console.log(`\nSemesters (${semesters.length}):`);
    semesters.forEach(s => {
      console.log(`  ${s.title} (ID: ${s.markingPeriodId}) — DoesGrades: ${s.doesGrades}, DoesExam: ${s.doesExam}`);
    });
  }

  const quarters = mpRes.quarters || [];
  if (quarters.length > 0) {
    console.log(`\nQuarters (${quarters.length}):`);
    quarters.forEach(q => {
      console.log(`  ${q.title} (ID: ${q.markingPeriodId}, SemesterId: ${q.semesterId}) — DoesGrades: ${q.doesGrades}, DoesExam: ${q.doesExam}`);
    });
  }

  const progressPeriods = mpRes.progressPeriods || [];
  if (progressPeriods.length > 0) {
    console.log(`\nProgress Periods (${progressPeriods.length}):`);
    progressPeriods.forEach(p => {
      console.log(`  ${p.title} (ID: ${p.markingPeriodId}, QuarterId: ${p.quarterId}) — DoesGrades: ${p.doesGrades}, DoesExam: ${p.doesExam}`);
    });
  }

  // Step 2: Grade scales
  console.log('\n--- Grade Scales ---');
  const gsRes = await apiPost('Grade/getAllGradeScaleList', {
    ...common,
    tenantId: TENANT_ID,
    schoolId,
    academicYear
  });

  const gradeScales = gsRes.gradeScaleList || [];
  if (gradeScales.length > 0) {
    gradeScales.forEach(gs => {
      console.log(`\nScale: ${gs.gradeScaleName} (ID: ${gs.gradeScaleId})`);
      console.log(`  CalculateGpa: ${gs.calculateGpa}, UseAsStandard: ${gs.useAsStandardGradeScale}`);
      if (gs.grade && gs.grade.length > 0) {
        gs.grade.forEach(g => {
          console.log(`    ${g.title} — Breakoff: ${g.breakoff}, UnweightedGP: ${g.unweightedGpValue}, WeightedGP: ${g.weightedGpValue} (GradeId: ${g.gradeId})`);
        });
      }
    });
  } else {
    console.log('  No grade scales found!');
  }

  // Step 3: Get courses first, then sections per course
  console.log('\n--- Courses & Sections ---');
  const courseRes = await apiPost('CourseManager/getAllCourseList', {
    ...common,
    tenantId: TENANT_ID,
    schoolId,
    academicYear
  });

  const courses = courseRes.courseViewModelList || [];
  console.log(`Total courses: ${courses.length}`);

  // Collect all sections across all courses
  let sections = [];
  for (const course of courses) {
    const cid = course.course?.courseId || course.courseId;
    const ctitle = course.course?.courseTitle || course.courseTitle || 'Unknown';
    if (!cid) continue;

    const csRes = await apiPost('CourseManager/getAllCourseSection', {
      ...common,
      tenantId: TENANT_ID,
      schoolId,
      academicYear,
      courseId: cid
    });

    const courseSections = csRes.getCourseSectionForView || [];
    if (courseSections.length > 0) {
      console.log(`\n  Course [${cid}] ${ctitle} — ${courseSections.length} section(s)`);
      sections = sections.concat(courseSections.map(s => ({ ...s, courseTitle: ctitle })));
    }
  }

  console.log(`\nTotal course sections across all courses: ${sections.length}`);

  // The view wraps section details in .courseSection and includes summary fields at top level
  let gradableSections = 0;
  let sectionsWithStudents = 0;
  let sectionsWithTeachers = 0;

  for (const view of sections) {
    const cs = view.courseSection || view;
    const secId = cs.courseSectionId;
    const crsId = cs.courseId;
    const name = cs.courseSectionName || view.courseTitle || 'Untitled';
    const students = view.totalStudentSchedule || 0;
    const staff = view.totalStaffSchedule || 0;
    const teacher = view.staffName || 'none';
    const mp = view.markingPeriod || '';

    if (students > 0) sectionsWithStudents++;
    if (staff > 0) sectionsWithTeachers++;
    if (cs.gradeScaleType && cs.gradeScaleType !== 'Ungraded') gradableSections++;

    console.log(`\n  [CS:${secId}] ${name} (CourseId: ${crsId})`);
    console.log(`    GradeScaleType: ${cs.gradeScaleType}, GradeScaleId: ${cs.gradeScaleId}`);
    console.log(`    IsWeighted: ${cs.isWeightedCourse}, AffectsClassRank: ${cs.affectsClassRank}, AffectsHonorRoll: ${cs.affectsHonorRoll}`);
    console.log(`    CreditHours: ${cs.creditHours}, MarkingPeriod: ${mp}`);
    console.log(`    MP IDs — Yr: ${cs.yrMarkingPeriodId}, Smstr: ${cs.smstrMarkingPeriodId}, Qtr: ${cs.qtrMarkingPeriodId}, PP: ${cs.prgrsprdMarkingPeriodId}`);
    console.log(`    Students: ${students}, Staff: ${staff}, Teacher: ${teacher}`);
  }

  console.log(`\n--- Summary ---`);
  console.log(`Total sections: ${sections.length}`);
  console.log(`Gradable (non-Ungraded): ${gradableSections}`);
  console.log(`With enrolled students: ${sectionsWithStudents}`);
  console.log(`With assigned teachers: ${sectionsWithTeachers}`);

  // Step 4: Check existing final grades for first section
  if (sections.length > 0) {
    console.log('\n--- Existing Final Grades (sample) ---');
    const firstSection = sections[0];
    const existingRes = await apiPost('InputFinalGrade/getAllStudentFinalGradeList', {
      ...common,
      tenantId: TENANT_ID,
      schoolId,
      academicYear,
      courseSectionId: firstSection.courseSectionId,
      courseId: firstSection.courseId
    });

    if (existingRes._failure) {
      console.log(`  Section ${firstSection.courseSectionId}: ${existingRes._message}`);
    } else {
      const existing = existingRes.studentFinalGradeList || [];
      console.log(`  Section ${firstSection.courseSectionId}: ${existing.length} grade records`);
      if (existing.length > 0) {
        existing.slice(0, 3).forEach(g => {
          console.log(`    StudentId: ${g.studentId}, Grade: ${g.gradeObtained}, Pct: ${g.percentMarks}, MP: yr=${g.yrMarkingPeriodId} sm=${g.smstrMarkingPeriodId} qtr=${g.qtrMarkingPeriodId} pp=${g.prgrsprdMarkingPeriodId}`);
        });
      }
    }
  }

  console.log('\n=== DISCOVERY COMPLETE ===');
}

main().catch(err => {
  console.error('Error:', err.message);
  process.exit(1);
});
