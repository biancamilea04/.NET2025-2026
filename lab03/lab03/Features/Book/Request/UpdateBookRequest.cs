namespace lab03.Features;

public record UpdateBookRequest(Guid Id, string? Title, string? Author, int? Year);