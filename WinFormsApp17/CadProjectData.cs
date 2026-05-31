namespace WinFormsApp17
{
    public class CadProjectData
    {
        public List<LineSaveData> Lines { get; set; } = new();

        // 上部
        public string CalculatedSiteArea { get; set; } = "";   // 敷地_面積計算
        public string TargetSiteArea { get; set; } = "";       // 敷地⇒面積合わせ

        // 全体計算
        public string SiteArea { get; set; } = "";             // 敷地面積
        public string BuildingCoverageRatio { get; set; } = ""; // 建蔽率
        public string AdoptedFloorAreaRatio { get; set; } = ""; // [採用] 容積率
        public string AllowedBuildingArea { get; set; } = "";  // 許容建築面積
        public string AllowedFloorArea { get; set; } = "";     // 許容延床面積

        // 用途地域①
        public string Zoning1 { get; set; } = "";              // 用途地域①
        public string Zoning1BuildingCoverageRatio { get; set; } = ""; // 建蔽率
        public string Zoning1FloorAreaRatio { get; set; } = "";        // 容積率
        public string Zoning1SiteArea { get; set; } = "";      // 敷地面積

        // 用途地域②
        public string Zoning2 { get; set; } = "";              // 用途地域②
        public string Zoning2BuildingCoverageRatio { get; set; } = ""; // 建蔽率
        public string Zoning2FloorAreaRatio { get; set; } = "";        // 容積率
        public string Zoning2SiteArea { get; set; } = "";      // 敷地面積

        // 道路
        public string RoadWidth { get; set; } = "";            // 道路幅員
        public string RoadFloorAreaRatio { get; set; } = "";   // 道路による容積率
    }

    public class LineSaveData
    {
        public decimal StartX { get; set; }
        public decimal StartY { get; set; }
        public decimal EndX { get; set; }
        public decimal EndY { get; set; }
        public int Layer { get; set; }
    }
}