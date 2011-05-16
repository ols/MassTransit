﻿// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Serialization
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Remoting.Messaging;

	public static class ExtensionsForBinaryMessageSerializer
	{
		public static void Add(this List<Header> headers, string key, Uri uri)
		{
			if (uri == null)
				return;

			headers.Add(new Header(key, uri));
		}

		public static void Add(this List<Header> headers, string key, string value)
		{
			if (string.IsNullOrEmpty(value))
				return;

			headers.Add(new Header(key, value));
		}

		public static void Add(this List<Header> headers, string key, int value)
		{
			if (value == 0)
				return;

			headers.Add(new Header(key, value));
		}

		public static void Add(this List<Header> headers, string key, DateTime value)
		{
			headers.Add(new Header(key, value));
		}
	}
}