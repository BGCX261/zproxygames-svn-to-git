using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bulldog.Promotion.Games
{
	public static class AvalonSpiderSolitaire
	{
		public const string Title = "Avalon Spider Solitaire";
		public const string Link = "http://avalon-spider-solitaire.tk/";

		public const string Image = GameReferenceExtensions.Host + "/assets/Bulldog.Promotion/Preview_Spider.png";
		
		public const int Width = 800;
		public const int Height = 600;

		public const string Embed = "http://games.mochiads.com/c/g/avalon-spider-solitare/SpiderFlash.swf";

		public static readonly GameReference Reference = GameReference.Of(Title, Image, Link, Embed, Width, Height);
	}
}
