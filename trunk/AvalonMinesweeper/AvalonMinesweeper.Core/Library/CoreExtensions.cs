using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptCoreLib.Shared.Avalon.Extensions;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
namespace AvalonMinesweeper.Core.Library
{
	public static class CoreExtensions
	{


		public static Action<T> ToAction<F, T>(this Action<F> handler, Func<T, F> selector)
		{
			return e => handler(selector(e));
		}

		public static Action<T> ToContinuation<F, T>(this Func<F, Action> handler, Func<T, F> selector)
		{
			return handler.ToContinuation().ToAction(selector);
		}

		public static Action<T> ToContinuation<T>(this Func<T, Action> handler)
		{
			var previous = default(Action);


			return 
				e =>
				{
					if (previous != null)
						previous();

					previous = handler(e);
				};
		}

		public static Action ToOrphanizeAction<T>(this IEnumerable<T> source, Func<T, int, FrameworkElement> selector)
		{
			var a = source.Select(selector).ToArray();

			return () => a.Orphanize();
		}

		public static T WithOpacity<T>(this T e, double Opacity)
			where T : UIElement
		{
			e.Opacity = Opacity;

			return e;
		}

		public static Image ToImage(this string source, int width, int height)
		{
			return new Image
			{
				Source = source.ToSource()
			}.SizeTo(width, height);
		}
	}
}
