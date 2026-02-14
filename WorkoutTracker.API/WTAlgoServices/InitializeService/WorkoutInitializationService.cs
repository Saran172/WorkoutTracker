using Shared.ExerDTO;
using Shared.MuscleDTO;
using Shared.ProfileDTO;

namespace WTAlgoServices.InitializeService
{
    public class WorkoutInitializationService
    {
        public List<Exercise> Initialize(UserProfile user, List<Exercise> definitions, Dictionary<string, SetLog> history)
        {
            var result = new List<Exercise>();

            foreach (var exercise in definitions)
            {
                if (!history.TryGetValue(exercise.Name, out var lastSet))
                {
                    double initWeight = CalculateInitialWeight(user, exercise.Name);
                    lastSet = new SetLog
                    {
                        Weight = initWeight,
                        Reps = exercise.TargetRepsMin,
                        RPE = 8
                    };
                }

                result.Add(new Exercise
                {
                    Name = exercise.Name,
                    Weight = lastSet.Weight,
                    Sets = exercise.Sets,
                    TargetRepsMin = exercise.TargetRepsMin,
                    TargetRepsMax = exercise.TargetRepsMax,
                    TargetMuscles = exercise.TargetMuscles
                });
            }

            return result;
        }

        static double CalculateInitialWeight(UserProfile user, string exercise)
        {
            double mult = exercise switch
            {
                "Bench Press" => user.ExperienceLevel == ExperienceLevel.Beginner ? 0.6 : 0.8,
                "Squat" => user.ExperienceLevel == ExperienceLevel.Beginner ? 0.8 : 1.1,
                "Overhead Press" => user.ExperienceLevel == ExperienceLevel.Beginner ? 0.4 : 0.6,
                _ => 0.5
            };
            return Math.Round(user.BodyWeightKg * mult / 2.5) * 2.5;
        }
    }

}
