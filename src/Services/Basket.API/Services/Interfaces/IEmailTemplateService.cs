namespace Basket.API.Services.Interfaces;

public interface IEmailTemplateService
{
    // All methods to generate email templates:
    string GenerateReminderCheckoutOrderEmail(string username, string checkoutUrl = "basket/checkout");
}