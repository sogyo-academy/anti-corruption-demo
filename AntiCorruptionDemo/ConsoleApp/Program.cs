namespace ConsoleApp;

public static class Program
{
    private static readonly MuppetWarehouse Warehouse = MuppetWarehouse.FromScratch();
    private static readonly OrderManager OrderManager = new();

    public static void Main(string[] args)
    {
        Console.WriteLine("Hello! I am the muppet vendor application.");
        ShowOfferings();
        TakeOrder();
    }

    private static void TakeOrder()
    {
        if (!StartOrder()) return;

        try
        {
            var reservation = BuildReservation();
            var order = Warehouse.MakeReservation(reservation);
            OrderManager.AcceptOrder(order);
            ConfirmOrder(reservation);
        }
        catch (InsufficientStockException e)
        {
            Console.WriteLine($"The stock was insufficient. {e.Message}");
        }
        catch (PriceCalculationException e)
        {
            Console.WriteLine($"Price calculation failed. {e.Message}");
        }
    }

    private static bool StartOrder()
    {
        Console.WriteLine("Would you like to place an order? Y/N");
        if (Console.ReadLine() == "Y") return true;
        Console.WriteLine("Okay. Feel free to come back if you want any muppets!");
        return false;
    }

    private static void ShowOfferings()
    {
        Console.WriteLine("I have the following on offer:");
        foreach (var muppet in Warehouse.Stock)
        {
            Console.WriteLine(muppet.PrintableEntry);
        }
    }

    private static void ConfirmOrder(Dictionary<string, int> reservation)
    {
        Console.WriteLine("Order accepted! Your ordered muppets:");
        foreach (var entry in reservation)
        {
            Console.WriteLine($"\t{entry.Value} {entry.Key}s");
        }
        Console.WriteLine($"The total price is {OrderManager.GetTotalPrice(Warehouse.Stock)} euros.");
        Console.WriteLine("Goodbye!");
    }

    private static Dictionary<string, int> BuildReservation()
    {
        var order = new Dictionary<string, int>();
        foreach (var muppet in Warehouse.Stock)
        {
            TakeMuppetOrder(muppet, order);
        }
        return order;
    }

    private static void TakeMuppetOrder(MuppetStock muppet, Dictionary<string, int> order)
    {
        Console.WriteLine($"Would you like to order a {muppet.Name}? Y/N");
        if (Console.ReadLine() != "Y") return;
        Console.WriteLine("Okay. How many would you like?");
        HandleQuantityInput(muppet, order);
    }

    private static void HandleQuantityInput(MuppetStock muppet, Dictionary<string, int> order)
    {
        if (!int.TryParse(Console.ReadLine(), out var quantity))
        {
            Console.WriteLine($"That wasn't a number. I'll assume you don't want any {muppet.Name}.");
            return;
        }
        if (quantity <= 0)
        {
            Console.WriteLine($"That wasn't a positive number. I'll assume you don't want any {muppet.Name}.");
            return;
        }
        order[muppet.Name] = quantity;
    }
}