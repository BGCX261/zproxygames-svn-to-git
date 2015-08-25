using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace DebugExperienceSinglePlayer.Library
{
	public static class MyExtensions
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

		public static List<T> ToListFromType<T>(this T e)
		{
			return new List<T>();
		}

		public static void ReadKeysUntil(this ConsoleKey k, Action<ConsoleKeyInfo> h)
		{
			var c = default(ConsoleKeyInfo);

			do
			{
				c = Console.ReadKey(true);

				if (k != c.Key)
					h(c);

			} while (k != c.Key);
		}

		public static void TryStart(this TcpListener l, Action h)
		{
			try
			{
				l.Start();
			}
			catch
			{
				return;
			}
			h();
		}

		public static void ToListener(this int port, Action<Stream> handler)
		{
			new Thread(
				delegate()
				{
					var r = new TcpListener(IPAddress.Loopback, port);

					r.TryStart(
						delegate
						{

							r.Start();

							while (true)
							{
								var c = r.AcceptTcpClient();

								var s = c.GetStream();

								new Thread(
									delegate()
									{
										handler(s);
									}
								)
								{
									IsBackground = true,
								}.Start();
							}
						}
					);
				}
			)
			{
				IsBackground = true,
			}.Start();

		}
	}
}
