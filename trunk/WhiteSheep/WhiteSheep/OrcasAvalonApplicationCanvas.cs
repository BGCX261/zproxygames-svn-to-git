using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using ScriptCoreLib;
using ScriptCoreLib.Shared.Lambda;

using ScriptCoreLib.Shared.Avalon.Extensions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;

namespace WhiteSheep.Shared
{
	[Script]
	public class OrcasAvalonApplicationCanvas : Canvas
	{
		public const int DefaultWidth = 800;
		public const int DefaultHeight = 500;

		public OrcasAvalonApplicationCanvas()
		{
			Width = DefaultWidth;
			Height = DefaultHeight;

			this.ClipToBounds = true;


			var bg = new Rectangle
			{
				Width = DefaultWidth,
				Height = DefaultHeight,
				Fill = Brushes.White
			}.AttachTo(this);


			var bgi = new Image
			{
				Width = DefaultWidth,
				Height = DefaultHeight,
				Source = "assets/WhiteSheep/bg.png".ToSource()
			}.AttachTo(this);

			30.Times(
				delegate()
				{
					new Image
					{
						Source = "assets/WhiteSheep/grass1.png".ToSource()
					}.AttachTo(this).MoveTo(DefaultWidth.Random(), DefaultHeight.Random());
				}
			);


			var birds = "assets/WhiteSheep/birds.mp3".ToSound();

			birds.PlaybackComplete += birds.Start;
			birds.Start();

			// say what you want but i have made my mind about you
			5.Times(
				delegate()
				{
					var x = DefaultWidth.Random();
					var y = DefaultHeight.Random();
					var s = new Sheep
					{
						Click = () => "assets/WhiteSheep/sheep.mp3".PlaySound()
					};

					s.Container.AttachTo(this).MoveTo(100, 100);
					(1000 / 24).AtIntervalWithCounter(c => s.Container.MoveTo((x + c) % (DefaultWidth + 64) - 32, y));

				}
			);
		}

		[Script]
		public class Sheep
		{
			public Canvas Container = new Canvas();
			public Image[] Frames;
			public Rectangle Touch;
			public Action Click;

			public Sheep()
			{
				var FrameContainer = new Canvas
				{

				}.AttachTo(Container);

				Touch = new Rectangle
				{
					Width = 64,
					Height = 64,
					Fill = Brushes.Red,
					Opacity = 0,
					Cursor = Cursors.Hand
				}.AttachTo(Container);

				Touch.MouseLeftButtonUp +=
					delegate
					{
						if (this.Click != null)
							this.Click();
					};

				Frames = new [] {
					new Image
					{
						Source = "assets/WhiteSheep/sheep-walk_1.png".ToSource()
					},
					new Image
					{
						Source = "assets/WhiteSheep/sheep-walk_2.png".ToSource()
					},
					new Image
					{
						Source = "assets/WhiteSheep/sheep-walk_3.png".ToSource()
					},
					new Image
					{
						Source = "assets/WhiteSheep/sheep-walk_4.png".ToSource()
					},
				};

				var j = default(Image);

				Frames.AsCyclicEnumerable().ForEach(
					(i, next) =>
					{
						if (j != null)
							j.Orphanize();

						j = i.AttachTo(FrameContainer);

						(1000 / 20).AtDelay(next);
					}
				);

			}
		}
	}
}
