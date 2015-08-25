using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using AvalonMinesweeper.Core.Library;
using AvalonMinesweeper.Promotion;
using Bulldog.Client.Promotion.Controls;
using ScriptCoreLib.Shared.Avalon.Cursors;
using ScriptCoreLib.Shared.Avalon.Extensions;
using ScriptCoreLib.Shared.Lambda;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Media.Effects;

namespace AvalonMinesweeper.Core
{
	partial class AvalonMinesweeperCanvas
	{


		public virtual bool IsBackgroundTransparent
		{
			get
			{
				return false;
			}
		}

		public virtual string BuildInformation
		{
			get
			{
				return ".net";
			}
		}

		const int ScrollMargin = 64;
		Rectangle ScrollTrap;

		//const int Parallax1Width = DefaultWidth + 100;
		//const int Parallax1Height = DefaultHeight + 100;
		//Canvas Parallax1;

		private void InitializeBackground()
		{

			//this.Parallax1 = new Canvas
			//{

			//}.SizeTo(Parallax1Width, Parallax1Height).AttachTo(this);

			var TileSize = 96;
			var TilesAtBorder = 10;

			#region tiled background
			if (!IsBackgroundTransparent)
			{
			

				var BackgroundColors = new[]
				{
					Brushes.Orange,
					Brushes.White,
					Brushes.Gray,
					Brushes.DarkGray,
					Brushes.Tan,
					Brushes.Yellow,
					Brushes.YellowGreen,
					Brushes.Red,
					Brushes.DarkGreen,
					Brushes.PaleVioletRed
				};

				var BackgroundFill = new Rectangle
				{
					Width = DefaultWidth + 2 * TileSize * TilesAtBorder,
					Height = DefaultHeight + 2 * TileSize * TilesAtBorder,
					Fill = BackgroundColors.Random()
				}.MoveTo(-TileSize * TilesAtBorder, -TileSize * TilesAtBorder).AttachTo(this);
			}

				for (int x = -TileSize * TilesAtBorder; x < DefaultWidth + 2 * TileSize * TilesAtBorder; x += TileSize)
					for (int y = -TileSize * TilesAtBorder; y < DefaultHeight + 2 * TileSize * TilesAtBorder; y += TileSize)
					{
						//var n = ("assets/AvalonMinesweeper.Core/bg_red.png").ToImage(TileSize, TileSize).MoveTo(x, y).AttachTo(this);
						var n = ("assets/AvalonMinesweeper.Core/bg_a50.png").ToImage(TileSize, TileSize).MoveTo(x, y).AttachTo(this);

						//var n = ("assets/AvalonMinesweeper.Core/bg_lightgreen.png").ToImage(TileSize, TileSize).MoveTo(x, y).AttachTo(this);
						//var n = ("assets/AvalonMinesweeper.Core/bg.png").ToImage(TileSize, TileSize).MoveTo(x, y).AttachTo(this);
					}
			
			#endregion

			// http://forums.somethingawful.com/showthread.php?threadid=2929126&userid=0&perpage=40&pagenumber=29
			// http://forums.somethingawful.com/showthread.php?threadid=2929126&userid=0&perpage=40&pagenumber=33

			// http://bruno.mirror.waffleimages.com/files/ae/aecdf0451bd36998d40e77c4cb7d8f3d022cc1b2.png
			// http://bruno.mirror.waffleimages.com/files/78/784fa6431868f96af926637a39ee047f1e377a45.png
			// http://forums.somethingawful.com/showthread.php?threadid=2929126&userid=0&perpage=40&pagenumber=36

			#region zak


			// parrallax effect?
			// allow to buy zak?
			// http://www.pixeldam.net/project.asp?id=586
			//("assets/AvalonMinesweeper.Core/zak.png").ToImage(ZakWidth, ZakHeight).MoveTo((DefaultWidth - ZakWidth) / 2, (DefaultHeight - ZakHeight) / 2).AttachTo(this);
			var PossibleBackgroundScripts = new Action[]
			{
				delegate
				{

					const int ZakWidth = 799;
					const int ZakHeight = 467;

					("assets/AvalonMinesweeper.Core/zak_a.png").ToImage(ZakWidth, ZakHeight).MoveTo((-ZakWidth) *0.25, (-ZakHeight) / 2).AttachTo(this);
					("assets/AvalonMinesweeper.Core/zak_a.png").ToImage(ZakWidth, ZakHeight).MoveTo(DefaultWidth + (-ZakWidth) * 0.75, DefaultHeight + (-ZakHeight) / 2).AttachTo(this);

				},
				delegate
				{

					// http://www.mixnmojo.com/features/read.php?article=zakmckracken&page=1
					// http://www.reloaded.org/download/New-Adventures-Zak-McKracken/28/
					// http://www.zak-site.com/fun.htm
					// http://www.zak-site.com/fun.htm

					const int StargateWidth = 541;
					const int StargateHeight = 375;

					("assets/AvalonMinesweeper.Core/stargate.png").ToImage(StargateWidth, StargateHeight).MoveTo((-StargateWidth) * 0.25, (-StargateHeight) / 2).AttachTo(this);
					("assets/AvalonMinesweeper.Core/stargate.png").ToImage(StargateWidth, StargateHeight).MoveTo(DefaultWidth + (-StargateWidth) * 0.75, DefaultHeight + (-StargateHeight) / 2).AttachTo(this);
					
				}
			};

			PossibleBackgroundScripts.Random()();


			#endregion


			//            this.ScrollTrap = new Rectangle
			//            {
			//                Fill = Brushes.White,
			//                Width = DefaultWidth - ScrollMargin * 2,
			//                Height = DefaultHeight - ScrollMargin * 2,
			//#if DEBUG
			//                Opacity = 0.2
			//#else
			//                Opacity = 0
			//#endif

			//            }.MoveTo(ScrollMargin, ScrollMargin).AttachTo(this);


			#region jsc logo
			("assets/AvalonMinesweeper.Core/jsc.png").ToImage(96, 96).MoveTo(
				(DefaultWidth - (96)),
				(DefaultHeight - (96))
			).AttachTo(this);
			#endregion




			#region Notice
			var Notice = new TextBox
			{
				IsReadOnly = true,
				Background = Brushes.Transparent,
				BorderThickness = new Thickness(0),
				Foreground = Brushes.Red,
				Text = Info.Title + " (" + BuildInformation + ")" + " is still under development. Please do not redistribute this game!",
				TextAlignment = TextAlignment.Center,
				Width = DefaultWidth,
				Height = 32,
				FontFamily = new FontFamily("Verdana")
			}.AttachTo(this).MoveTo(0, DefaultHeight - 32);
            //Notice.BitmapEffect = new DropShadowBitmapEffect();

			#endregion

			#region Title
			var TitleShadow = new TextBox
			{
				IsReadOnly = true,
				Background = Brushes.Transparent,
				BorderThickness = new Thickness(0),
				Foreground = Brushes.Black,
				Text = Info.Title,
				TextAlignment = TextAlignment.Center,
				Width = DefaultWidth,
				Height = 40,
				FontFamily = new FontFamily("Verdana"),
				FontSize = 30
			}.AttachTo(this).MoveTo(0 + 1, DefaultHeight - 32 - 40 + 1);

			var Title = new TextBox
			{
				IsReadOnly = true,
				Background = Brushes.Transparent,
				BorderThickness = new Thickness(0),
				Foreground = Brushes.White,
				Text = Info.Title,
				TextAlignment = TextAlignment.Center,
				Width = DefaultWidth,
				Height = 40,
				FontFamily = new FontFamily("Verdana"),
				FontSize = 30
			}.AttachTo(this).MoveTo(0, DefaultHeight - 32 - 40);
			#endregion

			#region Loading
			var LoadingShadow = new TextBox
			{
				IsReadOnly = true,
				Background = Brushes.Transparent,
				BorderThickness = new Thickness(0),
				Foreground = Brushes.Black,
				Text = "Loading...",
				TextAlignment = TextAlignment.Center,
				Width = DefaultWidth,
				Height = 40,
				FontFamily = new FontFamily("Verdana"),
				FontSize = 20
			}.AttachTo(this).MoveTo(0 + 1, DefaultHeight / 2 - 40 + 1);

			var Loading = new TextBox
			{
				IsReadOnly = true,
				Background = Brushes.Transparent,
				BorderThickness = new Thickness(0),
				Foreground = Brushes.White,
				Text = "Loading...",
				TextAlignment = TextAlignment.Center,
				Width = DefaultWidth,
				Height = 40,
				FontFamily = new FontFamily("Verdana"),
				FontSize = 20
			}.AttachTo(this).MoveTo(0, DefaultHeight / 2 - 40);
			#endregion

			var Status = new TextBox
			{
				IsReadOnly = true,
				Background = Brushes.Transparent,
				BorderThickness = new Thickness(0),
				Foreground = Brushes.Yellow,
				Text = BuildInformation,
				TextAlignment = TextAlignment.Center,
				Width = DefaultWidth,
				Height = 20,
				FontFamily = new FontFamily("Verdana")
			}.AttachTo(this).MoveTo(0, 8);

            //Status.Effect = new DropShadowEffect();

			this.Status = new Action<string>(t => Status.Text = t);
		}

		public Property<string> Status;
	}

}
