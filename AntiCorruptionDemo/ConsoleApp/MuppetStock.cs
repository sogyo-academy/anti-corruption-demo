namespace ConsoleApp;

public class InsufficientStockException(string message) : Exception(message);

public class MuppetStock
{
    public string Name { get; }
    private string Description { get; }
    public uint Availability { get; }
    public double Price { get; }
    public Guid Id { get; }
    public string PrintableEntry => $"\t{Name}! {Description} Yours for {Price} euros!";

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
            new MuppetStock(name: "Elmo", description: "Red, curious and utterly lovable.", availability: 12, price: 12.50),
            new MuppetStock(name: "Grover", description: "Blue, clumsy and eager to be your friend.", availability: 9, price: 11.20),
            new MuppetStock(name: "Oscar", description: "Green, grouchy. A softy inside!", availability: 4, price: 19.75),
            new MuppetStock(name: "Big Bird", description: "Tall but a little shy. ", availability: 3, price: 22.50)
        ];
    }
}