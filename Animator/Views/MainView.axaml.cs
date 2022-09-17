using System;
using System.Linq;
using Animator.ViewModels;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Animator.Views;

public class MainView : UserControl
{
    private readonly AnimationController _animationController;
    private Rectangle? _rectangle1;
    private Rectangle? _rectangle2;
    private Button? _playButton;
    private Slider? _slider;
    private NumericUpDown? _numericUpDown;
    private decimal _minimum;
    private decimal _maximum;
    private decimal _step;
    
    public MainView()
    {
        InitializeComponent();

        _animationController = new AnimationController();

        InitializeControls();

        var sync = false;

        if (_slider is { })
        {
            _slider.GetObservable(RangeBase.ValueProperty).Subscribe(x =>
            {
                // _animationController.TimelineClock.Step(TimeSpan.FromMilliseconds(x));

                if (sync)
                {
                    return;
                }

                sync = true;
                if (_animationController.PlaybackClock.PlayState == PlayState.Pause)
                {
                    _animationController.PlaybackClock.Step(TimeSpan.FromMilliseconds(x));
                }

                sync = false;
            });
        }

        _animationController.PlaybackClock.Subscribe(x =>
        {
            if (sync)
            {
                return;
            }

            sync = true;

            var milliseconds = x.TotalMilliseconds % ((double)_maximum + 1);
            if (_slider is { })
            {
                _slider.Value = milliseconds;
            }

            sync = false;
        });

        _animationController.CreateAnimation1();

        _animationController.CreateAnimation2();

        if (_rectangle1 is { })
        {
            _animationController.RunAnimation1(_rectangle1);
        }

        if (_rectangle2 is { })
        {
            _animationController.RunAnimation2(_rectangle2);
        }

        if (_playButton is { })
        {
            _playButton.Content = "Stop";
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
  
    private void InitializeControls()
    {
        _rectangle1 = this.FindControl<Rectangle>("Rectangle1");
        if (_rectangle1 is { })
        {
            _rectangle1.Clock = _animationController.PlaybackClock;
        }

        _rectangle2 = this.FindControl<Rectangle>("Rectangle2");
        if (_rectangle2 is { })
        {
            _rectangle2.Clock = _animationController.PlaybackClock;
        }

        _minimum = 0;
        _maximum = 4000;
        _step = 1;

        _playButton = this.FindControl<Button>("PlayButton");

        _slider = this.FindControl<Slider>("Slider");
        if (_slider is { })
        {
            _slider.Minimum = (double)_minimum;
            _slider.Maximum = (double)_maximum;
            _slider.SmallChange = (double)_step;
            _slider.LargeChange = (double)_step;
            _slider.TickFrequency = (double)_step;
            _slider.IsSnapToTickEnabled = true;
            _slider.Value = (double)_minimum;
        }

        _numericUpDown = this.FindControl<NumericUpDown>("NumericUpDown");
        if (_numericUpDown is { })
        {
            _numericUpDown.Minimum = _minimum;
            _numericUpDown.Maximum = _maximum;
            _numericUpDown.Increment = _step;
        }
    }

    private void PlayButton_OnClick(object? sender, RoutedEventArgs e)
    {
        _animationController.TogglePlaybackMode();

        if (_animationController.IsPlaying)
        {
            if (_playButton is { })
            {
                _playButton.Content = "Stop";
            }
        }
        else
        {
            if (_playButton is { })
            {
                _playButton.Content = "Play";
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
                    var style = ViewModelConverter.ToStyle(styleViewModel);

                    // TODO:
                }
            }
        }
    }
}
