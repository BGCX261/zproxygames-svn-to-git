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
	[ConfigurationProvider.UrlPattern(UrlPattern + "/*")]
	public class NonobaServlet : HTMLServlet
	{
		public const string UrlPattern = "/nonoba";

		public override string Render(RenderArguments args)
		{
			var PathAndQuery = args.PathAndQuery;
			var w = new StringBuilder();

			w.AppendLine(@"
<link rel='shortcut icon' href='/favicon.ico' />
<link rel='alternate' type='application/rss+xml' title='Games' href='http://zproxy.appspot.com/promotion.xml' />
<link rel='alternate' type='application/rss+xml' title='Blog' href='http://zproxy.wordpress.com/feed' />

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

a:hover
{
	color: #0000ff;
}

a
{
	color: #ffff00;
}

</style>
<title>zproxy games - " + PathAndQuery + @"</title>");

			//w.Append("<center>");
			//w.Append("<div style='height: 64px;'>");
			//w.Append("/assets/Bulldog/CarouselFlash.swf".ToTransparentFlashContainer(800, 300));
			//w.Append("</div>");
			//w.Append("</center>");

			@"
<img src='/assets/Bulldog/jsc.png' />

This page will take multiplayer game development on Nonoba to the next level!

".WriteTo(w);

			return w.ToString();
		}
	}



}
