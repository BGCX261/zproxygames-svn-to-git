using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using java.io;
using java.net;
using javax.servlet.http;
using Bulldog.Server.Library;
using ScriptCoreLib;
using ScriptCoreLibJava.Extensions;
using Bulldog.Server.CodeGenerators.Java;

namespace Bulldog.Server
{
	[Script]
	[ConfigurationProvider.UrlPattern(UrlPattern + "/*")]
	public class JavaCodeGeneratorServlet : HttpServlet
	{
		public const string UrlPattern = "/java";

		protected override void doGet(HttpServletRequest req, HttpServletResponse resp)
		{
			try
			{
				resp.setContentType("text/plain");

				var p = req.GetPathAndQuery();


				resp.getWriter().println(Launch(p.Substring(UrlPattern.Length + 1)));
				resp.getWriter().flush();
			}
			catch (csharp.ThrowableException ex)
			{
				// either swallow of throw a runtime exception

				((java.lang.Throwable)(object)ex).printStackTrace();

			}
		}


		private static string Launch(string PathAndQuery)
		{
			var w = new StringBuilder();



			var p = new DefinitionProvider(PathAndQuery, k => k.ToWebString());

		
			w.Append(p.GetString());


			return w.ToString();
		}
	}
}
