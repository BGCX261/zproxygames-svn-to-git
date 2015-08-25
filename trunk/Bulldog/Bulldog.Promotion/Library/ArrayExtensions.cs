using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Bulldog.Promotion.Library
{
	public static class ArrayExtensions
	{
		public static object[] MergeArray(this object[] a, Type t, object[] n, MatchDelegate match)
		{
			foreach (var k in n)
			{
				a = a.Merge(t, k, match);
			}

			return a;
		}

		public static object[] Merge(this object[] a, Type t, object n, MatchDelegate match)
		{
			var j = -1;
			for (int i = 0; i < a.Length; i++)
			{
				if (match(a[i], n))
				{
					j = i;
				}
			}

			if (j >= 0)
			{
				//a[j] = n;

				n.CopyAssignedFieldsTo(t, a[j]);
				return a;
			}

			var x = (object[])Array.CreateInstance(t, a.Length + 1);
			Array.Copy(a, x, a.Length);
			x[a.Length] = n;

			return x;
		}

		public static void CopyAssignedFieldsTo(this object source, Type t, object target)
		{

			foreach (var f in t.GetFields())
			{
				var n = f.GetValue(source);
				
				// in actionscript if you cast null object to string
				// you will get "null" instead of null...

				if (n != null)
					f.SetValue(target, (string)n);

			}
		}

	}

	public delegate bool MatchDelegate(object a, object e);

}
