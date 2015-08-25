using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace AssetProvider
{
	class Program
	{
		class SC
		{
			public int Sequence;

			public static implicit operator SC(int Sequence)
			{
				return new SC { Sequence = Sequence };
			}

			public static implicit operator Uri(SC e)
			{
				return new Uri("http://www.botb.com/PastCompetitions/SC" + e.Sequence + ".aspx");
			}
		}

		class SCImage
		{
			public SC Context;

			public bool IsClosure = true;

			public byte[] Download(bool IsClosure)
			{
				this.IsClosure = IsClosure;

				return this.Download();
			}

			public byte[] Download()
			{
				return new WebClient().DownloadData((Uri)this);
			}

			public static implicit operator SCImage(SC Context)
			{
				return new SCImage { Context = Context };
			}

			public static explicit operator Uri(SCImage e)
			{
				var h = new WebClient().DownloadString(e.Context);

				// http://www.botb.com/FlashData/GetImage.ashx?guid=088db604-6a6a-435b-a458-e7ca464fda83
				var x = "/PostClosureImage.ashx?guid=";

				var i = h.IndexOf(x);
				var j = h.IndexOf("\"", i);

				var n = h.Substring(i + x.Length, j - i - x.Length);
				var ii = n.IndexOf("&");
				if (ii >= 0)
					n = n.Substring(0, ii);

				return new Uri("http://www.botb.com" + 
					(e.IsClosure ?
					"/PostClosureImage.ashx?guid=" :
					"/FlashData/GetImage.ashx?guid=") + n);
			}
		}

		static void Main(string[] args)
		{
			foreach (var x in Enumerable.Range(1, 333).Reverse())
			{

				var f = "SC" + x + ".jpg";
				var fz = "SC" + x + "z.jpg";

				if (File.Exists(f))
				{

					//    Console.Write("{ " + x + ", ");

					//    var ImageOrig = System.Drawing.Image.FromFile(f);
					//    var ImageOrigHeight = ImageOrig.Height;
					//    var ImageOrigWidth = ImageOrig.Width;

					//    Console.Write(ImageOrigHeight + ", ");
					//    Console.Write(ImageOrigWidth);

					//    Console.WriteLine(" } ");
					//}
					//else
					//{

					//    // can download?
					//    continue;

					try
					{
						Console.WriteLine(x);

						var sc = ((SCImage)(SC)x).Download();
						File.WriteAllBytes(f, sc);

						var scz = ((SCImage)(SC)x).Download(false);
						File.WriteAllBytes(fz, scz);
					}
					catch
					{
						Console.WriteLine("failed!");
					}
				}
			}

			// /PostClosureImage.ashx?guid=

		}
	}
}
