using System;
using System.Collections.ObjectModel;
using Animator.ViewModels.Style;
using ReactiveUI;

namespace Animator.ViewModels.Animation
{
    public class KeyFrameViewModel : ViewModelBase
    {
        private TimeSpan _keyTime;
        private ObservableCollection<SetterViewModel>? _setters;

        public TimeSpan KeyTime
        {
            get => _keyTime;
            set => this.RaiseAndSetIfChanged(ref _keyTime, value);
        }

        public ObservableCollection<SetterViewModel>? Setters
        {
            get => _setters;
            set => this.RaiseAndSetIfChanged(ref _setters, value);
        }
    }
}