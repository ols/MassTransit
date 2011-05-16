// Copyright 2007-2010 The Apache Software Foundation.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Tests.Serialization
{
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using Context;
    using MassTransit.Serialization;
    using Messages;
    using NUnit.Framework;

    public abstract class GivenASimpleMessage<TSerializer> where TSerializer : IMessageSerializer, new()
    {
        public PingMessage Message { get; private set; }

        [SetUp]
        public void SetUp()
        {
            Message = new PingMessage();
        }

        [Test]
        public void ShouldWork()
        {
            byte[] serializedMessageData;

            var serializer = new TSerializer();

            using (var output = new MemoryStream())
            {
                serializer.Serialize(output, new SendContext<PingMessage>(Message));

                serializedMessageData = output.ToArray();

       //         Trace.WriteLine(Encoding.UTF8.GetString(serializedMessageData));
            }

            using (var input = new MemoryStream(serializedMessageData))
            {
				var receiveContext = new ConsumeContext(input);
				var receivedMessage = serializer.Deserialize(receiveContext) as PingMessage;

                Assert.AreEqual(Message, receivedMessage);
            }
        }
    }
 
    [TestFixture]
    public class WhenUsingTheXmlOnSimpleMessage :
        GivenASimpleMessage<XmlMessageSerializer>
    {
        
    }

    [TestFixture]
    public class WhenUsingTheBinaryOnSimpleMessage :
        GivenASimpleMessage<BinaryMessageSerializer>
    {
        
    }

    [TestFixture]
    public class WhenUsingDotNotXmlOnSimpleMessage :
        GivenASimpleMessage<DotNotXmlMessageSerializer>
    {
        
    }

    [TestFixture]
    public class WhenUsingJsonOnSimpleMessage :
        GivenASimpleMessage<JsonMessageSerializer>
    {
        
    }
}