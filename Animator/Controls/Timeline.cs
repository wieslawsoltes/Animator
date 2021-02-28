using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace Animator.Controls
{
    public class Timeline : Panel
    {
        private ObservableCollection<double> _cues;
        private ObservableCollection<Rect> _cueRects;
        private SolidColorBrush? _cueBrush;
        private int _cueSize;
        private double _cueTextAreaSize;
        private double _cueCornerRadius;
        private bool _dragCue;
        private int _dragCueIndex;

        public IList<double> Cues => _cues;

        public Timeline()
        {
            _cues = new ObservableCollection<double>();
            _cueRects = new ObservableCollection<Rect>();
            _cueBrush = new SolidColorBrush(Colors.Blue);
            _cueSize = 10;
            _cueTextAreaSize = 20;
            _cueCornerRadius = 3;

            AddHandler(PointerPressedEvent, PointerPressedHandler, RoutingStrategies.Tunnel);
            AddHandler(PointerReleasedEvent, PointerReleasedHandler, RoutingStrategies.Tunnel);
            AddHandler(PointerMovedEvent, PointerMovedHandler, RoutingStrategies.Tunnel);

            this.GetObservable(BoundsProperty).Subscribe(x => UpdateCueRects(x.Width, x.Height));
        }

        private double CalculateCue(Point point, double width)
        {
            var cue = point.X / width;
            cue = Math.Round(cue, 2);
            cue = Math.Clamp(cue, 0.0, 1.0);
            return cue;
        }

        private int AddCue(double cue)
        {
            if (_cues.Count == 0)
            {
                _cues.Add(cue);
                return 0;
            }

            for (var i = 0; i < _cues.Count; i++)
            {
                if (cue < _cues[i])
                {
                    _cues.Insert(i, cue);
                    return i;
                }
            }

            _cues.Add(cue);

            return _cues.Count - 1;
        }

        private void MoveCue(Point point, double width)
        {
            var cue = CalculateCue(point, width);
            _cues.RemoveAt(_dragCueIndex);
            _dragCueIndex = AddCue(cue);
        }

        private void PointerPressedHandler(object? sender, PointerPressedEventArgs e)
        {
            var point = e.GetPosition(this);

            for (var i = 0; i < _cueRects.Count; i++)
            {
                if (_cueRects[i].Contains(point))
                {
                    _dragCueIndex = i;
                    _dragCue = true;
                    Cursor = new Cursor(StandardCursorType.SizeWestEast);
                    return;
                }
            }

            var cue = CalculateCue(point, Bounds.Width);
            AddCue(cue);

            UpdateCueRects(Bounds.Width, Bounds.Height);
            InvalidateVisual();
        }

        private void PointerReleasedHandler(object? sender, PointerReleasedEventArgs e)
        {
            if (_dragCue)
            {
                Cursor = Cursor.Default;
                _dragCue = false;
            }
        }

        private void PointerMovedHandler(object? sender, PointerEventArgs e)
        {
            if (_dragCue)
            {
                var point = e.GetPosition(this);
                MoveCue(point, Bounds.Width);
                UpdateCueRects(Bounds.Width, Bounds.Height);
                InvalidateVisual();
            }
        }

        private void UpdateCueRects(double width, double height)
        {
            _cueRects.Clear();

            foreach (var cue in _cues)
            {
                var rect = GetCueRect(cue, width, height);
                _cueRects.Add(rect);
            }
        }

        private Rect GetCueRect(double cue, double width, double height)
        {
            var x = (width * cue) - _cueSize / 2.0;
            var rect = new Rect(x, _cueTextAreaSize, _cueSize, height - _cueTextAreaSize);
            return rect;
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            var typeface = new Typeface(FontFamily.Default, FontStyle.Normal, FontWeight.Normal);

            for (var i = 0; i < _cueRects.Count; i++)
            {
                var rect = _cueRects[i];
                context.DrawRectangle(_cueBrush, null, rect, _cueCornerRadius, _cueCornerRadius);

                var text = $"{(int)(_cues[i] * 100)}%";

                var formattedText = new FormattedText()
                {
                    Typeface = typeface,
                    Text = text,
                    TextAlignment = TextAlignment.Center,
                    TextWrapping = TextWrapping.NoWrap,
                    FontSize = 10,
                    Constraint = new Size(0, _cueTextAreaSize)
                };

                var origin = new Point(
                    rect.Center.X -  formattedText.Bounds.Width / 2.0, 
                    (_cueTextAreaSize - formattedText.Bounds.Height) / 2.0);

                context.DrawText(_cueBrush, origin, formattedText);
            }
        }
    }
}