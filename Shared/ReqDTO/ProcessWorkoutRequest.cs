using Shared.ExerDTO;
using Shared.MuscleDTO;
using Shared.TrainingMode;

namespace Shared.ReqDTO
{
    public class ProcessWorkoutRequest
    {
        public TrainingGoal TrainingGoal { get; set; }
        public Exercise Exercise { get; set; }
        public ExerciseSession Session { get; set; }
        public ExerciseProgress Progress { get; set; }
        public Dictionary<MuscleGroup, MuscleVolume> VolumeMap { get; set; }
    }

}
