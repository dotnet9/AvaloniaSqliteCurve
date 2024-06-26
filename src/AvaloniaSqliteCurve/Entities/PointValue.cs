namespace AvaloniaSqliteCurve.Entities
{
    public class PointValue
    {
        public int Id { get; set; }
        public double Value { get; set; }
        public byte Status { get; set; }
        public int UpdateTime { get; set; }
    }
}