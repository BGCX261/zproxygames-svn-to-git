using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using javax.servlet.http;
using ScriptCoreLibJava.Extensions;
using ScriptCoreLib;
using Bulldog.Promotion;
using ScriptCoreLib.YAML;
using Bulldog.Server.Library;

namespace Bulldog.Server
{
	[Script]
	[ConfigurationProvider.UrlPattern("/promotion.xml")]
	public class PromotionFeedServlet : HttpServlet
	{

		// http://www.adobe.com/devnet/articles/crossdomain_policy_file_spec.html#site-control
		// http://board.flashkit.com/board/showthread.php?t=782484

		protected override void doGet(HttpServletRequest req, HttpServletResponse resp)
		{
			try
			{
				resp.setContentType("application/rss+xml");

				resp.getWriter().print(GetContent());

				resp.getWriter().flush();
			}
			catch (csharp.ThrowableException ex)
			{
				// either swallow of throw a runtime exception

				((java.lang.Throwable)(object)ex).printStackTrace();

			}
		}

		private string GetContent()
		{
			var w = new StringBuilder();

			w.Append(@"<?xml version='1.0' encoding='UTF-8' standalone='yes'?>
<rss version='2.0' xmlns:media='http://search.yahoo.com/mrss' xmlns:atom='http://www.w3.org/2005/Atom'>
  <channel>
    <title>zproxy games</title>
    <description>zproxy games</description>
    <link>http://services.zproxybuzz.info/promotion.xml</link>");

			foreach (var g in GameReferenceExtensions.Default)
			{
				w.AppendLine("<item>");

				w.Append("<title>" + g.Title + "</title>");
				w.Append("<link>" + g.Link + "</link>");

				w.Append("<description><![CDATA[");
				w.Append("<a href='" + g.Link + "' >");
				w.Append("<img src='" + g.Image + "' />");
				w.Append("<br />");
				w.Append(g.Title);
				w.Append("</a>");
				w.Append(" ]]></description>");


				w.Append("<media:thumbnail url='" + g.Image + "' />");
				w.Append("<media:content " + g.Width.ToAttributeString("width") + " " + g.Height.ToAttributeString("height") + " " + g.Embed.ToAttributeString("url") + @" type='application/x-shockwave-flash' />");
				//w.Append("<media:description type='plain'>" + SmartTitle + " | " + IMDBRaiting.Value + " | " + IMDBTagline.Value + " | " + IMDBGenre.Value + "</media:description>");
				w.Append("<category>Games</category>");
				
				w.AppendLine("</item>");
			}


			w.Append(@"
	  </channel>
</rss>");

			return w.ToString();
		}


	}
}
