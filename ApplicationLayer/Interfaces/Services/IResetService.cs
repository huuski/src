namespace ApplicationLayer.Interfaces.Services;

public interface IResetService
{
    Task ResetAllDataAsync(CancellationToken cancellationToken = default);
}

