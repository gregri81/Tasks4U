using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Tasks4U.Behaviors
{
    /// <summary>
    /// This behavior scales the associated object according to the mouse wheel - 
    /// scales up when the mouse wheel scrolls up 
    /// and scales down when mouse wheel scrolls down.
    /// </summary>
    public class ScalingBehavior: Behavior<FrameworkElement>
    {
        private const double Step = 0.1;

        private double _xTransform = 1;
        private double _yTransform = 1;

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PreviewMouseWheel += AssociatedObject_PreviewMouseWheel;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseWheel -= AssociatedObject_PreviewMouseWheel;
            base.OnDetaching();
        }

        void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!Keyboard.IsKeyDown(Key.LeftCtrl))
                return;

            if (e.Delta > 0) // Then the mouse wheel scrolls up
            {
                _xTransform = Math.Min(_xTransform + Step, 2);
                _yTransform = Math.Min(_yTransform + Step, 2);
            }
            else // Then the mouse wheel scrolls down
            {
                _xTransform = Math.Max(_xTransform - Step, 0.5);
                _yTransform = Math.Max(_yTransform - Step, 0.5);
            }

            AssociatedObject.LayoutTransform = new ScaleTransform(_xTransform, _yTransform);

            e.Handled = true;
        }
    }
}
