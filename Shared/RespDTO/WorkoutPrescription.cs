using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.RespDTO
{
    public class WorkoutPrescription
    {
        public double Weight { get; set; }
        public int Sets { get; set; }
        public int TargetRepsMin { get; set; }
        public int TargetRepsMax { get; set; }
    }


}
