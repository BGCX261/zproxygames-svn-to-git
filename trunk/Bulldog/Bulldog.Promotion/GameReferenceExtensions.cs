using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Bulldog.Promotion.Library;
using ScriptCoreLib.YAML;

namespace Bulldog.Promotion
{
	public static partial class GameReferenceExtensions
	{
		// say hi to coral cache :)

		public const string Host = "http://zproxy.appspot.com.nyud.net";
		public const string Path = "/promotion.yaml";

		public const int Width = 120;
		public const int Height = 90;

	


		public static GameReference[] Partial = new[]
		{
			new GameReference
			{ 
				Title = "Avalon Tycoon Mansion", 
				Link = "http://avalon-tycoon-mansion.tk", 
				Image = Host + "/assets/Bulldog.Promotion/Preview_AvalonTycoonMansion.png" ,
			},
		

		};

		public static string ToYAML(this GameReference[] a)
		{
			return YAMLDocument.WriteMappingsSequence(typeof(GameReference), a);
		}

		

		public static GameReference[] FromYAML(this GameReference[] a, string data)
		{
			var z = (object[])YAMLDocument.FromMappingsSequence(typeof(GameReference), data);

			return (GameReference[])a.MergeArray(typeof(GameReference), z, GameReference.Equals);
		}

		public static string GetWebTitle(this GameReference e)
		{
			return e.Title.ToLower().Replace(" ", "-");
		}

		// this is LINQ without generics :)

		public delegate bool GameReferenceFilter(GameReference k);

		public static GameReference FirstOrDefault(this GameReference[] source, GameReferenceFilter filter)
		{
			var r = default(GameReference);

			foreach (var s in source)
			{
				if (filter(s))
				{
					r = s;
					break;
				}
			}

			return r;
		}
	}

}
