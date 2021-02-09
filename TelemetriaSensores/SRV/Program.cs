using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SRV
{
    static class Program
    {
        public static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public static string strUrl { get { return ConfigurationManager.AppSettings["URL"]; } }
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new srvTelemetria(),new srvTelegram()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
