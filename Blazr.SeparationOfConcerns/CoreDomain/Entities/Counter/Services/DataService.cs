namespace Blazr.SeparationOfConcerns.Core;

public class DataService : IDataService
{
    private readonly ProtectedLocalStorage _storage;

    public DataService(ProtectedLocalStorage storage)
        => _storage = storage;

    public async ValueTask<CommandResult> SaveAsync<TRecord>(CommandRequest<TRecord> request)
    {
        if (request.Record is not null)
            await _storage.SetAsync(request.StorageName, request.Record);

        // No return so we return success!
        return CommandResult.Success();
    }

    public async ValueTask<RecordQueryResult<TRecord>> ReadAsync<TRecord>(RecordQueryRequest<TRecord> request)
    {
        try
        {
            var result = await _storage.GetAsync<TRecord>(request.StorageName);
            return new RecordQueryResult<TRecord> { Successful = result.Success, Record = result.Value, Message = $"Failed to retrieve a value for {request.StorageName}" };
        }
        catch
        {
            return new RecordQueryResult<TRecord> { Successful = false, Message = $"Failed to retrieve a value for {request.StorageName}" };
        }
    }
}
