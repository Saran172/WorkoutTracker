using Shared.ExerDTO;
using Shared.MuscleDTO;
using WTAlgoServices.Interfaces;

namespace Learning.Services.Classes
{
    public class VolumeCalculator : IVolumeCalculator
    {
        public void Accumulate(
            Exercise exercise,
            ExerciseSession session,
            IDictionary<MuscleGroup, MuscleVolume> volumeMap)
        {
            int hardSets = session.Sets.Count(s => s.RIR <= 4);

            foreach (var muscle in exercise.TargetMuscles)
            {
                if (!volumeMap.ContainsKey(muscle))
                    volumeMap[muscle] = new MuscleVolume { Muscle = muscle };

                volumeMap[muscle].WeeklySets += hardSets;
            }
        }
    }

}
