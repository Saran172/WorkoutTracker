using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Entities
{
    public class WorkoutTemplate
    {
        [Key]
        public long Id { get; set; }

        public Guid PublicId { get; set; }

        public long UserId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(50)]
        public string Type { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
