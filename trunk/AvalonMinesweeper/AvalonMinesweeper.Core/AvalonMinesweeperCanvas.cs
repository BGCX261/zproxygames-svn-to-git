using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using AvalonMinesweeper.Core.Library;
using AvalonMinesweeper.Promotion;
using Bulldog.Client.Promotion.Controls;
using ScriptCoreLib.Archive.ZIP;
using ScriptCoreLib.Shared.Avalon.Cursors;
using ScriptCoreLib.Shared.Avalon.Extensions;
using ScriptCoreLib.Shared.Lambda;

namespace AvalonMinesweeper.Core
{
	public partial class AvalonMinesweeperCanvas : Canvas
	{
		// as you noticed this library is [Script] free
		// but we still need our special methods from ScriptCoreLib

		// we are one of the zproxy branded games so we gather common 
		// intel from a generic location
		public const int DefaultWidth = Bulldog.Promotion.Games.AvalonMinesweeper.Width;
		public const int DefaultHeight = Bulldog.Promotion.Games.AvalonMinesweeper.Height;

		public readonly Canvas Content;

		// buttons should have colors
		// explosions
		// link to youtube video of minesweepers
		// fullscreen mode
		// add names  to external mouses
		// mouse show/hide debugging

		// additional game modes:
		// 1. (+) medical help droplet - team members can play medic arrow
		// 2. cursor *10 style replay where you co-op with previous versions of yourself
		// 3. crushing ceilcing and a flow of fields to unravel



		public const int ContentHeight = DefaultHeight * 2;
		public const int ContentWidth = DefaultWidth * 2;

		public AvalonMinesweeperCanvas()
		{
			this.Width = DefaultWidth;
			this.Height = DefaultHeight;


			InitializeBackground();


			#region layers

			this.Content = new Canvas
			{
				Width = ContentWidth,
				Height = ContentHeight,
			}.AttachTo(this);

			var Shadows = new Canvas
			{
				Width = ContentWidth,
				Height = ContentHeight,
			}.AttachTo(this.Content);

			var Visuals = new Canvas
			{
				Width = ContentWidth,
				Height = ContentHeight,
			}.AttachTo(this.Content);

			var InfoOverlay = new Canvas
			{
				Width = ContentWidth,
				Height = ContentHeight,
			}.AttachTo(this.Content);


			#region lets insert a console
			var GameConsole = new GameConsole
			{


			};

			GameConsole.SizeTo(DefaultWidth, DefaultHeight);
			GameConsole.AttachContainerTo(this);
			var GameConsoleOpacity = GameConsole.Shade.ToAnimatedOpacity();

			Action GameConsoleShow = delegate
			{
				GameConsole.WriteLine(Info.Title + " console");

				GameConsole.Show();
				GameConsoleOpacity.Opacity = 0.4;
				GameConsole.TextBox.Show();
			};

			Action GameConsoleHide = delegate
			{
				GameConsole.TextBox.Hide();
				GameConsoleOpacity.SetOpacity(0, GameConsole.Hide);
			};
			GameConsoleHide();

			Action GameConsoleToggle = GameConsoleShow.WhereCounter(k => k % 2 == 0);


			GameConsoleToggle += GameConsoleHide.WhereCounter(k => k % 2 == 1);


			this.KeyUp +=
				(sender, args) =>
				{
					if (args.Key == Key.Oem7)
					{
						GameConsoleToggle();
					}
				};

			// we are going for the keyboard input
			// we want to enable the tilde console feature
			this.FocusVisualStyle = null;
			this.Focusable = true;
			this.Focus();

			this.MouseLeftButtonDown +=
				(sender, key_args) =>
				{
					this.Focus();
				};

			#endregion


			var TouchOverlay = new Canvas
			{
				Width = ContentWidth,
				Height = ContentHeight,
			}.AttachTo(this);

			var TouchOverlayTrap = new Rectangle
			{
				Fill = Brushes.Red,
				Width = ContentWidth,
				Height = ContentHeight,
				Opacity = 0
			}.AttachTo(TouchOverlay);
			#endregion




			var clone = new NamedArrowCursorControl
			{
				Text = "player revived!"
			}.AttachContainerTo(InfoOverlay);

			clone.Hide();

			var shadowoffset = 4;

			this.Content.MoveTo(-DefaultWidth / 2, -DefaultHeight / 2);
			TouchOverlay.MoveTo(-DefaultWidth / 2, -DefaultHeight / 2);

			// the button shall be atteched to the playfield with the following rules
			var MineFieldButtons = new BindingList<MineFieldButton>().WithEvents(
				k =>
				{
					var x = (ContentWidth / 2) + (k.IndexX * MineFieldButton.Size);
					var y = (ContentHeight / 2) + (k.IndexY * MineFieldButton.Size);

					k.Shadow.MoveTo(x + shadowoffset, y + shadowoffset).AttachTo(Shadows);
					k.MoveContainerTo(x, y).AttachContainerTo(Visuals);
					k.TouchRectangle.MoveTo(x, y).AttachTo(TouchOverlay);

					return delegate
					{
						k.Shadow.Orphanize();
						k.OrphanizeContainer();
						k.TouchRectangle.Orphanize();
					};
				}
			);


			Action HideCursor =
				delegate
				{
					TouchOverlay.Cursor = Cursors.None;
					MineFieldButtons.Source.ForEach(k => k.TouchRectangle.Cursor = Cursors.None);
				};

			Action ShowCursor =
				delegate
				{
					TouchOverlay.Cursor = Cursors.Arrow;
					MineFieldButtons.Source.ForEach(k => k.TouchRectangle.Cursor = Cursors.Hand);
				};

			var SoundFlag = ("assets/AvalonMinesweeper.Core/flag.mp3").ToSound();
			var SoundReveal = ("assets/AvalonMinesweeper.Core/reveal.mp3").ToSound();
			var SoundClick = ("assets/AvalonMinesweeper.Core/click.mp3").ToSound();
			var SoundTick = ("assets/AvalonMinesweeper.Core/tick.mp3").ToSound();
			var SoundBuzzer = ("assets/AvalonMinesweeper.Core/buzzer.mp3").ToSound();


			#region the tule
			MineFieldButtons.Added +=
				(k, i) =>
				{
					k.Click +=
						(sender, args) =>
						{
							if (Keyboard.Modifiers != ModifierKeys.None)
							{
								SoundFlag.Start();
								if (k.Flag.Visibility == Visibility.Visible)
								{
									k.Flag.Hide();
									k.Question.Show();
								}
								else if (k.Question.Visibility == Visibility.Visible)
									k.Question.Hide();
								else
									k.Flag.Show();

								return;
							}

							if (k.Flag.Visibility == Visibility.Visible)
							{
								SoundBuzzer.Start();
								return;
							}

							if (k.Question.Visibility == Visibility.Visible)
							{
								SoundBuzzer.Start();
								return;
							}

							if (!k.IsMined)
							{
								var Region = k.Region.ToArray();

								if (Region.Length == 1)
									SoundClick.Start();
								else
									SoundReveal.Start();

								foreach (var r in Region)
								{

									r.VisibleNumber.Value = r.Neighbours.Where(kk => kk.IsMined).Count();
									r.IsPressed.Value = true;
									r.IsEnabled = false;
									r.BackgroundColor = 0xFFc0c0c0.ToColor();
								}

								return;
							}



							k.Flag.Hide();
							k.Mine.Show();
							k.IsPressed.Value = true;
							k.Background.Fill = Brushes.Red;

							var p = args.GetPosition(TouchOverlay);

							Explosion.Create(Convert.ToInt32(p.X), Convert.ToInt32(p.Y), InfoOverlay);


							HideCursor();

							//150.AtDelay(
							//    delegate
							//    {
							//        Explosion.Create(Convert.ToInt32(p.X + 6), Convert.ToInt32(p.Y - 12), InfoOverlay, 48);
							//    }
							//);

							//300.AtDelay(
							//    delegate
							//    {
							//        Explosion.Create(Convert.ToInt32(p.X + 16), Convert.ToInt32(p.Y + 6), InfoOverlay);
							//    }
							//);


							#region revive
							2000.AtDelay(
								delegate
								{
									("assets/AvalonMinesweeper.Core/reveal.mp3").PlaySound();
									clone.Color = Colors.White;
									clone.Cursor.Show();
									clone.Show();

									// could we rewrite this as LINQ ?

									Action FlashRed = () => clone.Color = Colors.Red;
									Action FlashWhite = () => clone.Color = Colors.White;
									Action FlashHold = () => { clone.Cursor.Hide(); ShowCursor(); };
									Action<DispatcherTimer> FlashClose = t => { clone.Hide(); t.Stop(); };

									Func<int, bool> FlashRedFilter = c => { if (c > 8) return false; return c % 2 == 1; };
									Func<int, bool> FlashWhiteFilter = c => { if (c > 8) return false; return c % 2 == 0; };

									120.ToTimerBuilder()
										.With(FlashRed.WhereCounter(FlashRedFilter))
										.With(FlashWhite.WhereCounter(FlashWhiteFilter))
										.With(FlashHold.WhereCounter(8))
										.With(FlashClose.WhereCounter(8 + 10))
										.Start();



								}
							);
							#endregion




						};
				};
			#endregion


			var MapCollection = default(ZIPFile);
			var MapLoader = new MineMapLoader(MineFieldButtons.Source);

			2000.AtDelay(
				delegate
				{
					"assets/AvalonMinesweeper.Core/Levels.zip".ToMemoryStreamAsset(
						levels =>
						{
							MapCollection = levels;

							foreach (var k in MapCollection.Entries)
							{
								GameConsole.WriteLine("level: " + k.FileName);
							}
						}
					);
				}
			);




			3000.AtIntervalWithTimer(
				t =>
				{
					if (MapCollection == null)
						return;

					t.Stop();

					var map = new ASCIIImage(
						new ASCIIImage.ConstructorArguments
						{
							// this map will be built in 2 seconds
							value = MapCollection.Entries.Random().Text
						}
					);

					GameConsole.WriteLine("ready to load map");

					//var Gradient = Colors.LightGreen.ToGradient(Colors.Yellow, map.Height).ToArray();
					var Gradient = 0xFFd0d0d0.ToColor().ToGradient(0xff404040.ToColor(), map.Height).ToArray();

					var Entries = Enumerable.ToArray(
						from k in map
						where k.Value != "M"
						select new MineMapLoader.Entry
						{
							IndexX = k.X - map.Width / 2,
							IndexY = k.Y - map.Height / 2,
							BackgroundColor = Gradient[k.Y],
						}
					);

					Entries.Randomize().Take(Convert.ToInt32(Entries.Length * 0.2)).ForEach(k => k.IsMined = true);

					MapLoader.Prepare(Entries,
						delegate
						{
							GameConsole.WriteLine("map loaded!");
						}
					);
				}
			);









			var yellow = new NamedArrowCursorControl
			{
				Text = "Minesweeper 1",
				Color = Colors.Yellow
			}.AttachContainerTo(InfoOverlay).MoveContainerTo(64 + DefaultWidth, 64 + DefaultHeight);



			var white = new NamedArrowCursorControl
			{
				Text = "More is Less, a really long name in here to test automation"
			}.AttachContainerTo(InfoOverlay).MoveContainerTo(64 + 100 + DefaultWidth, 64 + 16 + DefaultHeight);

			var red = new NamedArrowCursorControl
			{
				Color = Colors.Red,
				Text = "John Doe"
			}.AttachContainerTo(InfoOverlay).MoveContainerTo(64 + 16 + DefaultWidth, 64 + 100 + DefaultHeight);





			(1000 / 24).AtIntervalWithCounter(
				c =>
				{
					yellow.MoveContainerTo(64 + 100 + Math.Cos(c * 0.1) * 12 + DefaultWidth, DefaultHeight + 64 + 64 + Math.Sin(c * 0.1) * 12);
				}
			);


			TouchOverlay.MouseMove +=
				(sender, args) =>
				{
					var p = args.GetPosition(TouchOverlay);
					//var r = args.GetPosition(ScrollTrap);

					//var rx = r.X.Min(DefaultWidth - ScrollMargin * 2).Max(0);
					//var ry = r.Y.Min(DefaultHeight - ScrollMargin * 2).Max(0);

					//this.Status.Value = "x: " + rx + ", y: " + ry;

					//this.Parallax1.MoveTo(
					//    (DefaultWidth - Parallax1Width) * rx / (DefaultWidth - ScrollMargin * 2),
					//    (DefaultHeight - Parallax1Height) * ry / (DefaultHeight - ScrollMargin * 2)
					//);

					clone.MoveContainerTo(p.X, p.Y);
				};




			//("assets/AvalonMinesweeper.Core/applause.mp3").PlaySound();



			#region SocialLinks
			var SocialLinks = new GameSocialLinks(this)
			{
				new GameSocialLinks.AddToGoogleButton { 
				    Hyperlink = new Uri(Info.AddLink)
				},
				new GameSocialLinks.TikiButton { 
					Hyperlink = new Uri(Info.Web)
				},
				new GameSocialLinks.StumbleUponButton { 
				    Hyperlink = new Uri( "http://www.stumbleupon.com/submit?url=" + Info.Web)
				},
				new GameSocialLinks.BlogFeedButton { 
			
					Hyperlink = new Uri( "http://zproxy.wordpress.com")
				}
			};
			#endregion




			#region GameMenuWithGames
			var ShadowSize = 24;
			var SocialLinksMenu = new GameMenuWithGames(DefaultWidth, DefaultHeight, ShadowSize);

			SocialLinksMenu.Carousel.Caption.FontFamily = new FontFamily("Verdana");
			SocialLinksMenu.IdleText = "-= zproxy games =-";

			SocialLinksMenu.DownloadStarting +=
				delegate
				{
					GameConsole.WriteLine("downloading game information...");
				};
			SocialLinksMenu.DownloadComplete +=
				delegate
				{
					GameConsole.WriteLine("downloading game information done...");
				};

			1000.AtDelay(SocialLinksMenu.Download);

			SocialLinksMenu.AttachContainerTo(this);

			SocialLinksMenu.AfterMove +=
				(x, y) =>
				{
					SocialLinks.OffsetY = y + SocialLinksMenu.ContentHeight + ShadowSize;
				};

			SocialLinksMenu.Hide();
			#endregion



		}



	}

}
