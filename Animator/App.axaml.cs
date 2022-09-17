using System;
using System.Collections.ObjectModel;
using Animator.ViewModels;
using Animator.ViewModels.Animation;
using Animator.ViewModels.Style;
using Animator.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Animator;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private static ProjectViewModel CreateDemo()
    {
        return new ProjectViewModel
        {
            Name = "Project1",
            Path = "",
            Styles = new ObservableCollection<StyleViewModel>
            {
                new StyleViewModel
                {
                    Name = "style1",
                    Selector = "Rectangle.animation",
                    Setters = new ObservableCollection<SetterViewModel>
                    {
                        new SetterViewModel { Property = "Opacity", Value = "1.0" }
                    },
                    Animations = new ObservableCollection<AnimationViewModel>
                    {
                        new AnimationViewModel
                        {
                            Name = "animation1",
                            Duration = TimeSpan.FromSeconds(2),
                            Delay = TimeSpan.FromSeconds(0),
                            KeyFrames = new ObservableCollection<KeyFrameViewModel>
                            {
                                new KeyFrameViewModel
                                {
                                    KeyTime = TimeSpan.FromSeconds(0),
                                    Setters = new ObservableCollection<SetterViewModel>
                                    {
                                        new SetterViewModel { Property = "Opacity", Value = "1.0" },
                                        new SetterViewModel { Property = "RotateTransform.Angle", Value = "0" },
                                    }
                                },
                                new KeyFrameViewModel
                                {
                                    KeyTime = TimeSpan.FromSeconds(2),
                                    Setters = new ObservableCollection<SetterViewModel>
                                    {
                                        new SetterViewModel { Property = "Opacity", Value = "0.0" },
                                        new SetterViewModel { Property = "RotateTransform.Angle", Value = "360" },
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var project = CreateDemo();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = project
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime single)
        {
            single.MainView = new MainView
            {
                DataContext = project
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
