# Json VS Toon VS MemoryPack

<img width="1382" height="702" alt="Screenshot 2025-11-25 021255" src="https://github.com/user-attachments/assets/009adaed-a6f2-48ee-90dc-f1892047a574" />

1. What measured

Runtime: .NET 10.0

Machine: i9-14900HX, Windows 11

Tools: BenchmarkDotNet, Bogus, three serializers:

System.Text.Json

MemoryPack

ToonSharp

Workload: Serialize/deserialize lists of fake Customer objects with sizes:

100, 1 000, 100 000

2. TL;DR – who wins?

Across all sizes, both serialize and deserialize:

🥇 MemoryPack

🥈 System.Text.Json

🥉 ToonSharp (way behind)

Rough feel from your table:

For 100k items, MemoryPack is several times faster than System.Text.Json and allocates dramatically less.

ToonSharp is much slower and allocates a lot more memory than both.

So yeah: the weird binary one absolutely bodies Microsoft’s JSON in raw speed and allocations.

3. Why is MemoryPack so fast?

Super-short version:

Binary, not text → no string parsing/formatting, just raw bytes.

Source-generated, tight loops → no heavy reflection, lots of Span<T>/buffer reuse.

Allocation-aware → far fewer temporary objects, less GC.

JSON is designed for humans and interoperability.
MemoryPack is designed for machines and throughput.

Not human-readable… but from your results, very clearly machine-lovable.

That’s what you’re seeing in the Gen0 and Allocated columns: less garbage, fewer collections.
