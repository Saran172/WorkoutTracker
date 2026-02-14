using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.PlannerDTO
{
    public class TemplateUpsertDto
    {
        public string Name { get; set; } = null!;
        public string Type { get; set; } = "Strength";
        public string? Description { get; set; }
        public List<long> ExerciseIds { get; set; } = new();
    }
}
