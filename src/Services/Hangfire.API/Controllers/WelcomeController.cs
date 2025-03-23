using Contracts.ScheduledJobs;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace Hangfire.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WelcomeController : ControllerBase
{
    private readonly IScheduledJobService _jobService;
    private readonly ILogger _logger;

    public WelcomeController(ILogger logger, IScheduledJobService jobService)
    {
        _jobService = jobService;
        _logger = logger;
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult Welcome()
    {
        var jobId = _jobService.Enqueue(() => ResponseWelcome("Welcome to Hangfire API"));

        return Ok($"Job Id: {jobId} - Enqueue Job");
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult DelayedWelcome()
    {
        const int seconds = 5;
        var jobId = _jobService.Schedule(() => ResponseWelcome("Welcome to Hangfire API"),
            TimeSpan.FromSeconds(seconds));

        return Ok($"Job Id: {jobId} - Delayed Job");
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult WelcomeAt()
    {
        var enqueueAt = DateTimeOffset.UtcNow.AddMinutes(1).AddSeconds(10);
        var jobId = _jobService.Schedule(() => ResponseWelcome("Welcome to Hangfire API"),
            enqueueAt);

        return Ok($"Job Id: {jobId} - Schedule Job");
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult ConfirmedWelcome()
    {
        const int timeInSeconds = 5;
        var parentJobId =
            _jobService.Schedule(() => ResponseWelcome("Welcome to Hangfire API"),
                TimeSpan.FromSeconds(timeInSeconds));

        var jobId = _jobService.ContinueQueueWith(parentJobId,
            () => ResponseWelcome("Welcome message is sent"));

        return Ok($"Job Id: {jobId} - Confirmed welcome will be sent in {timeInSeconds} seconds");
    }

    [NonAction]
    public void ResponseWelcome(string text)
    {
        _logger.Information(text);
    }
}