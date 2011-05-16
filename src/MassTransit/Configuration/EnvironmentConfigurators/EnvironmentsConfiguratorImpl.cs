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
namespace MassTransit.EnvironmentConfigurators
{
	using System;
	using System.Collections.Generic;
	using Configuration;
	using Configurators;

	public class EnvironmentsConfiguratorImpl :
		EnvironmentsConfigurator
	{
		readonly IDictionary<string, Func<IServiceBusEnvironment>> _environments;
		string _currentEnvironment;

		public EnvironmentsConfiguratorImpl()
		{
			_environments = new Dictionary<string, Func<IServiceBusEnvironment>>();
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (_currentEnvironment == null)
				yield return this.Failure("Current", "A current environment was not specified.");
		}


		public void Add(string environmentName, Func<IServiceBusEnvironment> environmentFactory)
		{
			_environments[environmentName.ToLowerInvariant()] = () => environmentFactory();
		}

		public void Select(string environmentName)
		{
			_currentEnvironment = environmentName;
		}

		public IServiceBusEnvironment GetCurrentEnvironment()
		{
			ConfigurationResult result = ConfigurationResultImpl.CompileResults(Validate());

			string environment = _currentEnvironment.ToLowerInvariant();

			if (_environments.ContainsKey(environment))
			{
				return _environments[environment]();
			}

			return null;
		}
	}
}