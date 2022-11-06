namespace Blazr.SeparationOfConcerns.Core;

public class CounterState1
{
    private CounterDro BaseRecord = default!;

    private int _counter;
    public int Counter
    {
        get => _counter;
        set => SetAndNotifyIfChanged(ref _counter, value, "Counter");
    }

    public CounterDro AsRecord() => new(
        Counter: this.Counter
        );

    public event EventHandler<string>? FieldChanged;
    public event EventHandler<bool>? StateChanged;
    
    public CounterState1(CounterDro record)
        => this.Load(record);

    public void Load(CounterDro record)
    {
        this.BaseRecord = record with { };
        Counter = record.Counter;
        this.NotifyStateMayHaveChanged(true);
    }

    public void Reset()
        => this.Load(BaseRecord);

    public void Update()
        => this.Load(AsRecord());

    private bool _wasDirty;
    public bool IsDirty
        => BaseRecord?.Equals(AsRecord())
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

    protected void NotifyStateMayHaveChanged(bool force = false)
    {
        var isDirty = this.IsDirty;
        if (_wasDirty != isDirty || force)
        {
            _wasDirty = isDirty;
            this.StateChanged?.Invoke(this, this.IsDirty);
        }
    }
}
