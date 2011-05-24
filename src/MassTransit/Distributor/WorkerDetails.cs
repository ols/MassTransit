// Copyright 2007-2008 The Apache Software Foundation.
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
	using System;
	using System.Threading;
	using Magnum;

	public class WorkerDetails
	{
		public int InProgress;
		public int InProgressLimit;
		public Uri ControlUri { get; set; }
		public Uri DataUri { get; set; }
		public int Pending { get; set; }
		public int PendingLimit { get; set; }

		public DateTime LastUpdate { get; set; }

		public void Add()
		{
			Interlocked.Increment(ref InProgress);

			LastUpdate = SystemUtil.UtcNow;
		}

		public void UpdateInProgress(int inProgress, int inProgressLimit, int pending, int pendingLimit, DateTime updated)
		{
			lock (this)
			{
				InProgress = inProgress;
				InProgressLimit = inProgressLimit;

				Pending = pending;
				PendingLimit = pendingLimit;
			}
		}
	}
}