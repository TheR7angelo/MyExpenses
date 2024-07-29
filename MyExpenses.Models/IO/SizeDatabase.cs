using MyExpenses.Models.Resources.Resx.IO;

namespace MyExpenses.Models.IO;

public class SizeDatabase
{
    public required string FileNameWithoutExtension { get; init; }
    private long _oldSize;
    private long _newSize;

    public long OldSize
    {
        get => _oldSize;
        set
        {
            _oldSize = value;
            UpdateGain();
        }
    } // in bytes

    public long NewSize
    {
        get => _newSize;
        set
        {
            _newSize = value;
            UpdateGain();
        }
    } // in bytes

    public string GainInBytes { get; private set; } = $"{0d:F2} {SizeDatabaseResources.ByteUnit}";

    public string GainInPercentage { get; private set; } = $"{0d: F2} %";

    private void UpdateGain()
    {
        UpdateGainInBytes();
        UpdateGainInPercentage();
    }

    private void UpdateGainInBytes()
    {
        var gainInBytes = _oldSize - _newSize;

        var absoluteGain = Math.Abs(gainInBytes);
        string[] units =
        [
            SizeDatabaseResources.ByteUnit, SizeDatabaseResources.KiloByteUnit, SizeDatabaseResources.MegaByteUnit,
            SizeDatabaseResources.GigaByteUnit, SizeDatabaseResources.TeraByteUnit, SizeDatabaseResources.PetaByteUnit,
            SizeDatabaseResources.ExaByteUnit, SizeDatabaseResources.ZettaByteUnit, SizeDatabaseResources.YottaByteUnit
        ];
        var unitIndex = 0;
        while (absoluteGain >= 1024 && unitIndex < units.Length - 1)
        {
            absoluteGain /= 1024;
            ++unitIndex;
        }

        var sign = gainInBytes >= 0 ? "-" : "+";

        GainInBytes = $"{sign}{absoluteGain:F2} {units[unitIndex]}";
    }

    private void UpdateGainInPercentage()
    {
        if (_newSize.Equals(0))
        {
            GainInPercentage = "N/A";
        }
        else
        {
            var gainInPercentage = ((double)_oldSize / _newSize - 1d) * 100d * -1d;
            gainInPercentage = double.Round(gainInPercentage, 2);

            GainInPercentage = $"{(gainInPercentage > 0 ? "+" : "")}{gainInPercentage} %";
        }
    }
}