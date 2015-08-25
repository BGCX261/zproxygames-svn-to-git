using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using javax.servlet.http;
using ScriptCoreLibJava.Extensions;
using ScriptCoreLib;
using Bulldog.Promotion;
using ScriptCoreLib.YAML;

namespace Bulldog.Server.Monetization
{
	[Script]
	[ConfigurationProvider.UrlPattern(Path)]
	public class DonationServlet : HttpServlet
	{
		public const string Path = "/donation";
		// http://www.mataf.net/en/forex/eurusd
		// https://www.paypal.com/ee/cgi-bin/webscr?cmd=_button-designer
		public const string Link = "https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=3XBRXEPACJFVJ&lc=EE&item_name=zproxy%20games&item_number=zproxy%20games&currency_code=EUR&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted";

		protected override void doGet(HttpServletRequest req, HttpServletResponse resp)
		{
			try
			{
				resp.sendRedirect(Link);
			}
			catch (csharp.ThrowableException ex)
			{
				// either swallow of throw a runtime exception

				((java.lang.Throwable)(object)ex).printStackTrace();

			}
		}
	}
}
