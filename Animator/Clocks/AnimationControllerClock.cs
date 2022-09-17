using System;
using Avalonia;
using Avalonia.Animation;

namespace Animator.Clocks;

public class AnimationControllerClock : ObservableClock
{
    private static IClock? GlobalClock => AvaloniaLocator.Current.GetService<IGlobalClock>();

    private readonly IDisposable? _parentSubscription;

    public AnimationControllerClock() : this(GlobalClock)
    {
    }

    private AnimationControllerClock(IClock? parent)
    {
        _parentSubscription = parent?.Subscribe(Pulse);
    }

    protected override void Stop()
    {
        _parentSubscription?.Dispose();
    }
}
