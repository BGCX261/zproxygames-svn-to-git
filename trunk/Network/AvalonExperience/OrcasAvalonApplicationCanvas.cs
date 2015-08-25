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
using ScriptCoreLib.CSharp.Avalon.Extensions;
using ScriptCoreLib.Shared.Lambda;

namespace AvalonExperience.Shared
{
	[Script]
	public class OrcasAvalonApplicationCanvas : InteractiveMatrixTransform.Shared.OrcasAvalonApplicationCanvas
	{
		public const int DefaultWidth = 800;
		public const int DefaultHeight = 400;

		[Script]
		public class AtArgumentsTuple
		{
			public int X;
			public int Y;
			public int Z;

			public AtArgumentsTuple Next;

			public void Apply(object Sender, OrcasAvalonApplicationCanvas World, Action<Action> AddDispose)
			{
				var Settings = new[] { 
					new {Brush = Brushes.Red, Opacity = 0.5 },
					new {Brush = Brushes.Yellow, Opacity = 0.5 },
					new {Brush = Brushes.Purple, Opacity = 0.2 },
				}.AtModulus(Z);

				var a = new Rectangle
				{
					Width = 4,
					Height = 8,
					Fill = Settings.Brush,
					Opacity = Settings.Opacity
				}.AttachTo(World).MoveTo(X + 2, Y);

				var b = new Rectangle
				{
					Width = 8,
					Height = 4,
					Fill = Settings.Brush,
					Opacity = Settings.Opacity
				}.AttachTo(World).MoveTo(X, Y + 2);

				AddDispose(() => a.Orphanize());
				AddDispose(() => b.Orphanize());

				if (Next != null)
					Next.Apply(Sender, World, AddDispose);
			}
		}

		public Action<object, AtArgumentsTuple> AtArguments;
		public Action<object> AtClear;

		public OrcasAvalonApplicationCanvas()
		{
			Width = DefaultWidth;
			Height = DefaultHeight;

			var Disposeables = new List<Action>();


			this.AtArguments = (s, a) => this.Dispatcher.Invoke(new Action(() => a.Apply(s, this, Disposeables.Add)));
			this.AtClear =
				delegate
				{
					this.Dispatcher.Invoke(
						new Action(
							delegate
							{
								foreach (var a in Disposeables)
								{
									a();
								}

								Disposeables.Clear();
							}
						)
					);
				};

			this.ClipToBounds = true;

			//Colors.Blue.ToGradient(Colors.Black, DefaultHeight / 4).Select(
			//    (c, i) =>
			//        new Rectangle
			//        {
			//            Fill = new SolidColorBrush(c),
			//            Width = DefaultWidth,
			//            Height = 4,
			//        }.MoveTo(0, i * 4).AttachTo(this)
			//).ToArray();

			var ClearButton = new Rectangle
			{
				Width = 64,
				Height = 64,
				Fill = Brushes.Black,
				Opacity = 0.7
			}.AttachTo(this);

			var Mouse = default(AtArgumentsTuple);

			this.MouseLeftButtonDown +=
				(s, a) =>
				{
					var p = a.GetPosition(this);

					Mouse = new AtArgumentsTuple { X = (int)p.X, Y = (int)p.Y };
				};


			this.MouseLeftButtonUp +=
				(s, a) =>
				{

					var p = a.GetPosition(this);

					if (p.X < 64)
						if (p.Y < 64)
						{
							Mouse = null;
							this.AtClear(null);
							return;
						}

					Mouse = new AtArgumentsTuple { X = (int)p.X, Y = (int)p.Y };
					Mouse = null;
				};

#if DEBUG
			this.StylusInAirMove +=
				(s, a) =>
				{
					var c = a.GetStylusPoints(this);

					foreach (var p in c)
					{
						var pp = new AtArgumentsTuple { X = (int)p.X, Y = (int)p.Y, Next = Mouse, Z = 2 };


						this.AtArguments(null, pp);
					}

					a.Handled = true;
				};

			this.StylusMove +=
				(s, a) =>
				{
					var c = a.GetStylusPoints(this);

					foreach (var p in c)
					{
						var pp = new AtArgumentsTuple { X = (int)p.X, Y = (int)p.Y, Next = Mouse, Z = 1 };


						this.AtArguments(null, pp);
					}

					a.Handled = true;
				};
#endif

			this.MouseMove +=
				(s, a) =>
				{
					return;

					if (Mouse != null)
					{
						var p = a.GetPosition(this);

						// we are sending multiple drawn commands...
						Mouse = new AtArgumentsTuple { X = (int)p.X, Y = (int)p.Y, Next = Mouse };

						Mouse.Next = new AtArgumentsTuple { X = DefaultWidth - Mouse.X, Y = DefaultHeight - Mouse.Y };

						this.AtArguments(null, Mouse);
					}
				};

		}




	}
}
