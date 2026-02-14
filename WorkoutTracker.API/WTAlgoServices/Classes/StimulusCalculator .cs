using Shared.ExerDTO;
using WTAlgoServices.Interfaces;

namespace WTAlgoServices.Classes
{
    public class StimulusCalculator : IStimulusCalculator
    {
        public double Calculate(ExerciseSession session)
        {
            return session.Sets.Sum(s =>
                Math.Max(0, 4 - s.RIR) * s.Weight);
        }
    }

}
