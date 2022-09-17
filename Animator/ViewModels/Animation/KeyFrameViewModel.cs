using System;
using System.Collections.ObjectModel;
using Animator.ViewModels.Style;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Animator.ViewModels.Animation;

public partial class KeyFrameViewModel : ViewModelBase
{
    [ObservableProperty] private TimeSpan _keyTime;
    [ObservableProperty] private ObservableCollection<SetterViewModel>? _setters;
}
