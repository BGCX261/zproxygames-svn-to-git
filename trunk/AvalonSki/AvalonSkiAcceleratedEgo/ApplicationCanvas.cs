using ScriptCoreLib.Extensions;
using ScriptCoreLib.Shared.Avalon.Extensions;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using AvalonSkiAcceleratedEgo.Avalon.Images;

namespace AvalonSkiAcceleratedEgo
{
    public class ApplicationCanvas : Canvas
    {
        public readonly Rectangle r = new Rectangle();

        public ApplicationCanvas()
        {
            //r.Fill = Brushes.Red;
            //r.AttachTo(this);
            //r.MoveTo(8, 8);
            //this.SizeChanged += (s, e) => r.SizeTo(this.Width - 16.0, this.Height - 16.0);

            var Scene = new Canvas().AttachTo(this);

            this.SizeChanged += (s, e) => Scene.MoveTo(this.Width / 2, this.Height / 2);


            var EgoSpeed = 0.5;
            var EgoPosition = 0.0;

            {
                var ego = new ski1();


                ego.AttachTo(Scene);

                (1000 / 60).AtIntervalWithCounter(
                    c =>
                    {
                        EgoPosition += EgoSpeed;

                        ego.MoveTo(0, (EgoPosition) % this.Height - this.Height / 2);
                    }
                );
            }

            Action<double, double> tree = (x, y) => new ski51().AttachTo(Scene).MoveTo(x, y);
            Action<double, double> deadtree = (x, y) => new ski50().AttachTo(Scene).MoveTo(x, y);
            Action<double, double> stone = (x, y) => new ski45().AttachTo(Scene).MoveTo(x, y);
            Action<double, double> stonefield = (x, y) => new ski27().AttachTo(Scene).MoveTo(x, y);


            tree(-64, 32);
            tree(64, 32);

            tree(-64, -32);
            tree(64, -32);


            var logo = new Avalon.Images.jsc().AttachTo(this);

            this.RotateScene =
                a =>
                {
                    Scene.RenderTransform = new RotateTransform(a);
                };

            RotateScene(12);

            this.Accelerate =
                (ax, ay, az) =>
                {
                    Scene.RenderTransform = new RotateTransform(ax * 90);

                    EgoSpeed = (1 - az) * 2; 
                };
        }

        public readonly Action<double> RotateScene;

        public readonly Action<double, double, double> Accelerate;

    }
}
