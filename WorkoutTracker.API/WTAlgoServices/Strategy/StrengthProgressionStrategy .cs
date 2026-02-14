using Shared.ExerDTO;
using Shared.RespDTO;
using WTAlgoServices.Interfaces;

namespace WTAlgoServices.Strategy
{
    public class StrengthProgressionStrategy : IProgressionStrategy
    {
        public WorkoutPrescription Evaluate(
            Exercise exercise,
            ExerciseSession session,
            ExerciseProgress progress)
        {
            // Heaviest set defines strength capacity
            var topSet = session.Sets
                .OrderByDescending(s => s.Weight)
                .First();

            bool increaseWeight = false;
            bool failed = false;

            /*
             STRENGTH RULES
             --------------
             Target reps usually 1–5
             Weight increases only if reps are hit cleanly
            */

            // ✅ Clean success → add weight
            if (topSet.Reps >= exercise.TargetRepsMax && topSet.RIR >= 1)
            {
                increaseWeight = true;
            }
            // ❌ Failed rep target or hit failure
            else if (topSet.Reps < exercise.TargetRepsMin || topSet.RIR <= 0)
            {
                failed = true;
            }

            if (increaseWeight)
            {
                // Smaller jumps than hypertrophy
                double nextWeight = RoundToNearestPlate(
                    topSet.Weight * 1.02); // ~2%

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

            if (failed)
            {
                progress.FailedSessionsInRow++;
            }

            // 🔻 Strength deload earlier
            if (progress.FailedSessionsInRow >= 2)
            {
                TriggerStrengthDeload(exercise, progress);
            }

            // Hold weight, same rep target
            return new WorkoutPrescription
            {
                Weight = exercise.Weight,
                Sets = exercise.Sets,
                TargetRepsMin = exercise.TargetRepsMin,
                TargetRepsMax = exercise.TargetRepsMax
            };
        }

        private static double RoundToNearestPlate(double weight)
            => Math.Ceiling(weight / 2.5) * 2.5;

        private static void TriggerStrengthDeload(
            Exercise exercise,
            ExerciseProgress progress)
        {
            // Strength deload = drop load, keep intensity
            exercise.Weight *= 0.9;      // −10%
            exercise.Sets = Math.Max(2, exercise.Sets - 1);

            progress.IsInDeload = true;
            progress.FailedSessionsInRow = 0;
        }
    }

}
