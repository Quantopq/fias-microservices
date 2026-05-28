using ClientApiService.Data;
using ClientApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace ClientApiService.Services;

public class LeadRandomizerBackgroundService : BackgroundService
{
	private readonly IServiceProvider _serviceProvider;
	private readonly ILeadGeneratorService _generator;
	private readonly ILogger<LeadRandomizerBackgroundService> _logger;
	private readonly Random _random = new();

	public LeadRandomizerBackgroundService(
		IServiceProvider serviceProvider,
		ILeadGeneratorService generator,
		ILogger<LeadRandomizerBackgroundService> logger)
	{
		_serviceProvider = serviceProvider;
		_generator = generator;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("Lead Randomizer Service started");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await GenerateAndProcessLeadAsync(stoppingToken);

				// Случайный интервал: 8-12 минут
				var delayMinutes = _random.Next(8, 13);
				_logger.LogInformation("Next lead generation in {Minutes} minutes", delayMinutes);

				await Task.Delay(TimeSpan.FromMinutes(delayMinutes), stoppingToken);
			}
			catch (OperationCanceledException)
			{
				_logger.LogInformation("Lead Randomizer Service stopping");
				break;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error in Lead Randomizer Service");
				await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
			}
		}
	}

	private async Task GenerateAndProcessLeadAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Generating new random lead...");

		var leadData = await _generator.GenerateRandomLeadAsync();
		_logger.LogInformation("Generated: {Name}, {Address}", leadData.ClientName, leadData.BadAddress);

		using var scope = _serviceProvider.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		var lead = new Lead
		{
			ClientId = $"AUTO_{Guid.NewGuid():N}",
			ClientName = leadData.ClientName,
			RawAddress = leadData.BadAddress,
			Status = "Pending",
			CreatedAt = DateTime.UtcNow
		};

		dbContext.Leads.Add(lead);
		await dbContext.SaveChangesAsync(cancellationToken);

		_logger.LogInformation("Lead {LeadId} created", lead.Id);
	}
}