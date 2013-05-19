using System;
using System.Diagnostics;
using System.IO;

namespace MiniDI.Tests.SampleCode
{
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

    internal struct StructWithInterface : IMailSender
    {
        public void Send()
        {
            Console.WriteLine("SEND");
        }
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

        public void Do()
        {
            Console.WriteLine("Do! + {0}", i);
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