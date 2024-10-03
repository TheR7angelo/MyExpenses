namespace MyExpenses.Smartphones.Resources.Styles.Assists;

public static class RippleAssist
{
    #region ClipToBounds

    public static readonly BindableProperty ClipToBoundsProperty = BindableProperty.CreateAttached(
        "ClipToBounds", typeof(bool), typeof(RippleAssist), true);

    public static void SetClipToBounds(BindableObject element, bool value)
    {
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
    public static readonly BindableProperty IsCenteredProperty = BindableProperty.CreateAttached(
        "IsCentered", typeof(bool), typeof(RippleAssist), false);

    /// <summary>
    /// Set to <c>true</c> to cause the ripple to originate from the centre of the
    /// content.  Otherwise the effect will originate from the mouse down position.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="value"></param>
    public static void SetIsCentered(BindableObject element, bool value)
    {
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
    public static readonly BindableProperty IsDisabledProperty = BindableProperty.CreateAttached(
        "IsDisabled", typeof(bool), typeof(RippleAssist), false);

    /// <summary>
    /// Set to <c>True</c> to disable ripple effect
    /// </summary>
    /// <param name="element"></param>
    /// <param name="value"></param>
    public static void SetIsDisabled(BindableObject element, bool value)
    {
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

    public static readonly BindableProperty RippleSizeMultiplierProperty = BindableProperty.CreateAttached(
        "RippleSizeMultiplier", typeof(double), typeof(RippleAssist), 1.0);

    public static void SetRippleSizeMultiplier(BindableObject element, double value)
    {
        element.SetValue(RippleSizeMultiplierProperty, value);
    }

    public static double GetRippleSizeMultiplier(BindableObject element)
    {
        return (double)element.GetValue(RippleSizeMultiplierProperty);
    }

    #endregion

    #region Feedback

    public static readonly BindableProperty FeedbackProperty = BindableProperty.CreateAttached(
        "Feedback", typeof(Brush), typeof(RippleAssist), default(Brush));

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

    public static readonly BindableProperty RippleOnTopProperty = BindableProperty.CreateAttached(
        "RippleOnTop", typeof(bool), typeof(RippleAssist), false);

    public static void SetRippleOnTop(BindableObject element, bool value)
    {
        element.SetValue(RippleOnTopProperty, value);
    }

    public static bool GetRippleOnTop(BindableObject element)
    {
        return (bool)element.GetValue(RippleOnTopProperty);
    }

    #endregion
}