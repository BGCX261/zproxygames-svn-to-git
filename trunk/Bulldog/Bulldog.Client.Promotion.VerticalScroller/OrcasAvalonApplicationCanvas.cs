using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Bulldog.Promotion;
using ScriptCoreLib;
using ScriptCoreLib.Shared.Lambda;
using ScriptCoreLib.Shared.Avalon.Extensions;
using ScriptCoreLib.Shared.Avalon.Tween;

namespace Bulldog.Client.Promotion.VerticalScroller.Shared
{
	[Script]
	public class OrcasAvalonApplicationCanvas : Canvas
	{
		public const int DefaultWidth = GameReferenceExtensions.Width;
		public const int DefaultHeight = GameReferenceExtensions.Height;

		[Script]
		public class Entry
		{
			public Canvas Canvas;
			public Image Image;
			public Rectangle TouchOverlay;
			public TextBox Text;
			public AnimatedOpacity Shadow;
			public AnimatedOpacity ShadowBottom;
		}

		public OrcasAvalonApplicationCanvas()
		{
			Width = DefaultWidth;
			Height = DefaultHeight;

			this.ClipToBounds = true;

			var x = GameReferenceExtensions.Default;




			var Content = new Canvas
			{
				Width = DefaultWidth,
				Height = DefaultHeight,
			}.AttachTo(this);

			int LogoSize = 32;

			new Image
			{
				Width = LogoSize,
				Height = LogoSize,
				Source = ("assets/Bulldog.Promotion/jsc.png").ToSource()
			}.MoveTo(DefaultWidth - LogoSize, DefaultHeight - LogoSize).AttachTo(this);

			var TouchOverlayContainer = new Canvas
			{
				Width = DefaultWidth,
				Height = DefaultHeight,
			}.AttachTo(this);

			var wc = new WebClient();

			var List = new List<Entry>();

			#region load
			wc.DownloadStringCompleted +=
				(sender, args) =>
				{

					var a = GameReferenceExtensions.Partial;

					a.ForEach(k => k.Source = k.Image.Substring(GameReferenceExtensions.Host.Length + 1));

					if (args.Error == null)
						a = a.FromYAML(args.Result);

					this.Dispatcher.Invoke(
						(Action)
						delegate
						{
							var i = 0;
							foreach (var k_ in a)
							{
								var k = k_;


								var Container = new Canvas
								{
									Width = GameReferenceExtensions.Width,
									Height = GameReferenceExtensions.Height,
								}.AttachTo(Content).MoveTo(0, i * GameReferenceExtensions.Height);


								var Image = new Image
								{
									Width = GameReferenceExtensions.Width,
									Height = GameReferenceExtensions.Height,

									Stretch = Stretch.Fill
								}.AttachTo(Container);

								var Shadow = new Rectangle
								{
									Fill = Brushes.Black,
									Opacity = 0.5,
									Width = GameReferenceExtensions.Width - 2 * 2,
									Height = GameReferenceExtensions.Height / 3,

								}.AttachTo(Container).MoveTo(2, GameReferenceExtensions.Height / 3);

								var ShadowBottom = new Rectangle
								{
									Fill = Brushes.Black,
									Width = GameReferenceExtensions.Width - 2 * 2,
									Height = GameReferenceExtensions.Height / 3 / 2,
								}.AttachTo(Container).MoveTo(2, GameReferenceExtensions.Height / 3 + GameReferenceExtensions.Height / 3 / 2);

								var AnimatedShadow = Shadow.ToAnimatedOpacity();
								AnimatedShadow.Opacity = 0;

								var AnimatedShadowBottom = ShadowBottom.ToAnimatedOpacity();
								AnimatedShadowBottom.Opacity = 0;

								var Text = new TextBox
								{
									BorderThickness = new Thickness(0),
									Width = GameReferenceExtensions.Width * 2,
									Background = Brushes.Transparent,
									Foreground = Brushes.White,
									IsReadOnly = true,
									Text = k.Title,
									TextAlignment = TextAlignment.Center,
									FontSize = 10,
									FontFamily = new FontFamily("Verdana")
								}.AttachTo(Container).MoveTo(-GameReferenceExtensions.Width / 2, 8 + GameReferenceExtensions.Height / 3);
								Text.Hide();


								var AnimatedImage = Container.ToAnimatedOpacity();

								AnimatedImage.Opacity = 0;

								ShowImageWhenReady(k, Image, Text, AnimatedImage);





								var TouchOverlay = new Rectangle
								{
									Fill = Brushes.White,
									Width = GameReferenceExtensions.Width,
									Height = GameReferenceExtensions.Height,
									Cursor = Cursors.Hand,
									Opacity = 0
								}.AttachTo(TouchOverlayContainer).MoveTo(0, i * GameReferenceExtensions.Height);



								TouchOverlay.MouseLeftButtonUp +=
									delegate
									{
										new Uri(k.Link).NavigateTo();
									};

								List.Add(new Entry
								{
									Shadow = AnimatedShadow,
									ShadowBottom = AnimatedShadowBottom,
									Text = Text,
									Canvas = Container,
									Image = Image,
									TouchOverlay = TouchOverlay
								});


								i++;
							}

							StartAnimation(List);
						}
					);
				};
			#endregion


			wc.DownloadStringAsync(new Uri(GameReferenceExtensions.Host + GameReferenceExtensions.Path));


		}

		private static void ShowImageWhenReady(GameReference k, Image Image, TextBox Text, AnimatedOpacity AnimatedImage)
		{
			if (k.Source == null)
			{
				var s = new BitmapImage();

				//Text.Text = "loading...";

				s.DownloadCompleted +=
					delegate
					{
						//Text.Text = "done!";
						//Image.Opacity = 1;

						AnimatedImage.Opacity = 1;
					};

				s.BeginInit();
				s.UriSource = new Uri(k.Image);
				s.EndInit();

				Image.Source = s;
				Text.Foreground = Brushes.Yellow;

			}
			else
			{
				//Text.Text = "done!!";
				Image.Source = k.Source.ToSource();
				//Image.Opacity = 1;
				AnimatedImage.Opacity = 1;
			}
		}

		private void StartAnimation(List<Entry> List)
		{
			double y = new Random().NextDouble() * List.Count * 3;

			const double maxspeed = 0.1;
			var aspeed = new AnimatedDouble(-maxspeed);
			var bspeed = new AnimatedDouble(1);

			this.MouseEnter +=
				delegate
				{
					bspeed.SetTarget(0.4);

					foreach (var k in List)
					{
						k.Shadow.Opacity = 0.5;
						k.ShadowBottom.Opacity = 0.7;
						k.Text.Show();
					}
				};

			this.MouseLeave +=
				delegate
				{
					bspeed.SetTarget(1);

					foreach (var k in List)
					{
						k.Shadow.Opacity = 0;
						k.ShadowBottom.Opacity = 0;
						k.Text.Hide();
					}
				};

			(1000 / 30).AtInterval(
				delegate
				{
					y += aspeed.Value * bspeed.Value * 0.1;

					if (aspeed.Value > 0)
						if (y > List.Count * 3)
						{
							aspeed.SetTarget(maxspeed / 2);


							300.AtDelay(
								() => aspeed.SetTarget(-maxspeed / 2)
							);

							500.AtDelay(
								() => aspeed.SetTarget(-maxspeed)
							);
						}

					if (aspeed.Value < 0)
						if (y < 0)
						{
							aspeed.SetTarget(-maxspeed / 2);

							300.AtDelay(
								() => aspeed.SetTarget(maxspeed / 2)
							);

							500.AtDelay(
								() => aspeed.SetTarget(maxspeed)
							);
						}

					List.ForEach(
						(Entry c, int i) =>
						{
							if (Convert.ToInt32(Math.Floor(y)) % 4 > 1)
							{
								var qy = ((i + 1) * GameReferenceExtensions.Height + GameReferenceExtensions.Height * y) % (GameReferenceExtensions.Height * List.Count);
								qy -= GameReferenceExtensions.Height;
								c.Canvas.MoveTo(0, qy);
								c.TouchOverlay.MoveTo(0, qy);
							}
							else
							{
								var qx = ((i + 1) * GameReferenceExtensions.Width + GameReferenceExtensions.Width * y) % (GameReferenceExtensions.Width * List.Count);
								qx -= GameReferenceExtensions.Width;
								c.Canvas.MoveTo(qx, 0);
								c.TouchOverlay.MoveTo(qx, 0);

							}
						}
					);

				}
			);
		}
	}
}
