using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AvalonMinesweeper.Promotion
{
	public static class Info
	{
		// Do not reuse this key. After building in mochimedia.com mode
		// the swf should send a ping to mochimedia and they will report:
		//   We have received a ping from your game!
		public const string MochiAdsKey = "786387eceb98be62";

		public const string Summary = "Minesweeper";


		public const string Category1 = "lifestyle";
		public const string Category2 = "funandgames";



		public const string Title = "Avalon Minesweeper";


		public const string Description = "Minesweeper";

		// It better be in sync what we actually size us to...
		public const string Resolution = "800x600";

		public const string EMail = "dadeval@gmail.com";

		public const string Author = "Arvo Sulakatko";

		public const string Blog = "http://zproxy.wordpress.com/";
		
		public const string Source = "http://games.mochiads.com/c/g/avalon-minesweeper/OrcasAvalonApplicationFlash.swf";

		

		// before knowing these values we need to take a screenshot
		// and make it loadable. We cannot use https as it is password protected.
		public const string ScreenshotURL = "http://zproxygames.googlecode.com/svn/trunk/AvalonMinesweeper/AvalonMinesweeper.Core/web/assets/AvalonMinesweeper.Core/Preview_AvalonMinesweeper_Preloader.png";
		public const string ScreenshotSmallURL = ScreenshotURL;

		// before we save it we need to upload our swf to somewhere/mochi
		// before knowing module link we need to actually save it/iGoogle
		public const string Module = "http://hosting.gmodules.com/ig/gadgets/file/112091410969506928037/AvalonMinesweeper.xml";
		public const string AddLink = "http://fusion.google.com/ig/add?synd=open&source=ggyp&moduleurl=" + Module;
		public const string ModuleFrame = "http://www.gmodules.com/ig/ifr?url=" + Module;

		// before we can issue a tk redirect we must have the gadget!
		// Note that all domain names that receive less than 25 hits per 90 days will be removed from your account.
		public const string Web = "http://avalon-minesweeper.tk";
	}
}
