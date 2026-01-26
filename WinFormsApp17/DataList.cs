namespace WinFormsApp17
{
    public class DimensionDataList
    {
        public List<Dimension> DimList { get; } = new();

    }
    public class SiteDataList
    {
        public List<(PointDec start, PointDec end)> LineList { get; } = new();
        public List<CircleEntity> CircleList { get; } = new();

    }
    public class RoadDataList
    {
        public List<(PointDec start, PointDec end)> LineList { get; } = new();
        public List<CircleEntity> CircleList { get; } = new();

    }
    public class BuildingDataList
    {
        public List<(PointDec start, PointDec end)> LineList { get; } = new();
        public List<CircleEntity> CircleList { get; } = new();

    }
}
