namespace MyExpenses.Models.Maui.CustomPopupFilter;

public interface ICustomPopupFilter<out T>
{
    public IEnumerable<T> GetFilteredItemChecked();

    public int GetFilteredItemCheckedCount();

    public int GetFilteredItemCount();
}

