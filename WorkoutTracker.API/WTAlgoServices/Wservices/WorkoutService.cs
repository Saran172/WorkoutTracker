using Shared.ExerDTO;
using Shared.MuscleDTO;
using Shared.RespDTO;
using Shared.TrainingMode;
using WTAlgoServices.Interfaces;
using WTAlgoServices.TrainingFactory;

namespace WTAlgoServices.Wservices
{
    public class WorkoutService
    {
        private readonly IProgressionStrategyFactory _factory;
        private readonly IStimulusCalculator _stimulus;
        private readonly IVolumeCalculator _volume;

        public WorkoutService(IProgressionStrategyFactory factory, IStimulusCalculator stimulus, IVolumeCalculator volume)
        {
            _factory = factory;
            _stimulus = stimulus;
            _volume = volume;
        }

        public WorkoutResultDto ProcessExercise(TrainingGoal goal, Exercise exercise, ExerciseSession session, ExerciseProgress progress,
                                                                       IDictionary<MuscleGroup, MuscleVolume> volumeMap)
        {
            var strategy = _factory.GetStrategy(goal);

            var prescription = strategy.Evaluate(exercise, session, progress);

            var stimulusScore = _stimulus.Calculate(session);
                                _volume.Accumulate(exercise, session, volumeMap);

            return new WorkoutResultDto
            {
                ExerciseName = exercise.Name,
                Prescription = prescription,
                Stimulus = stimulusScore
            };
        }
    }


}
