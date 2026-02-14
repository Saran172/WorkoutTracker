using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Entities
{
    public class DevicePushToken
    {
        public long Id { get; set; }
        public long DeviceId { get; set; }
        public string FcmToken { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
