using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using ScriptCoreLib;
using ScriptCoreLib.Shared.Avalon.Extensions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Bulldog.Client.Promotion.Controls;

namespace Bulldog.Client.Promotion.Carousel.Shared
{
	[Script]
	public class OrcasAvalonApplicationCanvas : Canvas
	{
		public const int DefaultWidth = 800;
		public const int DefaultHeight = 300;

		public OrcasAvalonApplicationCanvas()
		{
			Width = DefaultWidth;
			Height = DefaultHeight;

			this.ClipToBounds = true;

			//Colors.Blue.ToGradient(Colors.Red, DefaultHeight / 4).Select(
			//    (c, i) =>
			//        new Rectangle
			//        {
			//            Fill = new SolidColorBrush(c),
			//            Width = DefaultWidth,
			//            Height = 4,
			//        }.MoveTo(0, i * 4).AttachTo(this)
			//).ToArray();


			var m = new GameMenuWithGames(DefaultWidth, DefaultHeight, 32)
			{
				IdleText = "More Games!"
			}.AttachContainerTo(this);

			m.Hide();
		}
	}
}
