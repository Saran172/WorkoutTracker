using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.WorkoutDTO
{
    public class WorkoutSessionCreateDto
    {
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string? Notes {  get; set; }
        public string Status { get; set; } = "Completed";
        public int Version { get; set; } = 1;
        public List<WorkoutSessionExerciseDto> Exercises { get; set; } = new();
    }

    public class WorkoutSessionExerciseDto
    {
        public long ExerciseId { get; set; }
        public int ExerciseOrder { get; set; }
        public List<WorkoutSessionSetDto> Sets { get; set; } = new();
    }

    public class WorkoutSessionSetDto
    {
        public int SetNumber { get; set; }
        public int? Reps { get; set; }
        public decimal? Weight { get; set; }
        public bool IsFailure { get; set; }
        public int? RestSeconds { get; set; }
        public int? RIR { get; set; }
    }


}
