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
namespace MassTransit.Distributor
{
    using System.Collections.Generic;
    using System.Linq;

    public class DefaultWorkerSelectionStrategy<TMessage> :
        IWorkerSelectionStrategy<TMessage>
        where TMessage : class
    {
        public bool HasAvailableWorker(IEnumerable<WorkerDetails> candidates, TMessage message)
        {
            return candidates
                .Where(x => x.InProgress + x.Pending < x.InProgressLimit + x.PendingLimit)
                .Any();
        }

        public WorkerDetails SelectWorker(IEnumerable<WorkerDetails> candidates, TMessage message)
        {
            return candidates
                .Where(x => x.InProgress + x.Pending < x.InProgressLimit + x.PendingLimit)
                .OrderBy(x => x.InProgress + x.Pending)
                .ThenByDescending(x => x.LastUpdate)
                .FirstOrDefault();
        }
    }
}