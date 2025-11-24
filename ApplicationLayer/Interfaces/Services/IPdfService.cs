namespace ApplicationLayer.Interfaces.Services;

public interface IPdfService
{
    Task<byte[]> GenerateNegotiationPdfAsync(Guid negotiationId, CancellationToken cancellationToken = default);
}

