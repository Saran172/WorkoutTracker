using Shared.TrainingMode;
using WTAlgoServices.Interfaces;
using WTAlgoServices.Strategy;

namespace WTAlgoServices.TrainingFactory
{
    public class ProgressionStrategyFactory : IProgressionStrategyFactory
    {
        private readonly HypertrophyProgressionStrategy _hypertrophy;
        private readonly StrengthProgressionStrategy _strength;

        public ProgressionStrategyFactory(
            HypertrophyProgressionStrategy hypertrophy,
            StrengthProgressionStrategy strength)
        {
            _hypertrophy = hypertrophy;
            _strength = strength;
        }

        public IProgressionStrategy GetStrategy(TrainingGoal goal)
        {
            return goal switch
            {
                TrainingGoal.Hypertrophy => _hypertrophy,
                TrainingGoal.Strength => _strength,
                _ => throw new ArgumentOutOfRangeException(nameof(goal))
            };
        }
    }

}
