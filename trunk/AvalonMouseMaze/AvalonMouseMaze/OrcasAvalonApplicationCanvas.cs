using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Bulldog.Client.Promotion.Controls;
using ScriptCoreLib;
using ScriptCoreLib.Shared.Avalon.Extensions;
using ScriptCoreLib.Shared.Maze;
using ScriptCoreLib.Shared.Lambda;
using System.Windows.Input;

namespace AvalonMouseMaze.Shared
{
	[Script]
	public class OrcasAvalonApplicationCanvas : Canvas
	{
		public const int DefaultWidth = 800;
		public const int DefaultHeight = 600;

		public OrcasAvalonApplicationCanvas()
		{
			Width = DefaultWidth;
			Height = DefaultHeight;

			this.ClipToBounds = true;

			Colors.Blue.ToGradient(Colors.Black, DefaultHeight / 4).Select(
				(c, i) =>
					new Rectangle
					{
						Fill = new SolidColorBrush(c),
						Width = DefaultWidth,
						Height = 5,
					}.MoveTo(0, i * 4).AttachTo(this)
			).ToArray();






			var mouse = new Image
			{
				Source = (KnownAssets.Path.Assets + "/mouse.png").ToSource(),
				Width = 32,
				Height = 32
			}.MoveTo(0, 0).AttachTo(this);


			var img = new Image
			{
				Source = (KnownAssets.Path.Assets + "/jsc.png").ToSource()
			}.MoveTo(DefaultWidth - 96, 0).AttachTo(this);


			var Content = new Canvas
			{
				Width = DefaultWidth,
				Height = DefaultHeight,
				
			}.AttachTo(this);

			var ContentY = new AnimatedDouble(0);

			ContentY.ValueChanged += y => Content.MoveTo(0, y);
			
			{
				var maze = new MazeGenerator(12, 8, null);
				var blocks = new BlockMaze(maze);
				var w = new BlockMaze(maze);
				Colors.Black.ToGradient(Colors.Yellow, 30).ForEach(
					(c, i) =>
						RenderMaze(60 + i * 0.1, w, new SolidColorBrush(c), Content)
				);
			}

			var TouchOverlay = new Rectangle
			{
				Fill = Brushes.Yellow,
				Opacity = 0,
				Width = DefaultWidth,
				Height = DefaultHeight
			}.AttachTo(this);

			TouchOverlay.MouseEnter +=
				delegate
				{
					mouse.Show();
				};
			TouchOverlay.MouseLeave +=
				delegate
				{
					mouse.Hide();
				};
			TouchOverlay.Cursor = Cursors.None;
			TouchOverlay.MouseMove +=
				(s, args) =>
				{
					var p = args.GetPosition(this);

					mouse.MoveTo(p.X - 4, p.Y - 4);
				};

			TouchOverlay.MouseLeftButtonUp +=
				(s, args) =>
				{
					var p = args.GetPosition(this);

					ShowExplosion(Convert.ToInt32(p.X), Convert.ToInt32(p.Y), Content);
					("assets/AvalonMouseMaze/explosion.mp3").PlaySound();
					150.AtDelay(
						delegate
						{
							ShowExplosion(Convert.ToInt32(p.X + 6), Convert.ToInt32(p.Y - 6), Content);
						}
					);

					300.AtDelay(
						delegate
						{
							ShowExplosion(Convert.ToInt32(p.X + 4), Convert.ToInt32(p.Y + 6), Content);

							ContentY.SetTarget(DefaultHeight);
						}
					);

					1500.AtDelay(
						delegate
						{

							ContentY.SetTarget(0);
						}
					);

				};


			new GameMenuWithGames(DefaultWidth, DefaultHeight, 32).AttachContainerTo(this).Hide();
		}

		private void ShowExplosion(int _x, int _y, Canvas Content)
		{
			var exc = new Canvas
			{
				Width = 64,
				Height = 64
			}.MoveTo(_x - 32, _y - 32).AttachTo(Content);

			Enumerable.Range(1, 16).ForEach(
				(c, i, next) =>
				{

					var n = ("" + c).PadLeft(2, '0');


					var ex = new Image
					{
						Source = (KnownAssets.Path.Assets + "/ani6_" + n + ".png").ToSource()
					}.AttachTo(exc);

					(1000 / 24).AtDelay(
						delegate
						{
							ex.Orphanize();
							next();

							
						}
					);
				}
			)(
				delegate
				{
					exc.Orphanize();
				}
			);
		}

		private void RenderMaze(double z, BlockMaze w, Brush Fill, Canvas Content)
		{
			Action<double, double> fillRect =
				(_x, _y) =>
				{
					new Rectangle
					{
						Fill = Fill,
						Width = z / 2,
						Height = z / 2
					}.MoveTo(_x, _y).AttachTo(Content);

				};

			for (int x = 0; x < w.Width; x++)
				for (int y = 0; y < w.Height; y++)
				{
					var v = w.Walls[x][y];

					if (v)
						fillRect(
							 (DefaultWidth / 2) + (x - w.Width / 2) * z / 2,
							(DefaultHeight / 2) + (y - w.Height / 2) * z / 2);


				}
		}
	}
}
