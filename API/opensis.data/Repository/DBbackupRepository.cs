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

using MySql.Data.MySqlClient;
using opensis.data.Interface;
using opensis.data.ViewModels.DBBackup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opensis.data.Repository
{
    public class DBbackupRepository : IdbbackupRepository
    {
        public DBBackupViewModel DatabaseBackup(DBBackupViewModel dBBackupViewModel)
        {
            try
            {
                string rootDirectory = Directory.GetCurrentDirectory();

                //string fileName = "DBBackup.sql";
                string fileName = "DBBackup_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".sql";
                string filePath = Path.Combine(rootDirectory, fileName);

                if (!File.Exists(filePath))
                {
                    using (MySqlConnection conn = new MySqlConnection(dBBackupViewModel.ConnString))
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            using (MySqlBackup mb = new MySqlBackup(cmd))
                            {
                                cmd.Connection = conn;
                                conn.Open();
                                mb.ExportToFile(filePath);
                                conn.Close();
                                dBBackupViewModel.FileName = Path.GetFileName(filePath);
                                dBBackupViewModel.ConnString = null;
                                dBBackupViewModel._message = "Database backup sucessfully done.";
                                dBBackupViewModel._failure = false;

                            }
                        }
                    }
                }
            }
            catch (Exception es)
            {
                dBBackupViewModel._failure = true;
                dBBackupViewModel._message = es.Message;
            }
            return dBBackupViewModel;
        }
    }
}
