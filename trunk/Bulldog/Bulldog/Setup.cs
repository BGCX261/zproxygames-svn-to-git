using System;
using System.IO;
using Bulldog.Server;
using Bulldog.Server.CodeGenerators.Java;
using Bulldog.Server.Library;
using ScriptCoreLib;
using ScriptCoreLib.Archive.ZIP;
using ScriptCoreLibJava.Extensions;

namespace Bulldog
{
	class Setup
	{
		public const string SettingsFileName = "setup.settings.cmd";

		public static void DefineEntryPoint(IEntryPoint e)
		{
			e["java/WEB-INF/web.xml"] = typeof(DefaultServlet).Assembly.ToServletConfiguration();
		}

		public static void Main(string[] e)
		{
			
			//new DefinitionProvider("org.w3c.dom.Document");

			//new DefinitionProvider("org.w3c.dom.Document");
			//new DefinitionProvider("org.w3c.dom.Node", k => k.ToWebString());
			Console.WriteLine(
			    new DefinitionProvider(
					"javax.swing.JTable"
					//"java.lang.String"
					//"java.lang.reflect.Array"
					//"javax.servlet.ServletInputStream"
					//"javax.servlet.http.Cookie"
					//"javax.servlet.http.HttpServletRequest"
					//"javax.swing.event.MenuListener"
					//"java.awt.geom.Point2D"
					//"javax.swing.ToolTipUI"
					//"javax.swing.JMenuBar"
					//"robocode.BattleRules"
			        //"java.net.InetSocketAddress"
			        //"java.net.ServerSocket"
			        //"java.nio.channels.ServerSocketChannel"
			        , k => k.ToWebString()).GetString()
			);

			//TransformRobocode();
		}

		public static void TransformRobocode()
		{
			var w = new Uri("http://robocode.sourceforge.net/docs/robocode/allclasses-noframe.html").ToWebString();

			var zip = new ZIPFile();

			var o = 0;
			while (o >= 0)
			{
				const string pregfix = "<A HREF=\"";
				var i = w.IndexOf(pregfix, o);
				if (i >= 0)
				{
					var j = w.IndexOf("\"", i + pregfix.Length);

					if (j >= 0)
					{
						var type = w.Substring(i + pregfix.Length, j - (i + pregfix.Length));

						const string suffix = ".html";

						if (type.EndsWith(suffix))
						{
							o = j + 1;

							type = type.Substring(0, type.Length - suffix.Length).Replace("/", ".");

							Console.WriteLine(type);


							zip.Add(type.Replace(".", "/") + ".cs",
								new DefinitionProvider(
									type
								//        "robocode.BattleRules"
								//        //"java.net.InetSocketAddress"
								//        //"java.net.ServerSocket"
								//        //"java.nio.channels.ServerSocketChannel"
								, k => k.ToWebString()).GetString()
							);
							
							
						}
						else o = -1;
					}
					else o = -1;
				}
				else o = -1;
			}

			using (var ww = new BinaryWriter(File.OpenWrite("Robocode.zip")))
			{
				zip.WriteTo(ww);
			}
			
		}

	}
}
