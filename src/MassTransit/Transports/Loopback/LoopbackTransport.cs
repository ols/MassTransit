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
namespace MassTransit.Transports
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using Context;
	using Loopback;
	using Magnum.Extensions;
	using Util;

	public class LoopbackTransport :
		IDuplexTransport
	{
		readonly object _messageLock = new object();
		AutoResetEvent _messageReady = new AutoResetEvent(false);
		LinkedList<LoopbackMessage> _messages = new LinkedList<LoopbackMessage>();
		bool _disposed;

		public LoopbackTransport(IEndpointAddress address)
		{
			Address = address;
		}

		public IEndpointAddress Address { get; private set; }

		public IOutboundTransport OutboundTransport
		{
			get { return this; }
		}

		public IInboundTransport InboundTransport
		{
			get { return this; }
		}

		public void Send(ISendContext context)
		{
			GuardAgainstDisposed();

			LoopbackMessage message = null;
			try
			{
				message = new LoopbackMessage();

				if (context.ExpirationTime.HasValue)
				{
					message.ExpirationTime = context.ExpirationTime.Value;
				}

				context.SerializeTo(message.Body);
				message.ContentType = context.ContentType;

				lock (_messageLock)
				{
					GuardAgainstDisposed();

					_messages.AddLast(message);
				}

				if (SpecialLoggers.Messages.IsInfoEnabled)
					SpecialLoggers.Messages.InfoFormat("SEND:{0}:{1}:{2}", Address, context.MessageType, message.MessageId);
			}
			catch
			{
				if (message != null)
					message.Dispose();

				throw;
			}

			_messageReady.Set();
		}

		public void Receive(Func<IReceiveContext, Action<IReceiveContext>> callback, TimeSpan timeout)
		{
			int messageCount;
			lock (_messageLock)
			{
				GuardAgainstDisposed();

				messageCount = _messages.Count;
			}

			bool waited = false;

			if (messageCount == 0)
			{
				if (!_messageReady.WaitOne(timeout, true))
					return;

				waited = true;
			}

			bool monitorExitNeeded = true;
			if (!Monitor.TryEnter(_messageLock, timeout))
				return;

			try
			{
				for (LinkedListNode<LoopbackMessage> iterator = _messages.First; iterator != null; iterator = iterator.Next)
				{
					if (iterator.Value != null)
					{
						LoopbackMessage message = iterator.Value;
						if (message.ExpirationTime.HasValue && message.ExpirationTime <= DateTime.UtcNow)
						{
							_messages.Remove(iterator);
							return;
						}

						ReceiveContext context = ReceiveContext.FromBodyStream(message.Body);
						context.SetMessageId(message.MessageId);
						context.SetContentType(message.ContentType);
						if (message.ExpirationTime.HasValue)
							context.SetExpirationTime(message.ExpirationTime.Value);

						using (context.CreateScope())
						{
							Action<IReceiveContext> receive = callback(context);
							if (receive == null)
								continue;

							_messages.Remove(iterator);

							using (message)
							{
								Monitor.Exit(_messageLock);
								monitorExitNeeded = false;

								receive(context);
								return;
							}
						}
					}
				}

				if (waited)
					return;

				// we read to the end and none were accepted, so we are going to wait until we get another in the queue
				if (!_messageReady.WaitOne(timeout, true))
					return;
			}
			finally
			{
				if (monitorExitNeeded)
					Monitor.Exit(_messageLock);
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		void GuardAgainstDisposed()
		{
			if (_disposed)
				throw new ObjectDisposedException("The transport has already been disposed: " + Address);
		}

		void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				lock (_messageLock)
				{
					_messages.Each(x => x.Dispose());
					_messages.Clear();
					_messages = null;
				}

				_messageReady.Close();
				using (_messageReady)
				{
				}
				_messageReady = null;
			}

			_disposed = true;
		}

		~LoopbackTransport()
		{
			Dispose(false);
		}
	}
}