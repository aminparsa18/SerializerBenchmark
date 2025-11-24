using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Bogus;
using MemoryPack;
using System.Text.Json;
using ToonSharp;

namespace ToonVsJson;

[SimpleJob(RuntimeMoniker.Net10_0)]
[RPlotExporter]
[MemoryDiagnoser]
public class Benchmarks
{
    // Size of the dataset for each benchmark run
    [Params(100, 1_000, 10_000)]
    public int Count { get; set; }

    private List<Zombie> _zombies = null!;
    private string _json = string.Empty;
    private string _toon = string.Empty;
    private byte[]? _memory;

    private JsonSerializerOptions _jsonOptions = null!;
    // If you want custom TOON options, uncomment this:
    // private ToonSerializerOptions _toonOptions = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Configure Bogus faker to create realistic customers
        var customerFaker = new Faker<Zombie>()
            .RuleFor(c => c.Id, f => f.IndexFaker + 1)
            .RuleFor(c => c.FirstName, f => f.Name.FirstName())
            .RuleFor(c => c.LastName, f => f.Name.LastName())
            .RuleFor(c => c.Email, f => f.Internet.Email())
            .RuleFor(c => c.BirthDate,
                f => f.Date.Past(40, DateTime.UtcNow.AddYears(-18))) // 18–58 yrs
            .RuleFor(c => c.IsDangerous, f => f.Random.Bool())
            .RuleFor(c => c.Health, f => f.Finance.Amount(0, 10_000));

        _zombies = customerFaker.Generate(Count);

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        // If you want to tweak TOON behaviour:
        // _toonOptions = new ToonSerializerOptions
        // {
        //     IndentSize = 2,
        //     Delimiter = ToonDelimiter.Comma,
        //     Strict = true
        // };

        // Pre-serialize once for the deserialization benchmarks
        _json = JsonSerializer.Serialize(_zombies, _jsonOptions);
        _toon = ToonSerializer.Serialize(_zombies /*, _toonOptions*/);
        _memory = MemoryPackSerializer.Serialize(_zombies);
    }

    [Benchmark]
    public string Serialize_SystemTextJson()
    {
        return JsonSerializer.Serialize(_zombies, _jsonOptions);
    }

    [Benchmark]
    public byte[] Serialize_MemoryPack()
    {
        return MemoryPackSerializer.Serialize(_zombies);
    }

    [Benchmark]
    public string Serialize_ToonSharp()
    {
        return ToonSerializer.Serialize(_zombies /*, _toonOptions*/);
    }

    // ----------------- Deserialization -----------------

    [Benchmark]
    public List<Zombie>? Deserialize_SystemTextJson()
    {
        return JsonSerializer.Deserialize<List<Zombie>>(_json, _jsonOptions);
    }

    [Benchmark]
    public List<Zombie>? Deserialize_MemoryPack()
    {
        return MemoryPackSerializer.Deserialize<List<Zombie>>(_memory);
    }

    [Benchmark]
    public List<Zombie>? Deserialize_ToonSharp()
    {
        return ToonSerializer.Deserialize<List<Zombie>>(_toon);
    }
}
