using Shared.ExerDTO;
using Shared.MuscleDTO;

namespace WTAlgoServices.Interfaces
{
    public interface IVolumeCalculator
    {
        void Accumulate(
            Exercise exercise,
            ExerciseSession session,
            IDictionary<MuscleGroup, MuscleVolume> volumeMap);
    }

}
