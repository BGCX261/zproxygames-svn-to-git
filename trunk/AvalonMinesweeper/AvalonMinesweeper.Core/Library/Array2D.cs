using ScriptCoreLib;
using ScriptCoreLib.Shared;

using System.Linq;
using System;
using System.Collections.Generic;

namespace AvalonMinesweeper.Core.Library
{
	public class Array2D<T> : System.Collections.Generic.IEnumerable<T>
	{
		readonly T[] items;

		readonly int _XLength;
		readonly int _YLength;

		public int Length
		{
			get { return items.Length; }
		}

		public int XLength
		{
			get { return _XLength; }
		}

		public int YLength
		{
			get { return _YLength; }
		}


		public Array2D(int x, int y)
		{
			this._XLength = x;
			this._YLength = y;

			this.items = new T[x * y];
		}

		public class Tuple
		{
			public int X;
			public int Y;
		}

		public IEnumerable<Tuple> ToIndicies()
		{
			return Enumerable.Range(0, this._XLength).SelectMany(x => Enumerable.Range(0, this._YLength).Select(y => new Tuple { X = x, Y = y }));
		}

		public void ForEach(Action<int, int> a)
		{
			for (int i = 0; i < this._XLength; i++)
				for (int j = 0; j < this._YLength; j++)
					a(i, j);

		}

		public Array2D<bool> ToBooleanArray()
		{
			return new Array2D<bool>(_XLength, _YLength);
		}

		public readonly T EmptyValue;

		public T this[Tuple i]
		{
			get
			{
				return this[i.X, i.Y];
			}
		}

		public T this[int x, int y]
		{
			get
			{
				if (ContainsIndex(x, y))
					return this.items[this._XLength * y + x];

				return EmptyValue;
			}
			set
			{

				if (ContainsIndex(x, y))
					this.items[this._XLength * y + x] = value;
			}
		}

		#region IEnumerable<T> Members

		public System.Collections.Generic.IEnumerator<T> GetEnumerator()
		{
			return this.items.AsEnumerable().GetEnumerator();
		}

		#endregion



		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.items.AsEnumerable().GetEnumerator();
		}

		#endregion

		public bool ContainsIndex(int x, int y)
		{
			if (x < 0) return false;
			if (y < 0) return false;
			if (x >= this._XLength) return false;
			if (y >= this._YLength) return false;

			return true;
		}
	}

}
