using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Entities
{
    public class WorkoutTemplateExercise
    {
        [Key]
        public long Id { get; set; }

        public long TemplateId { get; set; }

        public long ExerciseId { get; set; }

        public int ExerciseOrder { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
