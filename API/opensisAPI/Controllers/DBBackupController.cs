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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using opensis.core.DBBackup.Interfaces;
using opensis.data.ViewModels.DBBackup;

namespace opensisAPI.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("{tenant}/DBBackup")]
    [ApiController]
    public class DBBackupController : ControllerBase
    {
        private IDBbackupService _dBbackupService;
        public IConfiguration _configuration { get; }
        public DBBackupController(IDBbackupService dBbackupService, IConfiguration configuration)
        {
            _dBbackupService = dBbackupService;
            _configuration = configuration;
        }


        [HttpPost("databaseBackup"), DisableRequestSizeLimit]

        public ActionResult<DBBackupViewModel> DatabaseBackup(DBBackupViewModel dBBackupViewModel)
        {
            DBBackupViewModel dBBackup = new DBBackupViewModel();
            try
            {
                dBBackupViewModel.ConnString = _configuration["ConnectionStringTemplateMySQL"].Replace("{tenant}", dBBackupViewModel._tenantName);

                dBBackup = _dBbackupService.DatabaseBackup(dBBackupViewModel);
                if (!dBBackup._failure)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), dBBackup.FileName);
                    if (!System.IO.File.Exists(filePath))
                        return NotFound();
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(filePath, FileMode.Open))
                    {
                        stream.CopyTo(memory);
                    }
                    memory.Position = 0;
                   
                    return File(memory, GetContentType(filePath), dBBackup.FileName);
                }
                else
                {
                    return dBBackup;
                }
            }
            catch (Exception es)
            {
                dBBackup._failure = true;
                dBBackup._message = es.Message;
                return dBBackup;
            }
        }

        private string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;

            if (!provider.TryGetContentType(path, out contentType))
            {
                contentType = "application/octet-stream";
            }
            System.IO.File.Delete(path);
            return contentType;
        }

    }
}
