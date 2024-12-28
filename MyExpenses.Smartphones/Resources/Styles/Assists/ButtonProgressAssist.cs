namespace MyExpenses.Smartphones.Resources.Styles.Assists;

    public static class ButtonProgressAssist
    {
        private const double DefaultMaximum = 100.0;

        #region AttachedProperty : MinimumProperty

        private static readonly BindableProperty MinimumProperty =
            BindableProperty.CreateAttached("Minimum", typeof(double), typeof(ButtonProgressAssist), default(double));

        public static double GetMinimum(BindableObject element) => (double)element.GetValue(MinimumProperty);
        public static void SetMinimum(BindableObject element, double value) => element.SetValue(MinimumProperty, value);
        #endregion

        #region AttachedProperty : MaximumProperty

        private static readonly BindableProperty MaximumProperty =
            BindableProperty.CreateAttached("Maximum", typeof(double), typeof(ButtonProgressAssist), DefaultMaximum);

        public static double GetMaximum(BindableObject element) => (double)element.GetValue(MaximumProperty);
        public static void SetMaximum(BindableObject element, double value) => element.SetValue(MaximumProperty, value);
        #endregion

        #region AttachedProperty : ValueProperty

        private static readonly BindableProperty ValueProperty =
            BindableProperty.CreateAttached("Value", typeof(double), typeof(ButtonProgressAssist), default(double));

        public static double GetValue(BindableObject element) => (double)element.GetValue(ValueProperty);
        public static void SetValue(BindableObject element, double value) => element.SetValue(ValueProperty, value);
        #endregion

        #region AttachedProperty : IsIndeterminate

        private static readonly BindableProperty IsIndeterminateProperty =
            BindableProperty.CreateAttached("IsIndeterminate", typeof(bool), typeof(ButtonProgressAssist), default(bool));

        public static bool GetIsIndeterminate(BindableObject element) => (bool)element.GetValue(IsIndeterminateProperty);
        public static void SetIsIndeterminate(BindableObject element, bool isIndeterminate) => element.SetValue(IsIndeterminateProperty, isIndeterminate);
        #endregion

        #region AttachedProperty : IndicatorForegroundProperty
        public static readonly BindableProperty IndicatorForegroundProperty =
            BindableProperty.CreateAttached("IndicatorForeground", typeof(Brush), typeof(ButtonProgressAssist), default(Brush));

        public static Brush GetIndicatorForeground(BindableObject element) => (Brush)element.GetValue(IndicatorForegroundProperty);
        public static void SetIndicatorForeground(BindableObject element, Brush indicatorForeground) => element.SetValue(IndicatorForegroundProperty, indicatorForeground);
        #endregion

        #region AttachedProperty : IndicatorBackgroundProperty
        public static readonly BindableProperty IndicatorBackgroundProperty =
            BindableProperty.CreateAttached("IndicatorBackground", typeof(Brush), typeof(ButtonProgressAssist), default(Brush));

        public static Brush GetIndicatorBackground(BindableObject element) => (Brush)element.GetValue(IndicatorBackgroundProperty);
        public static void SetIndicatorBackground(BindableObject element, Brush indicatorBackground) => element.SetValue(IndicatorBackgroundProperty, indicatorBackground);
        #endregion

        #region AttachedProperty : IsIndicatorVisibleProperty
        public static readonly BindableProperty IsIndicatorVisibleProperty =
            BindableProperty.CreateAttached("IsIndicatorVisible", typeof(bool), typeof(ButtonProgressAssist), default(bool));

        public static bool GetIsIndicatorVisible(BindableObject element) => (bool)element.GetValue(IsIndicatorVisibleProperty);
        public static void SetIsIndicatorVisible(BindableObject element, bool isIndicatorVisible) => element.SetValue(IsIndicatorVisibleProperty, isIndicatorVisible);
        #endregion

        #region AttachedProperty : OpacityProperty
        public static readonly BindableProperty OpacityProperty =
            BindableProperty.CreateAttached("Opacity", typeof(double), typeof(ButtonProgressAssist), default(double));

        public static double GetOpacity(BindableObject element) => (double)element.GetValue(OpacityProperty);
        public static void SetOpacity(BindableObject element, double opacity) => element.SetValue(OpacityProperty, opacity);
        #endregion
    }