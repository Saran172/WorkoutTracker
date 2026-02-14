using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.MuscleDTO
{
    public class SetLog
    {
        public double Weight { get; set; }
        public int Reps { get; set; }
        public double RPE { get; set; }
        public double RIR => Math.Max(0, 10 - RPE);
    }
}
