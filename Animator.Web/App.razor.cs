using Avalonia.ReactiveUI;
using Avalonia.Web.Blazor;

namespace Animator.Web;

public partial class App
{
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        WebAppBuilder.Configure<Animator.App>()
            .UseReactiveUI()
            .SetupWithSingleViewLifetime();
    }
}
