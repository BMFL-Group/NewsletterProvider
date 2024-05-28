using Data.Data.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace NewsletterProvider.Functions;

public class GetAllSubscribers
{
    private readonly ILogger<GetAllSubscribers> _logger;
    private readonly DataContext _context;

    public GetAllSubscribers(ILogger<GetAllSubscribers> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    [Function("GetAllSubscribers")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        try
        {
            var subscribers = await _context.Subscribers.ToListAsync();

            return new OkObjectResult(subscribers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the subscription request.");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
       
    }
}
