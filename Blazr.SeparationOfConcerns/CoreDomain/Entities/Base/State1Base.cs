namespace Blazr.SeparationOfConcerns.Core;

public abstract class StateBase
{
    public event EventHandler<string?>? FieldChanged;

    protected void SetAndNotifyIfChanged<TType>(ref TType? currentValue, TType? value, string fieldName)
    {
        if (!currentValue?.Equals(value) ?? value is not null)
        {
            currentValue = value;
            NotifyFieldChanged(fieldName);
        }
    }

    private void NotifyFieldChanged(string fieldName)
        => FieldChanged?.Invoke(this, fieldName);
}
