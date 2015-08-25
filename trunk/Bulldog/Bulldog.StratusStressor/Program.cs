using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Diagnostics;

namespace Bulldog.StratusStressor
{
	class Program
	{
		static void Main(string[] args)
		{

			//http://groups.google.com/group/google-appengine-java/browse_thread/thread/9464bcd52f1e2ced/bb1c8e87888fb4bb?lnk=raot

			for (int i = 0; i < 4; i++)
			{
				new Thread(Spawn).Start();
			}
			Console.ReadKey(true);
		}

		private static void Spawn()
		{

			var c = new WebClient();
			var o = default(string);
			var r = new Random().Next();
			for (int i = 0; i < 10000; i++)
			{
				var w = new Stopwatch();
				w.Start();
				try
				{

					//o = c.DownloadString("http://zproxy.appspot.com/stratus?stressor=" + i + "&random=" + r);
					o = c.DownloadString("http://2.latest.jsc-project.appspot.com/?stressor=" + i + "&random=" + r);
				
					Console.ForegroundColor = ConsoleColor.Green;
				}
				catch
				{
					Console.ForegroundColor = ConsoleColor.Red;
				}
				w.Stop();
				Console.WriteLine(i + " in " + w.Elapsed + " bytes " + o.Length);
			}
		}
	}
}
