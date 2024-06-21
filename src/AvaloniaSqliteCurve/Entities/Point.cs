namespace AvaloniaSqliteCurve.Entities;

internal class Point
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Type { get; set; }

    internal Point(string name, int type)
    {
        Name = name;
        Type = type;
    }
}

enum PointType
{
    Integer,
    Double
}