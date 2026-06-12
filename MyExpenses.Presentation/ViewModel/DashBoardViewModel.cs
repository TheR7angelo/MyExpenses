using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MyExpenses.Presentation.ViewModel;

public partial class DashBoardViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial int IndexOfPositiveNegativeChartValues { get; set; } = 1;

    private static (bool Positive, bool Negative)[] PositiveNegativeChartValues =>
    [
        (false, true),
        (true, true),
        (true, false)
    ];

    [RelayCommand]
    private void OnManagePieChart(int valueToAdd)
    {
        IndexOfPositiveNegativeChartValues =
            (IndexOfPositiveNegativeChartValues + valueToAdd + PositiveNegativeChartValues.Length)
            % PositiveNegativeChartValues.Length;

        // var (yearInt, monthInt) = ExtractMonthAndYearFromSelection();
        // UpdatePieChartData(null, monthInt, yearInt);
    }
}