
using Shared.MuscleDTO;

namespace Shared.ExerDTO
{
    public class Exercise
    {
        public string Name { get; set; }
        public double Weight { get; set; }
        public int Sets { get; set; }
        public int TargetRepsMin { get; set; }
        public int TargetRepsMax { get; set; }
        public List<MuscleGroup> TargetMuscles { get; set; } = new();
    }
}
