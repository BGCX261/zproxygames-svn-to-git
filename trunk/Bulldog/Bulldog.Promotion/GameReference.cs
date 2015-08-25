using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bulldog.Promotion
{
	public sealed class GameReference
	{
		public string Title;
		public string Image;
		public string Link;

		public string Embed;
		public string Width;
		public string Height;

		/// <summary>
		/// This is a special field and is used to mark local asset
		/// </summary>
		public string Source;

		public static bool Equals(object x, object y)
		{
			var x_ = x as GameReference;
			if (x_ == null)
				return false;

			var y_ = y as GameReference;
			if (y_ == null)
				return false;

			return x_.Title == y_.Title;
		}

		public static GameReference Of(
			string Title,
			string Image,
			string Link,

			string Embed,
			int Width,
			int Height
			)
		{
			return new GameReference
			{
				Title = Title,
				Image = Image,
				Link = Link,
				Embed = Embed,
				Width = "" + Width,
				Height = "" + Height,
			};
		}
	}



}
