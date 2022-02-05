using ReactiveUI;

namespace Animator.ViewModels.Style;

public class SetterViewModel : ViewModelBase
{
    private string? _property;
    private string? _value;

    public string? Property
    {
        get => _property;
        set => this.RaiseAndSetIfChanged(ref _property, value);
    }

    public string? Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }
}