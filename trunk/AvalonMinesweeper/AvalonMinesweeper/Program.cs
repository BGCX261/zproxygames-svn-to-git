using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ScriptCoreLib.CSharp.Avalon.Extensions;
using AvalonMinesweeper.Core;

namespace AvalonMinesweeper
{
	class Program
	{
		public class Canvas : AvalonMinesweeperCanvas
		{
			public override bool IsBackgroundTransparent
			{
				get
				{
#if transparent
					return true;
#else
					return false;
#endif
				}
			}
		}

		[STAThread]
		static public void Main(string[] args)
		{
			var c = new Canvas();

			var w = c.ToWindow();

			w.WithGlass();
			w.ShowDialog();
		}
	}
}
