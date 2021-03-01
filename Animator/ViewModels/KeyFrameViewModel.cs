using ReactiveUI;

namespace Animator.ViewModels
{
    public class KeyFrameViewModel : ReactiveObject
    {
        private double _cue;

        public double Cue
        {
            get => _cue;
            set => this.RaiseAndSetIfChanged(ref _cue, value);
        }
    }
}