using ScriptCoreLib;
using ScriptCoreLib.ActionScript.flash.display;
using ScriptCoreLib.ActionScript.flash.text;
using System.Collections.Generic;
using System;
using ScriptCoreLib.ActionScript;
using ScriptCoreLib.ActionScript.Extensions;
//using ScriptCoreLib.ActionScript.MochiLibrary;
using ScriptCoreLib.Shared;
using AvalonMinesweeper.Core;
namespace AvalonMinesweeper.ActionScript
{



	/// <summary>
	/// Default flash player entrypoint class. See 'tools/build.bat' for adding more entrypoints.
	/// </summary>
	[Script]
	[SWF(
		width = AvalonMinesweeperCanvas.DefaultWidth,
		height = AvalonMinesweeperCanvas.DefaultHeight,
		backgroundColor = 0xffffff,
		frameRate = 120
	)]

	// We cannot use preloader at this time if we like to use the live updates from mochimedia.
	// For other cases we can include the preloader.

#if mochimedia
	[Frame(typeof(Preloader))]
#endif

	[ScriptApplicationEntryPoint(
		Width = AvalonMinesweeperCanvas.DefaultWidth,
		Height = AvalonMinesweeperCanvas.DefaultHeight,
		Background = true,
		BackgroundColor = 0,

		// test with IE!
		AlignToCenter = true,

		WithResources = true
	)]
	public partial class OrcasAvalonApplicationFlash : Sprite
	{

		#region _mochiads_game_id
		// will mochimedia be able to read out this key?
		readonly static string _mochiads_game_id = Promotion.Info.MochiAdsKey;
		#endregion


		[Script]
		class Canvas : AvalonMinesweeperCanvas
		{
			public override string BuildInformation
			{
				get
				{
					return "actionscript3";
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

		public OrcasAvalonApplicationFlash()
		{
			this.InvokeWhenStageIsReady(
				delegate
				{
					this.stage.scaleMode = StageScaleMode.NO_SCALE;
					
					// spawn the wpf control
					AvalonExtensions.AttachToContainer(new Canvas(), this);

				}
			);
		}

		static OrcasAvalonApplicationFlash()
		{
			// add resources to be found by ImageSource
			KnownEmbeddedAssets.RegisterTo(
				KnownEmbeddedResources.Default.Handlers
			);

		}


        //[Script]
        //public class Preloader : MochiAdPreloader
        //{
        //    // at this time we are using mochimedia.com preloader

        //    [TypeOfByNameOverride]
        //    public override Type GetTargetType()
        //    {
        //        return typeof(OrcasAvalonApplicationFlash);
        //    }


        //    public override bool IsBackgroundVisible()
        //    {
        //        return false;
        //    }

        //    public override bool AdsEnabled()
        //    {

        //        return true;

        //    }

        //    [Embed("/assets/AvalonMinesweeper.Core/Preview_AvalonMinesweeper_Preloader.png")]
        //    internal static Class CustomBackground;

        //    public Preloader()
        //        : base(Promotion.Info.MochiAdsKey)
        //    {
        //    }

        //    public override void InitializeBackground()
        //    {
        //        CustomBackground.ToBitmapAsset().AttachTo(this);
        //    }
        //}
	}

	[Script]
	public class KnownEmbeddedAssets
	{


		public static void RegisterTo(List<Converter<string, Class>> Handlers)
		{
			//// assets from referenced assemblies
			//Handlers.Add(e => global::ScriptCoreLib.ActionScript.Avalon.Cursors.EmbeddedAssets.Default[e]);
			//Handlers.Add(e => global::ScriptCoreLib.ActionScript.Avalon.TiledImageButton.Assets.Default[e]);

		}
	}
}