using Product.API.Entities;
using ILogger = Serilog.ILogger;

namespace Product.API.Persistence;

public class ProductContextSeed
{
    public static async Task SeedProductAsync(ProductContext productContext, ILogger logger)
    {
        if (!productContext.Products.Any())
        {
            productContext.AddRange(entities: getCatalogProducts());
            await productContext.SaveChangesAsync();
            logger.Information(messageTemplate: "Seeded data for Product DB associated with context {DbContextName}",
                propertyValue: nameof(ProductContext));
        }
    }

    private static IEnumerable<CatalogProduct> getCatalogProducts()
    {
        return new List<CatalogProduct>
        {
            new()
            {
                No = "Lotus",
                Name = "Esprit",
                Summary = "Nondisplaced fracture of greater trochanter of right femur",
                Description = "Nondisplaced fracture of greater trochanter of right femur",
                Price = (decimal)177940.49
            },
            new()
            {
                No = "Cadillac",
                Name = "CTS",
                Summary = "Carbuncle of trunk",
                Description = "Carbuncle of trunk",
                Price = (decimal)114728.21
            },
            new()
            {
                No = "Apple",
                Name = "Iphone 16 Pro",
                Summary = "IP16 Pro",
                Description = "The latest version of Iphone",
                Price = (decimal)1000
            }
        };
    }
}