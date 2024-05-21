using Data.Data.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace NewsletterProvider.Functions;

public class GetSubscriber
{
    private readonly ILogger<GetSubscriber> _logger;
    private readonly DataContext _context;

    public GetSubscriber(ILogger<GetSubscriber> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    [Function("GetSubscriber")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "getsubscriber")] HttpRequest req, string email)
    {
        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            if (!string.IsNullOrEmpty(body))
            {
                var subscriber = await _context.Subscribers.FirstOrDefaultAsync(x => x.Email == email);
                if (subscriber != null)
                {
                    return new OkObjectResult(subscriber);
                }

                return new NotFoundObjectResult(new { Status = 404, Message = "Subscriber not found" });

            }

            return new BadRequestObjectResult(new { Status = 400, Message = "Email is required. " });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the subscription request.");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }



    }
}
