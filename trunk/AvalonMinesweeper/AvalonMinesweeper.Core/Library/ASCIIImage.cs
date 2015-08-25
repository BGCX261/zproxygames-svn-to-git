using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AvalonMinesweeper.Core.Library
{
	public class ASCIIImage : IEnumerable<ASCIIImage.Entry>
	{
		public readonly string[] Lines;

		public readonly int Height;
		public readonly int Width;

		public class ConstructorArguments
		{
			public string value;
			public int skipx = 0;
			public int skipy = 0;
			public int MultiplyX = 1;
			public int MultiplyY = 1;

			public Func<string, bool> IsComment = e => e.StartsWith("#");

			public static implicit operator ConstructorArguments(string value)
			{
				return new ConstructorArguments { value = value };
			}
		}

		public ASCIIImage(ConstructorArguments e)
		{
			if (e.value == null)
				throw new ArgumentNullException("ASCIIImage.ConstructorArguments.value");

			var s = new StringReader(e.value);

			var lines = new List<string>();

			var loop = true;
			while (loop)
			{
				var x = s.ReadLine();

				if (x == null)
				{
					loop = false;
				}
				else
				{
					lines.Add(x);
				}
			}

			var a = new List<string>();

			this.Height = 0;

			for (int i = 0; i < lines.Count; )
			{
				var v = lines[i];

				if (e.IsComment(v))
				{
					i++;
				}
				else
				{
					var n = "";

					for (int j = 0; j < v.Length; j += 1 + e.skipx)
					{
						for (int mj = 0; mj < e.MultiplyX; mj++)
							n += v.Substring(j, 1);
					}

					for (int mi = 0; mi < e.MultiplyY; mi++)
						a.Add(n);

					i += 1 + e.skipy;
				}
			}

			this.Lines = a.ToArray();

			this.Height = this.Lines.Length;

			if (Height > 0)
			{
				this.Width = this.Lines.Max(k => k.Length);
			}
		}

		public string this[int x, int y]
		{
			get
			{
				if (x < 0)
					return " ";
				if (y < 0)
					return " ";

				if (x >= Width)
					return " ";
				if (y >= Height)
					return " ";

				var Line = this.Lines[y];

				if (Line.Length <= x)
					return " ";

				return Line.Substring(x, 1);
			}
		}

		public class Entry
		{
			public readonly int X;
			public readonly int Y;

			readonly Func<int, int, string> Select;

			public static implicit operator Func<int, int, string>(Entry e)
			{
				return (x, y) => e[x, y];
			}

			public Entry(Func<int, int, string> Select, int X, int Y)
			{
				this.Select = Select;

				this.X = X;
				this.Y = Y;
			}

			public string Value
			{
				get
				{
					return this[0, 0];
				}
			}


			public string this[int x, int y]
			{
				get
				{
					return Select(this.X + x, this.Y + y);
				}
			}
		}


		#region IEnumerable<Item> Members

		public IEnumerator<ASCIIImage.Entry> GetEnumerator()
		{
			Func<int, int, string> Select = (x, y) => this[x, y];

			return (
				from x in Enumerable.Range(0, Width)
				from y in Enumerable.Range(0, Height)
				select new ASCIIImage.Entry(Select, x, y)
			).GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion
	}

}
