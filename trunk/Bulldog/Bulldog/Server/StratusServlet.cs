using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Bulldog.Promotion;
using Bulldog.Server.Library;
using javax.servlet.http;
using ScriptCoreLib;
using ScriptCoreLib.Archive.ZIP;
using ScriptCoreLib.YAML;
using ScriptCoreLibJava.AppEngine.API.memcache;
using ScriptCoreLibJava.AppEngine.Extensions.memcache;
using ScriptCoreLibJava.Extensions;

namespace Bulldog.Server
{
	[Script]
	[ConfigurationProvider.UrlPattern(UrlPattern + "/*")]
	public class StratusServlet : HttpServlet
	{
		public const string UrlPattern = "/stratus";

		[Script, Serializable]
		public class Tag
		{
			public string PathAndQuery;
			public long Index;
		}


		// http://www.adobe.com/devnet/articles/crossdomain_policy_file_spec.html#site-control
		// http://board.flashkit.com/board/showthread.php?t=782484

		protected override void doGet(HttpServletRequest req, HttpServletResponse resp)
		{
			try
			{
				var p = req.GetPathAndQuery();

				if (p.Length > UrlPattern.Length + 1)
					p = p.Substring(UrlPattern.Length + 1);
				else
					p = "";

				if ("zip" == p)
				{
					resp.setContentType(ZIPFile.ContentType);
					resp.getOutputStream().write((sbyte[])(object)GetContent());
					resp.getOutputStream().flush();
				}
				else
				{
					resp.setContentType("text/html");
					resp.getWriter().println(Launch(p));
					resp.getWriter().flush();
				}
			}
			catch (csharp.ThrowableException ex)
			{
				// either swallow of throw a runtime exception

				((java.lang.Throwable)(object)ex).printStackTrace();

			}
		}

		private static byte[] GetContent()
		{
			var z = new ZIPFile();

			var c = 0;

			foreach (Tag k in new MemcacheCollection { ElementType = typeof(Tag) })
			{
				c++;
				z.Add("file-" + c + ".txt", k.PathAndQuery);
			}

			return z.ToBytes();
		}

		private static string Launch(string PathAndQuery)
		{
			var w = new StringBuilder();

			w.AppendLine((UrlPattern).ToLink(typeof(Tag).FullName));
			w.AppendLine("<hr />");
			w.AppendLine((UrlPattern + "/zip").ToLink(typeof(Tag).FullName));

			w.AppendLine("<hr />");

			var c = new MemcacheCollection
			{
				ElementType = typeof(Tag),
				ElementExpiationInSeconds = 30
			};

			if (!string.IsNullOrEmpty(PathAndQuery.Trim()))
			{
				c.Add(
					new Tag
					{
						Index = c.Generation,
						PathAndQuery = PathAndQuery
					}
				);
			}

			w.AppendLine("Cache contains " + c.Count + " elements!");
			w.AppendLine("<ul>");
			foreach (Tag k in c)
			{
				w.AppendLine("<li>" + k.Index + " - " + k.PathAndQuery + "</li>");
			}
			w.AppendLine("</ul>");



			return w.ToString();
		}
	}
}
