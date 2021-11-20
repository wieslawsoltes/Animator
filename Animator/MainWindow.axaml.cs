using System;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using Animator.ViewModels;
using Animator.ViewModels.Animation;
using Animator.ViewModels.Style;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;

namespace Animator
{
    public enum PlaybackMode
    {
        Manual,
        Auto
    }

    public class MainWindow : Window
    {
        private Rectangle? _rectangle1;
        private Rectangle? _rectangle2;
        private Button? _playButton;
        private Slider? _slider;
        private NumericUpDown? _numericUpDown;
        private PlaybackMode _playbackMode;
        private bool _isPlaying;
        private TimelineClock _timelineClock;
        private Clock1 _playbackClock;
        private Subject<bool> _animationTrigger;
        private Animation? _animation1;
        private Animation? _animation2;
        // private IDisposable? _disposable1;
        // private IDisposable? _disposable2;
        private double _minimum;
        private double _maximum;
        private double _step;

        public MainWindow()
        {
            InitializeComponent();

            _playbackMode = PlaybackMode.Auto;
            _isPlaying = false;
            _timelineClock = new TimelineClock();
            _playbackClock = new Clock1();
            _animationTrigger = new Subject<bool>();
            _minimum = 0;
            _maximum = 4000;
            _step = 1;

            _rectangle1 = this.FindControl<Rectangle>("Rectangle1");
            _rectangle1.Clock = _playbackClock;
            _rectangle2 = this.FindControl<Rectangle>("Rectangle2");
            _rectangle2.Clock = _playbackClock;
            
            _playButton = this.FindControl<Button>("PlayButton");
            
            _slider = this.FindControl<Slider>("Slider");
            _slider.Minimum = _minimum;
            _slider.Maximum = _maximum;
            _slider.SmallChange = _step;
            _slider.LargeChange = _step;
            _slider.TickFrequency = _step;
            _slider.IsSnapToTickEnabled = true;
            _slider.Value = _minimum;

            _numericUpDown = this.FindControl<NumericUpDown>("NumericUpDown");
            _numericUpDown.Minimum = _minimum;
            _numericUpDown.Maximum = _maximum;
            _numericUpDown.Increment = _step;

            bool _sync = false;
            
            _slider.GetObservable(RangeBase.ValueProperty).Subscribe(x =>
            {
                //_timelineClock.Step(TimeSpan.FromMilliseconds(x));
                if (_sync)
                {
                    return;
                }
                _sync = true;
                if (_playbackClock.PlayState == PlayState.Pause)
                {
                    _playbackClock.Step(TimeSpan.FromMilliseconds(x));
                }
                _sync = false;
            });

            _playbackClock.Subscribe(x =>
            {
                if (_sync)
                {
                    return;
                }
                _sync = true;
                var milliseconds = x.TotalMilliseconds % (_maximum + 1);
                _slider.Value = milliseconds;
                _sync = false;
            });

            CreateAnimation1();
            CreateAnimation2();

            //_disposable1 = _animation1.Apply(_rectangle1, null, _animationTrigger, null);
            //_disposable2 = _animation2.Apply(_rectangle2, null, _animationTrigger, null);

            //_animation1.RunAsync(_rectangle1, _timelineClock);
            //_animation2.RunAsync(_rectangle2, _timelineClock);
            //_timelineClock.Step(TimeSpan.FromMilliseconds(0));

            _animation1?.RunAsync(_rectangle1, _playbackClock);
            _animation2?.RunAsync(_rectangle2, _playbackClock);

            _playbackClock.PlayState = PlayState.Run;
            //_animationTrigger.OnNext(true);
            _isPlaying = true;
            _playButton.Content = "Stop";

#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void PlayButton_OnClick(object? sender, RoutedEventArgs e)
        {
            switch (_playbackMode)
            {
                case PlaybackMode.Manual:
                {
                    //_animation1.RunAsync(_rectangle1);
                    //_animation2.RunAsync(_rectangle2);
                    //_playbackMode = PlaybackMode.Auto;
                    break;
                }
                case PlaybackMode.Auto:
                {
                    if (_isPlaying)
                    {
                        _playbackClock.PlayState = PlayState.Pause;
                        //_animationTrigger.OnNext(false);
                        _isPlaying = false;
                        if (_playButton is { })
                        {
                            _playButton.Content = "Play";
                        }
                    }
                    else
                    {
                        _playbackClock.PlayState = PlayState.Run;
                        //_animationTrigger.OnNext(true);
                        _isPlaying = true;
                        if (_playButton is { })
                        {
                            _playButton.Content = "Stop";
                        }
                    }
                    //_animation1.RunAsync(_rectangle1, _timelineClock);
                    //_animation2.RunAsync(_rectangle2, _timelineClock);
                    //_timelineClock.Step(TimeSpan.FromMilliseconds(0));
                    //_playbackMode = PlaybackMode.Manual;
                    break;
                }
            }
        }

        private void LoadButton_OnClick(object? sender, RoutedEventArgs e)
        {
            if (DataContext is ProjectViewModel projectViewModel)
            {
                if (projectViewModel.Styles is { })
                {
                    var styleViewModel = projectViewModel.Styles.FirstOrDefault();
                    if (styleViewModel is { })
                    {
                        var style = ToStyle(styleViewModel);

                        // TODO:
                    }
                }
            }
        }

        private static void ToXaml(AnimationViewModel animationViewModel, StringBuilder sb, string tab)
        {
            sb.AppendLine($"{tab}{tab}<Animation Delay=\"{animationViewModel.Delay}\" Duration=\"{animationViewModel.Duration}\">");

            if (animationViewModel.KeyFrames is { })
            {
                foreach (var keyFrameViewModel in animationViewModel.KeyFrames)
                {
                    sb.AppendLine($"{tab}{tab}{tab}<KeyFrame KeyTime=\"{keyFrameViewModel.KeyTime}\">");

                    if (keyFrameViewModel.Setters is { })
                    {
                        foreach (var setterViewModel in keyFrameViewModel.Setters)
                        {
                            sb.AppendLine(
                                $"{tab}{tab}{tab}{tab}<Setter Property=\"{setterViewModel.Property}\" Value=\"{setterViewModel.Value}\"/>");
                        }
                    }

                    sb.AppendLine($"{tab}{tab}{tab}</KeyFrame>");
                }
            }

            sb.AppendLine($"{tab}{tab}</Animation>");
        }

        private static string ToXaml(StyleViewModel styleViewModel)
        {
            var tab = "  ";
            var sb = new StringBuilder();

            sb.AppendLine($"<Style Selector=\"{styleViewModel.Selector}\" xmlns=\"https://github.com/avaloniaui\">");

            if (styleViewModel.Setters is { })
            {
                foreach (var setterViewModel in styleViewModel.Setters)
                {
                    sb.AppendLine(
                        $"{tab}<Setter Property=\"{setterViewModel.Property}\" Value=\"{setterViewModel.Value}\"/>");
                }
            }

            sb.AppendLine($"{tab}<Style.Animations>");

            if (styleViewModel.Animations is { })
            {
                foreach (var animationViewModel in styleViewModel.Animations)
                {
                    ToXaml(animationViewModel, sb, tab);
                }
            }

            sb.AppendLine($"{tab}</Style.Animations>");

            sb.AppendLine($"</Style>");

            var xaml = sb.ToString();
            return xaml;
        }

        private static Style ToStyle(StyleViewModel styleViewModel)
        {
            var xaml = ToXaml(styleViewModel);
            var style = AvaloniaRuntimeXamlLoader.Parse<Style>(xaml);
            return style;
        }

        private void CreateAnimation1()
        {
            _animation1 = new Animation()
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
        }

        private void CreateAnimation2()
        {
            _animation2 = new Animation()
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
        }
    }
}