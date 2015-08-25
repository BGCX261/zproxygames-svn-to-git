using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bulldog.Promotion.Games
{
	public static class AvalonMinesweeper
	{
		public const string Title = "Avalon Minesweeper";
		public const string Link = "http://avalon-minesweeper.tk";

		public const string Image = GameReferenceExtensions.Host + "/assets/Bulldog.Promotion/Preview_AvalonMinesweeper.png";

		public const int Width = 800;
		public const int Height = 600;

		// at this time we are guessing the url. we will be probably right :)
		public const string Embed = "http://games.mochiads.com/c/g/avalon-minesweeper/OrcasAvalonApplicationFlash.swf";

		public static readonly GameReference Reference = GameReference.Of(Title, Image, Link, Embed, Width, Height);
	}
}
