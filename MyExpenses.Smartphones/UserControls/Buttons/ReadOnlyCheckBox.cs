namespace MyExpenses.Smartphones.UserControls.Buttons;

public class ReadOnlyCheckBox : CheckBox
{
    public static readonly BindableProperty IsReadOnlyProperty =
        BindableProperty.Create(
            nameof(IsReadOnly),
            typeof(bool),
            typeof(ReadOnlyCheckBox),
            false,
            propertyChanged: OnIsReadOnlyChanged);

    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    private static void OnIsReadOnlyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ReadOnlyCheckBox checkBox)
        {
            checkBox.UpdateIsReadOnly((bool)newValue);
        }
    }

    protected override void OnParentSet()
    {
        base.OnParentSet();
        UpdateIsReadOnly(IsReadOnly);
    }

    private void UpdateIsReadOnly(bool isReadOnly)
    {
        InputTransparent = isReadOnly;
    }
}
