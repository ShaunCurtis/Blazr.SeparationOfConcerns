namespace Blazr.SeparationOfConcerns.Data;

public class CounterState : StateBase<CounterData>
{

    private int _counter;
    public int Counter
    {
        get => _counter;
        set => this.SetAndNotifyIfChanged(ref _counter, value, "Counter");
    }

    public override CounterData AsRecord() => new()
    {
        Counter =this.Counter,
    };
    
    public CounterState(CounterData baseRecord)
        : base(baseRecord)
    {
        this.Counter = baseRecord.Counter;
    }
}
