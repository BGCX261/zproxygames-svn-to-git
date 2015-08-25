using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptCoreLib;
using java.net;
using java.io;
using javax.servlet.http;
using System.Net;
using Bulldog.Server.CodeGenerators;

namespace Bulldog.Server.Library
{
	[Script]
	public static class Extensions
	{
		public static string Take(this string x, int length)
		{
			if (length < 0)
				return x;

			if (x.Length < length)
				return x;

			return x.Substring(0, length);
		}

		public static string Skip(this string x, int length)
		{
			if (x.Length > length)
				return x.Substring(length);

			return "";
		}

		public static string WriteTo(this string x, StringBuilder w)
		{
			w.Append(x);

			return x;
		}

		public static string ToAttributeString(this string value, string key)
		{
			return key + "='" + value + "'";
		}

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

		public static string GetPathAndQuery(this HttpServletRequest req)
		{
			var Path = req.getPathInfo();
			var Query = req.getQueryString();
			var PathAndQuery = req.getServletPath();

			if (Path != null)
				PathAndQuery += Path;

			if (Query != null)
				PathAndQuery += "?" + Query;

			return PathAndQuery;
		}

		public static string ToImage(this string src)
		{
			return "<img src='" + src + "' />";
		}

		public static string ToImage(this string src, string title)
		{
			return "<img src='" + src + "' title='" + title + "' />";
		}

		public static string ToLink(this string href, string text)
		{
			return "<a href='" + href + "' >" + text + "</a>";
		}

	}

	//[Script]
	//public class ToWebStringFunction : IToWebString
	//{
	//    public static readonly IToWebString Default = new ToWebStringFunction();

	//    #region IToWebString Members

	//    public string Invoke(Uri e)
	//    {
	//        return e.ToWebString();
	//    }

	//    #endregion
	//}

	[Script(Implements = typeof(ExtensionsNative))]
	internal static class ExtensionsScript
	{
		public static string ToWebString(Uri u)
		{
			var w = new StringBuilder();

			try
			{
				var url = new URL(u.ToString());
				var i = new InputStreamReader(url.openStream());
				var reader = new BufferedReader(i);


				var line = reader.readLine();
				while (line != null)
				{
					w.AppendLine(line);

					line = reader.readLine();
				}
				reader.close();
			}
			catch
			{
				// oops
			}

			return w.ToString();
		}
	}

	public static class ExtensionsNative
	{
		public static string ToWebString(this Uri u)
		{
			return new WebClient().DownloadString(u);
		}
	}
}
