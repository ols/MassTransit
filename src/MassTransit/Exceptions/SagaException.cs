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
namespace MassTransit.Exceptions
{
    using System;
    using System.Linq.Expressions;

    [Serializable]
    public class SagaException :
        MassTransitException
    {
        readonly Guid _correlationId;
        readonly Type _messageType;
        readonly Type _sagaType;

        public SagaException(string message, Type sagaType, Type messageType, Guid correlationId)
            : base(FormatMessage(sagaType, correlationId, messageType, message))
        {
            _sagaType = sagaType;
            _messageType = messageType;
            _correlationId = correlationId;
        }

        public SagaException(string message, Type sagaType, Type messageType, Expression findExpression)
            : base(string.Format("{0} {1}({2}) - {3}", sagaType.FullName, message, messageType.FullName, findExpression))
        {
            _sagaType = sagaType;
            _messageType = messageType;
        }

        public SagaException(string message, Type sagaType, Type messageType, Expression findExpression, Exception innerException)
            : base(string.Format("{0} {1}({2}) - {3}", sagaType.FullName, message, messageType.FullName, findExpression), innerException)
        {
            _sagaType = sagaType;
            _messageType = messageType;
        }

        public SagaException(string message, Type sagaType, Type messageType, Guid correlationId, Exception innerException)
            : base(FormatMessage(sagaType, correlationId, messageType, message), innerException)
        {
            _sagaType = sagaType;
            _messageType = messageType;
            _correlationId = correlationId;
        }

        public SagaException(string message, Type sagaType, Type messageType)
            : base(FormatMessage(sagaType, messageType, message))
        {
            _sagaType = sagaType;
            _messageType = messageType;
            _correlationId = Guid.Empty;
        }

        public Type SagaType
        {
            get { return _sagaType; }
        }

        public Type MessageType
        {
            get { return _messageType; }
        }

        public Guid CorrelationId
        {
            get { return _correlationId; }
        }

        static string FormatMessage(Type sagaType, Type messageType, string message)
        {
            return string.Format("{0} Saga exception on receipt of {1}: {2}", sagaType.FullName, messageType.FullName, message);
        }

        static string FormatMessage(Type sagaType, Guid correlationId, Type messageType, string message)
        {
            return string.Format("{0}({1}) Saga exception on receipt of {2}: {3}", sagaType.FullName, correlationId, messageType.FullName, message);
        }
    }
}