using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using javax.servlet.http;
using ScriptCoreLibJava.Extensions;
using ScriptCoreLib;
using Bulldog.Promotion;
using ScriptCoreLib.YAML;
using Bulldog.Server.Library.Servlets;

namespace Bulldog.Server
{
	[Script]
	[ConfigurationProvider.UrlPattern("/favicon.ico")]
	public class FaviconServlet : RedirectionServlet
	{
		// http://www.adobe.com/devnet/articles/crossdomain_policy_file_spec.html#site-control
		// http://board.flashkit.com/board/showthread.php?t=782484

		protected override string ToLocation(string PathAndQuery)
		{
			return "/assets/Bulldog/App.ico";
		}

		
		
	}
}
