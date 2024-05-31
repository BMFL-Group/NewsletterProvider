using Data.Data.Contexts;
using Data.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace NewsletterProvider.Functions
{
    public class Subscribe(ILogger<Subscribe> logger, DataContext context)
    {
        private readonly ILogger<Subscribe> _logger = logger;
        private readonly DataContext _context = context;

        [Function("Subscribe")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            try
            {
                var body = await new StreamReader(req.Body).ReadToEndAsync();
                if (!string.IsNullOrEmpty(body))
                {
                    var subscribeEntity = JsonConvert.DeserializeObject<SubscribeEntity>(body);
                    if (subscribeEntity != null)
                    {
                        var existingSubscriber = await _context.Subscribers.FirstOrDefaultAsync(x => x.Email == subscribeEntity.Email);
                        if (existingSubscriber != null)
                        {
                            _context.Entry(existingSubscriber).CurrentValues.SetValues(subscribeEntity);
                            await _context.SaveChangesAsync();
                            return new OkObjectResult(new { Status = 200, Message = "Subscriber was updated. " });
                        }

                        try
                        {
                            _context.Subscribers.Add(subscribeEntity);
                            var result = await _context.SaveChangesAsync();
                            if (result == 1)
                            {
                                return new OkObjectResult(new { Status = 200, Message = "Subscriber is now subscribed. " });
                            }
                        }
                        catch (Exception ex){}
                    }
                }

                return new BadRequestObjectResult(new { Status = 400, Message = "Unable to subscribe at the moment. " });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the subscription request.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

        }
    }
}
