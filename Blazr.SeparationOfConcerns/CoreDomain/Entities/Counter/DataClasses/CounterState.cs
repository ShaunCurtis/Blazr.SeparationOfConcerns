namespace Blazr.SeparationOfConcerns.Core;

public class CounterState : StateBase<CounterDro>
{
    private int _counter;
    public int Counter
    {
        get => _counter;
        set => SetAndNotifyIfChanged(ref _counter, value, "Counter");
    }

    public CounterState(CounterDro record)
        : base(record) { }

    public override void Load(CounterDro record)
    {
        this.BaseRecord = record with { };
        Counter = record.Counter;
        this.NotifyStateMayHaveChanged();
    }

    public override CounterDro AsRecord() 
        => new(Counter: this.Counter);

    public override void Reset()
        => this.Load(BaseRecord);

    public override void Update()
        => this.Load(AsRecord());
}
