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
        public void Container_Creates_Instance_Of_Registered_Type()
        {
            MiniDIContainer.Set<IMailSender, MailSender>();

            var mailSender = MiniDIContainer.Get<IMailSender>();

            Assert.That(mailSender, Is.Not.Null);
            Assert.That(mailSender, Is.InstanceOf<IMailSender>());
            Assert.That(mailSender, Is.InstanceOf<MailSender>());
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
        public void Container_Doesnt_Throw_During_Create_Instance_Of_NonRegistered_Type()
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
    }
}
