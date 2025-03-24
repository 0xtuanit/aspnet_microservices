using Hangfire.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.ScheduledJob;

namespace Hangfire.API.Controllers;

[ApiController]
[Route("api/scheduled-jobs")]
public class ScheduledJobsController : ControllerBase
{
    private readonly IBackgroundJobService _bgJobService;

    public ScheduledJobsController(IBackgroundJobService bgJobService)
    {
        _bgJobService = bgJobService;
    }

    [HttpPost]
    [Route("send-email-reminder-checkout-order")]
    public IActionResult SendReminderCheckoutOrderEmail([FromBody] ReminderCheckoutOrderDto model)
    {
        var jobId = _bgJobService.SendEmailContent(model.email, model.subject, model.emailContent, model.enqueueAt);
        return Ok(jobId);
    }
}