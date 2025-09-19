
# Parameterize.Net

[![Nuget](https://img.shields.io/nuget/v/Parameterize.Net)](https://www.nuget.org/packages/Parameterize.Net/)
[![.NET Tests](https://github.com/PasoUnleashed/Parameterize.Net/actions/workflows/dotnet-tests.yml/badge.svg)](https://github.com/PasoUnleashed/Parameterize.Net/actions/workflows/dotnet-tests.yml)

A small library to represent and generate arbitrarily complex objects using a flat `float[]`-useful for procedural generation, genetic algorithms, parameter optimization, and compact serialization of parameter spaces.

> Parameterize.Net lets you define the *shape* of an object declaratively (attributes or fluent builder), derive a `Resolver` that maps between a `float[]` (a “gene”) and concrete instances, and query ranges and mutation-friendly values for use in genetic/optimization workflows. 

---

## Status & Compatibility

* Current package: **Parameterize.Net 2.0.0**. Targets **.NET Standard 2.1** (compatible with .NET 5/6/7/8/9/10 and a variety of platforms).
* Language: **C#**. Repository contains source and examples.
* License: **MIT**.

---

## Key concepts (high level)

* **Gene / float\[]** - a compact array of `float` that represents all degrees of freedom required to build an instance.
* **Range** - for each parameter, the library exposes ranges describing how many floats it consumes and which value ranges they map to.
* **Resolver** - the runtime component that knows how to *resolve* a `float[]` (or `Span<float>`) into an instance of your model and the inverse operation (get a gene representation from an instance).
* **Declarative model (attributes)** - annotate your classes with `[Parameterize]`, `[Range(min,max)]`, `[Length(min,max)]`, `[Subtype(typeof(...))]`, `[Resolver(typeof(...))]` and the library will derive a `Resolver` for your type.
* **Programmatic model (ResolverBuilder)** - build resolvers explicitly in code for maximum control (good for generated code, editor tooling, or when you prefer fluent definitions).

(These concepts and examples are implemented and demonstrated in the repository README and examples.)

---

## Install

### NuGet (recommended)

```bash
dotnet add package Parameterize.Net --version 2.0.0
```

Package targets `.NET Standard 2.1` - see NuGet for details about supported platform compatibility. 

### Unity

Use Unity Package Manager → **Add package from git URL** and paste:

```
https://github.com/PasoUnleashed/Parameterize.Net.git?path=/Parameterize.Net/Parameterize.Net/Source#main
```

(This points the Unity package manager to the package path inside the repo.) 

---

## Quickstart - declarative usage

Imagine a `Zoo` that contains `Animal[]` where `Dog` and `Cat` are subtypes and each has parameters.

```csharp
public class Zoo
{
    [Parameterize]
    [Length(1, 10)]
    [Subtype(typeof(Dog))]
    [Subtype(typeof(Cat))]
    public Animal[] Animals;

    public override string ToString()
        => $"Zoo with {Animals.Length} animals:\n\t{string.Join("\n\t", Animals.Select(a => a.ToString()))}";
}

public abstract class Animal
{
    [Resolver(typeof(AnimalNameResolver))]
    public string Name;

    public override string ToString() => $"{GetType().Name} called {Name}";
}

public class Dog : Animal
{
    [Parameterize]
    [Range(1, 9)]
    public int Spots;
    public override string ToString() => base.ToString() + $" (Spots {Spots})";
}

public class Cat : Animal
{
    [Parameterize]
    [Range(1, 9)]
    public int Lives;
    public override string ToString() => base.ToString() + $" (Lives {Lives})";
}
```

Derive and use a resolver:

```csharp
Resolver resolver = ResolverDeriver.Derive<Zoo>();
var geneRange = resolver.GetRange();
var gene = geneRange.GetRandom();          // gene is a Span<float> or float[] describing the instance
var zoo = resolver.Resolve<Zoo>(gene);     // create an instance from gene
Console.WriteLine(zoo);
```

This yields a `Zoo` instance with randomized animals according to the constraints.

---

## Resolvers & custom resolvers

A resolver converts between `Span<float>` (or `float[]`) and concrete values. You can supply custom resolvers for complex values (strings, enums, structured types).

Example name resolver (selects name via a float index):

```csharp
public class AnimalNameResolver : Resolver
{
    private readonly List<string> names = new List<string>()
    {
        "Max","Buddy","Charlie","Rocky","Jack","Duke","Toby","Jake","Lucky","Cooper",
        "Milo","Oscar","Simba","Luna","Bella","Lucy","Daisy","Coco","Molly","Nala"
        // ...
    };

    public override int Length => 1;  // consumes one float

    public override object Resolve(Type type, Span<float> value)
    {
        int index = (int)(names.Count * value[0]) % names.Count;
        return names[index];
    }

    public override void GetRange(Span<Range> rangeSpan)
    {
        // indicate the resolver's internal float footprint is [0,1)
        rangeSpan[0] = new Range(0, 1);
    }

    public override void Get(Span<float> range, object representative)
    {
        range[0] = names.IndexOf(representative.ToString()) / (float)names.Count;
    }
}
```


## Fluent API (ResolverBuilder)

Parameterize.Net provides a **fluent, programmatic API** for building resolvers without using attributes. This is ideal for:

* Dynamically generated models
* Editor tooling
* Cases where you don’t want to decorate classes with attributes

### Building a resolver for a model

```csharp
// Custom resolver for names
var nameResolver = new AnimalNameResolver();

// Resolver for Dog, attach custom resolver to Name property
var dogResolver = ResolverBuilder.Model<Dog>()
    .Property(d => d.Spots)
        .WithRange(1, 9)
    .SetResolver(d => d.Name, nameResolver)  // attaches a custom resolver to the Name property
    .Get();

// Resolver for Cat, attach custom resolver to Name property
var catResolver = ResolverBuilder.Model<Cat>()
    .Property(c => c.Lives)
        .WithRange(1, 9)
    .SetResolver(c => c.Name, nameResolver)  // attaches a custom resolver to the Name property
    .Get();

// Resolver for arrays of polymorphic animals
var animalsResolver = ResolverBuilder.Many<Animal>()
    .ForType<Dog>(dogResolver)
    .ForType<Cat>(catResolver)
    .Get();

// Resolver for Zoo
var zooResolver = ResolverBuilder.Model<Zoo>()
    .Property(z => z.Animals, animalsResolver, min: 1, max: 10)
    .Get();
```

### Generating objects

```csharp
var gene = zooResolver.GetRange().GetRandom();
Zoo randomZoo = zooResolver.Resolve<Zoo>(gene);
Console.WriteLine(randomZoo);
```

### Key points

* `.Model<T>()` - define a resolver for a single type
* `.Property(...)` - define a property and optionally attach ranges
* `.SetResolver(property, resolver)` - attach a **custom resolver** to a property

  * First argument: the property to resolve (`Expression<Func<T, TProperty>>`)
  * Second argument: a **resolver instance or type** that maps floats to the property value
  * Ensures the resolver uses your custom logic for that property
* `.Many<T>()` - define subtype resolvers
* `.ForType<T>()` - associate a specific subtype resolver
* `.Get()` - finalize the builder and produce a `Resolver` instance

The fluent API gives **full control** over the resolver without modifying the original class. It’s perfect for runtime-generated schemas, procedural content pipelines, or scenarios where attributes are impractical.

It's also easy to combine the declaritive approach with the fluent api

---

If you want, I can **also create a side-by-side comparison of attribute-based vs fluent API** so readers can easily see when to use `SetResolver` vs `[Resolver]`. This usually helps users new to the library. Do you want me to do that?

---

## API reference (overview)

> Note: this section is a concise mapping of the main concepts. For full signatures, browse the source in `Source/` in the repository.

* `Resolver` (base class)

  * `int Length { get; }` - number of floats the resolver consumes
  * `object Resolve(Type, Span<float>)` - create instance from floats
  * `void GetRange(Span<Range>)` - get min/max mapping for floats used
  * `void Get(Span<float>, object representative)` - map a representative value back to floats

* `ResolverDeriver`

  * `Derive<T>()` - derive a resolver from attributes on `T`

* `ResolverBuilder` - fluent API to construct resolvers programmatically

* Attributes

  * `[Parameterize]` - marks a property/field to be included in the parameterization
  * `[Range(min,max)]` - numeric range mapping for the property
  * `[Length(min,max)]` - for arrays/lists, sets min/max length
  * `[Subtype(typeof(...))]` - polymorphic choices for arrays/lists of base types
  * `[Resolver(typeof(YourResolver))]` - attach a custom resolver to a field/property

(Examples and attribute usage are shown in the repository README.) 

---

## Example: how genes map to objects

* When you call `resolver.GetRange()` you receive a `Range[]` describing the full gene layout (how many floats and what's their value range).
* `GetRandom()` typically samples a random gene `Span<float>` inside the allowed ranges.
* `Resolve<T>(gene)` transforms the floats into a fully constructed `T`.
* `Get(Span<float>, representative)` writes floating values representing an existing object - useful for evaluating a known individual or seeding populations.

This design aims for a deterministic mapping: the same gene should resolve to the same instance (modulo any nondeterministic resolvers you implement).

---

## Tips & best practices

* **Keep resolvers deterministic** - avoid random seeds or time-based decisions inside a resolver's `Resolve` method unless you explicitly want nondeterminism.
* **Balance gene size** - small gene vectors are faster to evaluate in GA loops; pack only parameters you actually need to mutate.
* **Use custom resolvers for complex discrete/categorical values** (strings, enumerations, compound structs) so you can fully control mapping and rounding.
* **When evolving structures with variable length arrays**, prefer a combination of `Length(min,max)` and a separate `Resolver` to map lengths smoothly.

---

## Performance & threading

* The API uses `Span<float>` for efficient zero-copy slicing where possible; follow the `Span`-safe patterns when implementing custom resolvers.
* Thread-safety depends on your resolvers: the core library is designed to be used in performance loops, but if your `Resolver` stores mutable internal state, synchronize or avoid sharing it across threads.
* If you plan to run large evolutionary simulations, pre-derive resolvers once (e.g., at startup) and reuse them to minimize reflection overhead.

---

## Dependencies

* The NuGet package lists `Newtonsoft.Json` (>= 13.0.3) in its package dependencies. Check the package metadata for exact versions. 

---

## Examples & snippets

* The repository README includes a compact demo (the `Zoo` example) which demonstrates both attribute-based and fluent-builder approaches. Use the examples as a starting point to integrate Parameterize.Net into your project.

---

## Contributing

* Fork the repository, create a feature branch, and open a PR. Keep changes small and focused. Add unit tests for new behavior.
* Ensure any code that affects mapping is backwards-compatible or clearly documented and versioned.
* For bug reports or feature requests, open an issue in the repository.

---

## FAQ

**Q: Is Parameterize.Net a serializer?**
A: Not exactly. It's a parameterization layer that maps between compact float gene vectors and instances. While you can use it to serialize parameter space, its main use is generation and optimization workflows (procedural content, GA-like systems). 

**Q: Can I persist genes to disk?**
A: Yes - genes are just arrays of floats. Persist the array (binary or text) and re-load to `Resolve<T>` the same object.

**Q: Does it work in Unity?**
A: Yes - the repo includes Unity-compatible packaging instructions (use Unity Package Manager with the git URL path).

---

## License

MIT. See `LICENSE` in the repository.

---

## Contact & resources

* Source repository: PasoUnleashed / Parameterize.Net (see repo for code & examples).
* NuGet package & versioning: Parameterize.Net on NuGet (current version & supported TFMs).
