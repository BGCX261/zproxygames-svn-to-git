using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptCoreLib;
using javax.servlet.http;

namespace Bulldog.Server.Library
{
	[Script]
	public abstract class HTMLServlet : HttpServlet
	{
		protected override void doGet(HttpServletRequest req, HttpServletResponse resp)
		{
			try
			{
				
				resp.setContentType("text/html");
				var a = new RenderArguments
				{
					PathAndQuery = req.GetPathAndQuery(),
					ServerName = req.getServerName()
				};

				resp.getWriter().println(Render(a));
				resp.getWriter().flush();
			}
			catch (csharp.ThrowableException ex)
			{
				// either swallow of throw a runtime exception

				((java.lang.Throwable)(object)ex).printStackTrace();

			}
		}

		[Script]
		public class RenderArguments
		{
			public string PathAndQuery;
			public string ServerName;
		}

		public abstract string Render(RenderArguments args);
	}
}
