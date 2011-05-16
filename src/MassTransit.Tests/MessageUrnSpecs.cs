namespace MassTransit.Tests
{
    using System;
    using NUnit.Framework;
    using TestFramework.Examples.Messages;

    [TestFixture]
    public class MessageUrnSpecs
    {
        
        [Test]
        public void SimpleMessage()
        {
            var urn = new MessageUrn(typeof (Ping));
            Assert.AreEqual(urn.AbsolutePath, "message:MassTransit.TestFramework.Examples.Messages:Ping");
        }

        [Test]
        public void NestedMessage()
        {
            var urn = new MessageUrn(typeof (X));
            Assert.AreEqual(urn.AbsolutePath, "message:MassTransit.Tests:MessageUrnSpecs+X");
        }

        [Test]
        public void OpenGenericMessage()
        {
            var urn = new MessageUrn(typeof (G<>));
            var expected = new Uri("urn:message:MassTransit.Tests:G[[]]");
            Assert.AreEqual(expected.AbsolutePath, urn.AbsolutePath);
        }

        [Test]
        public void ClosedGenericMessage()
        {
            var urn = new MessageUrn(typeof (G<Ping>));
            var expected = new Uri("urn:message:MassTransit.Tests:G[[MassTransit.TestFramework.Examples.Messages:Ping]]");
            Assert.AreEqual(expected.AbsolutePath,urn.AbsolutePath) ;
        }

        class X{}
    }
    public class G<T>{}
}