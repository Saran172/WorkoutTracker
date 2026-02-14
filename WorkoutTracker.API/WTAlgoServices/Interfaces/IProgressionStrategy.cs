using Shared.ExerDTO;
using Shared.RespDTO;

namespace WTAlgoServices.Interfaces
{
    public interface IProgressionStrategy
    {
        WorkoutPrescription Evaluate(Exercise exercise, ExerciseSession session, ExerciseProgress progress);
    }
}
