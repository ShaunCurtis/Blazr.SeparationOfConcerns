namespace Blazr.SeparationOfConcerns.Core;

public class CounterDco : StateBase
{
    private int _counter;
    public int Counter
    {
        get => _counter;
        set => SetAndNotifyIfChanged(ref _counter, value, "Counter");
    }
}
