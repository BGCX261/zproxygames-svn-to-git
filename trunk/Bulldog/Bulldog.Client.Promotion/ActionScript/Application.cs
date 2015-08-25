using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using ScriptCoreLib;
using ScriptCoreLib.ActionScript;
using ScriptCoreLib.ActionScript.Extensions;
using ScriptCoreLib.ActionScript.flash.display;
using ScriptCoreLib.ActionScript.flash.text;
using ScriptCoreLib.ActionScript.flash.ui;
using ScriptCoreLib.ActionScript.flash.net;
using ScriptCoreLib.ActionScript.flash.system;
using System.Text;
using Bulldog.Promotion;

namespace Bulldog.Client.Promotion.ActionScript
{
	/// <summary>
	/// Default flash player entrypoint class. See 'tools/build.bat' for adding more entrypoints.
	/// </summary>
	[Script, ScriptApplicationEntryPoint(WithResources = true)]
	[SWF]
	public class Application : Sprite
	{
		public const string Host = "http://zproxy.appspot.com";

		/// <summary>
		/// Default constructor
		/// </summary>
		public Application()
		{
			// http://curtismorley.com/2007/08/31/flash-cs3-flex-2-as3-error-2148/

			Security.allowDomain("*");
			Security.allowInsecureDomain("*");

			Security.loadPolicyFile(Host + "/crossdomain.xml");


			var t = new TextField
			{
				multiline = true,
				wordWrap = false,
				text = "powered by jsc!!!!",
				background = true,
				width = 400,
				height = 400,
				x = 8,
				y = 8,
				alwaysShowSelection = true,
			}.AttachTo(this);



			// This game shall know less games than the server
			var games = GameReferenceExtensions.Partial;
			var gi = 0;

			new Uri(Host + "/promotion").ToWebString(
				data =>
				{
					if (data != null)
						games = games.FromYAML(data);

					// http://livedocs.adobe.com/flash/9.0/ActionScriptLangRefV3/flash/text/TextField.html
					// Error #2044: Unhandled IOErrorEvent:. text=Error #2124: Loaded file is an unknown type.
					// Error #2044: Unhandled IOErrorEvent:. text=Error #2035: URL Not Found.


					var w = new StringBuilder();

					foreach (var g in games)
					{
						new Uri(g.Image).ToWebBitmap(
							Host,
							img =>
							{
								img.AttachTo(this).MoveTo(200, 95 * gi + 5);
								gi++;
							}
						);

						w.Append(
							"<br><a href='" + g.Link + "'><u>" + g.Title + "</u></a><br>Link: " + g.Link + "<br>Image: " + g.Image + "<br>"

						);
					}

					t.htmlText = w.ToString();
				}
			);

		}


	}


}