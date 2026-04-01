namespace MyExpenses.Presentation.Enums;

public enum MessageBoxInputResult
{
    None,
    Cancel,
    Delete,
    Valid,
}

public enum MessageBoxResult
{
    None = 0,
    /// <summary>The result value of the message box is OK.</summary>
    Ok = 1,
    /// <summary>The result value of the message box is Cancel.</summary>
    Cancel = 2,
    /// <summary>The result value of the message box is Yes.</summary>
    Yes = 6,
    /// <summary>The result value of the message box is No.</summary>
    No = 7,
}

public enum MessageBoxButton
{
    /// <summary>The message box displays an OK button.</summary>
    Ok = 0,
    /// <summary>The message box displays OK and Cancel buttons.</summary>
    OkCancel = 1,
    /// <summary>The message box displays Yes, No, and Cancel buttons.</summary>
    YesNoCancel = 3,
    /// <summary>The message box displays Yes and No buttons.</summary>
    YesNo = 4,
}
