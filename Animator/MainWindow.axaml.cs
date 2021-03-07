using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
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

        private class TimelineClock : IClock, IDisposable
        {
            private TimeSpan _curTime;
            private readonly List<IObserver<TimeSpan>> _observers = new List<IObserver<TimeSpan>>();
            public PlayState PlayState { get; set; } = PlayState.Run;

            public void Dispose()
            {
                _observers.ForEach(x => x.OnCompleted());
            }

            public void Step(TimeSpan time)
            {
                _observers.ForEach(x => x.OnNext(time));
            }

            public void Pulse(TimeSpan time)
            {
                _curTime += time;
                _observers.ForEach(x => x.OnNext(_curTime));
            }

            public IDisposable Subscribe(IObserver<TimeSpan> observer)
            {
                _observers.Add(observer);
                return this;
            }
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

            slider.GetObservable(Slider.ValueProperty).Subscribe(x =>
            {
                clock.Step(TimeSpan.FromMilliseconds(x));
            });
#if true
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