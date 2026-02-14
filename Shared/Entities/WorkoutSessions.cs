using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Entities
{
    public class WorkoutSession
    {
        [Key]
        public long Id { get; set; }

        public long UserId { get; set; }

        public DateTime WorkoutDate { get; set; }  
        public DateTime? StartTime { get; set; }   
        public DateTime? EndTime { get; set; }     

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Guid PublicId { get; set; }

        public int Version { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = null!;
    }
}
