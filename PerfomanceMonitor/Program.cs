using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniDI;
using MiniDI.Tests.SampleCode;

namespace PerfomanceMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Read();
            Console.WriteLine("Start");
            MiniDIContainer.Set<IMailSender, MailSender>();

            int limit = 10000;
            int cycles = 5;
            var senders = new IMailSender[limit];
            Stopwatch sw = new Stopwatch();

            TimeSpan miniDi = TimeSpan.Zero;
            TimeSpan activator = TimeSpan.Zero;

            for (int j = 0; j < cycles; j++)
            {
                sw.Start();
                for (int i = 0; i < limit; i++)
                {
                    senders[i] = MiniDIContainer.Get<IMailSender>();
                }
                sw.Stop();
                miniDi += sw.Elapsed;


                sw.Restart();
                for (int i = 0; i < limit; i++)
                {
                    senders[i] = Activator.CreateInstance<MailSender>();
                }
                sw.Stop();
                activator += sw.Elapsed;

            }
            Console.WriteLine("{0,-30} {1}", "MiniDI", TimeSpan.FromMilliseconds(miniDi.TotalMilliseconds / cycles));
            Console.WriteLine("{0,-30} {1}", "Activator", TimeSpan.FromMilliseconds(activator.TotalMilliseconds / cycles));

            Console.Read();
            Console.Read();
        }
    }
}
