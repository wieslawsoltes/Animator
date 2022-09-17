using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Animator.ViewModels.Animation;

public partial class AnimationViewModel : ViewModelBase
{
    [ObservableProperty] private string? _name;
    [ObservableProperty] private TimeSpan _duration;
    [ObservableProperty] private TimeSpan _delay;
    [ObservableProperty] private ObservableCollection<KeyFrameViewModel>? _keyFrames;
}
