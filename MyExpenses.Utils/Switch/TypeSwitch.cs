namespace MyExpenses.Utils.Switch;

public class TypeSwitch
{
    private readonly Dictionary<Type, Func<object, object>> _matches = new();

    public TypeSwitch Case<T>(Func<T, object> action)
    {
        _matches.Add(typeof(T), x => action((T)x));
        return this;
    }

    public object Switch(object x)
    {
        return _matches[x.GetType()](x);
    }
}