using System.Collections.ObjectModel;
using Animator.ViewModels;
using Animator.ViewModels.Animation;
using Animator.ViewModels.Style;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Animator
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var project = CreateProject();

                desktop.MainWindow = new MainWindow()
                {
                    DataContext = project
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        private static ProjectViewModel CreateProject()
        {
            return new ProjectViewModel()
            {
                Name = "Project1",
                Path = "",
                Styles = new ObservableCollection<StyleViewModel>()
                {
                    new StyleViewModel()
                    {
                        Name = "style1",
                        Selector = "",
                        Setters = new ObservableCollection<SetterViewModel>()
                        {
                            new SetterViewModel() { Property = "", Value = "" }
                        },
                        Animations = new ObservableCollection<AnimationViewModel>()
                        {
                            new AnimationViewModel()
                            {
                                Name = "animation1",
                                KeyFrames = new ObservableCollection<KeyFrameViewModel>()
                                {
                                    new KeyFrameViewModel()
                                    {
                                        Setters = new ObservableCollection<SetterViewModel>()
                                        {
                                            new SetterViewModel() { Property = "", Value = "" }
                                        }
                                    },
                                    new KeyFrameViewModel()
                                    {
                                        Setters = new ObservableCollection<SetterViewModel>()
                                        {
                                            new SetterViewModel() { Property = "", Value = "" }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}