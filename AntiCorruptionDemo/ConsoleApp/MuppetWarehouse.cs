namespace ConsoleApp;

public class MuppetWarehouse(List<MuppetStock> stock)
{
    public List<MuppetStock> Stock { get; } = stock;

    public static MuppetWarehouse FromScratch()
    {
        return new MuppetWarehouse(
            MuppetStock.FromScratch()
        );
    }

    private MuppetStock? OfName(string name)
    {
        return Stock.FindLast(muppet => muppet.Name == name);
    }

    public Dictionary<Guid, uint> MakeReservation(Dictionary<string, int> reservations)
    {
        CanSatisfyOrder(reservations);
        return reservations
            .Select(kvp => kvp)
            .ToDictionary(
                kvp => OfName(kvp.Key)!.Id,
                kvp => (uint)kvp.Value
            );
    }

    private void CanSatisfyOrder(Dictionary<string, int> productReservations)
    {
        var reservableProducts = (from product in Stock select product.Name).ToList();
        foreach (var (product, quantity) in productReservations)
        {
            ValidateThatProductExists(reservableProducts, product);
            ValidateThatStocksAreSufficient(product, quantity);
        }
    }

    private void ValidateThatStocksAreSufficient(string product, int quantity)
    {
        var reservableQuantity = AvailabilityInStock(product);
        if (reservableQuantity < quantity)
        {
            throw new InsufficientStockException($"I only have {reservableQuantity} of {product} available.");
        }
    }

    private static void ValidateThatProductExists(List<string> reservableProducts, string product)
    {
        if (!reservableProducts.Contains(product))
        {
            throw new InsufficientStockException($"I do not have any {product} in stock.");
        }
    }

    private int AvailabilityInStock(string product)
    {
        return Stock
            .Where(reservableProduct => reservableProduct.Name == product)
            .Select(reservableProduct => (int)reservableProduct.Availability)
            .Sum();
    }
}