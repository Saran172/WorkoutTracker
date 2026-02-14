using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Entities
{
    public class WebLoginDto : LoginBaseDto
    {

    }
    public class WebRegisterDto
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
    public class MobileLoginDto : LoginBaseDto
    {
        public string DeviceCode { get; set; } = null!;
        public string? DeviceName { get; set; }
        public string? FcmToken { get; set; }
        public string Platform { get; set; } = "android";
    }

    public abstract class LoginBaseDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

}
