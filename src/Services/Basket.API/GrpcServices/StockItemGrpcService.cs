using Inventory.Grpc.Client;
using ILogger = Serilog.ILogger;

namespace Basket.API.GrpcServices;

public class StockItemGrpcService
{
    private readonly StockProtoService.StockProtoServiceClient _stockProtoService;
    private readonly ILogger _logger;

    public StockItemGrpcService(StockProtoService.StockProtoServiceClient stockProtoService, ILogger logger)
    {
        _stockProtoService = stockProtoService ?? throw new ArgumentNullException(nameof(stockProtoService));
        _logger = logger;
    }

    public async Task<StockModel> GetStock(string? itemNo)
    {
        try
        {
            _logger.Information($"BEGIN: Get Stock StockItemGrpcService Item No: {itemNo}");
            var stockItemRequest = new GetStockRequest { ItemNo = itemNo };

            var result = await _stockProtoService.GetStockAsync(stockItemRequest);
            _logger.Information(
                $"BEGIN: Get Stock StockItemGrpcService Item No: {itemNo} - Stock value: {result.Quantity}");

            return result;
        }
        catch (Exception e)
        {
            _logger.Error($"StockItemGrpcService failed: {e.Message}");
            throw;
        }
    }
}