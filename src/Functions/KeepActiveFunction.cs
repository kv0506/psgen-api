using Microsoft.Azure.Functions.Worker;

namespace PsGenApi.Functions;

public class KeepActiveFunction(ILoggerFactory loggerFactory)
{
	private readonly ILogger _logger = loggerFactory.CreateLogger<KeepActiveFunction>();

	[Function("keep-active")]
	public void Run([TimerTrigger("0 */15 * * * *")] MyInfo myTimer)
	{
		_logger.LogInformation($"keep-active function executed at: {DateTime.Now}");
		_logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
	}
}

public class MyInfo
{
	public MyScheduleStatus ScheduleStatus { get; set; }

	public bool IsPastDue { get; set; }
}

public class MyScheduleStatus
{
	public DateTime Last { get; set; }

	public DateTime Next { get; set; }

	public DateTime LastUpdated { get; set; }
}