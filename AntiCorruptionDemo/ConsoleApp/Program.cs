namespace ConsoleApp;

public static class Program
{
    private static List<MuppetStock> _muppetStock = MuppetStock.FromScratch();
    
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello! I am the muppet vendor application.");
        ShowOfferings();
    }

    private static void ShowOfferings()
    {
        Console.WriteLine("I have the following on offer:");
        foreach (MuppetStock muppet in _muppetStock)
        {
            Console.WriteLine($"\t{muppet.Name}! {muppet.Description} Yours for \u20ac{muppet.Price}!");
        }
    }
}

public class MuppetStock
{
    public string Name { get; }
    public string Description { get; }
    public uint Availability { get; }
    public double Price { get; }
    public Guid Id { get; }

    private MuppetStock(string name, string description, uint availability, double price)
    {
        Id = Guid.NewGuid();
        Availability = availability;
        Price = price;
        Name = name;
        Description = description;
    }

    public static List<MuppetStock> FromScratch()
    {
        return
        [
            new MuppetStock(name: "Elmo", description: "Red, curious.", availability: 12, price: 12.50),
            new MuppetStock(name: "Grover", description: "Blue, clumsy.", availability: 9, price: 11.20),
            new MuppetStock(name: "Oscar", description: "Green, grouchy.", availability: 4, price: 19.75),
            new MuppetStock(name: "Big Bird", description: "Tall, polite.", availability: 3, price: 22.50)
        ];
    }
}