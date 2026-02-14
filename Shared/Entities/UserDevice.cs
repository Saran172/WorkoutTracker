using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Entities
{
    public class UserDevice
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string DeviceCode { get; set; } = null!;
        public string Platform { get; set; } = null!;
        public string? Model { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastSeenAt { get; set; }
    }
}
