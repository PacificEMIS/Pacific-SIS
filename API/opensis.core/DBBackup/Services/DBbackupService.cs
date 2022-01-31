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

using opensis.core.DBBackup.Interfaces;
using opensis.core.helper.Interfaces;
using opensis.data.Interface;
using opensis.data.ViewModels.DBBackup;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.core.DBBackup.Services
{
    public class DBbackupService : IDBbackupService
    {
        //private static string SUCCESS = "success";
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string TOKENINVALID = "Token not Valid";

       // public IdbbackupRepository dbbackupRepository;
        public ICheckLoginSession tokenManager;
        public IdbbackupRepository dbbackupRepository;
        public DBbackupService(/*IdbbackupRepository dbbackupRepository*/ IdbbackupRepository dbbackupRepository, ICheckLoginSession checkLoginSession)
        {
            // this.dbbackupRepository = dbbackupRepository;
            this.dbbackupRepository = dbbackupRepository;
            this.tokenManager = checkLoginSession;
        }

        //Required for Unit Testing
        public DBbackupService() { }

        /// <summary>
        /// Database Backup
        /// </summary>
        /// <param name="dBBackupViewModel"></param>
        /// <returns></returns>
        public DBBackupViewModel DatabaseBackup(DBBackupViewModel dBBackupViewModel)
        {
            logger.Info("Method DatabaseBackup called.");
            DBBackupViewModel dBBackup = new DBBackupViewModel();
            try
            {
                if (tokenManager.CheckToken(dBBackupViewModel._tenantName + dBBackupViewModel._userName, dBBackupViewModel._token))
                {
                    string rootDirectory = System.IO.Directory.GetCurrentDirectory();
                    logger.Info("Method DatabaseBackup rootDirectory:" + rootDirectory);
                    //dBBackup = this.dbbackupRepository.DatabaseBackup(dBBackupViewModel);
                    dBBackup = this.dbbackupRepository.DatabaseBackup(dBBackupViewModel);
                    logger.Info("Method DatabaseBackup end with :" + dBBackup._message);
                }
                else
                {
                    dBBackup._failure = true;
                    dBBackup._message = TOKENINVALID;
                }
            }
            catch (Exception es)
            {
                dBBackup._failure = true;
                dBBackup._message = es.Message;
                logger.Error("Method DatabaseBackup end with error :" + es.Message);
            }
            return dBBackup;
        }
       
    }
}
