using System.Collections.ObjectModel;
using ReactiveUI;

namespace Animator.ViewModels.Animation
{
    public class KeyFrameViewModel : ViewModelBase
    {
        private ObservableCollection<SetterViewModel> _setters;

        public ObservableCollection<SetterViewModel> Setters
        {
            get => _setters;
            set => this.RaiseAndSetIfChanged(ref _setters, value);
        }
    }
}