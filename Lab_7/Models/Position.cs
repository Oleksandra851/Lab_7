using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab_7.Models
{
    [Table("Positions")]
    public class Position
    {
        [Key]
        public int PositionID { get; set; }

        [Required]
        [Display(Name = "Посада")]
        public string PositionName { get; set; } = string.Empty;

        [Display(Name = "Погодинна ставка")]
        public decimal HourlyRate { get; set; }

        public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
    }
}