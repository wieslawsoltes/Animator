using System;
using System.Reactive.Subjects;
using Animator.Clocks;
using Animator.Model;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Styling;

namespace Animator.Views;

public class AnimationController
{
    private PlaybackMode _playbackMode;
    private bool _isPlaying;

    private Clock1 _playbackClock;

    private TimelineClock _timelineClock;
    private Subject<bool> _animationTrigger;
    private IDisposable? _disposable1;
    private IDisposable? _disposable2;

    private Animation? _animation1;
    private Animation? _animation2;

    public AnimationController()
    {
        _playbackMode = PlaybackMode.Auto;
        _isPlaying = false;
        _playbackClock = new Clock1();

        _timelineClock = new TimelineClock();
        _animationTrigger = new Subject<bool>();
    }

    public Clock1 PlaybackClock => _playbackClock;

    public TimelineClock TimelineClock => _timelineClock;

    public bool IsPlaying => _isPlaying;

    public void TogglePlaybackMode()
    {
        switch (_playbackMode)
        {
            case PlaybackMode.Manual:
            {
                // _animation1.RunAsync(_rectangle1);
                // _animation2.RunAsync(_rectangle2);
                // _playbackMode = PlaybackMode.Auto;

                break;
            }
            case PlaybackMode.Auto:
            {
                if (_isPlaying)
                {
                    _playbackClock.PlayState = PlayState.Pause;
                    // _animationTrigger.OnNext(false);
                    _isPlaying = false;
                }
                else
                {
                    _playbackClock.PlayState = PlayState.Run;
                    // _animationTrigger.OnNext(true);
                    _isPlaying = true;
                }

                // _animation1.RunAsync(_rectangle1, _timelineClock);
                // _animation2.RunAsync(_rectangle2, _timelineClock);
                // _timelineClock.Step(TimeSpan.FromMilliseconds(0));
                // _playbackMode = PlaybackMode.Manual;

                break;
            }
        } 
    }
    
    public void CreateAnimation1()
    {
        _animation1 = new Animation
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
                new KeyFrame
                {
                    KeyTime = TimeSpan.FromSeconds(0),
                    Setters =
                    {
                        new Setter(Visual.OpacityProperty, 1.0),
                        new Setter(RotateTransform.AngleProperty, 0d),
                    }
                },
                new KeyFrame
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

    public void CreateAnimation2()
    {
        _animation2 = new Animation
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
                new KeyFrame
                {
                    KeyTime = TimeSpan.FromSeconds(0),
                    Setters =
                    {
                        new Setter(Visual.OpacityProperty, 0.0),
                        new Setter(ScaleTransform.ScaleXProperty, 0d),
                        new Setter(ScaleTransform.ScaleYProperty, 0d),
                    }
                },
                new KeyFrame
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

    public void RunAnimation1(Animatable animatable)
    {
        _animation1?.RunAsync(animatable, _playbackClock);
        // _disposable1 = _animation1.Apply(animatable, null, _animationTrigger, null);
        // _animation1.RunAsync(animatable, _timelineClock);
    }

    public void RunAnimation2(Animatable animatable)
    {
        _animation2?.RunAsync(animatable, _playbackClock);
        // _disposable2 = _animation2.Apply(animatable, null, _animationTrigger, null);
        // _animation2.RunAsync(animatable, _timelineClock);
    }

    public void Play()
    {
        _playbackClock.PlayState = PlayState.Run;
        // _animationTrigger.OnNext(true);
        // _timelineClock.Step(TimeSpan.FromMilliseconds(0));
        _isPlaying = true; 
    }
}
