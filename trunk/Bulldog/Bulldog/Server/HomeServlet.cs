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
	public class HomeServlet : HTMLServlet
	{
		public const string UrlPattern = "/home";

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

			w.Append("<center>");
			w.Append("<div style='height: 64px;'>");
			w.Append("/assets/Bulldog/CarouselFlash.swf".ToTransparentFlashContainer(800, 300));
			w.Append("</div>");
			w.Append("</center>");

			w.AppendLine(@"
<img src='/assets/Bulldog/jsc.png' />

<h2>Service by this server</h2>
<ul>
	<li><s>List recent games made by zproxy.wordpress.com via YAML</s></li>
	<li>Provide source code generation for ScriptCoreLib</li>
	<li>Provide multiplayer lobby service</li>
	<li>Provide user generated content storage and raiting</li>
	<li>Provide playback recording to resume play at any point</li>
</ul>
<div><a href='/crossdomain.xml'>crossdomain.xml</a></div>
<div><a href='" + AdSenseServlet.Path + @"'>adsense</a></div>
<div><a href='" + GameReferenceExtensions.Path + @"'>promotion</a></div>
<div><a href='" + PromotionZIPServlet.Path + @"'>promotion zip</a></div>
<div><a href='" + BuyLevelEditorServlet.Path + @"'>buy level editor</a></div>
<div><a href='" + DonationServlet.Path + @"'>support indie game development</a></div>
");


			w.Append("<center>");
			w.Append("/assets/Bulldog/VerticalScrollerLoader.swf".ToFlashContainer(120, 90));

			foreach (var k in GameReferenceExtensions.Default)
			{
				w.Append(
					//(k.Link).ToLink(k.Image.ToImage(k.Title))
					(Promotion.PromotionLobbyServlet.Pattern + "/" + k.GetWebTitle()).ToLink(k.Image.ToImage(k.Title))
				);
			}

			w.Append("/assets/Bulldog/VerticalScrollerFlash.swf".ToFlashContainer(120, 90));
			w.Append("</center>");



			w.Append(
				(JavaCodeGeneratorServlet.UrlPattern + "/" + "org.w3c.dom.Document").ToLink("org.w3c.dom.Document")
			);

			"<p>Blog | Terms | Home | Watch Demo | About</p>".WriteTo(w);

			return w.ToString();
		}
	}



}
