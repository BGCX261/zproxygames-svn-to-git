using System.Threading;
using System;

using ScriptCoreLib;


namespace Bulldog.Promotion.Console
{

	[Script]
	public class Program
	{
		public static void Main(string[] args)
		{
			foreach (var ff in typeof(GameReference).GetFields())
			{
				System.Console.WriteLine(ff.Name);
			
			}

			var x = GameReferenceExtensions.Partial.ToYAML();
			var y = GameReferenceExtensions.Default.FromYAML(x);
			var z = y.ToYAML();

			System.Console.WriteLine(z);

		}

		
	}


}
