using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniDI;

namespace TestApp
{
    public class Program
    {
        private static void Main(string[] args)
        {
            MiniDIContainer.Set<IMailSender, MailSender>();
            MiniDIContainer.Set<IService, Service>();
            MiniDIContainer.Set<IDataReader, SqlReader>();
            MiniDIContainer.Set<IStorage, Storage>();

            CheckTime(() =>
                          {
                             // for (int i = 0; i < 1000; i++)
                              try
                              {
                                  var service = MiniDIContainer.Get<IService>();
                                  service.Do();
                              }
                              catch(ResolveException e)
                              {
                                  Console.WriteLine(e);
                              }
                          });

            Console.Read();
        }
        static void CheckTime(Action a)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            a();

            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }
    }

    internal interface IMailSender
    {
        void Send();
    }
    internal interface IDataReader
    {
        void Read();
    }
    internal interface IStorage
    {
        void Open();
    }
    internal interface IService
    {
        void Do();
    }

    internal class Service : IService
    {
        private readonly IMailSender _sender;
        private readonly IDataReader _reader;
        private int i = 0;
        private string f;

        public Service(IMailSender sender, IDataReader reader)
        {
            _sender = sender;
            _reader = reader;
            i++;
            //    f = s;
        }

        //public SomeWorker()
        //{
        //    i++;
        //}

        public void Do()
        {
         //   Console.WriteLine("Do! + {0}", i);
            _sender.Send();
            _reader.Read();
        }
    }

    internal class MailSender : IMailSender
    {
        public void Send()
        {
         //   Console.WriteLine("Mail sent!");
        }
    }

    internal class SqlReader : IDataReader
    {
        private readonly IStorage _storage;

        public SqlReader(StreamReader sr)
        {
            //_storage = storage;
        }

        [Injected]
        public SqlReader(IStorage storage)
        {
            _storage = storage;
        }

        public void Read()
        {
            _storage.Open();
          //  Console.WriteLine("Data read!");
        }
    }
    internal class Storage : IStorage
    {
        public void Open()
        {
           // Console.WriteLine("Storage opened!");
        }
    }
}