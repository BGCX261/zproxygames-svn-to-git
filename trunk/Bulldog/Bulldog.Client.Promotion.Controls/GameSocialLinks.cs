using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.ComponentModel;
using ScriptCoreLib.Shared.Lambda;
using ScriptCoreLib.Shared.Avalon.Extensions;
using System.Windows.Media;
using System.Windows.Input;

namespace Bulldog.Client.Promotion.Controls
{
	public partial class GameSocialLinks : ISupportsContainer, IEnumerable<GameSocialLinks.Button>
	{
		public Canvas Container { get; set; }


	
		public readonly BindingList<Button> Buttons = new BindingList<Button>();

		public int Margin = 8;

		public int OffsetY
		{
			set
			{
				foreach (var e in Buttons)
				{
					e.Image.MoveTo(e.X, e.Y + value);
					e.Overlay.MoveTo(e.X, e.Y + value);
				}
			}
		}
		public GameSocialLinks(Canvas Container)
		{
			this.Container = Container;

			Buttons.ForEachNewItem(
				e =>
				{
					var i = true;
					//var x = Buttons.Where(k => i).Aggregate(Margin,
					//    (s, k) =>
					//    {
					//        if (k == e)
					//        {
					//            i = false;
					//            return s;
					//        }

					//        return s + k.Width + Margin;
					//    }
					//);

					//var y = this.Container.Height - Margin - e.Height;

					e.Y = Buttons.Where(k => i).Aggregate(Margin,
						(s, k) =>
						{
							if (k == e)
							{
								i = false;
								return s;
							}

							return s + k.Height + Margin;
						}
					);


					e.X = Margin;

					e.Image = new Image
					{
						Source = e.Source,
						Width = e.Width,
						Height = e.Height,
						Name = "GameSocialLinks_Button_Image",
						Opacity = 0.6
					}.AttachTo(this).MoveTo(e.X, e.Y);

					e.Overlay = new Rectangle
					{
						Fill = Brushes.White,
						Width = e.Width,
						Height = e.Height,
						Opacity = 0,
						Cursor = Cursors.Hand,
						Name = "GameSocialLinks_Button_Overlay",
					}.AttachTo(this).MoveTo(e.X, e.Y);

					var ImageOpacity = e.Image.ToAnimatedOpacity();
					ImageOpacity.Opacity = 0.4;

					e.Overlay.MouseEnter +=
						delegate
						{
							ImageOpacity.Opacity = 1;
						};

					e.Overlay.MouseLeave +=
						delegate
						{
							ImageOpacity.Opacity = 0.4;
						};

					e.Overlay.MouseLeftButtonUp +=
						delegate
						{
							if (e.Click != null)
								e.Click();

							if (e.Hyperlink != null)
								e.Hyperlink.NavigateTo();
						};
				}

			);
		}

		public void Add(Button e)
		{
			this.Buttons.Add(e);
		}

		#region IEnumerable<Button> Members

		public IEnumerator<Button> GetEnumerator()
		{
			return this.Buttons.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.Buttons.GetEnumerator();
		}

		#endregion
	}
}
