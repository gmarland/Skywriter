using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace GroupClipboardClient.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ServiceBase[] servicesToRun = new ServiceBase[] 
            {
            };

            ServiceBase.Run(servicesToRun);
        }
    }
}
