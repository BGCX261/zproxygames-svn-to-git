using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using DebugExperience.Library;
using System.Net;
using System.IO;
using DebugExperienceSinglePlayer;
using DebugExperienceSinglePlayer.Library;

namespace DebugExperience
{
	class Program
	{
		

		static void Main(string[] args)
		{
			// we need to connect to cloud
			// the cloud shall act as a router

			const int AtAdministration = 800;
			const int AtIdentityIndex = 1000;
			const int AtJoin = 1001;



			// if the router is not up we ought to create one

			var port = 33333;

			var RouterClients = new
			{
				Index = default(int),
				Reader = default(BinaryReader),
				Writer = default(BinaryWriter),

				AtIdentityIndex = default(Action<int>),
				AtJoin = default(Action<int>),

				AtUserInput = default(Action<int, int>),
				AtMove = default(Action<int, int, int>),
			}.ToListFromType();

			RouterClients.Add(null);

			port.ToListener(
				s =>
				{
					//Console.WriteLine("router: client connected to router");

					var Writer = new BinaryWriter(s);



					var RouterClient = new
					{
						Index = RouterClients.Count,
						Reader = new BinaryReader(s),
						Writer = Writer,

						AtIdentityIndex =
							 Writer.AsBuilder()
								.Int32(AtAdministration)
								.Int32(AtIdentityIndex)
								.Int32()
							.ToAction(),

						AtJoin =
							 Writer.AsBuilder()
								.Int32(AtAdministration)
								.Int32(AtJoin)
								.Int32()
							.ToAction(),

						AtUserInput =
							 Writer.AsBuilder()
								.Int32(ProgramDefinition.TokenAtUserInput)
								.Int32()
								.Int32()
							.ToAction(),

							
						AtMove =
							 Writer.AsBuilder()
								.Int32(ProgramDefinition.TokenAtMove)
								.Int32()
								.Int32()
								.Int32()
							.ToAction()
					};

					var Others = RouterClients.Except(new[] { RouterClient, null });

					// 0 means server to client communication

					RouterClient.AtIdentityIndex(RouterClient.Index);

					lock (RouterClient)
					{
						RouterClients.Add(RouterClient);

						foreach (var other in Others)
						{
							other.AtJoin(RouterClient.Index);
							RouterClient.AtJoin(other.Index);
						}
					}

					while (true)
					{
						var Token = RouterClient.Reader.ReadInt32();

						Thread.Sleep(500);

						#region AtUserInput
						if (Token == ProgramDefinition.TokenAtUserInput)
						{
							var Parameter = RouterClient.Reader.ReadInt32();

							foreach (var other in Others)
								other.AtUserInput(RouterClient.Index, Parameter);
						}
						#endregion
						#region AtMove
						else if (Token == ProgramDefinition.TokenAtMove)
						{
							var Parameter_x = RouterClient.Reader.ReadInt32();
							var Parameter_y = RouterClient.Reader.ReadInt32();

							foreach (var other in Others)
								other.AtMove(RouterClient.Index, Parameter_x, Parameter_y);
						}
						#endregion
						else
						{
							Console.WriteLine("router: unknown token: " + Token);
						}
					}
				}
			);

			var c = new TcpClient();

			c.Connect(IPAddress.Loopback, port);


			var a = new ProgramDefinition
			{
				Identity = new ProgramDefinition.EndPointIdentity()
			};

			Console.Title = "frame 1";

			{
				var Reader = new BinaryReader(c.GetStream());
				var Writer = new BinaryWriter(c.GetStream());

				#region AtUserInput
				var AtUserInput = a.AtUserInput;

				a.AtUserInput =
					(sender, data) =>
					{
						Writer.Write((int)ProgramDefinition.TokenAtUserInput);
						Writer.Write((int)data);

						AtUserInput(sender, data);
					};
				#endregion

				#region AtMove
				var AtMove = a.AtMove;

				a.AtMove =
					(sender, x, y) =>
					{
						Writer.Write((int)ProgramDefinition.TokenAtMove);
						Writer.Write((int)x);
						Writer.Write((int)y);

						AtMove(sender, x, y);
					};
				#endregion

				var Peers = new List<ProgramDefinition.EndPointIdentity>();

				0.AtDelay(
					delegate
					{
						while (c.Connected)
						{
							var Token = Reader.ReadInt32();

							if (Token == AtAdministration)
							{
								var Operation = Reader.ReadInt32();

								#region AtIdentityIndex
								if (Operation == AtIdentityIndex)
								{
									var Parameter = Reader.ReadInt32();

									a.Identity.Index = Parameter;

									Console.WriteLine("identity " + Parameter);
								}
								#endregion
								#region AtJoin
								else if (Operation == AtJoin)
								{
									var Parameter = Reader.ReadInt32();

									Console.WriteLine("join " + Parameter);

									Peers.Add(new ProgramDefinition.EndPointIdentity { Index = Parameter });
								}
								#endregion

							}
							#region AtUserInput
							else if (Token == ProgramDefinition.TokenAtUserInput)
							{
								var sender = Reader.ReadInt32();
								var data = Reader.ReadInt32();


								AtUserInput(Peers.Single(k => k.Index == sender), (char)data);
							}
							#endregion
							#region AtUserInput
							else if (Token == ProgramDefinition.TokenAtMove)
							{
								var sender = Reader.ReadInt32();
								var x = Reader.ReadInt32();
								var y = Reader.ReadInt32();


								AtMove(Peers.Single(k => k.Index == sender), x, y);
							}
							#endregion
							else
							{
								// some user token
								Console.WriteLine("client: unknown token: " + Token);
							}

						}
					}
				);

				1000.AtInterval(
					delegate
					{
						a.Identity.Frame++;
						Console.Title = "frame " + a.Identity.Frame;
					}
				);
			}

			a.GatherUserInput();

			c.Close();
		}
	}




}
