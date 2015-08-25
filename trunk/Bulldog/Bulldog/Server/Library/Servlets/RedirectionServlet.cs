using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using javax.servlet.http;
using ScriptCoreLib;

namespace Bulldog.Server.Library.Servlets
{
	[Script]
	public abstract class RedirectionServlet : HttpServlet
	{

		// http://www.adobe.com/devnet/articles/crossdomain_policy_file_spec.html#site-control
		// http://board.flashkit.com/board/showthread.php?t=782484

		protected override void doGet(HttpServletRequest req, HttpServletResponse resp)
		{
			try
			{
				resp.sendRedirect(ToLocation(req.GetPathAndQuery()));
			}
			catch (csharp.ThrowableException ex)
			{
				// either swallow of throw a runtime exception

				((java.lang.Throwable)(object)ex).printStackTrace();

			}
		}

		protected abstract string ToLocation(string PathAndQuery);
	}
}
