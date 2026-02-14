using Shared.ExerDTO;
using Shared.MuscleDTO;
using Shared.ProfileDTO;

namespace Shared.ReqDTO
{
    public class InitializeWorkoutRequest
    {
        public UserProfile User { get; set; }
        public List<Exercise> ExerciseDefinitions { get; set; }
        public Dictionary<string, SetLog> ExerciseHistory { get; set; }
    }

}
