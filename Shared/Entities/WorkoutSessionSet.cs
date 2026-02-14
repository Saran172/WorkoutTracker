using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Entities
{
    public class WorkoutSessionSet
    {
        [Key]
        public long Id { get; set; }

        public long WorkoutExerciseId { get; set; }

        public int SetNumber { get; set; }

        public int? Reps { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal? Weight { get; set; }

        public int? RIR { get; set; }

        public int? RestSeconds { get; set; }

        public bool IsFailure { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid PublicId { get; set; }
    }
}
