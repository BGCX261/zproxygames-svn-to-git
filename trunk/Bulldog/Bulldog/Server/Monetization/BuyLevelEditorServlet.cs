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
	public class BuyLevelEditorServlet : HttpServlet
	{
		public const string Path = "/buy-leveleditor";

		// Do we need Premier or Business account?

		// http://forums.digitalpoint.com/showthread.php?t=1063598
		// You are logging into the account of the seller for this purchase. Please change your login information and try again.

		// https://www.paypal.com/ee/cgi-bin/webscr?cmd=_wp-standard-integration
		// https://www.paypal.com/ee/cgi-bin/webscr?cmd=_button-designer
		public const string Link = "https://www.paypal.com/cgi-bin/webscr?cmd=_xclick&business=3XBRXEPACJFVJ&lc=EE&item_name=zproxy%20games%20level%20editor%20service&amount=10%2e00&currency_code=EUR&button_subtype=services&bn=PP%2dBuyNowBF%3abtn_paynowCC_LG%2egif%3aNonHosted";

		// level editor
		// - save locally
		// -- as cookie
		// -- to google gears
		// -- to file
		// - save to the bulldog system
		// -- description
		// -- preview
		// - upload new assets for custom levels (pay extra for storage to avoid spam)

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
