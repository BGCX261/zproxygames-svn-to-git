using ScriptCoreLib;
using ScriptCoreLib.ActionScript.flash.display;
using ScriptCoreLib.ActionScript.flash.text;
using ScriptCoreLib.ActionScript.Extensions;
using System.Collections.Generic;
using System;
using System.IO;
using ScriptCoreLib.ActionScript;
using ScriptCoreLib.ActionScript.flash.ui;
using ScriptCoreLib.ActionScript.flash.net;
using ScriptCoreLib.ActionScript.flash.system;
//using Bulldog.Client.Promotion.VerticalScroller.Loader.Shared;

namespace Bulldog.Client.Promotion.VerticalScroller.Loader.ActionScript
{
	/// <summary>
	/// Default flash player entrypoint class. See 'tools/build.bat' for adding more entrypoints.
	/// </summary>
	[Script, ScriptApplicationEntryPoint(WithResources = true)]
	[SWF(width = 120, height = 90, backgroundColor = 0)]
	public class VerticalScrollerLoader : Sprite
	{
		public VerticalScrollerLoader()
		{
			// http://www.iheartactionscript.com/loading-an-external-swf-in-as3/
			// http://kb2.adobe.com/cps/141/tn_14190.html
			Security.allowDomain("*");
			Security.allowInsecureDomain("*");

			var myLoader = new ScriptCoreLib.ActionScript.flash.display.Loader(); 
			
			addChild(myLoader);

			var url = new URLRequest("http://zproxy.appspot.com.nyud.net/assets/Bulldog/VerticalScrollerFlash.swf"); 
			
			
			myLoader.load(url);
		}
	}
}