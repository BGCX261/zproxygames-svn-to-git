using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Threading;

namespace LayerBuilder.Server.Library
{
	public static class Extensions
	{
		public static void AtInterval(this int e, Action h)
		{
			e.AtDelay(
				delegate
				{
					h();

					AtInterval(e, h);
				}
			);
		}

		public static void AtDelay(this int e, Action h)
		{
			new Thread(
				delegate()
				{
					if (e > 0)
						Thread.Sleep(e);
					h();
				}
			) { IsBackground = true }.Start();
		}

		public static void TryStart(this TcpListener l, Action h)
		{
			Console.WriteLine("enter TryStart");

			try
			{
				l.Start();
				h();
			}
			catch
			{
				Console.WriteLine("error!");
			}
			Console.WriteLine("exit TryStart");

		}

		public delegate void StreamAction(Stream s);

		public static void ToListener(this int port, StreamAction handler)
		{
			//Console.WriteLine("#1");
			new Thread(
				delegate()
				{
					//Console.WriteLine("#2");
					var r = new TcpListener(IPAddress.Loopback, port);
					//Console.WriteLine("#3");

					r.TryStart(
						delegate
						{
							//Console.WriteLine("#4");


							while (true)
							{
								var c = r.AcceptTcpClient();
								//Console.WriteLine("#5");

								var s = c.GetStream();

								new Thread(
									delegate()
									{
										//Console.WriteLine("#6");

										handler(s);
									}
								)
								{
									IsBackground = true,
								}.Start();

								//Console.WriteLine("#7");

							}
						}
					);
				}
			)
			{
				IsBackground = true,
			}.Start();

		}

		public static void ToConsole(this string text)
		{
			Console.WriteLine(text);
		}

		public static byte[] GetBytesFromPort(this string host, int port, int count)
		{
			var bytes = default(byte[]);

			using (var tcp = new TcpClient())
			{
				tcp.Connect(host, port);
				var r = new BinaryReader(tcp.GetStream());
				bytes = r.ReadBytes(count);
				tcp.Close();
			}

			return bytes;
		}
	}

}
