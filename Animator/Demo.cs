using System;
using System.Collections.ObjectModel;
using Animator.ViewModels;
using Animator.ViewModels.Animation;
using Animator.ViewModels.Style;

namespace Animator;

public static class Demo
{
    public static ProjectViewModel Create()
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
}
