using System.Collections.ObjectModel;
using Animator.ViewModels.Style;
using ReactiveUI;

namespace Animator.ViewModels;

public class ProjectViewModel : ViewModelBase
{
    private string? _name;
    private string? _path;
    private ObservableCollection<StyleViewModel>? _styles;

    public string? Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public string? Path
    {
        get => _path;
        set => this.RaiseAndSetIfChanged(ref _path, value);
    }

    public ObservableCollection<StyleViewModel>? Styles
    {
        get => _styles;
        set => this.RaiseAndSetIfChanged(ref _styles, value);
    }
}
