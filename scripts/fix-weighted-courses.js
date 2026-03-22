/**
 * Sets isWeightedCourse = false on all course sections at Lewetik Elementary.
 * Uses the CourseManager/updateCourseSection API endpoint.
 */

const https = require('https');
const fs = require('fs');

const TOKEN = (function () {
  try { return fs.readFileSync(__dirname + '/.token', 'utf8').trim(); } catch { return null; }
})();
if (!TOKEN) { console.error('Create scripts/.token with JWT'); process.exit(1); }

const payload = JSON.parse(Buffer.from(TOKEN.split('.')[1], 'base64').toString());
const parts = payload.unique_name.split('|');
const TENANT_ID = parts[2];
const USER_NAME = parts[0].replace(/^fedsis/, '');
const EMAIL = parts[1];

const common = { _tenantName: 'fedsis', _token: TOKEN, _userName: USER_NAME, _academicYear: 2025 };
const SCHOOL_ID = 163;

function apiPost(path, body) {
  return new Promise((resolve, reject) => {
    const url = new URL('https://localhost:5001/fedsis/' + path);
    const data = JSON.stringify(body);
    const req = https.request({
      hostname: url.hostname, port: url.port, path: url.pathname, method: 'POST',
      headers: { 'Content-Type': 'application/json', 'Content-Length': Buffer.byteLength(data) },
      rejectUnauthorized: false
    }, (res) => {
      let b = '';
      res.on('data', c => b += c);
      res.on('end', () => { try { resolve(JSON.parse(b)); } catch { resolve(b); } });
    });
    req.on('error', reject);
    req.write(data);
    req.end();
  });
}

function apiPut(path, body) {
  return new Promise((resolve, reject) => {
    const url = new URL('https://localhost:5001/fedsis/' + path);
    const data = JSON.stringify(body);
    const req = https.request({
      hostname: url.hostname, port: url.port, path: url.pathname, method: 'PUT',
      headers: { 'Content-Type': 'application/json', 'Content-Length': Buffer.byteLength(data) },
      rejectUnauthorized: false
    }, (res) => {
      let b = '';
      res.on('data', c => b += c);
      res.on('end', () => { try { resolve(JSON.parse(b)); } catch { resolve(b); } });
    });
    req.on('error', reject);
    req.write(data);
    req.end();
  });
}

async function main() {
  // Get all courses
  const courseRes = await apiPost('CourseManager/getAllCourseList', {
    ...common, tenantId: TENANT_ID, schoolId: SCHOOL_ID, academicYear: 2025
  });
  const courses = courseRes.courseViewModelList || [];
  console.log(`Found ${courses.length} courses`);

  let updated = 0;
  let skipped = 0;

  for (const course of courses) {
    const courseId = course.course?.courseId || course.courseId;
    if (!courseId) continue;

    const csRes = await apiPost('CourseManager/getAllCourseSection', {
      ...common, tenantId: TENANT_ID, schoolId: SCHOOL_ID, academicYear: 2025, courseId
    });

    const sectionViews = csRes.getCourseSectionForView || [];
    for (const view of sectionViews) {
      const cs = view.courseSection;
      if (cs.isWeightedCourse === false) {
        skipped++;
        continue;
      }

      // Update the section with isWeightedCourse = false
      cs.isWeightedCourse = false;
      cs.updatedBy = EMAIL;

      const updateRes = await apiPut('CourseManager/updateCourseSection', {
        ...common,
        tenantId: TENANT_ID,
        schoolId: SCHOOL_ID,
        courseSection: cs
      });

      if (updateRes._failure) {
        console.log(`  ERROR updating CS:${cs.courseSectionId} ${cs.courseSectionName}: ${updateRes._message}`);
      } else {
        console.log(`  Updated CS:${cs.courseSectionId} ${cs.courseSectionName} — isWeightedCourse = false`);
        updated++;
      }
    }
  }

  console.log(`\nDone. Updated: ${updated}, Already false: ${skipped}`);
}

main().catch(e => { console.error('Error:', e.message); process.exit(1); });
