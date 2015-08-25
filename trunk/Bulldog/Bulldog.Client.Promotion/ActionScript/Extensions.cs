using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptCoreLib;
using ScriptCoreLib.ActionScript.flash.net;
using ScriptCoreLib.ActionScript.flash.display;
using ScriptCoreLib.ActionScript.flash.system;
using ScriptCoreLib.ActionScript.Extensions;

namespace Bulldog.Client.Promotion.ActionScript
{
	/// <summary>
	/// This class defines the extension methods for this project
	/// </summary>
	[Script]
	internal static class Extensions
	{
		static readonly LoaderContext Context = new LoaderContext(true);
		public static void ToWebBitmap(this Uri uri, string Host, Action<Bitmap> handler)
		{
			var x = uri.ToString();

			if (x.StartsWith(Host))
			{
				handler(
					KnownEmbeddedResources.Default[x.Substring(Host.Length + 1)].ToBitmapAsset()
				);
				return;
			}

			var e = new Loader();

			var url = new URLRequest(uri.ToString());


			e.contentLoaderInfo.complete +=
				ev =>
				{
					try
					{
						var v = (Bitmap)e.content;

						handler(v);
					}
					catch
					{
					}
				};

			e.load(url, Context);
		}


		public static void ToWebString(this Uri e, Action<string> handler)
		{
			var request = new URLRequest(e.ToString());
			request.method = URLRequestMethod.GET;

			var loader = new URLLoader();
			loader.complete +=
				args =>
				{
					handler("" + loader.data);
				};

			loader.ioError +=
				args =>
				{
					handler(null);
				};


			loader.securityError +=
				args =>
				{
					handler(null);
				};

			loader.load(request);
		}
	}
}
