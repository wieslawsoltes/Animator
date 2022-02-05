using System;
using Avalonia;
using Avalonia.Animation;

namespace Animator.Clocks;

internal class Clock1 : ClockBase1
{
    public static IClock? GlobalClock => AvaloniaLocator.Current.GetService<IGlobalClock>();

    private readonly IDisposable? _parentSubscription;

    public Clock1()
        : this(GlobalClock)
    {
    }
        
    public Clock1(IClock? parent)
    {
        _parentSubscription = parent?.Subscribe(Pulse);
    }

    protected override void Stop()
    {
        _parentSubscription?.Dispose();
    }
}