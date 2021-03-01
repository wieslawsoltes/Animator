using System;
using System.Collections.ObjectModel;
using Animator.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace Animator.Controls
{
    public class Timeline : Panel
    {
        private readonly AnimationViewModel _animation;
        private readonly SolidColorBrush? _backgroundBrush;
        private readonly SolidColorBrush? _gripBrush;
        private Rect _backgroundRect;
        private Rect _leftGripRect;
        private Rect _rightGripRect;
        private bool _drag;
        private Point _dragStart;
        private TimelineHitTestResult _dragResult;

        public AnimationViewModel Animation => _animation;

        public Timeline()
        {
            _animation = new AnimationViewModel()
            {
                KeyFrames = new ObservableCollection<KeyFrameViewModel>()
            };

            _backgroundBrush = new SolidColorBrush(Colors.WhiteSmoke);
            _gripBrush = new SolidColorBrush(Colors.WhiteSmoke, 0.6);

            AddHandler(PointerPressedEvent, PointerPressedHandler, RoutingStrategies.Tunnel);
            AddHandler(PointerReleasedEvent, PointerReleasedHandler, RoutingStrategies.Tunnel);
            AddHandler(PointerMovedEvent, PointerMovedHandler, RoutingStrategies.Tunnel);
            AddHandler(PointerLeaveEvent, PointerLeaveHandler, RoutingStrategies.Tunnel);

            this.GetObservable(BoundsProperty).Subscribe(x => UpdateRects(x.Width, x.Height));
        }

        private TimelineHitTestResult HitTest(Point point, out int index)
        {
            var cueIndex = _animation.HitTest(point);
            if (cueIndex >= 0)
            {
                index = cueIndex;
                return TimelineHitTestResult.Cue;
            }

            if (_leftGripRect.Contains(point))
            {
                index = -1;
                return TimelineHitTestResult.LeftGrip;
            }

            if (_rightGripRect.Contains(point))
            {
                index = -1;
                return TimelineHitTestResult.RightGrip;
            }

            if (_backgroundRect.Contains(point))
            {
                index = -1;
                return TimelineHitTestResult.Background;
            }
            
            index = -1;
            return TimelineHitTestResult.None;
        }

        private void UpdateRects(double width, double height)
        {
            _backgroundRect = new Rect(
                _animation.CuesMarginLeft,
                0,
                Bounds.Width - _animation.CuesMarginLeft - _animation.CuesMarginRight,
                height);

            _leftGripRect = new Rect(
                0,
                0,
                _animation.CuesMarginLeft,
                height);

            _rightGripRect = new Rect(
                width - _animation.CuesMarginRight,
                0,
                _animation.CuesMarginRight,
                height);
  
            _animation.Invalidate(width, height);
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
                    _animation.BeginMoveCue(hitTestIndex);
                    _drag = true;
                    _dragResult = hitTestResult;
                    _dragStart = position;
                    SetCursor(hitTestResult, e.KeyModifiers);
                }
                else if (pointerPoint.Properties.IsRightButtonPressed)
                {
                    if (hitTestResult == TimelineHitTestResult.Cue)
                    {
                        _animation.RemoveCue(hitTestIndex);
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
                
                if (point.X < _animation.CuesMarginLeft || point.X > Bounds.Width - _animation.CuesMarginRight)
                {
                    return;
                }

                var cue = _animation.CalculateCue(point, Bounds.Width);
                var newCueIndex = _animation.AddCue(cue);

                _animation.BeginMoveCue(newCueIndex);
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
                    Cursor = new Cursor(StandardCursorType.SizeWestEast);
                    break;
                case TimelineHitTestResult.Background:
                    Cursor = keyModifiers == KeyModifiers.Control 
                        ? new Cursor(StandardCursorType.Cross) 
                        : new Cursor(StandardCursorType.Hand);
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
                    {
                        break;
                    }
                    case TimelineHitTestResult.Cue:
                    {
                        _animation.MoveCue(point, Bounds.Width);
                        UpdateRects(Bounds.Width, Bounds.Height);
                        InvalidateVisual();
                        break;
                    }
                    case TimelineHitTestResult.Background:
                    {
                        MoveBackground(position);
                        UpdateRects(Bounds.Width, Bounds.Height);
                        InvalidateVisual();
                        _dragStart = position;
                        break;
                    }
                    case TimelineHitTestResult.LeftGrip:
                    {
                        MoveLeftGrip(position);
                        UpdateRects(Width, Bounds.Height);
                        InvalidateVisual();
                        _dragStart = position;
                        break;
                    }
                    case TimelineHitTestResult.RightGrip:
                    {
                        MoveRightGrip(position);
                        UpdateRects(Width, Bounds.Height);
                        InvalidateVisual();
                        _dragStart = position;
                        break;
                    }
                }
                
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

        private void MoveBackground(Point position)
        {
            var deltaX = position.X - _dragStart.X;
            var currentLeft = Canvas.GetLeft(this);
            var left = Math.Round(currentLeft + deltaX, 0);
            if (left >= 0)
            {
                Canvas.SetLeft(this, left);
            }
        }

        private void MoveLeftGrip(Point position)
        {
            var deltaX = position.X - _dragStart.X;
            var currentLeft = Canvas.GetLeft(this);
            var left = Math.Round(currentLeft + deltaX, 0);
            if (left >= 0)
            {
                Canvas.SetLeft(this, left);
                Width = Math.Round(Width - deltaX, 0);
            }
        }

        private void MoveRightGrip(Point position)
        {
            var deltaX = position.X - _dragStart.X;
            Width = Math.Round(Width + deltaX, 0);
        }

        private void PointerLeaveHandler(object? sender, PointerEventArgs e)
        {
            Cursor = Cursor.Default;
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

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            DrawBackground(context);
            DrawLeftGrip(context);
            DrawRightGrip(context);

            _animation.Render(context);
        }
    }
}