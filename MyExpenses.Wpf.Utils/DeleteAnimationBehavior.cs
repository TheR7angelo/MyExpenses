using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Xaml.Behaviors;

namespace MyExpenses.Wpf.Utils;

public class DeleteAnimationBehavior : Behavior<FrameworkElement>
{
    private bool _isAnimationRunning;

    public ICommand DeleteCommand
    {
        get => (ICommand)GetValue(DeleteCommandProperty);
        set => SetValue(DeleteCommandProperty, value);
    }

    public static readonly DependencyProperty DeleteCommandProperty =
        DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(DeleteAnimationBehavior), new PropertyMetadata(null));

    public object DeleteCommandParameter {
        get => GetValue(DeleteCommandParameterProperty);
        set => SetValue(DeleteCommandParameterProperty, value);
    }
    public static readonly DependencyProperty DeleteCommandParameterProperty =
        DependencyProperty.Register(nameof(DeleteCommandParameter), typeof(object), typeof(DeleteAnimationBehavior), new PropertyMetadata(null));

    public static readonly DependencyProperty IsDeletingProperty =
        DependencyProperty.Register(nameof(IsDeleting), typeof(bool), typeof(DeleteAnimationBehavior),
            new PropertyMetadata(false, OnIsDeletingChanged));

    public bool IsDeleting
    {
        get => (bool)GetValue(IsDeletingProperty);
        set => SetValue(IsDeletingProperty, value);
    }

    private static void OnIsDeletingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is DeleteAnimationBehavior behavior && (bool)e.NewValue)
        {
            behavior.StartAnimation();
        }
    }


    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(nameof(Duration), typeof(TimeSpan), typeof(DeleteAnimationBehavior),
            new PropertyMetadata(TimeSpan.FromSeconds(0.3)));

    public TimeSpan Duration
    {
        get => (TimeSpan)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    private void StartAnimation()
    {
        if (_isAnimationRunning || AssociatedObject == null) return;
        _isAnimationRunning = true;

        var element = AssociatedObject;
        if (element is null) return;

        if (element.LayoutTransform is not ScaleTransform) element.LayoutTransform = new ScaleTransform(1, 1);

        var sb = new Storyboard();

        var duration = new Duration(Duration);
        var easing = new CubicEase { EasingMode = EasingMode.EaseIn };

        var fadeOut = new DoubleAnimation(0, duration);
        Storyboard.SetTarget(fadeOut, element);
        Storyboard.SetTargetProperty(fadeOut, new PropertyPath(UIElement.OpacityProperty));

        var scaleX = new DoubleAnimation(0, duration) { EasingFunction = easing };
        Storyboard.SetTarget(scaleX, element);
        Storyboard.SetTargetProperty(scaleX, new PropertyPath("(FrameworkElement.LayoutTransform).(ScaleTransform.ScaleX)"));

        var scaleY = new DoubleAnimation(0, duration) { EasingFunction = easing };
        Storyboard.SetTarget(scaleY, element);
        Storyboard.SetTargetProperty(scaleY, new PropertyPath("(FrameworkElement.LayoutTransform).(ScaleTransform.ScaleY)"));

        sb.Children.Add(fadeOut);
        sb.Children.Add(scaleX);
        sb.Children.Add(scaleY);

        sb.Completed += (_, _) =>
        {
            if (_isAnimationRunning || AssociatedObject == null) return;
            _isAnimationRunning = true;

            var cmd = DeleteCommand;
            var param = DeleteCommandParameter;

            if (cmd.CanExecute(param))
            {
                cmd.Execute(param);
            }
        };

        sb.Begin();
    }
}