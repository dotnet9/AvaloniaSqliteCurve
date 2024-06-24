﻿namespace AvaloniaSqliteCurve.Entities
{
    public class PointValue
    {
        public int Id { get; set; }
        
        public int PointId { get; set; }
        public double Value { get; set; }
        public int Status { get; set; }
        public long UpdateTime { get; set; }
    }
}