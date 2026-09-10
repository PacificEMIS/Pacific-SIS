/***********************************************************************************
openSIS is a free student information system for public and non-public
schools from Open Solutions for Education, Inc.Website: www.os4ed.com.

Visit the openSIS product website at https://opensis.com to learn more.
If you have question regarding this software or the license, please contact
via the website.

The software is released under the terms of the GNU Affero General Public License as
published by the Free Software Foundation, version 3 of the License.
See https://www.gnu.org/licenses/agpl-3.0.en.html.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

Copyright (c) Open Solutions for Education, Inc.

All rights reserved.
***********************************************************************************/

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NUnit.Framework;
using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.Repository;
using opensis.data.ViewModels.SuperAdministrator;

namespace opensis.NunitTest.SuperAdministrator
{
    /// <summary>
    /// Exercises SuperAdministratorRepository against an in-memory copy of
    /// the tenant model. Each test gets a fresh database seeded with two
    /// schools, the system memberships, a calling Super Administrator, a
    /// second Super Administrator and a Teacher with a portal login.
    /// </summary>
    [TestFixture]
    public class SuperAdministratorRepositoryTests
    {
        private static readonly Guid Tenant = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private const string CallerEmail = "caller@example.org";
        private const string OtherEmail = "other@example.org";
        private const string TeacherEmail = "teacher@example.org";
        private const int CallerStaffId = 1;
        private const int OtherStaffId = 2;
        private const int TeacherStaffId = 3;

        private sealed class InMemoryFactory : IDbContextFactory
        {
            private readonly DbContextOptions options;
            public InMemoryFactory(string name)
            {
                var builder = new DbContextOptionsBuilder();
                builder.UseInMemoryDatabase(name)
                       .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                options = builder.Options;
            }
            public string? TenantName { get; set; }
            public string? ApiKeyValue { get; set; }
            public CRMContext? Create() => new CRMContextMySQL(options);
        }

        private InMemoryFactory factory = null!;

        [SetUp]
        public void Seed()
        {
            factory = new InMemoryFactory(Guid.NewGuid().ToString());
            using var db = factory.Create()!;
            foreach (var schoolId in new[] { 1, 2 })
            {
                db.SchoolMaster.Add(new SchoolMaster { TenantId = Tenant, SchoolId = schoolId, SchoolGuid = Guid.NewGuid(), SchoolInternalId = schoolId.ToString(), SchoolName = "School " + schoolId });
                db.SchoolDetail.Add(new SchoolDetail { Id = schoolId, TenantId = Tenant, SchoolId = schoolId, Status = true });
                db.Membership.AddRange(
                    new Membership { TenantId = Tenant, SchoolId = schoolId, MembershipId = 1, Profile = "Super Administrator", ProfileType = "Super Administrator", IsSuperadmin = true, IsSystem = true, IsActive = true },
                    new Membership { TenantId = Tenant, SchoolId = schoolId, MembershipId = 2, Profile = "School Administrator", ProfileType = "School Administrator", IsSystem = true, IsActive = true },
                    new Membership { TenantId = Tenant, SchoolId = schoolId, MembershipId = 3, Profile = "Admin Assistant", ProfileType = "Admin Assistant", IsSystem = true, IsActive = true },
                    new Membership { TenantId = Tenant, SchoolId = schoolId, MembershipId = 4, Profile = "Teacher", ProfileType = "Teacher", IsSystem = true, IsActive = true },
                    new Membership { TenantId = Tenant, SchoolId = schoolId, MembershipId = 5, Profile = "Homeroom Teacher", ProfileType = "Homeroom Teacher", IsSystem = true, IsActive = true });
            }
            AddSuperAdmin(db, CallerStaffId, 1, "Cal", "Caller", CallerEmail);
            AddSuperAdmin(db, OtherStaffId, 1, "Oth", "Other", OtherEmail);
            db.StaffMaster.Add(new StaffMaster { TenantId = Tenant, SchoolId = 2, StaffId = TeacherStaffId, StaffGuid = Guid.NewGuid(), FirstGivenName = "Tea", LastFamilyName = "Cher", LoginEmailAddress = TeacherEmail, PortalAccess = true, Profile = "Teacher", IsActive = true });
            db.UserMaster.Add(new UserMaster { TenantId = Tenant, SchoolId = 2, UserId = TeacherStaffId, Name = "Tea", EmailAddress = TeacherEmail, PasswordHash = "x", LangId = 1, MembershipId = 4, IsActive = true });
            db.StaffSchoolInfo.Add(new StaffSchoolInfo { Id = 1, TenantId = Tenant, SchoolId = 2, StaffId = TeacherStaffId, SchoolAttachedId = 2, SchoolAttachedName = "School 2", Profile = "Teacher", MembershipId = 4, StartDate = DateTime.UtcNow.Date.AddYears(-1) });
            db.LoginSession.Add(new LoginSession { Id = 1, TenantId = Tenant, SchoolId = 1, EmailAddress = OtherEmail, Token = "t", IsExpired = false });
            db.SaveChanges();
        }

        private static void AddSuperAdmin(CRMContext db, int staffId, int schoolId, string first, string last, string email)
        {
            db.StaffMaster.Add(new StaffMaster { TenantId = Tenant, SchoolId = schoolId, StaffId = staffId, StaffGuid = Guid.NewGuid(), FirstGivenName = first, LastFamilyName = last, LoginEmailAddress = email, PortalAccess = true, Profile = "Super Administrator", JobTitle = "Super Administrator", IsActive = true });
            db.UserMaster.Add(new UserMaster { TenantId = Tenant, SchoolId = schoolId, UserId = staffId, Name = first, EmailAddress = email, PasswordHash = "x", LangId = 1, MembershipId = 1, IsActive = true });
        }

        private SuperAdministratorRepository Repo() => new SuperAdministratorRepository(factory);

        private static SuperAdministratorActionViewModel Action(int staffId, string caller = CallerEmail) =>
            new SuperAdministratorActionViewModel { TenantId = Tenant, SchoolId = 1, StaffId = staffId, CallerEmail = caller, UpdatedBy = "test" };

        /// <summary>Mirror of Utility.Decrypt so tests can hand the repository a UI-shaped password.</summary>
        private static string Encrypt(string plain)
        {
            var salt = RandomNumberGenerator.GetBytes(16);
            var iv = RandomNumberGenerator.GetBytes(16);
            using var aes = Aes.Create();
            aes.Key = new Rfc2898DeriveBytes("oPen$!$.b14Ca5898a4e4133b!", salt, 100).GetBytes(32);
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                var bytes = Encoding.UTF8.GetBytes(plain);
                cs.Write(bytes, 0, bytes.Length);
            }
            return Convert.ToBase64String(salt.Concat(iv).Concat(ms.ToArray()).ToArray());
        }

        [Test]
        public void Encrypt_RoundTripsThroughUtilityDecrypt()
        {
            Assert.AreEqual("Secret123", Utility.Decrypt(Encrypt("Secret123")));
        }

        // ---- list -----------------------------------------------------------

        [Test]
        public void GetSuperAdministrators_RefusesNonSuperAdminCaller()
        {
            var result = Repo().GetSuperAdministrators(new SuperAdministratorListViewModel { TenantId = Tenant, CallerEmail = TeacherEmail });
            Assert.IsTrue(result._failure);
            Assert.AreEqual(SuperAdministratorRepository.NOT_AUTHORISED, result._message);
            Assert.IsEmpty(result.SuperAdministrators);
        }

        [Test]
        public void GetSuperAdministrators_RefusesUnknownCaller()
        {
            var result = Repo().GetSuperAdministrators(new SuperAdministratorListViewModel { TenantId = Tenant, CallerEmail = null });
            Assert.IsTrue(result._failure);
        }

        [Test]
        public void GetSuperAdministrators_ListsBothAndFlagsSelf()
        {
            var result = Repo().GetSuperAdministrators(new SuperAdministratorListViewModel { TenantId = Tenant, CallerEmail = CallerEmail });
            Assert.IsFalse(result._failure, result._message);
            Assert.AreEqual(2, result.SuperAdministrators.Count);
            var me = result.SuperAdministrators.Single(x => x.StaffId == CallerStaffId);
            Assert.IsTrue(me.IsSelf);
            Assert.IsTrue(me.HasLogin);
            Assert.IsTrue(me.IsActive);
            Assert.IsFalse(me.HasSchoolAttachments);
            Assert.AreEqual("School 1", me.SchoolName);
            Assert.IsFalse(result.SuperAdministrators.Single(x => x.StaffId == OtherStaffId).IsSelf);
        }

        [Test]
        public void GetSuperAdministrators_IncludesLoginWhoseStaffProfileDrifted()
        {
            using (var db = factory.Create()!)
            {
                db.StaffMaster.Single(s => s.StaffId == OtherStaffId).Profile = "Teacher";
                db.SaveChanges();
            }
            var result = Repo().GetSuperAdministrators(new SuperAdministratorListViewModel { TenantId = Tenant, CallerEmail = CallerEmail });
            Assert.IsTrue(result.SuperAdministrators.Any(x => x.StaffId == OtherStaffId));
        }

        // ---- add ------------------------------------------------------------

        [Test]
        public void Add_CreatesStaffAndLoginWithoutSchoolAttachment()
        {
            var model = new SuperAdministratorAddViewModel
            {
                TenantId = Tenant, SchoolId = 2, CallerEmail = CallerEmail, CreatedBy = "test",
                FirstGivenName = " New ", LastFamilyName = "Admin", LoginEmailAddress = "new@example.org", PasswordHash = Encrypt("Secret123")
            };
            var result = Repo().AddSuperAdministrator(model);
            Assert.IsFalse(result._failure, result._message);
            Assert.AreEqual(4, result.StaffId);
            Assert.IsNull(result.PasswordHash, "password must not be echoed back");

            using var db = factory.Create()!;
            var staff = db.StaffMaster.Single(s => s.StaffId == 4);
            Assert.AreEqual("Super Administrator", staff.Profile);
            Assert.AreEqual("New", staff.FirstGivenName);
            Assert.AreEqual(2, staff.SchoolId);
            Assert.AreEqual(true, staff.PortalAccess);
            Assert.AreEqual("4", staff.StaffInternalId);
            var user = db.UserMaster.Single(u => u.EmailAddress == "new@example.org");
            Assert.AreEqual(1, user.MembershipId);
            Assert.AreEqual(2, user.SchoolId);
            Assert.AreEqual(4, user.UserId);
            Assert.AreEqual(Utility.GetHashedPassword("Secret123"), user.PasswordHash);
            Assert.IsFalse(db.StaffSchoolInfo.Any(i => i.StaffId == 4));
        }

        [Test]
        public void Add_RefusesNonSuperAdminCaller()
        {
            var result = Repo().AddSuperAdministrator(new SuperAdministratorAddViewModel
            {
                TenantId = Tenant, SchoolId = 1, CallerEmail = TeacherEmail,
                FirstGivenName = "A", LastFamilyName = "B", LoginEmailAddress = "ab@example.org", PasswordHash = Encrypt("Secret123")
            });
            Assert.IsTrue(result._failure);
            Assert.AreEqual(SuperAdministratorRepository.NOT_AUTHORISED, result._message);
            using var db = factory.Create()!;
            Assert.AreEqual(3, db.StaffMaster.Count());
        }

        [Test]
        public void Add_RefusesDuplicateEmail()
        {
            var result = Repo().AddSuperAdministrator(new SuperAdministratorAddViewModel
            {
                TenantId = Tenant, SchoolId = 1, CallerEmail = CallerEmail,
                FirstGivenName = "A", LastFamilyName = "B", LoginEmailAddress = TeacherEmail, PasswordHash = Encrypt("Secret123")
            });
            Assert.IsTrue(result._failure);
            StringAssert.Contains("already exists", result._message);
        }

        [TestCase("", "B", "ab@example.org", "Secret123")]
        [TestCase("A", "", "ab@example.org", "Secret123")]
        [TestCase("A", "B", "not-an-email", "Secret123")]
        [TestCase("A", "B", "ab@example.org", "")]
        public void Add_RefusesMissingFields(string first, string last, string email, string password)
        {
            var result = Repo().AddSuperAdministrator(new SuperAdministratorAddViewModel
            {
                TenantId = Tenant, SchoolId = 1, CallerEmail = CallerEmail,
                FirstGivenName = first, LastFamilyName = last, LoginEmailAddress = email,
                PasswordHash = password.Length == 0 ? null : Encrypt(password)
            });
            Assert.IsTrue(result._failure);
            using var db = factory.Create()!;
            Assert.AreEqual(3, db.StaffMaster.Count());
        }

        [Test]
        public void Add_RefusesUnreadablePassword()
        {
            var result = Repo().AddSuperAdministrator(new SuperAdministratorAddViewModel
            {
                TenantId = Tenant, SchoolId = 1, CallerEmail = CallerEmail,
                FirstGivenName = "A", LastFamilyName = "B", LoginEmailAddress = "ab@example.org", PasswordHash = "plain-text"
            });
            Assert.IsTrue(result._failure);
            StringAssert.Contains("could not be read", result._message);
        }

        // ---- activate / deactivate ------------------------------------------

        [Test]
        public void SetActiveStatus_RefusesSelf()
        {
            var model = Action(CallerStaffId);
            model.IsActive = false;
            var result = Repo().SetActiveStatus(model);
            Assert.IsTrue(result._failure);
            Assert.AreEqual(SuperAdministratorRepository.SELF_ACTION, result._message);
        }

        [Test]
        public void SetActiveStatus_RefusesNonSuperAdminTarget()
        {
            var model = Action(TeacherStaffId);
            model.IsActive = false;
            var result = Repo().SetActiveStatus(model);
            Assert.IsTrue(result._failure);
            Assert.AreEqual(SuperAdministratorRepository.NOT_SUPER_ADMIN, result._message);
        }

        [Test]
        public void SetActiveStatus_DeactivatesStaffLoginAndSessions()
        {
            var model = Action(OtherStaffId);
            model.IsActive = false;
            var result = Repo().SetActiveStatus(model);
            Assert.IsFalse(result._failure, result._message);

            using var db = factory.Create()!;
            Assert.AreEqual(false, db.StaffMaster.Single(s => s.StaffId == OtherStaffId).IsActive);
            Assert.AreEqual(false, db.UserMaster.Single(u => u.EmailAddress == OtherEmail).IsActive);
            Assert.AreEqual(true, db.LoginSession.Single(s => s.EmailAddress == OtherEmail).IsExpired);

            model.IsActive = true;
            result = Repo().SetActiveStatus(model);
            Assert.IsFalse(result._failure, result._message);
            using var db2 = factory.Create()!;
            Assert.AreEqual(true, db2.StaffMaster.Single(s => s.StaffId == OtherStaffId).IsActive);
            Assert.AreEqual(true, db2.UserMaster.Single(u => u.EmailAddress == OtherEmail).IsActive);
        }

        [Test]
        public void GetSuperAdministrators_StatusFollowsLoginNotStaffFlag()
        {
            // Seeded data has staff rows carrying the staff disable flag under
            // logins that are enabled. Such a person can still sign in, so the
            // page reports Active and flags the staff record separately.
            using (var db = factory.Create()!)
            {
                db.StaffMaster.Single(s => s.StaffId == OtherStaffId).IsActive = false;
                db.SaveChanges();
            }
            var other = Repo().GetSuperAdministrators(new SuperAdministratorListViewModel { TenantId = Tenant, CallerEmail = CallerEmail })
                .SuperAdministrators.Single(x => x.StaffId == OtherStaffId);
            Assert.IsTrue(other.IsActive);
            Assert.IsTrue(other.StaffRecordDisabled);

            var model = Action(OtherStaffId);
            model.IsActive = false;
            Assert.IsFalse(Repo().SetActiveStatus(model)._failure, "the caller's own login is enabled, so another active Super Administrator remains");
        }

        // ---- delete ---------------------------------------------------------

        [Test]
        public void Delete_RefusesSelf()
        {
            var result = Repo().DeleteSuperAdministrator(Action(CallerStaffId));
            Assert.IsTrue(result._failure);
            Assert.AreEqual(SuperAdministratorRepository.SELF_ACTION, result._message);
        }

        [Test]
        public void Delete_RefusesWhenSchoolAttachmentsExist()
        {
            using (var db = factory.Create()!)
            {
                db.StaffSchoolInfo.Add(new StaffSchoolInfo { Id = 2, TenantId = Tenant, SchoolId = 1, StaffId = OtherStaffId, SchoolAttachedId = 1, Profile = "Teacher", MembershipId = 4 });
                db.SaveChanges();
            }
            var result = Repo().DeleteSuperAdministrator(Action(OtherStaffId));
            Assert.IsTrue(result._failure);
            StringAssert.Contains("school attachments", result._message);
            using var db2 = factory.Create()!;
            Assert.IsTrue(db2.StaffMaster.Any(s => s.StaffId == OtherStaffId));
        }

        [Test]
        public void Delete_RefusesWhenAttendanceRecordsExist()
        {
            using (var db = factory.Create()!)
            {
                db.StudentAttendance.Add(new StudentAttendance { TenantId = Tenant, SchoolId = 1, StudentId = 1, StaffId = OtherStaffId, AttendanceDate = DateTime.UtcNow.Date });
                db.SaveChanges();
            }
            var result = Repo().DeleteSuperAdministrator(Action(OtherStaffId));
            Assert.IsTrue(result._failure);
            StringAssert.Contains("attendance", result._message);
        }

        [Test]
        public void Delete_RefusesWhenAccountHasSignedIn()
        {
            // created_by/updated_by hold the staff GUID with no foreign key, so
            // anyone who ever signed in may be named in an audit trail.
            using (var db = factory.Create()!)
            {
                db.UserAccessLog.Add(new UserAccessLog { Id = 1, TenantId = Tenant, SchoolId = 1, Emailaddress = OtherEmail, LoginAttemptDate = DateTime.UtcNow, LoginStatus = true });
                db.UserAccessLog.Add(new UserAccessLog { Id = 2, TenantId = Tenant, SchoolId = 1, Emailaddress = CallerEmail, LoginAttemptDate = DateTime.UtcNow, LoginStatus = true });
                db.SaveChanges();
            }
            var result = Repo().DeleteSuperAdministrator(Action(OtherStaffId));
            Assert.IsTrue(result._failure);
            StringAssert.Contains("signed in before", result._message);
            using var db2 = factory.Create()!;
            Assert.IsTrue(db2.StaffMaster.Any(s => s.StaffId == OtherStaffId));
            Assert.IsTrue(db2.UserMaster.Any(u => u.EmailAddress == OtherEmail));
        }

        [Test]
        public void Delete_AllowedWhenOnlyFailedLoginsExist()
        {
            using (var db = factory.Create()!)
            {
                db.UserAccessLog.Add(new UserAccessLog { Id = 1, TenantId = Tenant, SchoolId = 1, Emailaddress = OtherEmail, LoginAttemptDate = DateTime.UtcNow, LoginStatus = false, LoginFailureCount = 1 });
                db.SaveChanges();
            }
            var result = Repo().DeleteSuperAdministrator(Action(OtherStaffId));
            Assert.IsFalse(result._failure, result._message);
        }

        [Test]
        public void Delete_RemovesStaffLoginAndSessions()
        {
            var result = Repo().DeleteSuperAdministrator(Action(OtherStaffId));
            Assert.IsFalse(result._failure, result._message);
            using var db = factory.Create()!;
            Assert.IsFalse(db.StaffMaster.Any(s => s.StaffId == OtherStaffId));
            Assert.IsFalse(db.UserMaster.Any(u => u.EmailAddress == OtherEmail));
            Assert.IsFalse(db.LoginSession.Any(s => s.EmailAddress == OtherEmail));
            Assert.IsTrue(db.StaffMaster.Any(s => s.StaffId == CallerStaffId), "other rows untouched");
        }

        [Test]
        public void Delete_RefusesNonSuperAdminTarget()
        {
            var result = Repo().DeleteSuperAdministrator(Action(TeacherStaffId));
            Assert.IsTrue(result._failure);
            Assert.AreEqual(SuperAdministratorRepository.NOT_SUPER_ADMIN, result._message);
            using var db = factory.Create()!;
            Assert.IsTrue(db.StaffMaster.Any(s => s.StaffId == TeacherStaffId));
        }

        // ---- promote --------------------------------------------------------

        [Test]
        public void GetPromotionCandidates_ListsStaffWithLoginsOnly()
        {
            using (var db = factory.Create()!)
            {
                db.StaffMaster.Add(new StaffMaster { TenantId = Tenant, SchoolId = 1, StaffId = 9, StaffGuid = Guid.NewGuid(), FirstGivenName = "No", LastFamilyName = "Login", Profile = "Teacher", IsActive = true });
                db.SaveChanges();
            }
            var result = Repo().GetPromotionCandidates(new SuperAdministratorCandidateListViewModel { TenantId = Tenant, CallerEmail = CallerEmail, SearchText = "" });
            Assert.IsFalse(result._failure, result._message);
            CollectionAssert.AreEquivalent(new[] { TeacherStaffId }, result.Candidates.Select(c => c.StaffId));
            Assert.AreEqual("School 2", result.Candidates[0].SchoolName);

            result = Repo().GetPromotionCandidates(new SuperAdministratorCandidateListViewModel { TenantId = Tenant, CallerEmail = CallerEmail, SearchText = "zzz" });
            Assert.IsEmpty(result.Candidates);
        }

        [Test]
        public void Promote_SetsLoginMembershipAndStaffProfile()
        {
            var result = Repo().PromoteToSuperAdministrator(Action(TeacherStaffId));
            Assert.IsFalse(result._failure, result._message);
            using var db = factory.Create()!;
            Assert.AreEqual("Super Administrator", db.StaffMaster.Single(s => s.StaffId == TeacherStaffId).Profile);
            Assert.AreEqual(1, db.UserMaster.Single(u => u.EmailAddress == TeacherEmail).MembershipId);
            Assert.AreEqual("Teacher", db.StaffSchoolInfo.Single(i => i.StaffId == TeacherStaffId).Profile, "attachment history is kept");
        }

        [Test]
        public void Promote_RefusesStaffWithoutLogin()
        {
            using (var db = factory.Create()!)
            {
                db.StaffMaster.Add(new StaffMaster { TenantId = Tenant, SchoolId = 1, StaffId = 9, StaffGuid = Guid.NewGuid(), FirstGivenName = "No", LastFamilyName = "Login", Profile = "Teacher", IsActive = true });
                db.SaveChanges();
            }
            var result = Repo().PromoteToSuperAdministrator(Action(9));
            Assert.IsTrue(result._failure);
            StringAssert.Contains("no portal login", result._message);
        }

        [Test]
        public void Promote_RefusesExistingSuperAdmin()
        {
            var result = Repo().PromoteToSuperAdministrator(Action(OtherStaffId));
            Assert.IsTrue(result._failure);
            StringAssert.Contains("already", result._message);
        }

        // ---- demote ---------------------------------------------------------

        [Test]
        public void Demote_RefusesSelfAndBadProfile()
        {
            var self = Action(CallerStaffId);
            self.TargetSchoolId = 2;
            self.TargetProfileType = "Teacher";
            Assert.AreEqual(SuperAdministratorRepository.SELF_ACTION, Repo().DemoteSuperAdministrator(self)._message);

            var bad = Action(OtherStaffId);
            bad.TargetSchoolId = 2;
            bad.TargetProfileType = "Student";
            Assert.IsTrue(Repo().DemoteSuperAdministrator(bad)._failure);

            var noSchool = Action(OtherStaffId);
            noSchool.TargetSchoolId = 99;
            noSchool.TargetProfileType = "Teacher";
            Assert.IsTrue(Repo().DemoteSuperAdministrator(noSchool)._failure);
        }

        [Test]
        public void Demote_TenantWideAccount_AssignsSchoolAndMovesLogin()
        {
            var model = Action(OtherStaffId);
            model.TargetSchoolId = 2;
            model.TargetProfileType = "Homeroom Teacher";
            var result = Repo().DemoteSuperAdministrator(model);
            Assert.IsFalse(result._failure, result._message);

            using var db = factory.Create()!;
            var staff = db.StaffMaster.Single(s => s.StaffId == OtherStaffId);
            Assert.AreEqual(2, staff.SchoolId, "home school becomes the chosen school");
            Assert.AreEqual("Homeroom Teacher", staff.Profile);
            Assert.IsNull(staff.JobTitle);
            var info = db.StaffSchoolInfo.Single(i => i.StaffId == OtherStaffId);
            Assert.AreEqual(2, info.SchoolId);
            Assert.AreEqual(2, info.SchoolAttachedId);
            Assert.AreEqual("Homeroom Teacher", info.Profile);
            Assert.AreEqual(5, info.MembershipId);
            var users = db.UserMaster.Where(u => u.EmailAddress == OtherEmail).ToList();
            Assert.AreEqual(1, users.Count, "the lone login is moved, not duplicated");
            Assert.AreEqual(2, users[0].SchoolId);
            Assert.AreEqual(5, users[0].MembershipId);
            Assert.AreEqual(2, users[0].LastUsedSchoolId);
            Assert.AreEqual("x", users[0].PasswordHash, "password survives the move");
            Assert.AreEqual(true, db.LoginSession.Single(s => s.EmailAddress == OtherEmail).IsExpired);
        }

        [Test]
        public void Demote_PromotedStaff_RestoresAttachmentAtItsSchool()
        {
            Assert.IsFalse(Repo().PromoteToSuperAdministrator(Action(TeacherStaffId))._failure);

            var model = Action(TeacherStaffId);
            model.TargetSchoolId = 2;
            model.TargetProfileType = "School Administrator";
            var result = Repo().DemoteSuperAdministrator(model);
            Assert.IsFalse(result._failure, result._message);

            using var db = factory.Create()!;
            var staff = db.StaffMaster.Single(s => s.StaffId == TeacherStaffId);
            Assert.AreEqual(2, staff.SchoolId);
            Assert.AreEqual("School Administrator", staff.Profile);
            var info = db.StaffSchoolInfo.Single(i => i.StaffId == TeacherStaffId);
            Assert.AreEqual("School Administrator", info.Profile, "existing attachment row is updated, not duplicated");
            Assert.AreEqual(2, info.MembershipId);
            var user = db.UserMaster.Single(u => u.EmailAddress == TeacherEmail);
            Assert.AreEqual(2, user.SchoolId);
            Assert.AreEqual(2, user.MembershipId);
        }

        [Test]
        public void Demote_ThenListNoLongerShowsTheAccount()
        {
            var model = Action(OtherStaffId);
            model.TargetSchoolId = 1;
            model.TargetProfileType = "Teacher";
            Assert.IsFalse(Repo().DemoteSuperAdministrator(model)._failure);
            var list = Repo().GetSuperAdministrators(new SuperAdministratorListViewModel { TenantId = Tenant, CallerEmail = CallerEmail });
            CollectionAssert.AreEquivalent(new[] { CallerStaffId }, list.SuperAdministrators.Select(x => x.StaffId));
        }
    }
}
