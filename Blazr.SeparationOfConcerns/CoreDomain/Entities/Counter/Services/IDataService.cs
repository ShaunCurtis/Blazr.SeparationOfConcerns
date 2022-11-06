namespace Blazr.SeparationOfConcerns.Core;

public interface IDataService
{
    public ValueTask<CommandResult> SaveAsync<TRecord>(CommandRequest<TRecord> request);

    public ValueTask<RecordQueryResult<TRecord>> ReadAsync<TRecord>(RecordQueryRequest<TRecord> request);
}
