using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SRV
{
    static class Program
    {
        public static readonly Logger logger = LogManager.GetCurrentClassLogger();
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new srvChatBot()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
