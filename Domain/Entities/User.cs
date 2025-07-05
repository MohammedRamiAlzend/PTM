using PTM.Domain.Entities.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PTM.Domain.Entities
{
    public class User : Entity , ISoftDeletable
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string PasswordHash { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }

        public bool IsDeleted { get; set; }

    }
}
