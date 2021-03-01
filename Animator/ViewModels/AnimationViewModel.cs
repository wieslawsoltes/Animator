using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Media;
using ReactiveUI;

namespace Animator.ViewModels
{
    public class AnimationViewModel : ReactiveObject
    {
        private ObservableCollection<KeyFrameViewModel> _keyFrames;
        private ObservableCollection<Rect> _cueRects;
        private SolidColorBrush? _cueBrush;
        private int _cueDigitsPrecision;
        private int _cueSize;
        private double _cuesMarginLeft;
        private double _cuesMarginRight;
        private bool _drawCueLabels;
        private double _cueLabelsHeight;
        private double _cueCornerRadius;
        private int _dragCueIndex;

        public ObservableCollection<KeyFrameViewModel> KeyFrames
        {
            get => _keyFrames;
            set => this.RaiseAndSetIfChanged(ref _keyFrames, value);
        }

        public double CuesMarginLeft => _cuesMarginLeft;

        public double CuesMarginRight => _cuesMarginRight;
        
        public AnimationViewModel()
        {
            _keyFrames = new ObservableCollection<KeyFrameViewModel>();
            _cueRects = new ObservableCollection<Rect>();
            _cueBrush = new SolidColorBrush(Colors.Blue);
            _cueDigitsPrecision = 2;
            _cueSize = 10;
            _cuesMarginLeft = 20;
            _cuesMarginRight = 20;
            _cueLabelsHeight = 15;
            _drawCueLabels = false;
            _cueCornerRadius = 0;
        }

        private Rect GetCueRect(double cue, double width, double height)
        {
            var x = ((width - _cuesMarginLeft - _cuesMarginRight) * cue) - _cueSize / 2.0 + _cuesMarginLeft;
            var y = _drawCueLabels ? _cueLabelsHeight : 0;
            var rect = new Rect(x, y, _cueSize, height - y);
            return rect;
        }

        public void Invalidate(double width, double height)
        {
            _cueRects.Clear();

            foreach (var keyFrame in _keyFrames)
            {
                var rect = GetCueRect(keyFrame.Cue, width, height);
                _cueRects.Add(rect);
            }
        }

        public double CalculateCue(Point point, double width)
        {
            var cue = (point.X - _cuesMarginLeft) / (width - _cuesMarginLeft - _cuesMarginRight);
            cue = Math.Round(cue, _cueDigitsPrecision);
            cue = Math.Clamp(cue, 0.0, 1.0);
            return cue;
        }

        public int AddCue(double cue)
        {
            var keyFrame = new KeyFrameViewModel() {Cue = cue};

            if (_keyFrames.Count == 0)
            {
                _keyFrames.Add(keyFrame);
                return 0;
            }

            for (var i = 0; i < _keyFrames.Count; i++)
            {
                if (cue < _keyFrames[i].Cue)
                {
                    _keyFrames.Insert(i, keyFrame);
                    return i;
                }
            }

            _keyFrames.Add(keyFrame);

            return _keyFrames.Count - 1;
        }

        public void RemoveCue(int index)
        {
            _keyFrames.RemoveAt(index);
        }

        public void BeginMoveCue(int index)
        {
            _dragCueIndex = index;
        }
        
        public void MoveCue(Point point, double width)
        {
            var cue = CalculateCue(point, width);
            _keyFrames.RemoveAt(_dragCueIndex);
            _dragCueIndex = AddCue(cue);
        }

        public int HitTest(Point point)
        {
            for (var i = 0; i < _cueRects.Count; i++)
            {
                if (_cueRects[i].Contains(point))
                {
                    return i;
                }
            }

            return -1;
        }

        private void DrawCueLabels(DrawingContext context)
        {
            var typeface = new Typeface(FontFamily.Default);

            for (var i = 0; i < _cueRects.Count; i++)
            {
                var rect = _cueRects[i];
                var text = $"{(int) (_keyFrames[i].Cue * 100.0)}%";

                var formattedText = new FormattedText()
                {
                    Typeface = typeface,
                    Text = text,
                    TextAlignment = TextAlignment.Center,
                    TextWrapping = TextWrapping.NoWrap,
                    FontSize = 10,
                    Constraint = new Size(0, _cueLabelsHeight)
                };

                var origin = new Point(
                    rect.Center.X - formattedText.Bounds.Width / 2.0,
                    (_cueLabelsHeight - formattedText.Bounds.Height) / 2.0);

                context.DrawText(_cueBrush, origin, formattedText);
            }
        }
        
        private void DrawCues(DrawingContext context)
        {
            for (var i = 0; i < _cueRects.Count; i++)
            {
                var cueRect = _cueRects[i];
                context.DrawRectangle(_cueBrush, null, cueRect, _cueCornerRadius, _cueCornerRadius);
            }
        }
        
        public void Render(DrawingContext context)
        {
            if (_drawCueLabels)
            {
                DrawCueLabels(context);
            }

            DrawCues(context);
        }
    }
}