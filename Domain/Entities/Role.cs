using System.ComponentModel.DataAnnotations;

namespace PTM.Domain.Entities
{
    public class Role : Entity 
    {
        [Required]
        public string Name { get; set; }

        public ICollection<User> Users { get; set; }
    }
}