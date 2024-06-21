using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvaloniaSqliteCurve.Entities
{
    [Table("PointValue")]
    internal class PointValue
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public int PointId { get; set; }
        public double Value { get; set; }
        public int Status { get; set; }
        public long UpdateTime { get; set; }
    }
}