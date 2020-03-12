using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using log4net;
using log4net.Config;

namespace BTS_Services
{
    class log4net
    {        
        private static ILog log = LogManager.GetLogger("RollingFile");

        private static string AppPath = AppDomain.CurrentDomain.BaseDirectory;        

        public static void OnstartLog()
        {
            XmlConfigurator.Configure(new System.IO.FileInfo(AppPath + @"bin\App.config"));
            log.Info("============== Started BizTalkService HostInstance Restart ==============");
        }

        public static void SetwriteLog(string type, string message)
        {
            XmlConfigurator.Configure(new System.IO.FileInfo(AppPath + @"bin\App.config"));
            switch (type)
            {
                case "info":
                    log.Info(message);
                    break;
                case "debug":
                    log.Debug(message);
                    break;
                case "warn":
                    log.Warn(message);
                    break;
                case "error":
                    log.Error(message);
                    break;
                case "fatal":
                    log.Fatal(message);
                    break;
                default:
                    break;
            }            
        }

        public static void OnendLog()
        {
            XmlConfigurator.Configure(new System.IO.FileInfo(AppPath + @"bin\App.config"));
            log.Info("============== End BizTalkService HostInstance Restart ==============");
        }
    }
}
