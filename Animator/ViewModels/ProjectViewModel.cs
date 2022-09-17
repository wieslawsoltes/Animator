using System.Collections.ObjectModel;
using Animator.ViewModels.Style;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Animator.ViewModels;

public partial class ProjectViewModel : ViewModelBase
{
    [ObservableProperty] private string? _name;
    [ObservableProperty] private string? _path;
    [ObservableProperty] private ObservableCollection<StyleViewModel>? _styles;
}
