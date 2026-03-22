const https = require('https');
const fs = require('fs');
const TOKEN = process.env.TOKEN || fs.readFileSync(__dirname + '/.token', 'utf8').trim();
if (!TOKEN) { console.error('TOKEN required'); process.exit(1); }

const payload = JSON.parse(Buffer.from(TOKEN.split('.')[1], 'base64').toString());
const parts = payload.unique_name.split('|');
const TENANT_ID = parts[2];
const USER_NAME = parts[0].replace(/^fedsis/, '');
const common = { _tenantName: 'fedsis', _token: TOKEN, _userName: USER_NAME, _academicYear: 2025 };

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

async function main() {
  // Get sections for first course that has sections (course 68 = G1 Listening & Speaking)
  const csRes = await apiPost('CourseManager/getAllCourseSection', {
    ...common, tenantId: TENANT_ID, schoolId: 163, academicYear: 2025, courseId: 68
  });

  const sections = csRes.getCourseSectionForView || [];
  console.log('Section count:', sections.length);

  if (sections.length > 0) {
    const first = sections[0];
    console.log('\nSection keys:', Object.keys(first).join(', '));
    // Print non-null/non-undefined values
    console.log('\nNon-null values:');
    for (const [k, v] of Object.entries(first)) {
      if (v !== null && v !== undefined && v !== '' && typeof v !== 'object') {
        console.log(`  ${k}: ${v}`);
      }
    }
    // Print object keys for nested objects
    for (const [k, v] of Object.entries(first)) {
      if (v && typeof v === 'object' && !Array.isArray(v)) {
        console.log(`  ${k}: [object] keys=${Object.keys(v).join(', ')}`);
      }
      if (Array.isArray(v) && v.length > 0) {
        console.log(`  ${k}: [array len=${v.length}]`);
      }
    }
  }

  // Also get students for a section - need the right section id first
  if (sections.length > 0) {
    const sec = sections[0];
    const secId = sec.courseSectionId || sec.CourseSectionId;
    const crsId = sec.courseId || sec.CourseId || 68;
    console.log('\n\nTrying student schedule for section:', secId, 'course:', crsId);

    const studentRes = await apiPost('CourseManager/getAllStudentSchedule', {
      ...common, tenantId: TENANT_ID, schoolId: 163, academicYear: 2025,
      courseSectionId: secId, courseId: crsId,
      pageNumber: 1, pageSize: 200, _pageSize: 200
    });

    console.log('Student response keys:', Object.keys(studentRes).join(', '));
    console.log('Failure:', studentRes._failure, 'Message:', studentRes._message);
    const students = studentRes.studentCoursesectionScheduleList || studentRes.studentScheduleList || [];
    console.log('Students:', students.length);
    if (students.length > 0) {
      console.log('First student keys:', Object.keys(students[0]).join(', '));
      for (const [k, v] of Object.entries(students[0])) {
        if (v !== null && v !== undefined && v !== '' && typeof v !== 'object') {
          console.log(`  ${k}: ${v}`);
        }
      }
    }
  }
}

main().catch(e => console.error(e.message));
