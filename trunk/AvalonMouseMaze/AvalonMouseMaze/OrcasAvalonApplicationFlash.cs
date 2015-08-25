using ScriptCoreLib;
using ScriptCoreLib.ActionScript.flash.display;
using ScriptCoreLib.ActionScript.flash.text;
using System.Collections.Generic;
using System;
using ScriptCoreLib.ActionScript;
using ScriptCoreLib.ActionScript.Extensions;

namespace AvalonMouseMaze.ActionScript
{
	using TargetCanvas = global::AvalonMouseMaze.Shared.OrcasAvalonApplicationCanvas;

	/// <summary>
	/// Default flash player entrypoint class. See 'tools/build.bat' for adding more entrypoints.
	/// </summary>
	[Script, ScriptApplicationEntryPoint(Width = TargetCanvas.DefaultWidth, Height = TargetCanvas.DefaultHeight, WithResources = true)]
	[SWF(width = TargetCanvas.DefaultWidth, height = TargetCanvas.DefaultHeight, backgroundColor = 0)]
	public class OrcasAvalonApplicationFlash : Sprite
	{
		public OrcasAvalonApplicationFlash()
		{
			// spawn the wpf control
			AvalonExtensions.AttachToContainer(new TargetCanvas(), this);
		}


	}


}