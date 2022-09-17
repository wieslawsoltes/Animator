using System;
using System.Reactive.Linq;
using Avalonia.Animation;
using Avalonia.Reactive;

namespace Animator.Clocks;

public class ClockBase1 : IClock
{
    private ClockObservable _observable;
    private IObservable<TimeSpan> _connectedObservable;
    private TimeSpan? _previousTime;
    private TimeSpan _internalTime;

    protected ClockBase1()
    {
        _observable = new ClockObservable();
        _connectedObservable = _observable.Publish().RefCount();
    }

    protected bool HasSubscriptions => _observable.HasSubscriptions;

    public PlayState PlayState { get; set; }

    public void Pulse(TimeSpan systemTime)
    {
        if (!_previousTime.HasValue)
        {
            _previousTime = systemTime;
            _internalTime = TimeSpan.Zero;
        }
        else
        {
            if (PlayState == PlayState.Pause)
            {
                _previousTime = systemTime;
                return;
            }
            var delta = systemTime - _previousTime;
            _internalTime += delta.Value;
            _previousTime = systemTime;
        }

        _observable.Pulse(_internalTime);

        if (PlayState == PlayState.Stop)
        {
            Stop();
        }
    }

    public void Step(TimeSpan time)
    {
        _internalTime = time;

        _observable.Pulse(_internalTime);

        if (PlayState == PlayState.Stop)
        {
            Stop();
        }
    }

    protected virtual void Stop()
    {
    }

    public IDisposable Subscribe(IObserver<TimeSpan> observer)
    {
        return _connectedObservable.Subscribe(observer);
    }

    private class ClockObservable : LightweightObservableBase<TimeSpan>
    {
        public bool HasSubscriptions { get; private set; }
        public void Pulse(TimeSpan time) => PublishNext(time);
        protected override void Initialize() => HasSubscriptions = true;
        protected override void Deinitialize() => HasSubscriptions = false;
    }
}
