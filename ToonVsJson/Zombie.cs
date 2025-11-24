using MemoryPack;

namespace ToonVsJson;

[MemoryPackable]
public partial class Zombie
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public DateTime BirthDate { get; set; }
    public bool IsDangerous { get; set; }
    public decimal Health { get; set; }
}