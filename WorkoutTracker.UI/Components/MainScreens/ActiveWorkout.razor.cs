using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Shared.Entities;
using Shared.UiModels;
using Shared.WorkoutDTO;
using System.Timers;
using WorkoutTracker.UI.Components.Services;

namespace WorkoutTracker.UI.Components.MainScreens
{
    public partial class ActiveWorkout
    {
        [Inject] private WorkoutStateService WorkoutState { get; set; }
        private DateTime workoutStartTime;
        private DateTime? workoutEndTime;

        class WorkoutExercise
        {
            public long ExerciseId { get; set; }
            public string Name { get; set; } = "";
            public List<ExSet> Sets { get; set; } = new();
        }
        class ExSet
        {
            public int Number { get; set; }
            public double Weight { get; set; }
            public int Reps { get; set; }
            public bool IsDone { get; set; }
        }

        public string JWT { get; set; }
        private List<Exercise> exerciseLibrary = new();
        private List<WorkoutExercise> activeExercises = new();
        private IEnumerable<Exercise> FilteredLibrary =>
                                                     exerciseLibrary.Where(e =>
                                                         (selectedCategory == "All" || e.MuscleGroup == selectedCategory) &&
                                                         (string.IsNullOrWhiteSpace(searchText) ||
                                                          e.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                                                     );
        private List<string> categories = new()
        {
            "All", "Chest", "Back", "Legs", "Shoulders", "Biceps", "Triceps", "Core", "Cardio"
        };


        protected override async Task OnInitializedAsync()
        {
            var token = await SessionStorage.GetAsync<SessionDTO>("Session");
            JWT = token.Value.JWT;
            var exerciseResponse = await Flow.GetExercises(JWT);
            if (exerciseResponse.RespData != null)
            {
                exerciseLibrary = JsonConvert.DeserializeObject<List<Exercise>>(exerciseResponse.RespData.ToString());
            }

            if (WorkoutState.PendingTemplate != null)
            {
                activeExercises.Clear();

                foreach (var id in WorkoutState.PendingTemplate.ExerciseIds)
                {
                    var libEx = exerciseLibrary.FirstOrDefault(e => e.Id == id);
                    if (libEx != null)
                    {
                        activeExercises.Add(new WorkoutExercise
                        {
                            ExerciseId = libEx.Id,
                            Name = libEx.Name,
                            Sets = Enumerable.Range(1, 3).Select(n => new ExSet { Number = n }).ToList()
                        });
                    }
                }

                WorkoutState.Clear();
            }
            StartTimer();
            StateHasChanged();
        }

        private void SelectExercise(Exercise ex)
        {
            if (activeExercises.Any(a => a.ExerciseId == ex.Id))
                return;

            var newExercise = new WorkoutExercise
            {
                ExerciseId = ex.Id,
                Name = ex.Name,
                Sets = new List<ExSet>()
            };

            for (int i = 1; i <= 3; i++)
            {
                newExercise.Sets.Add(new ExSet { Number = i });
            }

            activeExercises.Add(newExercise);

            showExerciseSelector = false;
            searchText = "";
        }

        private void StartTimer()
        {
            workoutStartTime = DateTime.UtcNow;

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += (s, e) =>
            {
                if (!isPaused)
                    secondsElapsed++;

                if (restSeconds > 0)
                    restSeconds--;

                InvokeAsync(StateHasChanged);
            };

            timer.Start();
        }

        private async Task FinishWorkout()
        {
            totalVolume = activeExercises.SelectMany(ex => ex.Sets).Where(s => s.IsDone).Sum(s => s.Weight * s.Reps);
            completedSetsCount = activeExercises.SelectMany(ex => ex.Sets).Count(s => s.IsDone);

            workoutEndTime = DateTime.UtcNow;
            isPaused = true;

            var sessionDto = new WorkoutSessionCreateDto
            {
                StartDateTime = workoutStartTime,
                EndDateTime = workoutEndTime.Value,
                Status = "Completed",
                Version = 1
            };

            

            int order = 1;

            foreach (var ex in activeExercises)
            {
                var validCompletedSets = ex.Sets.Where(IsValidCompletedSet).ToList();

                if (validCompletedSets.Count == 0)
                    continue;

                var exDto = new WorkoutSessionExerciseDto
                {
                    ExerciseId = ex.ExerciseId,
                    ExerciseOrder = order++
                };

                foreach (var set in validCompletedSets)
                {
                    exDto.Sets.Add(new WorkoutSessionSetDto
                    {
                        SetNumber = set.Number,
                        Reps = set.Reps == 0 ? null : set.Reps,
                        Weight = set.Weight == 0 ? null : (decimal)set.Weight,
                        IsFailure = false,
                        RestSeconds = null,
                        RIR = null
                    });
                }

                sessionDto.Exercises.Add(exDto);
            }

            if (!sessionDto.Exercises.Any())
            {
                ToastService.ShowError("No completed exercises to save");
                isPaused = false;
                return;
            }

            var response = await Flow.AddWorkoutSession(sessionDto, JWT);

            if (response.RespCode == "200")
            {
                ToastService.ShowSuccess("Workout saved successfully");
                NavService.SetActiveMenu("Dashboard");
            }
            else
            {
                ToastService.ShowError("Failed to save workout");
                isPaused = false;
            }

            StateHasChanged();
        }
        private static bool IsValidCompletedSet(ExSet set)
        {
            return set.IsDone
                && set.Reps > 0
                && set.Weight > 0;
        }

    }
}
