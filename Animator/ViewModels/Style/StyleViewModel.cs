using System.Collections.ObjectModel;
using Animator.ViewModels.Animation;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Animator.ViewModels.Style;

public partial class StyleViewModel : ViewModelBase
{
    [ObservableProperty] private string? _name;
    [ObservableProperty] private string? _selector;
    [ObservableProperty] private ObservableCollection<SetterViewModel>? _setters;
    [ObservableProperty] private ObservableCollection<AnimationViewModel>? _animations;
}
