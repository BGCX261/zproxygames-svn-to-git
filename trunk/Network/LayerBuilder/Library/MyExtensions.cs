using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.IO;

namespace LayerBuilder.Library
{
	public static class MyExtensions
	{


		public static void AddResource(this AssemblyBuilder a, string key, string value)
		{
			File.WriteAllText(key, value);
			a.AddResourceFile(key, key);
		}
	}
}
