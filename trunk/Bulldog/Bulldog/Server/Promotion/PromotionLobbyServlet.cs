using System;
using System.Collections.Generic;
using System.Text;
using Bulldog.Server.Library;
using ScriptCoreLib;
using ScriptCoreLibJava.Extensions;
using Bulldog.Promotion;

namespace Bulldog.Server.Promotion
{
	[Script]
	[ConfigurationProvider.UrlPattern(Pattern + "/*")]
	public class PromotionLobbyServlet : HTMLServlet
	{
		public const string Pattern = "/game";

		public override string Render(RenderArguments args)
		{
			var PathAndQuery = args.PathAndQuery;

			var w = new StringBuilder();

			w.AppendLine("This is a deep link into a game either a room inside a game or a multiplayer session.");
			w.AppendLine("<hr />");

			var WebTitle = PathAndQuery.Skip(Pattern.Length + 1);
			var Location = "";

			var i = WebTitle.IndexOf("/");
			if (i > 0)
			{
				Location = WebTitle.Skip(i + 1);
				WebTitle = WebTitle.Take(i);
			}

			w.AppendLine(Location);

			var Game = GameReferenceExtensions.Default.FirstOrDefault(r => r.GetWebTitle() == WebTitle);

			if (Game != null)
			{
				w.AppendLine("<center>");
				w.AppendLine(Game.Embed.ToFlashContainer(int.Parse(Game.Width), int.Parse(Game.Height)));
				w.AppendLine("</center>");
			}

			return w.ToString();
		}

	}
}
