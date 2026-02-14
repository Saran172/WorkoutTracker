using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Entities
{
    public class Exercise
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string MuscleGroup { get; set; } = null!;
        public string Equipment { get; set; } = null!;
        public bool IsCompound { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
