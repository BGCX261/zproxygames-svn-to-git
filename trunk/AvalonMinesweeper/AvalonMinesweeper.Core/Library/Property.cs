using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AvalonMinesweeper.Core.Library
{
	public class Property<T>
	{
		public Action<T> SetValue;
		public Func<T> GetValue;

		public T Value
		{
			get
			{
				return GetValue();
			}
			set
			{
				SetValue(value);
			}
		}

		public static implicit operator Property<T>(Action<T> setter)
		{

			var p = default(Property<T>);

			p = new Property<T>
			{
				SetValue =
					value =>
					{
						p.GetValue = () => value;

						setter(value);
					}
			};

			return p;
		}
	}
}
