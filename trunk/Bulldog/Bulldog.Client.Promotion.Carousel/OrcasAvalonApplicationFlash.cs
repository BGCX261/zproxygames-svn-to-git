using ScriptCoreLib;
using ScriptCoreLib.ActionScript.flash.display;
using ScriptCoreLib.ActionScript.flash.text;
using System.Collections.Generic;
using System;
using ScriptCoreLib.ActionScript;
using ScriptCoreLib.ActionScript.Extensions;

namespace Bulldog.Client.Promotion.Carousel.ActionScript
{
	using TargetCanvas = global::Bulldog.Client.Promotion.Carousel.Shared.OrcasAvalonApplicationCanvas;

	/// <summary>
	/// Default flash player entrypoint class. See 'tools/build.bat' for adding more entrypoints.
	/// </summary>
	[Script, ScriptApplicationEntryPoint(Width = TargetCanvas.DefaultWidth, Height = TargetCanvas.DefaultHeight, WithResources = true)]
	[SWF(width = TargetCanvas.DefaultWidth, height = TargetCanvas.DefaultHeight)]
	public class CarouselFlash : Sprite
	{
		public CarouselFlash()
		{
			// spawn the wpf control
			AvalonExtensions.AttachToContainer(new TargetCanvas(), this);
		}


	}

}