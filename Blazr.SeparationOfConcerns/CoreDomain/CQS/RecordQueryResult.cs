namespace Blazr.SeparationOfConcerns.Core;

public record RecordQueryResult<TRecord>
{
    public TRecord? Record { get; init; }
    public bool Successful { get; init; }
    public string Message { get; init; } = string.Empty;

    public static RecordQueryResult<TRecord> Success(TRecord record)
        => new RecordQueryResult<TRecord> { Record = record, Successful = true };

    public static RecordQueryResult<TRecord> Failure(string message)
        => new RecordQueryResult<TRecord> { Successful = false };
}
