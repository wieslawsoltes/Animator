using System.Text;
using Animator.ViewModels.Animation;
using Animator.ViewModels.Style;
using Avalonia.Markup.Xaml;

namespace Animator.Services;

public static class ViewModelConverter
{
    public static void ToXaml(AnimationViewModel animationViewModel, StringBuilder sb, string tab)
    {
        sb.AppendLine($"{tab}{tab}<Animation Delay=\"{animationViewModel.Delay}\" Duration=\"{animationViewModel.Duration}\">");

        if (animationViewModel.KeyFrames is { })
        {
            foreach (var keyFrameViewModel in animationViewModel.KeyFrames)
            {
                sb.AppendLine($"{tab}{tab}{tab}<KeyFrame KeyTime=\"{keyFrameViewModel.KeyTime}\">");

                if (keyFrameViewModel.Setters is { })
                {
                    foreach (var setterViewModel in keyFrameViewModel.Setters)
                    {
                        sb.AppendLine(
                            $"{tab}{tab}{tab}{tab}<Setter Property=\"{setterViewModel.Property}\" Value=\"{setterViewModel.Value}\"/>");
                    }
                }

                sb.AppendLine($"{tab}{tab}{tab}</KeyFrame>");
            }
        }

        sb.AppendLine($"{tab}{tab}</Animation>");
    }

    public static string ToXaml(StyleViewModel styleViewModel)
    {
        var tab = "  ";
        var sb = new StringBuilder();

        sb.AppendLine($"<Style Selector=\"{styleViewModel.Selector}\" xmlns=\"https://github.com/avaloniaui\">");

        if (styleViewModel.Setters is { })
        {
            foreach (var setterViewModel in styleViewModel.Setters)
            {
                sb.AppendLine(
                    $"{tab}<Setter Property=\"{setterViewModel.Property}\" Value=\"{setterViewModel.Value}\"/>");
            }
        }

        sb.AppendLine($"{tab}<Style.Animations>");

        if (styleViewModel.Animations is { })
        {
            foreach (var animationViewModel in styleViewModel.Animations)
            {
                ToXaml(animationViewModel, sb, tab);
            }
        }

        sb.AppendLine($"{tab}</Style.Animations>");

        sb.AppendLine($"</Style>");

        var xaml = sb.ToString();
        return xaml;
    }

    public static Avalonia.Styling.Style ToStyle(StyleViewModel styleViewModel)
    {
        var xaml = ToXaml(styleViewModel);
        var style = AvaloniaRuntimeXamlLoader.Parse<Avalonia.Styling.Style>(xaml);
        return style;
    }
}
