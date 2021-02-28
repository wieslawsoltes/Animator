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
        private double _cuesMarginLeft;
        private double _cuesMarginRight;
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
            _cuesMarginLeft = 20;
            _cuesMarginRight = 20;
            _cueSize = 10;
            _cueTextAreaSize = 20;
            _cueCornerRadius = 3;

            AddHandler(PointerPressedEvent, PointerPressedHandler, RoutingStrategies.Tunnel);
            AddHandler(PointerReleasedEvent, PointerReleasedHandler, RoutingStrategies.Tunnel);
            AddHandler(PointerMovedEvent, PointerMovedHandler, RoutingStrategies.Tunnel);
            AddHandler(PointerLeaveEvent, PointerLeaveHandler, RoutingStrategies.Tunnel);

            this.GetObservable(BoundsProperty).Subscribe(x => UpdateCueRects(x.Width, x.Height));
        }

        private int HitTestCue(Point point)
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

        private double CalculateCue(Point point, double width)
        {
            var cue = (point.X - _cuesMarginLeft) / (width - _cuesMarginLeft - _cuesMarginRight);
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

            if (point.X < _cuesMarginLeft || point.X > Bounds.Width - _cuesMarginRight)
            {
                return;
            }

            var hitTestIndex = HitTestCue(point);
            if (hitTestIndex >= 0)
            {
                _dragCueIndex = hitTestIndex;
                _dragCue = true;
                Cursor = new Cursor(StandardCursorType.Hand);
                return;
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
            var point = e.GetPosition(this);

            if (_dragCue)
            {
                MoveCue(point, Bounds.Width);
                UpdateCueRects(Bounds.Width, Bounds.Height);
                InvalidateVisual();
            }
            else
            {
                var hitTestIndex = HitTestCue(point);
                if (hitTestIndex >= 0)
                {
                    Cursor = new Cursor(StandardCursorType.Hand);
                }
                else
                {
                    
                    if (point.X < _cuesMarginLeft || point.X > Bounds.Width - _cuesMarginRight)
                    {
                        Cursor = Cursor.Default;
                    }
                    else
                    {
                        Cursor = new Cursor(StandardCursorType.Cross);
                    }
                }
            }
        }

        private void PointerLeaveHandler(object? sender, PointerEventArgs e)
        {
            Cursor = Cursor.Default;
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
            var x = ((width - _cuesMarginLeft - _cuesMarginRight) * cue) - _cueSize / 2.0 + _cuesMarginLeft;
            var rect = new Rect(x, _cueTextAreaSize, _cueSize, height - _cueTextAreaSize);
            return rect;
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            var typeface = new Typeface(FontFamily.Default);

            for (var i = 0; i < _cueRects.Count; i++)
            {
                var rect = _cueRects[i];
                context.DrawRectangle(_cueBrush, null, rect, _cueCornerRadius, _cueCornerRadius);

                var text = $"{(int)(_cues[i] * 100.0)}%";

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