namespace AbySalto.Junior.Application.DTOs.Common;

public abstract class BaseResponseDto
{
    public Guid Id { get; init; }

    public string CreatedBy { get; init; } = string.Empty;

    public DateTimeOffset CreatedOn { get; init; }

    public string? UpdatedBy { get; init; }

    public DateTimeOffset? UpdatedOn { get; init; }
}
