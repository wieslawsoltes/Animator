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
    public enum TimelineHitTestResult
    {
        None,
        Cue,
        Background,
        LeftGrip,
        RightGrip
    }

    public class Timeline : Panel
    {
        private ObservableCollection<double> _cues;
        private ObservableCollection<Rect> _cueRects;
        private Rect _backgroundRect;
        private Rect _leftGripRect;
        private Rect _rightGripRect;
        private SolidColorBrush? _cueBrush;
        private SolidColorBrush? _backgroundBrush;
        private SolidColorBrush? _gripBrush;
        private int _cueDigitsPrecision;
        private double _cuesMarginLeft;
        private double _cuesMarginRight;
        private int _cueSize;
        private double _cueLabelsHeight;
        private bool _drawCueLabels;
        private double _cueCornerRadius;
        private bool _drag;
        private Point _dragStart;
        private int _dragCueIndex;
        private TimelineHitTestResult _dragResult;

        public IList<double> Cues => _cues;

        public Timeline()
        {
            _cues = new ObservableCollection<double>();

            _cueRects = new ObservableCollection<Rect>();

            _cueBrush = new SolidColorBrush(Colors.Blue);

            _backgroundBrush = new SolidColorBrush(Colors.WhiteSmoke);

            _gripBrush = new SolidColorBrush(Colors.WhiteSmoke, 0.6);

            _cueDigitsPrecision = 2;
            _cuesMarginLeft = 20;
            _cuesMarginRight = 20;
            _cueSize = 10;
            _cueLabelsHeight = 15;
            _drawCueLabels = false;
            _cueCornerRadius = 0;

            AddHandler(PointerPressedEvent, PointerPressedHandler, RoutingStrategies.Tunnel);
            AddHandler(PointerReleasedEvent, PointerReleasedHandler, RoutingStrategies.Tunnel);
            AddHandler(PointerMovedEvent, PointerMovedHandler, RoutingStrategies.Tunnel);
            AddHandler(PointerLeaveEvent, PointerLeaveHandler, RoutingStrategies.Tunnel);

            this.GetObservable(BoundsProperty).Subscribe(x => UpdateRects(x.Width, x.Height));
        }

        private TimelineHitTestResult HitTest(Point point, out int index)
        {
            index = -1;

            for (var i = 0; i < _cueRects.Count; i++)
            {
                if (_cueRects[i].Contains(point))
                {
                    index = i;
                    return TimelineHitTestResult.Cue;
                }
            }

            if (_leftGripRect.Contains(point))
            {
                return TimelineHitTestResult.LeftGrip;
            }

            if (_rightGripRect.Contains(point))
            {
                return TimelineHitTestResult.RightGrip;
            }

            if (_backgroundRect.Contains(point))
            {
                return TimelineHitTestResult.Background;
            }
            
            return TimelineHitTestResult.None;
        }

        private double CalculateCue(Point point, double width)
        {
            var cue = (point.X - _cuesMarginLeft) / (width - _cuesMarginLeft - _cuesMarginRight);
            cue = Math.Round(cue, _cueDigitsPrecision);
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

        private void RemoveCue(int index)
        {
            _cues.RemoveAt(index);
        }

        private void MoveCue(Point point, double width)
        {
            var cue = CalculateCue(point, width);
            _cues.RemoveAt(_dragCueIndex);
            _dragCueIndex = AddCue(cue);
        }

        private void UpdateRects(double width, double height)
        {
            _backgroundRect = new Rect(
                _cuesMarginLeft,
                0,
                Bounds.Width - _cuesMarginLeft - _cuesMarginRight,
                height);

            _leftGripRect = new Rect(
                0,
                0,
                _cuesMarginLeft,
                height);

            _rightGripRect = new Rect(
                width - _cuesMarginRight,
                0,
                _cuesMarginRight,
                height);
  
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
            var y = _drawCueLabels ? _cueLabelsHeight : 0;
            var rect = new Rect(x, y, _cueSize, height - y);
            return rect;
        }

        private void PointerPressedHandler(object? sender, PointerPressedEventArgs e)
        {
            var point = e.GetPosition(this);
            var position = e.GetPosition(Parent);
            var pointerPoint = e.GetCurrentPoint(this);

            var hitTestResult = HitTest(point, out var hitTestIndex);
            if (hitTestResult != TimelineHitTestResult.None)
            {
                if (pointerPoint.Properties.IsLeftButtonPressed)
                {
                    _dragCueIndex = hitTestIndex;
                    _drag = true;
                    _dragResult = hitTestResult;
                    _dragStart = position;
                    SetCursor(hitTestResult, e.KeyModifiers);
                }
                else if (pointerPoint.Properties.IsRightButtonPressed)
                {
                    if (hitTestResult == TimelineHitTestResult.Cue)
                    {
                        RemoveCue(hitTestIndex);
                        UpdateRects(Bounds.Width, Bounds.Height);
                        InvalidateVisual();
                        Cursor = Cursor.Default;
                    }
                }
            }

            if (pointerPoint.Properties.IsLeftButtonPressed || pointerPoint.Properties.IsMiddleButtonPressed)
            {
                if (pointerPoint.Properties.IsLeftButtonPressed && e.KeyModifiers != KeyModifiers.Control)
                {
                    return;
                }
                
                if (point.X < _cuesMarginLeft || point.X > Bounds.Width - _cuesMarginRight)
                {
                    return;
                }

                var cue = CalculateCue(point, Bounds.Width);
                var newCueIndex = AddCue(cue);

                _dragCueIndex = newCueIndex;
                _drag = true;
                _dragResult = TimelineHitTestResult.Cue;
                _dragStart = position;
                SetCursor(TimelineHitTestResult.Cue, e.KeyModifiers);

                UpdateRects(Bounds.Width, Bounds.Height);
                InvalidateVisual();
            }
        }

        private void SetCursor(TimelineHitTestResult hitTestResult, KeyModifiers keyModifiers)
        {
            switch (hitTestResult)
            {
                case TimelineHitTestResult.None:
                    Cursor = Cursor.Default;
                    break;
                case TimelineHitTestResult.Cue:
                    Cursor = new Cursor(StandardCursorType.Hand);
                    break;
                case TimelineHitTestResult.Background:
                    Cursor = keyModifiers == KeyModifiers.Control 
                        ? new Cursor(StandardCursorType.Cross) 
                        : new Cursor(StandardCursorType.SizeWestEast);
                    break;
                case TimelineHitTestResult.LeftGrip:
                    Cursor = new Cursor(StandardCursorType.Hand);
                    break;
                case TimelineHitTestResult.RightGrip:
                    Cursor = new Cursor(StandardCursorType.Hand);
                    break;
            }
        }

        private void PointerReleasedHandler(object? sender, PointerReleasedEventArgs e)
        {
            if (_drag)
            {
                Cursor = Cursor.Default;
                _drag = false;
            }
        }

        private void PointerMovedHandler(object? sender, PointerEventArgs e)
        {
            var point = e.GetPosition(this);
            var position = e.GetPosition(Parent);

            if (_drag)
            {
                switch (_dragResult)
                {
                    case TimelineHitTestResult.None:
                        break;
                    case TimelineHitTestResult.Cue:
                        MoveCue(point, Bounds.Width);
                        break;
                    case TimelineHitTestResult.Background:
                        {
                            var deltaX = position.X - _dragStart.X;
                            var left = Canvas.GetLeft(this);
                            Canvas.SetLeft(this, Math.Round(left + deltaX, 0));
                            _dragStart = position;
                        }
                        break;
                    case TimelineHitTestResult.LeftGrip:
                        {
                            var deltaX = position.X - _dragStart.X;
                            var left = Canvas.GetLeft(this);
                            Canvas.SetLeft(this,  Math.Round(left + deltaX, 0));
                            Width =  Math.Round(Width - deltaX, 0);
                            UpdateRects(Width, Bounds.Height);
                            _dragStart = position;
                        }
                        break;
                    case TimelineHitTestResult.RightGrip:
                        {
                            var deltaX = position.X - _dragStart.X;
                            Width = Math.Round(Width + deltaX, 0);
                            UpdateRects(Width, Bounds.Height);
                            _dragStart = position;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                UpdateRects(Bounds.Width, Bounds.Height);
                InvalidateVisual();
            }
            else
            {
                var hitTestResult = HitTest(point, out _);
                if (hitTestResult != TimelineHitTestResult.None)
                {
                    SetCursor(hitTestResult, e.KeyModifiers);
                }
            }
        }

        private void PointerLeaveHandler(object? sender, PointerEventArgs e)
        {
            Cursor = Cursor.Default;
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            DrawBackground(context);
            DrawLeftGrip(context);
            DrawRightGrip(context);

            DrawCues(context);

            if (_drawCueLabels)
            {
                DrawCueLabels(context);
            }
        }

        private void DrawBackground(DrawingContext context)
        {
            context.DrawRectangle(_backgroundBrush, null, _backgroundRect);
        }
        
        private void DrawLeftGrip(DrawingContext context)
        {
            context.DrawRectangle(_gripBrush, null, _leftGripRect);
        }

        private void DrawRightGrip(DrawingContext context)
        {
            context.DrawRectangle(_gripBrush, null, _rightGripRect);
        }

        private void DrawCues(DrawingContext context)
        {
            for (var i = 0; i < _cueRects.Count; i++)
            {
                var cueRect = _cueRects[i];
                context.DrawRectangle(_cueBrush, null, cueRect, _cueCornerRadius, _cueCornerRadius);
            }
        }

        private void DrawCueLabels(DrawingContext context)
        {
            var typeface = new Typeface(FontFamily.Default);

            for (var i = 0; i < _cueRects.Count; i++)
            {
                var rect = _cueRects[i];
                var text = $"{(int) (_cues[i] * 100.0)}%";

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
    }
}