using System;
using System.Linq;
using Animator.Services;
using Animator.ViewModels;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace Animator.Views;

public partial class MainView : UserControl
{
    private readonly AnimationController _animationController;
    private decimal _minimum;
    private decimal _maximum;
    private decimal _step;
    
    public MainView()
    {
        InitializeComponent();

        _animationController = new AnimationController();

        Rectangle1.Clock = _animationController.PlaybackClock;
        Rectangle2.Clock = _animationController.PlaybackClock;

        _minimum = 0;
        _maximum = 4000;
        _step = 1;

        Slider.Minimum = (double)_minimum;
        Slider.Maximum = (double)_maximum;
        Slider.SmallChange = (double)_step;
        Slider.LargeChange = (double)_step;
        Slider.TickFrequency = (double)_step;
        Slider.IsSnapToTickEnabled = true;
        Slider.Value = (double)_minimum;

        NumericUpDown.Minimum = _minimum;
        NumericUpDown.Maximum = _maximum;
        NumericUpDown.Increment = _step;

        var sync = false;

        Slider.GetObservable(RangeBase.ValueProperty).Subscribe(x =>
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

        _animationController.PlaybackClock.Subscribe(x =>
        {
            if (sync)
            {
                return;
            }

            sync = true;

            var milliseconds = x.TotalMilliseconds % ((double)_maximum + 1);

            Slider.Value = milliseconds;

            sync = false;
        });

        _animationController.CreateAnimation1();
        _animationController.CreateAnimation2();

        _animationController.RunAnimation1(Rectangle1);
        _animationController.RunAnimation2(Rectangle2);

        _animationController.Play();

        UpdatePlayButton();
    }

    private void UpdatePlayButton()
    {
        PlayButton.Content = _animationController.IsPlaying ? "Pause" : "Play";
    }

    private void PlayButton_OnClick(object? sender, RoutedEventArgs e)
    {
        _animationController.TogglePlaybackMode();

        UpdatePlayButton();
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
