using System.Collections.ObjectModel;
using Animator.ViewModels.Animation;
using ReactiveUI;

namespace Animator.ViewModels.Style
{
    public class StyleViewModel : ViewModelBase
    {
        private string _name;
        private string _selector;
        private ObservableCollection<SetterViewModel> _setters;
        private ObservableCollection<AnimationViewModel> _animations;

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public string Selector
        {
            get => _selector;
            set => this.RaiseAndSetIfChanged(ref _selector, value);
        }

        public ObservableCollection<SetterViewModel> Setters
        {
            get => _setters;
            set => this.RaiseAndSetIfChanged(ref _setters, value);
        }

        public ObservableCollection<AnimationViewModel> Animations
        {
            get => _animations;
            set => this.RaiseAndSetIfChanged(ref _animations, value);
        }
    }
}