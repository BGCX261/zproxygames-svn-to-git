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
using ScriptCoreLib.Shared.Lambda;

namespace AvalonSkillGame.Shared
{
	[Script]
	public class OrcasAvalonApplicationCanvas : Canvas
	{
		public const int DefaultWidth = 930;
		public const int DefaultHeight = 554;
		// http://www.gamecolony.com/no_gambling.html
		// http://en.wikipedia.org/wiki/Online_skill-based_game
		[Script]
		public class SCImage
		{
			public readonly Image Image;
			public readonly Image ClosureImage;

			public SCImage(int Sequence, int Width, int Height)
			{
				this.ClosureImage = new Image
				{

					Source = ("assets/AvalonSkillGame/SC" + Sequence + ".jpg").ToSource(),
					Width = DefaultWidth,
					Height = DefaultHeight

				};

				this.Image = new Image
				{

					Source = ("assets/AvalonSkillGame/SC" + Sequence + "z.jpg").ToSource(),
					Width = DefaultWidth,
					Height = DefaultHeight

				};

				this.ClosureImage.MouseLeftButtonUp +=
					delegate
					{
						this.ClosureImage.Hide();
						this.Image.Show();
					};

				this.Image.MouseLeftButtonUp +=
					delegate
					{
						this.Image.Hide();
						this.ClosureImage.Show();
					};
			}
		}
		public OrcasAvalonApplicationCanvas()
		{
			Width = DefaultWidth;
			Height = DefaultHeight;

			this.ClipToBounds = true;

			var Images = new[]
			{
				//new SCImage ( 334, 558, 937 ),
				
				new SCImage ( 333, 840, 1410 ),
				
				new SCImage ( 332, 840, 1410 ),
				new SCImage ( 331, 840, 1410 ),
				new SCImage ( 330, 840, 1410 ),
				new SCImage ( 329, 860, 1410 ),
				new SCImage ( 328, 560, 940 ),
				new SCImage ( 327, 700, 900 ),
				new SCImage ( 325, 700, 900 ),
				new SCImage ( 324, 700, 900 ),
				new SCImage ( 323, 700, 900 ),
				new SCImage ( 322, 700, 900 ),
				new SCImage ( 321, 700, 900 ),
				new SCImage ( 309, 700, 900 ),
				new SCImage ( 308, 700, 900 ),
				new SCImage ( 306, 703, 903 ),
				new SCImage ( 305, 768, 1024 ),
				new SCImage ( 303, 768, 1024 ),
				new SCImage ( 302, 703, 937 ),
				new SCImage ( 300, 768, 1024 ),
				new SCImage ( 289, 790, 1024 ),
				new SCImage ( 287, 842, 1024 ),
				new SCImage ( 286, 722, 1024 ),
				new SCImage ( 284, 933, 1024 ),
				new SCImage ( 272, 781, 1000 ),
				new SCImage ( 271, 746, 1000 ),
				new SCImage ( 269, 838, 1000 ),
				new SCImage ( 267, 768, 983 ),
				new SCImage ( 265, 768, 949 ),
				new SCImage ( 253, 777, 1020 ),
				new SCImage ( 247, 692, 1020 ),
				new SCImage ( 244, 768, 951 ),
				new SCImage ( 243, 746, 1020 ),
				new SCImage ( 240, 768, 995 ),
				new SCImage ( 239, 847, 1020 ),
				new SCImage ( 237, 768, 980 ),
				new SCImage ( 236, 760, 947 ),
				new SCImage ( 233, 673, 1024 ),
				new SCImage ( 230, 730, 987 ),
				new SCImage ( 224, 739, 950 ),
				new SCImage ( 220, 744, 1020 ),
				new SCImage ( 214, 938, 1020 ),
				new SCImage ( 209, 760, 867 ),
				new SCImage ( 204, 551, 1020 ),
				new SCImage ( 200, 725, 1018 ),
				new SCImage ( 195, 703, 1000 ),
				new SCImage ( 193, 703, 1000 ),
				new SCImage ( 189, 703, 1000 ),
				new SCImage ( 184, 703, 1000 ),

				/*
				 * 
				*/
			 
			};


			var r = Images.Random();

			r.Image.AttachTo(this);
			r.ClosureImage.AttachTo(this);
		}
	}
}
