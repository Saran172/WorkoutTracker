using Shared.MuscleDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ExerDTO
{
    public class ExerciseSession
    {
        public string ExerciseName { get; set; }
        public List<SetLog> Sets { get; set; } = new();
    }

}
