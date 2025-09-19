// See https://aka.ms/new-console-template for more information

using Parameterize.Net;
using Parameterize.Net.Builder;
using Parameterize.Net.Reflection;
using Range = Parameterize.Net.Range;

internal class Program
{
    public static void Main(string[] args)
    {
        Resolver resolver = ResolverDeriver.Derive<Zoo>();
        var gene = resolver.GetRange().GetRandom();
        var zoo = resolver.Resolve<Zoo>(gene);
        Console.WriteLine(zoo.ToString());


        var animalNameResolver = new AnimalNameResolver();
        var resolver2 = ResolverBuilder.Model<Zoo>().Property(i => i.Animals,
                ResolverBuilder.Many<Animal>()
                    .ForType<Cat>(ResolverBuilder.Model<Cat>().Property(i => i.Lives).WithRange(1, 9).SetResolver(i=>i.Name, animalNameResolver).Get())
                    .ForType<Dog>(ResolverBuilder.Model<Dog>().Property(i => i.Spots).WithRange(1, 9).SetResolver(i=>i.Name, animalNameResolver).Get())
                    .Get(), 1,
                10)
            .Get();
        gene = resolver2.GetRange().GetRandom();
        zoo = resolver2.Resolve<Zoo>(gene);
        Console.WriteLine(zoo.ToString());
    }
}

public class Zoo
{
    [Parameterize] [Length(1, 10)] [Subtype(typeof(Dog))] [Subtype(typeof(Cat))]
    public Animal[] Animals;

    public override string ToString()
    {
        return $"Zoo with {Animals.Length} animals:\n\t {string.Join("\n\t", Animals.ToList())}";
    }
}

public abstract class Animal
{
    [Resolver(typeof(AnimalNameResolver))] public string Name;

    public override string ToString()
    {
        return $"{this.GetType().Name} Called {this.Name}";
    }
}

public class Dog : Animal
{
    [Parameterize] [Range(1, 9)] public int Spots;

    public override string ToString()
    {
        return base.ToString() + $" (Spots {Spots})";
    }
}

public class Cat : Animal
{
    [Parameterize] [Range(1, 9)] public int Lives;

    public override string ToString()
    {
        return base.ToString() + $" (Lives {Lives})";
    }
}

public class AnimalNameResolver : Resolver
{
    private List<string> names = new List<string>()
    {
        "Max", "Buddy", "Charlie", "Rocky", "Jack", "Duke", "Toby", "Jake", "Lucky", "Cooper",
        "Bear", "Teddy", "Rex", "Zeus", "Oscar", "Milo", "Simba", "Tiger", "Sammy", "Oliver",
        "Leo", "Louis", "Finn", "Loki", "Murphy", "Marley", "Bentley", "Bruno", "Diesel", "Rusty",
        "Bailey", "Maggie", "Bella", "Lucy", "Luna", "Daisy", "Chloe", "Lily", "Sophie", "Sadie",
        "Coco", "Molly", "Stella", "Nala", "Gracie", "Zoe", "Mia", "Rosie", "Ellie", "Ruby",
        "Kitty", "Cleo", "Princess", "Angel", "Mittens", "Oreo", "Smokey", "Shadow", "Boots", "Whiskers",
        "Peanut", "Pumpkin", "Pepper", "Cookie", "Mocha", "Brownie", "Snickers", "Marshmallow", "Cupcake", "Sugar",
        "Snowball", "Fluffy", "Fuzzy", "Patches", "Spot", "Dotty", "Stripe", "Midnight", "Stormy", "Ash",
        "Sunny", "Lucky", "Honey", "Biscuit", "Muffin", "Tinkerbell", "Pixie", "Ginger", "Socks", "Bandit"
    };

    public override int Length => 1;

    public override object Resolve(Type type, Span<float> value)
    {
        return names[(int)(names.Count * value[0]) % names.Count];
    }

    public override void GetRange(Span<Range> rangeSpan)
    {
        rangeSpan[0] = new Range(0, 1);
    }

    public override void Get(Span<float> range, object representative)
    {
        range[0] = names.IndexOf(representative.ToString()) / (float)names.Count;
    }
}