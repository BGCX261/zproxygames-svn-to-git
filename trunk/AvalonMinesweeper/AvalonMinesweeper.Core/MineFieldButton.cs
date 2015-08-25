using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptCoreLib.Shared.Avalon.Extensions;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using AvalonMinesweeper.Core.Library;
using System.Windows.Input;
using System.Diagnostics;
using ScriptCoreLib.Shared.Lambda;

namespace AvalonMinesweeper.Core
{
	public class MineFieldButton : ISupportsContainer
	{
		public const int Size = 16;

		public Canvas Container { get; set; }

		public int IndexX;
		public int IndexY;

		public readonly Rectangle Shadow;
		public readonly Rectangle Background;
		public readonly AnimatedOpacity BackgroundOpacity;

		public readonly Image Flag;
		public readonly Image Question;
		public readonly Image Mine;
		public readonly Image Strike;

		public readonly Rectangle TouchRectangle;

		public readonly Property<int> VisibleNumber;
		public readonly Property<bool> IsPressed;

		public bool IsEnabled;

		public readonly Stopwatch ConstructorStopwatch;

		public Func<int, int, MineFieldButton> ByOffset;

		public bool IsMined;

		public Color BackgroundColor
		{
			set
			{
				this.Background.Fill = new SolidColorBrush(value);
			}
		}

		public MineFieldButton()
		{
			ConstructorStopwatch = new Stopwatch();
			ConstructorStopwatch.Start();

			this.Shadow = new Rectangle
			{
				Fill = Brushes.Black,
				Opacity = 0.2,
			}.SizeTo(Size, Size);

			this.Container = new Canvas
			{

			}.SizeTo(Size, Size);

			this.Background = new Rectangle
			{
				Fill = 0xffc0c0c0.ToSolidColorBrush()
			}.SizeTo(Size, Size).AttachTo(Container);

			this.BackgroundOpacity = this.Background.ToAnimatedOpacity();


			this.Flag = ("assets/AvalonMinesweeper.Core/flag_a.png").ToImage(Size, Size).AttachTo(Container);
			this.Question = ("assets/AvalonMinesweeper.Core/question_a.png").ToImage(Size, Size).AttachTo(Container);
			this.Mine = ("assets/AvalonMinesweeper.Core/mine_a.png").ToImage(Size, Size).AttachTo(Container);
			this.Strike = ("assets/AvalonMinesweeper.Core/strike_a.png").ToImage(Size, Size).AttachTo(Container);

			this.Flag.Hide();
			this.Question.Hide();
			this.Mine.Hide();
			this.Strike.Hide();

			this.TouchRectangle = new Rectangle
			{
				Fill = Brushes.Black,
				Opacity = 0,
				Cursor = Cursors.Hand
			}.SizeTo(Size, Size);

			var a = ("assets/AvalonMinesweeper.Core/button_a.png").ToImage(Size, Size).WithOpacity(0.8).AttachTo(Container);
			var b = ("assets/AvalonMinesweeper.Core/button_b.png").ToImage(Size, Size).WithOpacity(0.335).AttachTo(Container);
			var e = ("assets/AvalonMinesweeper.Core/empty_a.png").ToImage(Size, Size).WithOpacity(0.335).AttachTo(Container);
			e.Hide();

			this.IsPressed = new Action<bool>(
				value =>
				{
					a.Show(!value);
					b.Show(!value);
					e.Show(value);
				}
			);

			this.VisibleNumber = new Func<int, Action>(
				n =>
				{
					// if the following expressions were combined
					// jsc would emit: if (!(((n < 1)) ? true : !(n < 9)))
					// which will make mxmlc complain
					// jsc could be updated to actually detect short-circut expression

					if (n < 1)
						return null;

					if (n > 8)
						return null;

					var i = ("assets/AvalonMinesweeper.Core/a" + n + ".png").ToImage(Size, Size).AttachTo(Container);

					return () => i.Orphanize();
				}
			).ToContinuation();

			var CanClick = false;
			this.TouchRectangle.MouseLeftButtonDown +=
				delegate
				{
					if (!IsEnabled)
						return;

					CanClick = true;
					IsPressed.Value = true;
				};

			this.TouchRectangle.MouseLeave +=
				(sender, args) =>
				{
					if (!CanClick)
						return;

					CanClick = false;
					IsPressed.Value = false;
				};

			this.TouchRectangle.MouseLeftButtonUp +=
				(sender, args) =>
				{
					if (!CanClick)
						return;

					CanClick = false;
					IsPressed.Value = false;

					if (this.Click != null)
						this.Click(this, args);
				};

			this.IsEnabled = true;

			ConstructorStopwatch.Stop();
		}

		public event Action<MineFieldButton, MouseEventArgs> Click;

		public MineFieldButton[] CachedNeighbours;
		public IEnumerable<MineFieldButton> Neighbours
		{
			get
			{
				if (CachedNeighbours != null)
					return CachedNeighbours.AsEnumerable();

				Func<int, int, Func<MineFieldButton>> f =
					(x, y) => this.ByOffset.FixFirstParam(x).FixParam(y);

				return new[]
				{
					f(-1, -1),
					f(-1, 0),
					f(-1, 1),
					
					f(0, 1),
					f(1, 1),

					f(1, 0),
					f(1, -1),
					f(0, -1),
				}.Select(k => k()).Where(k => k != null);
			}
		}

		public MineFieldButton[] CachedRegion;
		public IEnumerable<MineFieldButton> Region
		{
			get
			{
				if (CachedRegion != null)
					return CachedRegion.AsEnumerable();

				var a = new List<MineFieldButton>();
				var s = new Stack<MineFieldButton>();

				Action<MineFieldButton> Consider =
					x =>
					{
						if (a.Contains(x))
							return;

						if (x.IsMined)
							return;

						a.Add(x);

						var n = x.Neighbours.ToArray();

						if (n.Any(k => k.IsMined))
							return;

						foreach (var k in n)
						{
							s.Push(k);
						}
					};

				s.Push(this);
				while (s.Count > 0)
					Consider(s.Pop());

				return a.Where(k => !k.IsMined);
			}
		}


	}
}
