using Shared.ExerDTO;

namespace WTAlgoServices.Interfaces
{
    public interface IStimulusCalculator
    {
        double Calculate(ExerciseSession session);
    }

}
