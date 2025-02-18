using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Person
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Gender { get; set; }

    [DataType(DataType.Date)]
    public DateTime Birthday { get; set; }

    // ✅ One-to-One: Person → ApplicationUser
    public virtual ApplicationUser? User { get; set; }
}