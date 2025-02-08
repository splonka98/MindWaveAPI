using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindWaveApi.Infrastructure.Entities
{
    public class UserEntitie
    {
        [Key]
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string? AccessToken { get; set; }

        [ForeignKey("RoleId")]
        public int RoleId { get; set; }
        public virtual RoleEntitie Role { get; set; }
    }
}
