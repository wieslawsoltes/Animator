using System.Collections.ObjectModel;
using ReactiveUI;

namespace Animator.ViewModels.Animation
{
    public class AnimationViewModel : ViewModelBase
    {
        private string _name;
        private ObservableCollection<KeyFrameViewModel> _keyFrames;

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public ObservableCollection<KeyFrameViewModel> KeyFrames
        {
            get => _keyFrames;
            set => this.RaiseAndSetIfChanged(ref _keyFrames, value);
        }
    }
}