using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HealthApp.Server.Models.DatabaseModels
{
    public class Practices
    {
        [Key]
        public int Id { get; set; }

        public string Practice { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<Licence> Licences { get; set; } = new List<Licence>();
    }
}
