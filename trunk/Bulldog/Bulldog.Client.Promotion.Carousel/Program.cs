using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Bulldog.Client.Promotion.Carousel.Shared;
using ScriptCoreLib.CSharp.Avalon.Extensions;

namespace Bulldog.Client.Promotion.Carousel
{
	class Program
	{

		[STAThread]
		static public void Main(string[] args)
		{
			var w = new OrcasAvalonApplicationCanvas().ToWindow();

			w.Background = Brushes.White;
			w.ShowDialog();
		}
	}
}
