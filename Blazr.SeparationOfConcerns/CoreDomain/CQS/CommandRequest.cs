namespace Blazr.SeparationOfConcerns.Core;

public record CommandRequest<TRecord>(string StorageName, TRecord Record);
