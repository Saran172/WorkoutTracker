using Shared.ExerDTO;
using Shared.RespDTO;
using WTAlgoServices.Interfaces;

namespace WTAlgoServices.Strategy
{
    public class HypertrophyProgressionStrategy : IProgressionStrategy
    {
        public WorkoutPrescription Evaluate(
            Exercise exercise,
            ExerciseSession session,
            ExerciseProgress progress)
        {
            var topSet = session.Sets
                .OrderByDescending(s => s.Weight)
                .First();

            bool progressWeight =
                topSet.Reps >= exercise.TargetRepsMax && topSet.RIR <= 2 ||
                topSet.Reps >= exercise.TargetRepsMin + 1 && topSet.RIR <= 1;

            if (progressWeight)
            {
                var nextWeight = RoundToPlate(topSet.Weight * 1.025);
                exercise.Weight = nextWeight;

                progress.FailedSessionsInRow = 0;
                progress.IsInDeload = false;

                return new WorkoutPrescription
                {
                    Weight = nextWeight,
                    Sets = exercise.Sets,
                    TargetRepsMin = exercise.TargetRepsMin,
                    TargetRepsMax = exercise.TargetRepsMax
                };
            }

            progress.FailedSessionsInRow++;

            return new WorkoutPrescription
            {
                Weight = exercise.Weight,
                Sets = exercise.Sets,
                TargetRepsMin = Math.Min(
                    topSet.Reps + 1,
                    exercise.TargetRepsMax - 1),
                TargetRepsMax = exercise.TargetRepsMax
            };
        }

        private static double RoundToPlate(double weight)
            => Math.Ceiling(weight / 2.5) * 2.5;
    }

}
