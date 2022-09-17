using CommunityToolkit.Mvvm.ComponentModel;

namespace Animator.ViewModels.Style;

public partial class SetterViewModel : ViewModelBase
{
    [ObservableProperty] private string? _property;
    [ObservableProperty] private string? _value;
}
