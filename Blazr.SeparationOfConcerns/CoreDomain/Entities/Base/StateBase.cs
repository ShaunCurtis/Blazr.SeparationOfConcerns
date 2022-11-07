namespace Blazr.SeparationOfConcerns.Core;

public abstract class StateBase<TRecord>
{
    protected TRecord BaseRecord = default!;

    public event EventHandler<string>? FieldChanged;
    public event EventHandler<bool>? StateChanged;

    public StateBase(TRecord record)
        => this.Load(record);

    public abstract TRecord AsRecord();

    public abstract void Reset();

    public abstract void Update();

    public abstract void Load(TRecord record);

    public bool IsDirty 
        => !BaseRecord?.Equals(AsRecord()) 
            ?? this.AsRecord() is not null;

    protected void SetAndNotifyIfChanged<TType>(ref TType? currentValue, TType? value, string fieldName)
    {
        if (!currentValue?.Equals(value) ?? value is not null)
        {
            currentValue = value;
            this.NotifyFieldChanged(fieldName);
            this.NotifyStateMayHaveChanged();
        }
    }

    protected void NotifyFieldChanged(string fieldName)
        => FieldChanged?.Invoke(this, fieldName);

    protected void NotifyStateMayHaveChanged()
        =>  this.StateChanged?.Invoke(this, this.IsDirty);
}
