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

using jsreport.Binary;
using jsreport.Local;
using jsreport.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JSReport
{
    public class GenerateTranscript
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public async Task<string> Generate(object data)
        {
            string msg = "success";
            try
            {
                System.Diagnostics.Debug.WriteLine("Initializing local jsreport.exe utility");

                Console.WriteLine("Initializing local jsreport.exe utility");
                bool isWindows = System.Runtime.InteropServices.RuntimeInformation
                                           .IsOSPlatform(OSPlatform.Windows);
                if (isWindows)
                {
                    var rs = new LocalReporting()
                        .RunInDirectory(Path.Combine(Directory.GetCurrentDirectory(), "jsreport"))
                       .KillRunningJsReportProcesses()
                  .UseBinary(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                      jsreport.Binary.JsReportBinary.GetBinary() :
                      jsreport.Binary.Linux.JsReportBinary.GetBinary())
                  .Configure(cfg =>
                  {
                      //  cfg.TemplatingEngines.Timeout = 26000;
                      cfg.HttpPort = 1000;
                      cfg.AllowedLocalFilesAccess().FileSystemStore().BaseUrlAsWorkingDirectory();
                      cfg.Chrome = new ChromeConfiguration { Timeout = 10000 };
                      return cfg;
                  })
                  .AsUtility()
                  .Create();

                    var studentReport = rs.RenderByNameAsync("TranscriptTemplate", data).Result;

                    using (var fs = File.Create("ReportCard\\StudentTranscript.pdf"))
                    {
                        studentReport.Content.CopyTo(fs);
                        fs.Close();
                    }
                }
                else
                {
                    var rs = new LocalReporting()
                         .TempDirectory("/var/www/opensisapi/temp")
                         .RunInDirectory(Path.Combine(Directory.GetCurrentDirectory(), "jsreport"))
                        .KillRunningJsReportProcesses()
                   .UseBinary(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                       jsreport.Binary.JsReportBinary.GetBinary() :
                       jsreport.Binary.Linux.JsReportBinary.GetBinary())
                   .Configure(cfg =>
                   {
                       //  cfg.TemplatingEngines.Timeout = 26000;
                       cfg.HttpPort = 1000;
                       cfg.AllowedLocalFilesAccess().FileSystemStore().BaseUrlAsWorkingDirectory();
                       cfg.Chrome = new ChromeConfiguration { Timeout = 10000 };
                       return cfg;
                   })
                   .AsUtility()
                   .Create();

                    var studentReport = rs.RenderByNameAsync("TranscriptTemplate", data).Result;

                    using (var fs = File.Create("ReportCard/StudentTranscript.pdf"))
                    {
                        studentReport.Content.CopyTo(fs);
                        fs.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Method GenerateTranscript end with error :" + ex.Message);
                msg = ex.Message;
            }
            return msg;
        }
    }

}
