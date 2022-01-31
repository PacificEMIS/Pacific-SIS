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
    public class GenerateYapReportCard
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public async Task<string> Generate(object data)
        {
            string msg = "success";
            try
            {
                System.Diagnostics.Debug.WriteLine("Initializing local jsreport.exe utility");
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

                    var studentReport = rs.RenderByNameAsync("YapReportTemplate", data).Result;

                    using (var fs = File.Create("ReportCard\\StudentYapReportCard.pdf"))
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

                    var studentReport = rs.RenderByNameAsync("YapReportTemplate", data).Result;
                    using (var fs = File.Create("ReportCard/StudentYapReportCard.pdf"))
                    {
                        studentReport.Content.CopyTo(fs);
                        fs.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error("Method GenerateYapReportCard end with error :" + ex.Message);
                msg = ex.Message;
            }
            return msg;
        }
    }
}
