using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using javax.servlet.http;
using ScriptCoreLibJava.Extensions;
using ScriptCoreLib;
using Bulldog.Promotion;
using ScriptCoreLib.YAML;

namespace Bulldog.Server
{
	[Script]
	[ConfigurationProvider.UrlPattern(GameReferenceExtensions.Path)]
	public class PromotionServlet : HttpServlet
	{

		// http://www.adobe.com/devnet/articles/crossdomain_policy_file_spec.html#site-control
		// http://board.flashkit.com/board/showthread.php?t=782484

		protected override void doGet(HttpServletRequest req, HttpServletResponse resp)
		{
			try
			{
				resp.setContentType(YAMLDocument.ContentType);

				resp.getWriter().print(GameReferenceExtensions.Default.ToYAML());

				resp.getWriter().flush();
			}
			catch (csharp.ThrowableException ex)
			{
				// either swallow of throw a runtime exception

				((java.lang.Throwable)(object)ex).printStackTrace();

			}
		}
	}
}
