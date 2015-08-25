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
	partial class GameSocialLinks
	{

		public class Button
		{
			public ImageSource Source;

			public int Width;
			public int Height;

			public Action Click;

			public Uri Hyperlink;

			internal Image Image;
			internal Rectangle Overlay;

			public int X;
			public int Y;
		}

		public class AddToGoogleButton : Button
		{
			public AddToGoogleButton()
			{
				Source = ("assets/Bulldog.Client.Promotion.Controls/plus_google.png").ToSource();
				Width = 62;
				Height = 17;
			}
		}

		public class TikiButton : Button
		{
			public TikiButton()
			{
				Source = ("assets/Bulldog.Client.Promotion.Controls/tk.png").ToSource();
				Width = 16;
				Height = 16;
			}
		}

		public class StumbleUponButton : Button
		{
			public StumbleUponButton()
			{
				Source = ("assets/Bulldog.Client.Promotion.Controls/su.png").ToSource();
				Width = 16;
				Height = 16;
			}
		}

		public class BlogFeedButton : Button
		{

			public BlogFeedButton()
			{
				Source = ("assets/Bulldog.Client.Promotion.Controls/rss.png").ToSource();
				Width = 14;
				Height = 14;
			}
		}
	}
}
