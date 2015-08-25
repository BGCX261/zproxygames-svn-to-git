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
using Bulldog.Server.Monetization;

namespace Bulldog.Server
{
	[Script]
	[ConfigurationProvider.UrlPattern]
	public class DefaultServlet : HTMLServlet
	{
		// http://www.oreillynet.com/onlamp/blog/2002/11/selling_open_source_to_managem.html

		public override string Render(RenderArguments args)
		{
			if (args.ServerName.StartsWith("musi."))
			{
				var ww = new StringBuilder();

				ww.Append(
				   "<center>" +
				   "/assets/Bulldog/musi.swf".ToFlashContainer(800, 500) +
				   "</center>"
					);

				return ww.ToString();
			}

			if (args.ServerName.StartsWith("nonoba."))
			{
				return new NonobaServlet().Render(args);
			}

			var PathAndQuery = args.PathAndQuery;
			var w = new StringBuilder();

			w.AppendLine(@"
<link rel='shortcut icon' href='/favicon.ico' />
<link rel='alternate' type='application/rss+xml' title='Blog' href='http://zproxy.wordpress.com/feed' />
<link rel='alternate' type='application/rss+xml' title='Games' href='http://services.zproxybuzz.info/promotion.xml' />
<style>
	body 
	{
		font-family: 'Verdana';
		background: gray url('/assets/Bulldog/bg.png');
		color: white;
	}
	a img
	{
		border: 0;
	}

a:hover
{
	color: #0000ff;
}

a
{
	color: #ffff00;
}

</style>
<title>zproxy solutions - " + args.ServerName + " - " + PathAndQuery + @"</title>

<a href='http://zproxy.wordpress.com'>
<img src='/assets/Bulldog/jsc.png' />Visit our development blog</a>


<br />
<br />
");

			w.Append(
				"<center>" +
				"/assets/Bulldog/AvalonExampleGalleryFlash.swf".ToFlashContainer(800, 640) +
				"</center>"
			);

			w.AppendLine(@"
<br />
<br />
<br />
<p><a href='http://blog.zproxybuzz.info'>Blog</a> 
| Terms 
| <a href='mailto:jobs@zproxybuzz.info'>Jobs</a> 
| <a href='mailto:sales@zproxybuzz.info'>Sales</a> 
| <a href='mailto:support@zproxybuzz.info'>Support</a> 
| <a href='/home'>Home</a> 
| Watch Demo 
| About 
| <a href='http://news.zproxybuzz.info'>News</a>
| <a href='http://jsc.zproxybuzz.info'>Technolodgy</a>
| <a href='http://services.zproxybuzz.info'>Services</a>
| <a href='http://lobby.zproxybuzz.info'>Lobby</a>
| <a href='http://games.zproxybuzz.info'>Games</a>

</p>"
			);

			return w.ToString();
		}
	}



}
