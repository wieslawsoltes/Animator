using System;
using System.Collections.Generic;
using Avalonia.Animation;

namespace Animator.Clocks;

internal class TimelineClock : IClock, IDisposable
{
    private TimeSpan _curTime;
    private readonly List<IObserver<TimeSpan>> _observers;

    public TimelineClock()
    {
        _observers = new();
    }

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