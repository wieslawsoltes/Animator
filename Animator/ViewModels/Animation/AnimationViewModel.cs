using System;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace Animator.ViewModels.Animation
{
    public class AnimationViewModel : ViewModelBase
    {
        private string _name;
        private TimeSpan _duration;
        private TimeSpan _delay;
        private ObservableCollection<KeyFrameViewModel> _keyFrames;

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public TimeSpan Duration
        {
            get => _duration;
            set => this.RaiseAndSetIfChanged(ref _duration, value);
        }

        public TimeSpan Delay
        {
            get => _delay;
            set => this.RaiseAndSetIfChanged(ref _delay, value);
        }

        public ObservableCollection<KeyFrameViewModel> KeyFrames
        {
            get => _keyFrames;
            set => this.RaiseAndSetIfChanged(ref _keyFrames, value);
        }
    }
}