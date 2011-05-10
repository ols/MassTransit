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
namespace Starbucks.Customer
{
	using System;
	using System.IO;
	using System.Windows.Forms;
	using log4net.Config;
	using MassTransit;
	using StructureMap;

	internal static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			XmlConfigurator.Configure(new FileInfo("customer.log4net.xml"));

			IContainer c = BootstrapContainer();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new OrderDrinkForm(c.GetInstance<IServiceBus>()));
		}

		private static IContainer BootstrapContainer()
		{
			var container = new Container(x => { x.AddType(typeof (OrderDrinkForm)); });

			container.Configure(cfg =>
				{
					cfg.For<IServiceBus>().Use(context => ServiceBusFactory.New(sbc =>
						{
							sbc.ReceiveFrom("rabbitmq://localhost/starbucks_customer");
							sbc.UseRabbitMq();
							sbc.UseRabbitMqRouting();

							sbc.Subscribe(subs => { subs.LoadFrom(container); });
						}));
				});

			return container;
		}
	}
}