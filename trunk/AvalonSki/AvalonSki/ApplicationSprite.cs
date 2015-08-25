using ScriptCoreLib.ActionScript.Extensions;
using ScriptCoreLib.ActionScript.flash.display;
using ScriptCoreLib.Extensions;
using System.Windows.Media;
using ScriptCoreLib.ActionScript.flash.sensors;
using System;

namespace AvalonSki
{
    internal sealed class ApplicationSprite : Sprite
    {
        /* http://ski.ihoc.net/
         * http://www.everything2.com/index.pl?lastnode_id=32619&node_id=814916
         * http://stackoverflow.com/questions/4474508/access-accelerometer-via-javascript-in-android
         * 01. Samsung Galaxy S screen: 800x358 fullscreen: 800x480
         * 02. Accelerometer? doesnt seem to work? 
         * 
         */

        public readonly ApplicationCanvas content = new ApplicationCanvas();

        // local variable Accelerators will be deactivated after going out of scope
        Accelerometer a;

        public ApplicationSprite()
        {
            Action<string, Action> Try =
                (ErrorText, Handler) =>
                {
                    try
                    {
                        Handler();
                    }
                    catch
                    {
                        content.t2.Text = "error: " + ErrorText;
                    }
                };

            this.click +=
                delegate
                {
                    content.Background = Brushes.Black;

                    this.stage.SetFullscreen(true);

                    if (Accelerometer.isSupported)
                    {
                        //var a = new Accelerometer();
                        a = new Accelerometer();

                        content.t2.Text = "with Accelerometer!! before add update or status";

                        a.status +=
                            e =>
                            {
                                // Note: On some devices, the accelerometer is always available. 
                                // On such devices, an Accelerometer object never dispatches a 
                                // status event.
                                Try("status",
                                    delegate
                                    {
                                        content.t2.Text = "status: " + new { e.code, e.type, e.level };
                                    }
                                );
                            };


                        a.update +=
                            e =>
                            {
                                Try("update",
                                      delegate
                                      {
                                          content.t2.Text = "with Accelerometer! at update";

                                          content.Accelerate(
                                              e.accelerationX,
                                              e.accelerationY,
                                              e.accelerationZ
                                          );
                                      }
                                );
                            };

                        a.setRequestedUpdateInterval(1000 / 30);

                        if (a.muted)
                        {
                            content.t2.Text = "with Accelerometer! after add update (muted)";
                        }
                        else
                        {
                            content.t2.Text = "with Accelerometer! after add update (not muted)";

                        }
                    }
                    else
                    {
                        content.t2.Text = "no Accelerometer!!";
                    }
                };

            this.InvokeWhenStageIsReady(
                delegate()
                {
                    content.AttachToContainer(this);
                    content.AutoSizeTo(this.stage);






                }
            );
        }

    }
}
