using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Entities
{
    public class User
    {
        public long Id { get; set; }
        public string UserCode { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? UserName { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
