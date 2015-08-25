using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bulldog.Promotion.Games
{
	public static class AvalonTycoonMansion
	{
		public const string Title = "Avalon Tycoon Mansion";
		public const string Link = "http://avalon-tycoon-mansion.tk";

		public const string Image = GameReferenceExtensions.Host + "/assets/Bulldog.Promotion/Preview_AvalonTycoonMansion.png";

		public const int Width = 600;
		public const int Height = 400;

		public const string Embed = "http://games.mochiads.com/c/g/avalon-tycoon-mansion/OrcasAvalonApplicationFlash.swf";

		public static readonly GameReference Reference = GameReference.Of(Title, Image, Link, Embed, Width, Height);
	}
}
