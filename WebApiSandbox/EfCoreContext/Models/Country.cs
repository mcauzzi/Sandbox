using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EfCoreContext.Models;

[Index(nameof(Name), IsUnique = true)]
public class Country
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public required string Name { get; set; }

    public HashSet<State> States { get; set; } = new();
}