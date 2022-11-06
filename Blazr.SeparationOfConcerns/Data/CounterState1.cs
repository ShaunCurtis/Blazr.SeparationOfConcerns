namespace Blazr.SeparationOfConcerns.Data;

public class CounterState1
{
    private CounterData _baseRecord;

    private int _counter;
    public int Counter
    {
        get => _counter;
        set => this.SetIfChanged(ref _counter, value, "Counter");
    }

    public event EventHandler<string?>? StateChanged;

    public CounterData AsRecord => new()
    {
        Counter =this.Counter,
    };
    
    public CounterState1(CounterData baseRecord)
    {
        _baseRecord = baseRecord;
        this.Counter = baseRecord.Counter;
    }

    protected void SetIfChanged<TType>(ref TType? currentValue, TType? value, string fieldName)
    {

        if (HasChanged(currentValue, value))
        {
            currentValue = value;
            NotifyFieldChanged(fieldName);
        }
    }

    private void NotifyFieldChanged(string fieldName)
        => this.StateChanged?.Invoke(this, fieldName);

    public static bool HasChanged<TType>(TType oldValue, TType newValue)
    {
        var oldIsNotNull = oldValue != null;
        var newIsNotNull = newValue != null;

        // Only one is null so different
        if (oldIsNotNull != newIsNotNull)
            return true;

        var oldValueType = oldValue!.GetType();
        var newValueType = newValue!.GetType();

        if (oldValueType != newValueType)
            return true;

        return !oldValue.Equals(newValue);
    }
}
