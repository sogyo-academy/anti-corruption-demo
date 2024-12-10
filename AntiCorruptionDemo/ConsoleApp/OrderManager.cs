using Dependency;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

namespace ConsoleApp;

public class PriceCalculationException(string message) : Exception(message);

public class OrderManager
{
    /* This class abstracts the implementation and design choices of the internal library from the client.
     * These classes are also often called 'clients', as they make use of a 'service' provided by other code.
     * Anti-corruption layers such as these tend to apply the facade design pattern. It is common to find that
     * anti-corruption layers use adapters to transform the service's representation of information to the
     * client's representation and vice versa.
     */
    private Internals.Order _order = new (new FSharpMap<Guid, Internals.ProductQuantity>([]));
    
    private static class PriceListAdapter
    {
        public static Internals.PriceList FromStocks(List<MuppetStock> stocks)
        {
            var prices = new FSharpMap<Guid, double>([]);
            prices = stocks.Aggregate(prices, (current, stock) => current.Add(stock.Id, stock.Price));

            return new Internals.PriceList(
                prices
            );
        }
    }
    
    private static class PriceAdapter {
        public static double FromResult(FSharpResult<double, string> price)
        {
            if (price.IsError)
            {
                throw new PriceCalculationException(price.ErrorValue);
            }
            return price.ResultValue;
        }
    }

    public void AcceptOrder(Dictionary<Guid, uint> order)
    {
        foreach (var line in order)
        {
            _order = _order.AddToOrder(
                new Internals.ProductQuantity(
                    line.Key, line.Value
                )
            );
        }
    }

    public double GetTotalPrice(List<MuppetStock> stocks)
    {
        var prices = PriceListAdapter.FromStocks(stocks);
        var result = _order.TotalPrice(prices);
        return PriceAdapter.FromResult(result);
    }
}