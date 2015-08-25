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
using System.Windows.Input;

namespace AvalonBetris.Shared
{
	public class AvalonBetrisCanvas : Canvas
	{
		// http://www.flickr.com/search/show/?q=soviet+appartment
		// http://technabob.com/blog/2009/10/01/berlin-block-tetris-video/

		public const int DefaultWidth = 600;
		public const int DefaultHeight = 478;

		public AvalonBetrisCanvas()
		{
			Width = DefaultWidth;
			Height = DefaultHeight;

			this.ClipToBounds = true;

			Colors.Blue.ToGradient(Colors.White, DefaultHeight / 4).Select(
				(c, i) =>
					new Rectangle
					{
						Fill = new SolidColorBrush(c),
						Width = DefaultWidth,
						Height = 4,
					}.MoveTo(0, i * 4).AttachTo(this)
			).ToArray();


			var t = new TextBox
			{
				FontSize = 10,
				Text = "powered by jsc",
				BorderThickness = new Thickness(0),
				Foreground = 0xffffffff.ToSolidColorBrush(),
				Background = Brushes.Transparent,
				IsReadOnly = true,
				Width = DefaultWidth
			}.MoveTo(8, 8).AttachTo(this);


			var t2 = new TextBox
			{
				FontSize = 10,
				AcceptsReturn = true,
				Text = @"
BETRIS
",
				BorderThickness = new Thickness(0),
				Foreground = 0xffffffff.ToSolidColorBrush(),
				Background = Brushes.Transparent,
				IsReadOnly = true,
				Width = DefaultWidth,
				Height = 128,
			}.MoveTo(8, 32).AttachTo(this);


			var bg = new Image
			{
				Source = ("assets/AvalonBetris/building_block_tetris.png").ToSource()
			}.MoveTo(0, 0).AttachTo(this);

	
			var img = new Image
			{
				Source = ("assets/AvalonBetris/jsc.png").ToSource()
			}.MoveTo(DefaultWidth - 128, DefaultHeight - 128).AttachTo(this);


			

			bg.MouseLeftButtonUp +=
				(sender, args) =>
				{
					var p = args.GetPosition(this);
					t2.AppendTextLine(new { p.X, p.Y }.ToString());
				};

			//help.Opacity = 1;
			img.Opacity = 0.5;

	
			



			var aw = 49;
			var ah = 49;

			var tri = new Image
			{
				Width = aw,
				Height = ah,
				Source = "assets/AvalonBetris/_19a.png".ToSource()
			}.AttachTo(this);


			var trig = new Image
			{
				Width = aw,
				Height = ah,
				Source = "assets/AvalonBetris/_19b.png".ToSource()
			}.AttachTo(this);

			var tri2 = new Image
			{
				Width = aw,
				Height = ah,
				Source = "assets/AvalonBetris/_18a.png".ToSource()
			}.AttachTo(this);


			var trig2 = new Image
			{
				Width = aw,
				Height = ah,
				Source = "assets/AvalonBetris/_18b.png".ToSource()
			}.AttachTo(this);


			// cursor position calculations are not ready
			// for transofrmed elements.
			// we will provide a floor for those events...
			var shadow = new Rectangle
			{
				Width = DefaultWidth,
				Height = DefaultHeight,

				Fill = Brushes.Black,
			}.AttachTo(this);

			var shadowa = shadow.ToAnimatedOpacity();

			shadowa.Opacity = 0;

			Func<Brush, int, int, Movable> m =
				(Color, X, Y) =>
				{
					var m0 = new Movable
					{
						Context = this,
						Color = Color
					};

					m0.MoveTo(X, Y);

					return m0;
				};

			var m1 = m(Brushes.Green, 197, 123);
			var m2 = m(Brushes.Red, 224, 110);
			var m3 = m(Brushes.Blue, 197, 161);
			var m4 = m(Brushes.Yellow, 222, 159);

			var m5 = m(Brushes.Green, 265, 118);
			var m6 = m(Brushes.Blue, 265, 166);



			Action Update =
				delegate
				{


					tri.RenderTransform = new AffineTransform
					{
						Left = 0,
						Top = 0,
						Width = aw,
						Height = ah,

						X1 = m2.X,
						Y1 = m2.Y,

						X2 = m3.X,
						Y2 = m3.Y,

						X3 = m1.X,
						Y3 = m1.Y,

					};


					trig.RenderTransform = new AffineTransform
					{
						Left = 0,
						Top = 0,
						Width = aw,
						Height = ah,

						X1 = m3.X,
						Y1 = m3.Y,

						X2 = m2.X,
						Y2 = m2.Y,

						X3 = m4.X,
						Y3 = m4.Y,

					};

					tri2.RenderTransform = new AffineTransform
					{
						Left = 0,
						Top = 0,
						Width = aw,
						Height = ah,

						X1 = m5.X,
						Y1 = m5.Y,

						X2 = m4.X,
						Y2 = m4.Y,

						X3 = m2.X,
						Y3 = m2.Y,

					};


					trig2.RenderTransform = new AffineTransform
					{
						Left = 0,
						Top = 0,
						Width = aw,
						Height = ah,

						X1 = m4.X,
						Y1 = m4.Y,

						X2 = m5.X,
						Y2 = m5.Y,

						X3 = m6.X,
						Y3 = m6.Y,

					};
				};

			foreach (var k in new[] { m1, m2, m3, m4, m5, m6 })
			{
				k.Container.MouseLeftButtonDown += delegate { shadowa.Opacity = 0.2; };
				k.Container.MouseLeftButtonUp += delegate { shadowa.Opacity = 0; };
				k.Changed += Update;
			}

			Update();
		}


	}
}
