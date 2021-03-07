using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;

namespace Animator
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            CreateAnimation();

#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void CreateAnimation()
        {
            var animation1 = new Animation()
            {
                Duration = TimeSpan.FromSeconds(2),
                IterationCount = new IterationCount(0, IterationType.Infinite),
                PlaybackDirection = PlaybackDirection.Alternate,
                FillMode = FillMode.None,
                Easing = new SplineEasing(new KeySpline(0.4, 0, 0.6, 1)),
                Delay = TimeSpan.FromSeconds(0),
                DelayBetweenIterations = TimeSpan.FromSeconds(0),
                SpeedRatio = 1d,
                Children =
                {
                    new KeyFrame()
                    {
                        KeyTime = TimeSpan.FromSeconds(0),
                        Setters =
                        {
                            new Setter(Visual.OpacityProperty, 1.0),
                            new Setter(RotateTransform.AngleProperty, 0d),
                        }
                    },
                    new KeyFrame()
                    {
                        KeyTime = TimeSpan.FromSeconds(2),
                        Setters =
                        {
                            new Setter(Visual.OpacityProperty, 0.0),
                            new Setter(RotateTransform.AngleProperty, 360d),
                        }
                    }
                }
            };

            var animation2 = new Animation()
            {
                Duration = TimeSpan.FromSeconds(2),
                IterationCount = new IterationCount(0, IterationType.Infinite),
                PlaybackDirection = PlaybackDirection.Alternate,
                FillMode = FillMode.None,
                Easing = new SplineEasing(new KeySpline(0.4, 0, 0.6, 1)),
                Delay = TimeSpan.FromSeconds(0),
                DelayBetweenIterations = TimeSpan.FromSeconds(0),
                SpeedRatio = 1d,
                Children =
                {
                    new KeyFrame()
                    {
                        KeyTime = TimeSpan.FromSeconds(0),
                        Setters =
                        {
                            new Setter(Visual.OpacityProperty, 0.0),
                            new Setter(ScaleTransform.ScaleXProperty, 0d),
                            new Setter(ScaleTransform.ScaleYProperty, 0d),
                        }
                    },
                    new KeyFrame()
                    {
                        KeyTime = TimeSpan.FromSeconds(2),
                        Setters =
                        {
                            new Setter(Visual.OpacityProperty, 1.0),
                            new Setter(ScaleTransform.ScaleXProperty, 1d),
                            new Setter(ScaleTransform.ScaleYProperty, 1d),
                        }
                    }
                }
            };

            var rectangle1 = this.FindControl<Rectangle>("Rectangle1");
            var rectangle2 = this.FindControl<Rectangle>("Rectangle2");
            var slider = this.FindControl<Slider>("Slider");

            var clock = new TimelineClock();

            slider.GetObservable(RangeBase.ValueProperty).Subscribe(x =>
            {
                clock.Step(TimeSpan.FromMilliseconds(x));
            });
#if false
            animation1.RunAsync(rectangle1);
            animation2.RunAsync(rectangle2);
#else
            animation1.RunAsync(rectangle1, clock);
            animation2.RunAsync(rectangle2, clock);
            clock.Step(TimeSpan.FromMilliseconds(0));
#endif
        }
    }
}