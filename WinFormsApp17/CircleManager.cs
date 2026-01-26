using WinFormsApp17;

internal class CircleManager
{
    private LineManager lineManager;
    private SiteDataList siteDataList;
    private RoadDataList roadDataList;
    private BuildingDataList buildingDataList;

    public CircleManager(
        LineManager manager,
        SiteDataList site,
        RoadDataList road,
        BuildingDataList building)
    {
        this.lineManager = manager;
        siteDataList = site;
        roadDataList = road;
        buildingDataList = building;
    }

    public void AddCircle(PointDec center, decimal radius, TargetType target)
    {
        var circle = new CircleEntity
        {
            center = center,
            radius = radius,
            Layer = (int)target
        };

        lineManager.CircleFile.Add(circle);

        switch (target)
        {
            case TargetType.Site:
                siteDataList.CircleList.Add(circle);
                break;

            case TargetType.Road:
                roadDataList.CircleList.Add(circle);
                break;

            case TargetType.Building:
                buildingDataList.CircleList.Add(circle);
                break;
        }
    }

    public bool RemoveCircleAt(PointDec click, decimal tol)
    {
        for (int i = lineManager.CircleFile.Count - 1; i >= 0; i--)
        {
            var c = lineManager.CircleFile[i];

            decimal dx = click.x - c.center.x;
            decimal dy = click.y - c.center.y;
            decimal dist = (decimal)Math.Sqrt((double)(dx * dx + dy * dy));

            // 円周付近をクリックしたか？
            if (Math.Abs(dist - c.radius) <= tol)
            {
                lineManager.CircleFile.RemoveAt(i);
                // DECFILEのレイヤでfileわけしてカテゴリfileを上書き
                RebuildCategoryCircleLists();
                return true;
            }
        }
        return false;
    }

    private void RebuildCategoryCircleLists()
    {
        siteDataList.CircleList.Clear();
        roadDataList.CircleList.Clear();
        buildingDataList.CircleList.Clear();

        foreach (var c in lineManager.CircleFile)
        {
            switch ((TargetType)c.Layer)
            {
                case TargetType.Site:
                    siteDataList.CircleList.Add(c);
                    break;

                case TargetType.Road:
                    roadDataList.CircleList.Add(c);
                    break;

                case TargetType.Building:
                    buildingDataList.CircleList.Add(c);
                    break;
            }
        }
    }



}
