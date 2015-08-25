using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bulldog.Promotion;
using Bulldog.Server.CodeGenerators.Java;
using Bulldog.Server.Library;
using java.io;
using java.net;
using javax.servlet.http;
using ScriptCoreLib;
using ScriptCoreLibJava.Extensions;

namespace Bulldog.Server.Monetization
{
	[Script]
	[ConfigurationProvider.UrlPattern(Path)]
	public class AdSenseServlet : HttpServlet
	{
		public const string Path = "/adsense";



		// http://www.adobe.com/devnet/articles/crossdomain_policy_file_spec.html#site-control
		// http://board.flashkit.com/board/showthread.php?t=782484

		protected override void doGet(HttpServletRequest req, HttpServletResponse resp)
		{
			try
			{
				resp.setContentType("text/html");
				resp.getWriter().println(Launch(req.GetPathAndQuery()));
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

			w.Append(@"
<html>
<head>
<link rel='shortcut icon' href='/favicon.ico' />

<style>
	body 
	{
		background: gray url('/assets/Bulldog/bg.png');
		color: white;
	}
	a img
	{
		border: 0;
	}

a:visited
{
	color: #8080ef;
}

a:hover
{
	color: #b0b0ff;
}

a
{
	color: #8080ef;
}

</style>
</head>
<body>
");

			// http://www.actionscript.org/forums/showthread.php3?t=76579

			w.Append("This page is about adsense. You as a visitor should see some ads now. Do you see them? You might not see ads if you are using ad blocking software:)");
			w.Append("<br />");

			w.Append("Google AdSense cannot be included within casual games, because this option is for large scale publishers only.");
			w.Append("<br />");

			w.Append(@"
<script type='text/javascript'><!--
google_ad_client = 'pub-8822516748467753';
/* 120x240, created 7/19/09 */
google_ad_slot = '0528866791';
google_ad_width = 120;
google_ad_height = 240;
//-->
</script>
<script type='text/javascript'
src='http://pagead2.googlesyndication.com/pagead/show_ads.js'>
</script>
");
			w.Append("<hr />");

			"http://zproxy.appspot.com".ToLink("assets/Bulldog/zproxygames200x128a.png".ToImage()).WriteTo(w);
			"http://zproxy.appspot.com".ToLink("assets/Bulldog/zproxygames200x128.png".ToImage()).WriteTo(w);
			"http://zproxy.appspot.com".ToLink("assets/Bulldog/zproxygames100x64.png".ToImage()).WriteTo(w);


			w.Append("<hr />");
			w.Append("Now, you have seen the ad. " +  DonationServlet.Link.ToLink("Please make a donation to support <b>indie game development</b>") + "!");

			// https://www.paypal.com/ee/cgi-bin/webscr?cmd=_button-designer



			w.Append("<hr />");

			w.Append(@"
<form action='https://www.paypal.com/cgi-bin/webscr' method='post'>
<input type='hidden' name='cmd' value='_s-xclick'>
<input type='hidden' name='encrypted' value='-----BEGIN PKCS7-----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-----END PKCS7-----
'>
<input type='image' src='https://www.paypal.com/en_US/i/btn/btn_donateCC_LG.gif' border='0' name='submit' alt='PayPal - The safer, easier way to pay online!'>
<img alt='' border='0' src='https://www.paypal.com/en_US/i/scr/pixel.gif' width='1' height='1'>
</form>
");

			w.Append("<hr />");

			w.Append("Playing games is fun. Creating your own levels for yourself and your friends is twice as cool." + BuyLevelEditorServlet.Link.ToLink("You need to pay 10€ to gain access to the level editor for zproxy games") + ". This will support developing better level editors.");


			w.Append(@"
<form action='https://www.paypal.com/cgi-bin/webscr' method='post'>
<input type='hidden' name='cmd' value='_s-xclick'>
<input type='hidden' name='encrypted' value='-----BEGIN PKCS7-----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-----END PKCS7-----
'>
<input type='image' src='https://www.paypal.com/en_US/i/btn/btn_paynowCC_LG.gif' border='0' name='submit' alt='PayPal - The safer, easier way to pay online!'>
<img alt='' border='0' src='https://www.paypal.com/en_US/i/scr/pixel.gif' width='1' height='1'>
</form>

");

			w.Append(@"
</body>
</html>
");
			return w.ToString();
		}
	}
}
