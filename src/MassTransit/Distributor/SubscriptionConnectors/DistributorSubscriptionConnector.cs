// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Distributor.SubscriptionConnectors
{
	using System;
	using MassTransit.Pipeline;
	using MassTransit.SubscriptionConnectors;
	using Pipeline;

	public class DistributorSubscriptionConnector<TMessage> :
		InstanceSubscriptionConnector
		where TMessage : class
	{
		public Type MessageType
		{
			get { return typeof (TMessage); }
		}

		public UnsubscribeAction Connect(IInboundPipelineConfigurator configurator, object instance)
		{
			var distributor = instance as IDistributor<TMessage>;
			var sink = new DistributorMessageSink<TMessage>(message =>
				{
					// rock it
					return distributor.Accept(message) ? (Action<TMessage>) distributor.Consume : null;
				});

			return configurator.Pipeline.ConnectToRouter(sink, () => configurator.SubscribedTo<TMessage>());
		}
	}
}