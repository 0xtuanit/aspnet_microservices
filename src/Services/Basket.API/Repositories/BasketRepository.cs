﻿using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Basket.API.Services;
using Basket.API.Services.Interfaces;
using Contracts.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Shared.DTOs.ScheduledJob;
using ILogger = Serilog.ILogger;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCacheService;
        private readonly ISerializeService _serializeService;
        private readonly ILogger _logger;
        private readonly BackgroundJobHttpService _backgroundJobHttp;
        private readonly IEmailTemplateService _emailTemplateService;

        public BasketRepository(IDistributedCache redisCacheService, ISerializeService serializeService, ILogger logger,
            BackgroundJobHttpService backgroundJobHttp, IEmailTemplateService emailTemplateService)
        {
            _redisCacheService = redisCacheService;
            _serializeService = serializeService;
            _logger = logger;
            _backgroundJobHttp = backgroundJobHttp;
            _emailTemplateService = emailTemplateService;
        }

        public async Task<Cart?> GetBasketByUsername(string username)
        {
            _logger.Information($"BEGIN: GetBasketByUserName {username}");
            // var basket = await _redisCacheService.GetStringAsync(username);
            // _logger.Information($"END: GetBasketByUserName {username}");
            //
            // return string.IsNullOrEmpty(basket) ? null : _serializeService.Deserialize<Cart>(basket);

            /** For retrieving TotalPrice information, then Log it onto ElasticSearch **/
            var basket = await _redisCacheService.GetStringAsync(username);
            if (!string.IsNullOrEmpty(basket))
            {
                var result = _serializeService.Deserialize<Cart>(basket);
                var totalPrice = result.TotalPrice;
                _logger.Information("END: GetBasketByUserName {username} - Total Price: {totalPrice}", username,
                    totalPrice);

                return result;
            }

            return null;
        }

        public async Task<Cart?> UpdateBasket(Cart cart, DistributedCacheEntryOptions? options = null)
        {
            DeleteReminderCheckoutOrder(cart.Username);

            _logger.Information($"BEGIN: UpdateBasket for {cart.Username}");

            if (options != null)
                await _redisCacheService.SetStringAsync(cart.Username, _serializeService.Serialize(cart), options);
            else
                await _redisCacheService.SetStringAsync(cart.Username, _serializeService.Serialize(cart));

            _logger.Information($"END: UpdateBasket for {cart.Username}");

            try
            {
                // throw new Exception("Update basket failed by custom reason");
                await TriggerSendEmailReminderCheckout(cart);
            }
            catch (Exception e)
            {
                _logger.Error($"UpdateBasket failed: {e.Message}");
            }

            return await GetBasketByUsername(cart.Username);
        }

        private async Task TriggerSendEmailReminderCheckout(Cart cart)
        {
            var emailTemplate = _emailTemplateService.GenerateReminderCheckoutOrderEmail(cart.Username);

            var model = new ReminderCheckoutOrderDto(cart.EmailAddress, "Reminder checkout", emailTemplate,
                DateTimeOffset.UtcNow.AddSeconds(30));

            var jobId = await _backgroundJobHttp.SendEmailReminderCheckout(model);
            if (!string.IsNullOrEmpty(jobId))
            {
                cart.JobId = jobId;
                await _redisCacheService.SetStringAsync(
                    cart.Username,
                    _serializeService.Serialize(cart));
            }
        }

        private async Task DeleteReminderCheckoutOrder(string username)
        {
            var cart = await GetBasketByUsername(username);
            if (cart == null || string.IsNullOrEmpty(cart.JobId)) return;

            var jobId = cart.JobId;
            _backgroundJobHttp.DeleteReminderCheckoutOrder(jobId);
            _logger.Information($"DeleteReminderCheckoutOrder:Deleted JobId: {jobId}");
        }

        public async Task<bool> DeleteBasketFromUsername(string username)
        {
            DeleteReminderCheckoutOrder(username);
            try
            {
                _logger.Information($"BEGIN: DeleteBasketFromUserName {username}");
                await _redisCacheService.RemoveAsync(username);
                _logger.Information($"END: DeleteBasketFromUserName {username}");

                return true;
            }
            catch (Exception e)
            {
                _logger.Error("Error DeleteBasketFromUserName: " + e.Message);
                throw;
            }
        }
    }
}