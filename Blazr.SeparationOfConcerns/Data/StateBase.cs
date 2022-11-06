namespace Blazr.SeparationOfConcerns.Data;

public abstract class StateBase<TRecord>
{
    protected TRecord _baseRecord;

    public event EventHandler<string?>? StateChanged;

    public abstract TRecord AsRecord();
    
    public StateBase(TRecord baseRecord)
    {
        _baseRecord = baseRecord;
    }

    protected void SetAndNotifyIfChanged<TType>(ref TType? currentValue, TType? value, string fieldName)
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
