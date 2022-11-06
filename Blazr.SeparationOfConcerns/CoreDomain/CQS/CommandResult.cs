namespace Blazr.SeparationOfConcerns.Core;

public record CommandResult
{
    public bool Successful { get; init; }
    public string Message { get; init; } = string.Empty;

    public static CommandResult Success()
        => new CommandResult { Successful = true };

    public static CommandResult Failure(string message)
        => new CommandResult { Successful = false };

}
