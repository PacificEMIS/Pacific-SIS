using jsreport.Binary;
using jsreport.Binary.Linux;
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
  public  class GenerateChuukReportCard
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public async Task<string> Generate(object data)
        {
            string msg = "success";
            try
            {
                System.Diagnostics.Debug.WriteLine("Initializing local jsreport.exe utility");

                Console.WriteLine("Initializing local jsreport.exe utility");

                //var rs = new LocalReporting()
                //    .RunInDirectory(Path.Combine(Directory.GetCurrentDirectory(), "jsreport"))
                //    .KillRunningJsReportProcesses()
                //    .UseBinary(jsreport.Binary.JsReportBinary.GetBinary())
                //    .Configure(cfg => cfg.AllowedLocalFilesAccess().FileSystemStore().BaseUrlAsWorkingDirectory())
                //    .AsUtility()
                //    .Create();
                bool isWindows = System.Runtime.InteropServices.RuntimeInformation
                                             .IsOSPlatform(OSPlatform.Windows);
                if (isWindows)
                {                   
                    var rsa = new LocalReporting()
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

                    Console.WriteLine("Rendering localy stored template jsreport/data/templates/Invoice into invoice.pdf");
                    var studentReport = rsa.RenderByNameAsync("ChuukReportTemplate", data).Result;

                    using (var fs = File.Create("ReportCard\\StudentChuukReportCard.pdf"))
                    {
                        studentReport.Content.CopyTo(fs);
                        fs.Close();
                    }
                }
                else
                {
                    var rsa = new LocalReporting()
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

                    var studentReport = rsa.RenderByNameAsync("ChuukReportTemplate", data).Result;

                    using (var fs = File.Create("ReportCard/StudentChuukReportCard.pdf"))
                    {
                        studentReport.Content.CopyTo(fs);
                        fs.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Method AddReportCard end with error :" + ex.Message);
                msg = ex.Message;
            }
            return msg;
        }
    }
}
