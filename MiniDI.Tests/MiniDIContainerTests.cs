using MiniDI.Tests.SampleCode;
using NUnit.Framework;

namespace MiniDI.Tests
{
    [TestFixture]
    public class MiniDIContainerTests
    {
        [TearDown]
        public void TearDown()
        {
            MiniDIContainer.RemoveAll();
        }

        [Test]
        public void Container_Creates_Instance_Of_Registered_Reference_Type()
        {
            MiniDIContainer.Set<IMailSender, MailSender>();

            var mailSender = MiniDIContainer.Get<IMailSender>();

            Assert.That(mailSender, Is.Not.Null);
            Assert.That(mailSender, Is.InstanceOf<IMailSender>());
            Assert.That(mailSender, Is.InstanceOf<MailSender>());
        }

        [Test]
        public void Container_Creates_Instance_Of_Registered_Value_Type()
        {
            MiniDIContainer.Set<IMailSender, StructWithInterface>();

            var mailSender = MiniDIContainer.Get<IMailSender>();

            Assert.That(mailSender, Is.InstanceOf<IMailSender>());
            Assert.That(mailSender, Is.InstanceOf<StructWithInterface>());
        }

        [Test]
        [ExpectedException(typeof(ResolveException))]
        public void Container_Throws_During_Create_Instance_Of_NonRegistered_Type()
        {
            IMailSender mailSender;
            mailSender = MiniDIContainer.Get<IMailSender>();

            Assert.That(mailSender, Is.Null);
        }

        [Test]
        public void Container_TryGet_Doesnt_Throw_During_Create_Instance_Of_NonRegistered_Type()
        {
            IMailSender mailSender;
            bool result = MiniDIContainer.TryGet(out mailSender);

            Assert.That(result, Is.False);
            Assert.That(mailSender, Is.Null);
        }

        [Test]
        public void Container_Removes_All_Types()
        {
            MiniDIContainer.Set<IMailSender, MailSender>();

            MiniDIContainer.RemoveAll();

            IMailSender mailSender;
            bool result = MiniDIContainer.TryGet(out mailSender);

            Assert.That(result, Is.False);
        }
        [Test]
        public void Singleton_Reference_Object()
        {
            MiniDIContainer.Set<IMailSender, MailSender>(LifeTime.Singleton);


            var ms1 = MiniDIContainer.Get<IMailSender>();
            var ms2 = MiniDIContainer.Get<IMailSender>();

            Assert.That(ms1, Is.SameAs(ms2));
        }

        [Test]
        public void Singleton_ValueType_Object()
        {
            MiniDIContainer.Set<IMailSender, StructWithInterface>(LifeTime.Singleton);


            var ms1 = MiniDIContainer.Get<IMailSender>();
            var ms2 = MiniDIContainer.Get<IMailSender>();

            Assert.That(ms1, Is.EqualTo(ms2));
        }
    }
}
