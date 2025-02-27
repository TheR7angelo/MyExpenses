namespace MyExpenses.Smartphones.Resources.Styles.Assists;

public static class RippleAssist
{
    #region ClipToBounds

    public static readonly BindableProperty ClipToBoundsProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.CreateAttached("ClipToBounds", typeof(bool), typeof(RippleAssist), true);

    public static void SetClipToBounds(BindableObject element, bool value)
    {
        // ReSharper disable once HeapView.BoxingAllocation
        element.SetValue(ClipToBoundsProperty, value);
    }

    public static bool GetClipToBounds(BindableObject element)
    {
        return (bool)element.GetValue(ClipToBoundsProperty);
    }

    #endregion

    #region StayOnCenter

    /// <summary>
    /// Set to <c>true</c> to cause the ripple to originate from the centre of the
    /// content.  Otherwise the effect will originate from the mouse down position.
    /// </summary>
    public static readonly BindableProperty IsCenteredProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.CreateAttached("IsCentered", typeof(bool), typeof(RippleAssist), false);

    /// <summary>
    /// Set to <c>true</c> to cause the ripple to originate from the centre of the
    /// content.  Otherwise the effect will originate from the mouse down position.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="value"></param>
    public static void SetIsCentered(BindableObject element, bool value)
    {
        // ReSharper disable once HeapView.BoxingAllocation
        element.SetValue(IsCenteredProperty, value);
    }

    /// <summary>
    /// Set to <c>true</c> to cause the ripple to originate from the centre of the
    /// content.  Otherwise the effect will originate from the mouse down position.
    /// </summary>
    /// <param name="element"></param>
    public static bool GetIsCentered(BindableObject element)
    {
        return (bool)element.GetValue(IsCenteredProperty);
    }

    #endregion

    #region IsDisabled

    /// <summary>
    /// Set to <c>True</c> to disable ripple effect
    /// </summary>
    private static readonly BindableProperty IsDisabledProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.CreateAttached("IsDisabled", typeof(bool), typeof(RippleAssist), false);

    /// <summary>
    /// Set to <c>True</c> to disable ripple effect
    /// </summary>
    /// <param name="element"></param>
    /// <param name="value"></param>
    public static void SetIsDisabled(BindableObject element, bool value)
    {
        // ReSharper disable once HeapView.BoxingAllocation
        element.SetValue(IsDisabledProperty, value);
    }

    /// <summary>
    /// Set to <c>True</c> to disable ripple effect
    /// </summary>
    /// <param name="element"></param>
    public static bool GetIsDisabled(BindableObject element)
    {
        return (bool)element.GetValue(IsDisabledProperty);
    }

    #endregion

    #region RippleSizeMultiplier

    private static readonly BindableProperty RippleSizeMultiplierProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.CreateAttached("RippleSizeMultiplier", typeof(double), typeof(RippleAssist), 1.0);

    public static void SetRippleSizeMultiplier(BindableObject element, double value)
    {
        // ReSharper disable once HeapView.BoxingAllocation
        element.SetValue(RippleSizeMultiplierProperty, value);
    }

    public static double GetRippleSizeMultiplier(BindableObject element)
    {
        return (double)element.GetValue(RippleSizeMultiplierProperty);
    }

    #endregion

    #region Feedback

    public static readonly BindableProperty FeedbackProperty = BindableProperty.CreateAttached(
        "Feedback", typeof(Brush), typeof(RippleAssist), null);

    public static void SetFeedback(BindableObject element, Brush value)
    {
        element.SetValue(FeedbackProperty, value);
    }

    public static Brush GetFeedback(BindableObject element)
    {
        return (Brush)element.GetValue(FeedbackProperty);
    }

    #endregion

    #region RippleOnTop

    private static readonly BindableProperty RippleOnTopProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.CreateAttached("RippleOnTop", typeof(bool), typeof(RippleAssist), false);

    public static void SetRippleOnTop(BindableObject element, bool value)
    {
        // ReSharper disable once HeapView.BoxingAllocation
        element.SetValue(RippleOnTopProperty, value);
    }

    public static bool GetRippleOnTop(BindableObject element)
    {
        return (bool)element.GetValue(RippleOnTopProperty);
    }

    #endregion
}