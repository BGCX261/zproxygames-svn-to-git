using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bulldog.Server.CodeGenerators
{
	public static class Extensions
	{
	

		public static string ToElementText(this string e)
		{
			var w = new StringBuilder();

			var ic = -1;
			var i = e.IndexOf("<");


			if (i > -1)
			{
				while (i > -1)
				{
					w.Append(e.Substring(ic + 1, i - (ic + 1)));

					ic = e.IndexOf(">", i);

					if (ic > -1)
					{
						i = e.IndexOf("<", ic);

					}
					else
					{
						i = -1;
					}
				}

				w.Append(e.Substring(ic + 1));
			}
			else
			{
				w.Append(e);
			}

			return w.ToString();
		}

	}
}
