using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.UiModels
{
    public class SessionDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserCode { get; set; }
        public string JWT { get; set; }
    }
}
