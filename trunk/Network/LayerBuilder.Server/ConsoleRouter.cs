using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LayerBuilder.Server.Library;
using System.IO;
using System.Collections;
using ScriptCoreLib.Reflection.Options;

namespace LayerBuilder.Server
{
	public abstract class ConsoleRouter
	{
		// This class shall serve as a router
		// both in .net an jvm via jsc.meta

		void ShowVersion()
		{
			Console.WriteLine("version 1");
			Console.WriteLine("This application uses LayerBuilder.Server marked as [assembly: Obfuscation(Feature = \"script\")]");

		}


		public string[] Arguments;

		public delegate void WriterDelegate(BinaryWriter w);
		public delegate void BroadcastDelegate(WriterDelegate w);

		public class DispatcherArguments
		{
			public int LocalIndex;

			public byte Operation;

			public BinaryReader Reader;

			public BroadcastDelegate Broadcast;
		}

		protected abstract void Dispatcher(DispatcherArguments arg);


		public void Invoke()
		{
			ShowVersion();

			var Arguments = new ConsoleRouterArguments();

			if (this.Arguments == null)
				throw new NotSupportedException("Arguments null");

			this.Arguments.AsParametersTo(Arguments);

			Console.WriteLine("starting server... port: " + Arguments.ServerPort);

			var Clients = new ArrayList();

			var BroadcastLock = new object();

			Arguments.ServerPort.ToListener(
				s =>
				{
					Console.WriteLine("new connection at " + Arguments.ServerPort);

					// lock Clients
					var a = new DispatcherArguments
					{
						Reader = new BinaryReader(s),
						LocalIndex = Clients.Add(new BinaryWriter(s)),
					};

					#region Broadcast
					a.Broadcast =
						h =>
						{
							// why start broadcasting without content?
							if (h == null)
								return;

							var others = new ArrayList();

							// lock Clients
							for (int i = 0; i < Clients.Count; i++)
							{
								if (i != a.LocalIndex)
									if (Clients[i] != null)
										h((BinaryWriter)Clients[i]);
							}

						};
					#endregion

					var Operation = s.ReadByte();

					while (Operation >= 0)
					{
						a.Operation = (byte)Operation;

						//Console.WriteLine("Operation: " + a.Operation);

						Dispatcher(a);

						//Console.WriteLine("Operation: " + a.Operation + " broadcasted!");

						Operation = s.ReadByte();
					}
				}
			);

			if (Arguments.Trunk != null)
				foreach (var t in Arguments.Trunk)
				{
					Console.WriteLine("trunk: " + t.TrunkPort);
				}
			Console.WriteLine("press enter to exit!");
			Console.ReadLine();
		}
	}
}
