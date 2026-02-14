using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Entities
{
    public class WorkoutSessionExercise
    {
        [Key]
        public long Id { get; set; }

        public long WorkoutSessionId { get; set; }

        public long ExerciseId { get; set; }

        public int ExerciseOrder { get; set; }

        public Guid PublicId { get; set; }
    }
}
