using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceProcess;
using System.Configuration;
using BTS_Services.BiztalkServerConfig;

namespace BTS_Services
{
    class Services
    {
        public static void ExecuteProgram()
        {
            log4net.OnstartLog();

            string serviceName = string.Empty;
            string[] serviceLists = null;

            try
            {
                string services = ConfigurationSettings.AppSettings["serviceLists"].ToString();
                int timeoutMilliseconds =
                    (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["timeoutMilliseconds"].ToString())) ? int.Parse(ConfigurationSettings.AppSettings["timeoutMilliseconds"].ToString()) : 0;


                if (!string.IsNullOrEmpty(services))
                {
                    log4net.SetwriteLog("info", "ServiceLists: " + services + ", Timeout: " + timeoutMilliseconds);
                    if (services.IndexOf('|') > -1)
                        serviceLists = services.Split('|');
                    else
                        serviceLists = new string[1] { services };


                    if (serviceLists != null)
                    {
                        foreach (string item in serviceLists)
                        {
                            //serviceName = "BizTalk Service BizTalk Group : " + item;
                            string[] hostList = null;
                            if (item.Equals("ALL"))
                                hostList = BTSHostUtil.GetALLData();
                            else
                                hostList = BTSHostUtil.GetSearchData(item);

                            if (hostList != null)
                            {
                                for (int i = 0; i < hostList.Length; i++)
                                {
                                    serviceName = "BizTalk Service BizTalk Group : " + hostList[i];

                                    log4net.SetwriteLog("info", "service name: " + serviceName);
                                    Services.RestartService(serviceName, timeoutMilliseconds);
                                }
                            }
                        }
                    }
                    else
                        log4net.SetwriteLog("warn", "Service Not Exists List");
                }
                else
                {
                    log4net.SetwriteLog("warn", "ServiceLists Not Setting Value!! ");
                }

                log4net.OnendLog();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    log4net.SetwriteLog("error", "Program-InnerException: " + ex.InnerException.Message);
                }

                log4net.SetwriteLog("error", "Program-Exception: " + ex.Message);
            }
        }

        public static void RestartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                if (service != null)
                {
                    string status = service.Status.ToString();

                    int millisec1 = Environment.TickCount;
                    TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                    log4net.SetwriteLog("info", "Service Status: " + status);

                    if (!string.IsNullOrEmpty(status) && !status.Equals("Stopped"))
                    {
                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                        log4net.SetwriteLog("info", "Service Stop Complete!!");
                    }

                    // count the rest of the timeout
                    int millisec2 = Environment.TickCount;
                    timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                    log4net.SetwriteLog("info", "Service Start Complete!!");
                }
                else
                {
                    log4net.SetwriteLog("warn", "Service didn't exists, Check Service");
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    log4net.SetwriteLog("error", "RestartService-InnerException: " + ex.InnerException.Message);
                }

                log4net.SetwriteLog("error", "RestartService-Exception: " + ex.Message);
            }
        }

    }
}
