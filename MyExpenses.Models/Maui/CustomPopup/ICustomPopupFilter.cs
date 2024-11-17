namespace MyExpenses.Models.Maui.CustomPopup;

public interface ICustomPopupFilter<out T>
{
    public IEnumerable<T> GetFilteredItemChecked();

    public int GetFilteredItemCheckedCount();

    public int GetFilteredItemCount();
}

