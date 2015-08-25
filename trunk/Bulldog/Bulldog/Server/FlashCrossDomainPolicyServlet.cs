using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using javax.servlet.http;
using ScriptCoreLibJava.Extensions;
using ScriptCoreLib;

namespace Bulldog.Server
{
	[Script]
	[ConfigurationProvider.UrlPattern(UrlPattern)]
	public class FlashCrossDomainPolicyServlet : HttpServlet
	{
		public const string UrlPattern = "/crossdomain.xml";

		// http://www.adobe.com/devnet/articles/crossdomain_policy_file_spec.html#site-control
		// http://board.flashkit.com/board/showthread.php?t=782484
		// http://www.adobe.com/devnet/flashplayer/articles/fplayer9_security_02.html

		protected override void doGet(HttpServletRequest req, HttpServletResponse resp)
		{
			try
			{
				resp.setContentType("text/x-cross-domain-policy");

				Console.WriteLine(UrlPattern);
				resp.getWriter().print(@"
<?xml version=""1.0""?>
<!DOCTYPE cross-domain-policy SYSTEM ""http://www.adobe.com/xml/dtds/cross-domain-policy.dtd"">
<cross-domain-policy>
	<site-control permitted-cross-domain-policies=""all""/>
	<allow-access-from domain=""*"" secure=""false""/>
	<allow-http-request-headers-from domain=""*"" headers=""*"" secure=""false""/>
</cross-domain-policy>
".Trim());

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
