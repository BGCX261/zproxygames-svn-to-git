using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptCoreLib;
using ScriptCoreLib.JavaScript.Extensions;
using ScriptCoreLib.JavaScript.DOM.HTML;

namespace AvalonMinesweeper.JavaScript
{
	using AvalonMinesweeper.Core;

	[Script, ScriptApplicationEntryPoint]
	public class OrcasAvalonApplicationDocument
	{
		[Script]
		class Canvas : AvalonMinesweeperCanvas
		{
			public override string BuildInformation
			{
				get
				{
					return "javascript";
				}
			}

			public override bool IsBackgroundTransparent
			{
				get
				{
#if transparent
					return true;
#else
					return false;
#endif
				}
			}
		}

		public OrcasAvalonApplicationDocument(IHTMLElement e)
		{
			// wpf here
			var clip = new IHTMLDiv();

			clip.style.position = ScriptCoreLib.JavaScript.DOM.IStyle.PositionEnum.relative;
			clip.style.SetSize(AvalonMinesweeperCanvas.DefaultWidth, AvalonMinesweeperCanvas.DefaultHeight);
			clip.style.overflow = ScriptCoreLib.JavaScript.DOM.IStyle.OverflowEnum.hidden;

			if (e == null)
				clip.AttachToDocument();
			else
				e.insertPreviousSibling(clip);

			AvalonExtensions.AttachToContainer(new Canvas(), clip);

		}

		static OrcasAvalonApplicationDocument()
		{
			typeof(OrcasAvalonApplicationDocument).SpawnTo(i => new OrcasAvalonApplicationDocument(i));
		}

	}
}
