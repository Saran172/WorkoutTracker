using Shared.TrainingMode;
using WTAlgoServices.Interfaces;

namespace WTAlgoServices.TrainingFactory
{
    public interface IProgressionStrategyFactory
    {
        IProgressionStrategy GetStrategy(TrainingGoal goal);
    }
}
