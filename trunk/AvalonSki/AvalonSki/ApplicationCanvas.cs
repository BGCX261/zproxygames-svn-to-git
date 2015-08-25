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
using AvalonSki.Avalon.Images;
using ScriptCoreLib.Shared.Lambda;

namespace AvalonSki
{
    public class ApplicationCanvas : Canvas
    {
        public readonly Rectangle r = new Rectangle();

        public TextBox t2 = new TextBox
        {
            Text = "does this device have accelerometer?",
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0)
        };


        public readonly Action<double, double, double> Accelerate;

        public ApplicationCanvas()
        {
            this.Background = Brushes.Red;

            r.Fill = Brushes.White;
            r.AttachTo(this);
            r.MoveTo(4, 4);



            this.SizeChanged += (s, e) => r.SizeTo(this.Width - 8.0, this.Height - 8.0);

            {
                var ego = new ski1();


                ego.AttachTo(this).MoveTo(164, 96);

                (1000 / 60).AtIntervalWithCounter(
                    c =>
                    {
                        ego.MoveTo(164, c % this.Height);
                    }
                );
            }

            {
                var ego = new ski1();

                var dc = -64;

                ego.AttachTo(this).MoveTo(320, dc);

                (1000 / 60).AtIntervalWithCounter(
                    c =>
                    {
                        ego.MoveTo(320, (c + dc) % this.Height);
                    }
                );
            }

            Action<double, double> tree = (x, y) => new ski51().AttachTo(this).MoveTo(x, y);
            Action<double, double> deadtree = (x, y) => new ski50().AttachTo(this).MoveTo(x, y);
            Action<double, double> stone = (x, y) => new ski45().AttachTo(this).MoveTo(x, y);
            Action<double, double> stonefield = (x, y) => new ski27().AttachTo(this).MoveTo(x, y);

            tree(64, 32);

            var logo = new Avalon.Images.jsc().AttachTo(this);

            this.SizeChanged += (s, e) => logo.MoveTo(this.Width - logo.Width, this.Height - logo.Height);


            this.MouseLeftButtonUp +=
                (s, a) =>
                {
                    var pos = a.GetPosition(this);

                    var f = new[] { 
                        tree, 
                        tree,
                        tree,
                        tree,
                        deadtree, 
                        deadtree, 
                        stone, 
                        stone,
                        stone,
                        stonefield };

                    f.Random()(pos.X, pos.Y);
                };

            var t = new TextBox
            {
                Text = "ski",
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0)
            };

            t.AttachTo(this).MoveTo(96, 8);


            var ax = 0.0;
            var ay = 0.0;
            var az = 0.0;

            Action SizeChangedOrAccelerated =
                delegate
                {
                    t.Text = "ski: " + new { this.Width, this.Height, ax, ay, az };
                };

            this.SizeChanged += (s, e) => { SizeChangedOrAccelerated(); };



            t2.AttachTo(this).MoveTo(96, 24);

            Accelerate =
                (_ax, _ay, _az) =>
                {
                    ax = _ax;
                    ay = _ay;
                    az = _az;

                
                    SizeChangedOrAccelerated();
                };
        }

    }
}
