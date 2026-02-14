using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.PlannerDTO
{
    public class TemplateListDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Description { get; set; } = "";
        public int ExerciseCount { get; set; }
    }

}
