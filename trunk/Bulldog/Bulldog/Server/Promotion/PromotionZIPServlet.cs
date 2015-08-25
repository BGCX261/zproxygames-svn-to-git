using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using javax.servlet.http;
using ScriptCoreLibJava.Extensions;
using ScriptCoreLib;
using Bulldog.Promotion;
using ScriptCoreLib.YAML;
using Bulldog.Server.Library;
using ScriptCoreLib.Archive.ZIP;
using System.IO;

namespace Bulldog.Server
{
	[Script]
	[ConfigurationProvider.UrlPattern(Path)]
	public class PromotionZIPServlet : HttpServlet
	{
		public const string Path = "/promotion.zip";

		// http://www.adobe.com/devnet/articles/crossdomain_policy_file_spec.html#site-control
		// http://board.flashkit.com/board/showthread.php?t=782484

		protected override void doGet(HttpServletRequest req, HttpServletResponse resp)
		{
			try
			{
				resp.setContentType(ZIPFile.ContentType);

				resp.getOutputStream().write((sbyte[])(object)GetContent());

				resp.getOutputStream().flush();
			}
			catch (csharp.ThrowableException ex)
			{
				// either swallow of throw a runtime exception

				((java.lang.Throwable)(object)ex).printStackTrace();

			}
		}

		private byte[] GetContent()
		{
			var z = new ZIPFile
			{
				{ "default1.txt", "hello" },
				{ "default2.txt", "world" },
				{ "task1.zip", 
					new ZIPFile
					{
						{ "default3.txt", "cool" },
						{ "ken/default4.txt", "huh" },
					}
				}
			};


			return z.ToBytes();
		}


	}
}
