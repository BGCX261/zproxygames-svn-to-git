using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media;
using ScriptCoreLib.Shared.Lambda;
using ScriptCoreLib.Shared.Avalon.Extensions;
using AvalonMinesweeper.Core.Library;

namespace AvalonMinesweeper.Core
{
	public class MineMapLoader
	{
		public readonly BindingList<MineFieldButton> Context;

		public class Entry
		{
			public int IndexX;
			public int IndexY;
			public Color BackgroundColor = Colors.Gray;
			public bool IsMined;
		}

		public readonly Action<IEnumerable<Entry>, Action> Prepare;

		// lets introduce a limitation of maximum map size
		readonly Array2D<MineFieldButton> Lookup = new Array2D<MineFieldButton>(1024, 1024);

		public MineMapLoader(BindingList<MineFieldButton> Context)
		{
			this.Context = Context;

			const int ProgressbarWidth = 16;

			var Progressbar = Enumerable.Range(0, ProgressbarWidth).ToArray(
				i =>
				{
					var k = new MineFieldButton
					{
						IndexX = i - ProgressbarWidth / 2,
						IndexY = 0,
						IsEnabled = false,
						BackgroundColor = Colors.Gray
					};



					Context.Add(k);

					return k;
				}
			);

			var Gradient = new[] {
				Colors.Red,
				Colors.Gray,
				Colors.Gray
			}.ToGradient(Progressbar.Length);

			var ProgressbarTimer = (1000 / 24).AtIntervalWithCounter(
				c =>
				{
					for (int i = 0; i < Progressbar.Length; i++)
					{
						Progressbar[Progressbar.Length - 1 - i].BackgroundColor = Gradient.AtModulus(c + i);
					}
				}
			);

			Action<double, Color> SetPercentage =
				(value, color) =>
				{
					var k = Math.Ceiling(ProgressbarWidth * value);

					for (int i = 0; i < Progressbar.Length; i++)
					{
						if (i < k)
							Progressbar[i].BackgroundColor = color;
						else
							Progressbar[i].BackgroundColor = Colors.Gray;
					}
				};

			const int ChunkLoad = 8;


			Action<MineFieldButton[], Action> PrepareCache =
				(a, done) =>
				{
					var n = new List<MineFieldButton>();
					a.ForEach(
						(j, next) =>
						{
							j.CachedNeighbours = j.Neighbours.ToArray();
							j.CachedRegion = j.Region.ToArray();

							n.Add(j);
							SetPercentage((double)n.Count / (double)a.Length, Colors.Red);


							if (n.Count % ChunkLoad == 0)
								1.AtDelay(next);
							else
								next();
						}
					)(
						delegate
						{
							foreach (var k in Progressbar)
							{
								Context.Remove(k);
							}

							Context.AddRange(a.ToArray());

							done();
						}
					);
				};

			this.Prepare =
				(i, done) =>
				{
					ProgressbarTimer.Stop();

					foreach (var k in Progressbar)
					{
						k.BackgroundColor = Colors.Gray;
					}

					var a = i.ToArray();
					var n = new List<MineFieldButton>();


					a.ForEach(
						(j, next) =>
						{
							var k = new MineFieldButton
							{
								IndexX = j.IndexX,
								IndexY = j.IndexY,
								BackgroundColor = j.BackgroundColor,
								IsMined = j.IsMined
							};

							var lx = j.IndexX + this.Lookup.XLength / 2;
							var ly = j.IndexY + this.Lookup.YLength / 2;

							this.Lookup[lx, ly] = k;

							k.ByOffset = (x, y) => this.Lookup[lx + x, ly + y];

							n.Add(k);
							SetPercentage((double)n.Count / (double)a.Length, Colors.Blue);


							if (n.Count % ChunkLoad == 0)
								1.AtDelay(next);
							else
								next();
						}
					)(
						delegate
						{
							PrepareCache(n.ToArray(), done);
						}
					);
				};
		}
	}
}
