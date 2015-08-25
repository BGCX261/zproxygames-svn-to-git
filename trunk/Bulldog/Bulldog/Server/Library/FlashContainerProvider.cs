using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptCoreLib;

namespace Bulldog.Server.Library
{
	[Script]
	public static class FlashContainerProvider
	{
		public static string ToFlashContainer(this string path, int width, int height)
		{
			return @"<object type=""application/x-shockwave-flash"" data=""" + path + @"""  width=""" + width + @""" height=""" + height + @""" allowFullScreen=""true"" allowNetworking=""all"" allowScriptAccess=""always"">
  <param name=""movie"" value=""" + path + @""" />
</object>";
		}

		public static string ToTransparentFlashContainer(this string path, int width, int height)
		{
			return @"<object wmode='transparent' type=""application/x-shockwave-flash"" data=""" + path + @"""  width=""" + width + @""" height=""" + height + @""" allowFullScreen=""true"" allowNetworking=""all"" allowScriptAccess=""always"">
  <param name=""movie"" value=""" + path + @""" />
</object>";
		}
	}
}
