using jsc.meta.Commands.Rewrite.RewriteToUltraApplication;
using ScriptCoreLib.Desktop.Extensions;
using System;

namespace AvalonSki
{
    /// <summary>
    /// You can debug your application by hitting F5.
    /// </summary>
    internal static class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
			DesktopAvalonExtensions.Launch(
				() => 
                {
                    var content = new ApplicationCanvas();

                    content.Accelerate(
                          0,
                          0,
                          1
                      );

                    return content;
                }
			);
#else
            RewriteToUltraApplication.AsProgram.Launch(typeof(Application));
#endif
        }

    }
}
