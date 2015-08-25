using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ScriptCoreLib;
using ScriptCoreLib.Shared.Avalon.Carousel;
using ScriptCoreLib.Shared.Avalon.Extensions;
using ScriptCoreLib.Shared.Avalon.Tween;
using ScriptCoreLib.Shared.Lambda;
using System.ComponentModel;

namespace Bulldog.Client.Promotion.Controls
{
	public class GameMenu : ISupportsContainer, IEnumerable<GameMenu.Option>
	{
		public Canvas Container { get; set; }


		public class Option
		{
			public ImageSource Source;

			public string Text;

			public Action Click;

			public Uri Hyperlink;

			public double MarginBefore;
			public double MarginAfter;

			internal SimpleCarouselControl.EntryInfo CarouselEntry;


		}


		string InternalIdleText = "";
		public string IdleText
		{
			get
			{
				return InternalIdleText;
			}
			set
			{
				InternalIdleText = value;
				this.Carousel.Caption.Text = InternalIdleText;
			}
		}

		internal readonly BindingList<Option> Options = new BindingList<Option>();

		public void Add(Option e)
		{
			Options.Add(e);
		}

		public SimpleCarouselControl Carousel { get; set; }

		public readonly int ContentHeight;

		public GameMenu(int Width, int Height, int ShadowHeight)
		{
			this.ContentHeight = Height * 3 / 4;

			this.Container = new Canvas
			{
				Width = Width,
				Height = ContentHeight + ShadowHeight,
				Name = "GameMenu_Container",

			};

			var ShadowContainer = new Canvas
			{
				Width = Width,
				Height = ContentHeight + ShadowHeight,
				Opacity = 0.5,
				Name = "GameMenu_ShadowContainer",
			}.AttachTo(this.Container);

			new Rectangle
		   {
			   Fill = Brushes.Black,
			   Width = Width,
			   Height = ContentHeight,
		   }.MoveTo(0, 0).AttachTo(ShadowContainer);



			Colors.Black.ToTransparentGradient(ShadowHeight).Select(
				(c, i) =>
				{
					return new Rectangle
					{
						Fill = new SolidColorBrush(c),
						Width = Width,
						Height = 1,
						Opacity = c.A / 255.0
					}.MoveTo(0, ContentHeight + i).AttachTo(ShadowContainer);
				}
			).ToArray();



			this.Carousel = new SimpleCarouselControl(Width, ContentHeight).MoveContainerTo(0, 0);
			
			

			Carousel.Caption.FontSize = 36;
			Carousel.Caption.Height = 60;
			Carousel.Caption.Text = IdleText;

			Action<int, int> AnimatedShadowOpacity = NumericEmitter.Of(
				(x, y) => ShadowContainer.Opacity = x * 0.01
			);

			var ShadowOpacity = 50;

			AnimatedShadowOpacity(ShadowOpacity, 0);


			Carousel.Hover +=
				delegate
				{
					ShadowOpacity = 70;
					AnimatedShadowOpacity(ShadowOpacity, 0);
				};

			Carousel.Idle +=
				delegate
				{
					Carousel.Caption.Text = IdleText;
					ShadowOpacity = 50;
					AnimatedShadowOpacity(ShadowOpacity, 0);
				};

			this.Options.ForEachNewItem(
				e =>
				{
					double p = 0;

					this.Where(k => k != e).LastOrDefault().Apply(k => p = k.CarouselEntry.Position + k.MarginAfter);

					e.CarouselEntry = new SimpleCarouselControl.EntryInfo
					{
						Source = e.Source,
						Text = e.Text,
						Position = p + e.MarginBefore,
						Click =
							delegate
							{
								if (e.Click != null)
									e.Click();

								if (e.Hyperlink != null)
									e.Hyperlink.NavigateTo();
							}
					};

					Carousel.AddEntry(e.CarouselEntry);
				}
			);



			Carousel.AttachContainerTo(this);

			Carousel.Overlay.AttachTo(this);



			Action<int, int> AnimatedTop = NumericEmitter.Of(
				(x, y) =>
				{
					this.Container.MoveTo(0, y);

					if (AfterMove != null)
						AfterMove(x, y);
				}
			);

			this.MoveContainerTo(0, -ContentHeight);
			AnimatedTop(0, -ContentHeight);

			this.Show = delegate
			{
				this.Carousel.Timer.Start();
				this.Carousel.Show();
				AnimatedTop(0, 0);

				AnimatedShadowOpacity(ShadowOpacity, 0);
			};

			this.Hide = delegate
			{
				this.Carousel.Hide();
				this.Carousel.Timer.Stop();

				AnimatedTop(0, -ContentHeight);
				AnimatedShadowOpacity(50, 0);
			};

			var Delayed = this.Container.ToDelayedMouseEvents();

			Delayed.ValidateMouseEnter = () => ValidateShow();
			Delayed.ValidateMouseLeave = () => ValidateHide();
			Delayed.MouseEnter += Show;
			Delayed.MouseLeave += Hide;



			ShadowContainer.Opacity = 0;

		}

		public event Action<int, int> AfterMove;

		public readonly Action Show;
		public readonly Action Hide;

		public Func<bool> ValidateHide = () => true;
		public Func<bool> ValidateShow = () => true;

		#region IEnumerable<Option> Members

		public IEnumerator<GameMenu.Option> GetEnumerator()
		{
			return this.Options.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.Options.GetEnumerator();
		}

		#endregion
	}
}
