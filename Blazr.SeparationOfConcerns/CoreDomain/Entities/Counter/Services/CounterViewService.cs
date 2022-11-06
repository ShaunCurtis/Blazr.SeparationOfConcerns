namespace Blazr.SeparationOfConcerns.Core;

public class CounterViewService
{
    public readonly CounterState StateContext = new CounterState(new CounterDro(0));

    private readonly IDataService _counterDataService;

    public CounterViewService(IDataService counterDataService)
        => _counterDataService = counterDataService;

    public async Task GetCounterAsync()
    {
        var result = await _counterDataService.ReadAsync<CounterDro>(new RecordQueryRequest<CounterDro>("Counter"));
        this.StateContext.Load(result.Record ?? new CounterDro(0));
    }

    public async Task SaveCounterAsync()
    {
        var request = new CommandRequest<CounterDro>(
            StorageName: "Counter", 
            Record: this.StateContext.AsRecord());

        var result = await _counterDataService.SaveAsync<CounterDro>(request);
    }

    public async Task Increment()
    {
        StateContext.Counter++;
        await SaveCounterAsync();
    }
}
