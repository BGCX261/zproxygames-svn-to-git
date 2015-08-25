using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;

namespace DebugExperienceSinglePlayer.Window
{
	public class TheCanvas : Canvas
	{

		public TheCanvas()
		{
			this.Width = 200;
			this.Height = 200;

			var Overlay = new Rectangle
			{
				Width = 200,
				Height = 200,
				Fill = Brushes.Yellow,
			};

			this.Children.Add(Overlay);

			Overlay.MouseLeftButtonUp +=
				(sender, e) =>
				{
					var p = e.GetPosition(this);

					var r = new Rectangle
					{
						Width = 16,
						Height = 16,
						Fill = Brushes.Red,
						
					};

					this.Children.Add(r);
					Canvas.SetLeft(r, p.X - 8);
					Canvas.SetTop(r, p.Y - 8);
				};
		}

		
	}

	public class Property<T>
	{
		public T Value;
	}

	public static class TheCanvasExtensions
	{
		public static void OpenWindow()
		{
			var w = new System.Windows.Window();

			w.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
			w.Content = new TheCanvas();

			w.ShowDialog();
		}
	}
}
