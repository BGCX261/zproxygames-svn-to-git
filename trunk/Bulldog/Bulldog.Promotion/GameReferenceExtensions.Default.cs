using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Bulldog.Promotion.Library;
using ScriptCoreLib.YAML;

namespace Bulldog.Promotion
{
	partial class GameReferenceExtensions
	{
		
		public static GameReference[] Default = new[]
		{
			Games.AvalonMinesweeper.Reference,
			Games.AvalonTycoonMansion.Reference,
			Games.AvalonSpiderSolitaire.Reference,
			new GameReference 
			{ 
				Title = "FreeCell", 
				Link = "http://nonoba.com/zproxy/avalon-freecell", 
				Image = Host + "/assets/Bulldog.Promotion/Preview_FreeCell.png" 
			},
			new GameReference 
			{ 
				Title = "Multiplayer Mahjong", 
				Link = "http://nonoba.com/zproxy/mahjong-multiplayer", 
				Image = Host + "/assets/Bulldog.Promotion/Preview_Mahjong.png" 
			},
			new GameReference 
			{ 
				Title = "Treasure Hunt", 
				Link = "http://nonoba.com/zproxy/treasure-hunt", 
				Image = Host + "/assets/Bulldog.Promotion/Preview_TreasureHunt.png" 
			},
		};


		
	}

}
