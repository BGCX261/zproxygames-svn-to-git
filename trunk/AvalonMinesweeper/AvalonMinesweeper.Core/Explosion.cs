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

namespace AvalonMinesweeper.Core
{
	public class Explosion
	{
		public static void Create(int _x, int _y, Canvas Content)
		{
			Create(_x, _y, Content, 64);
		}

		public static void Create(int _x, int _y, Canvas Content, double size)
		{
			("assets/AvalonMinesweeper.Core/explosion.mp3").PlaySound();

			var exc = new Canvas
			{
				Width = size,
				Height = size
			}.MoveTo(_x - size / 2, _y - size / 2).AttachTo(Content);

			Enumerable.Range(1, 16).ForEach(
				(c, i, next) =>
				{

					var n = ("" + c).PadLeft(2, '0');


					var ex = new Image
					{
						Source = ("assets/AvalonMinesweeper.Core/ani6_" + n + ".png").ToSource()
					}.SizeTo(size, size).AttachTo(exc);

					(1000 / 15).AtDelay(
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
	}
}
