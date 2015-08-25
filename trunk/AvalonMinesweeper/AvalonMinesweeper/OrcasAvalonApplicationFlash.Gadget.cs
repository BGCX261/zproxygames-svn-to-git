using ScriptCoreLib;
using ScriptCoreLib.ActionScript.flash.display;
using ScriptCoreLib.ActionScript.flash.text;
using System.Collections.Generic;
using System;
using ScriptCoreLib.ActionScript;
using ScriptCoreLib.ActionScript.Extensions;
using AvalonMinesweeper.Promotion;
using AvalonMinesweeper.Core;

namespace AvalonMinesweeper.ActionScript
{
	[GoogleGadget(
	   author_email = Info.EMail,
	   author_link = Info.Blog,
	   author = Info.Author,
	   category = Info.Category1,
	   category2 = Info.Category2,
	   screenshot = Info.ScreenshotURL,
	   thumbnail = Info.ScreenshotSmallURL,
	   description = Info.Description,
	   width = AvalonMinesweeperCanvas.DefaultWidth,
	   height = AvalonMinesweeperCanvas.DefaultHeight,
	   title = Info.Title,

	   // Note that all domain names that receive less than 25 hits per 90 days will be removed from your account.
	   title_url = Info.Web,
	   src = Info.Source
	)]
	partial class OrcasAvalonApplicationFlash
	{

	}
}