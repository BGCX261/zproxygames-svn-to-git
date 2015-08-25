using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptCoreLib.Shared.Avalon.Extensions;
using System.Windows.Controls;
using ScriptCoreLib.Shared.Avalon.Cursors;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using ScriptCoreLib.Shared.Lambda;

namespace AvalonMinesweeper.Core.Library
{
	public class NamedArrowCursorControl : ISupportsContainer
	{
		public readonly ArrowCursorControl Cursor;
		public Canvas Container { get; set; }
		public readonly TextBox Label;
		public readonly TextBox LabelShadow;

		public Color Color
		{
			set
			{
				this.Cursor.Color = value;
				this.Label.Foreground = new SolidColorBrush(value);
			}
		}

		public string Text
		{
			set
			{
				this.Label.Text = value;
				this.LabelShadow.Text = value;

				this.CreateShadow(value);
			}
		}

		readonly Action<string> CreateShadow;

		public NamedArrowCursorControl()
		{
			this.Container = new Canvas();
			this.Cursor = new ArrowCursorControl().AttachContainerTo(this.Container);

			var ShadowCointainer = new Canvas().AttachTo(this.Container);

			Func<int, Action> CreateShadow =
				ShadowLength => Enumerable.Range(2, ShadowLength).ToOrphanizeAction(
					(c, i) => new Rectangle
					{
						Fill = Brushes.Black,
						Width = 3,
						Opacity = 1.0 - (double)i / (double)ShadowLength,
						Height = 18
					}
					.AttachTo(ShadowCointainer)
					.MoveTo(12 + i * 3, 24)
				);

			//this.CreateShadow = CreateShadow.ToContinuation((string t) => 10 + t.Length);
			this.CreateShadow = t => { };



			this.LabelShadow = new TextBox
			{
				IsReadOnly = true,
				Background = Brushes.Transparent,
				BorderThickness = new Thickness(0),
				Foreground = Brushes.Black,
				Width = 400,
				Height = 18,
				TextAlignment = TextAlignment.Left,
				FontFamily = new FontFamily("Verdana")
			}.AttachTo(Container).MoveTo(12 + 1, 24 + 1);

			this.Label = new TextBox
			{
				IsReadOnly = true,
				Background = Brushes.Transparent,
				BorderThickness = new Thickness(0),
				Foreground = Brushes.White,
				Width = 400,
				Height = 18,
				TextAlignment = TextAlignment.Left,
				FontFamily = new FontFamily("Verdana")
			}.AttachTo(Container).MoveTo(12, 24);


		}
	}
}
