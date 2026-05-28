namespace ClientApiService.Services;

public interface ILeadGeneratorService
{
    Task<GeneratedLeadDto> GenerateRandomLeadAsync();
}

public class GeneratedLeadDto
{
    public string ClientName { get; set; } = string.Empty;
    public string BadAddress { get; set; } = string.Empty;
}