namespace Blazr.SeparationOfConcerns.Core;

public class CounterService
{
    public CounterData Data { get; private set; } = new();

    public void Increment()
        => Data.Counter++;
}
