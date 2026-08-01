using System.Diagnostics;

namespace WinFormsApp17
{
    public class LineManager
    {
        public TargetType TargetM { get; private set; } = TargetType.None;
        public List<LineEntity> decFile { get; } = new();
        public List<CircleEntity> CircleFile { get; } = new();

        private readonly SiteDataList site;
        private readonly RoadDataList road;
        private readonly BuildingDataList building;

        public enum LayerType
        {
            Site = 1,
            Road = 2,
            Building = 3,

            SiteUseBoundary = 11
        }

        public LineManager(
           SiteDataList site,
           RoadDataList road,
           BuildingDataList building)
        {
            this.site = site;
            this.road = road;
            this.building = building;
        }

        //  Form2からcurrentTargetを受け取り、TargetMに代入するメソッド
        public void SetCurrentTarget(TargetType target)
        {
            this.TargetM = target;
        }

        private int GetCurrentLayer()
        {
            return TargetM switch
            {
                TargetType.Site => 1,
                TargetType.Road => 2,
                TargetType.Building => 3,
                _ => 3 // デフォルトは建物
            };
        }

        public bool IsBackgroundLayer(int layer)
        {
            return layer == 8;  //背景用DXF（将来、編集可能/ロック/非表示を切替予定）
        }

        public bool IsGuideLayer(int layer)
        {
            return layer == 9;  //仮線（下書き・常に編集不可）
        }

        public bool IsEditable(LineEntity line)
        {
            return !IsBackgroundLayer(line.Layer) && !IsGuideLayer(line.Layer);
        }

        //==========================
        //  線の追加（decimal）
        //==========================
        // public void AddLine(PointDec start, PointDec end)
        public void AddLine(PointDec start, PointDec end, bool isUseBoundaryMode = false)
        {
            int layer = GetCurrentLayer();

            if (TargetM == TargetType.Site && isUseBoundaryMode)
            {
                layer = (int)LayerType.SiteUseBoundary; // 11
            }

            decFile.Add(new LineEntity
            {
                start = start,
                end = end,
                Layer = layer
            });

            // 今は decFile を正本にするため、各 Target の LineList には追加しない
            /*
            switch (TargetM)
            {
                case TargetType.Site:
                    site.LineList.Add((start, end));
                    break;

                case TargetType.Road:
                    road.LineList.Add((start, end));
                    break;

                case TargetType.Building:
                    building.LineList.Add((start, end));
                    break;
            }
            */
        }


        //--------------------------
        // 線の追加　Site
        //--------------------------
        public void AddSiteLine(PointDec start, PointDec end)
        {
            int layer = (int)LayerType.Site;

            decFile.Add(new LineEntity
            {
                start = start,
                end = end,
                Layer = layer
            });
        }


        //  線の削除

        public void RemoveLine(LineEntity line)
        {
            if (!IsEditable(line))
                return;

            // ★ 建物モードのときは敷地ライン（Layer 1）を消去禁止
            if (TargetM == TargetType.Building && line.Layer == (int)LayerType.Site)
                return;

            // 共通の描画用リスト
            decFile.Remove(line);
            var tuple = (line.start, line.end);

            // カレントターゲット別リスト
            switch (TargetM)
            {
                case TargetType.Site:
                    site.LineList.Remove(tuple);
                    break;

                case TargetType.Road:
                    road.LineList.Remove(tuple);
                    break;

                case TargetType.Building:
                    building.LineList.Remove(tuple);
                    break;
            }
        }

        //  線の挿入（消した線をCtrl+Zで戻す
        public void InsertLine(int index, PointDec start, PointDec end)
        {
            int layer = GetCurrentLayer();

            // 正本
            decFile.Insert(index, new LineEntity
            {
                start = start,
                end = end,
                Layer = layer
            });

            switch (TargetM)
            {
                case TargetType.Site:
                    site.LineList.Add((start, end));
                    break;

                case TargetType.Road:
                    road.LineList.Add((start, end));
                    break;

                case TargetType.Building:
                    building.LineList.Add((start, end));
                    break;
            }
        }

        public void CornerLine(
                      int decFirstIndex,
                      int decSecondIndex,
                      LineEntity newLine1,
                      LineEntity newLine2,
                      int siteFirstIndex,
                      int siteSecondIndex,
                      int roadFirstIndex,
                      int roadSecondIndex,
                      int buildingFirstIndex,
                      int buildingSecondIndex
                     )
        {
            // =========================
            // decFile 更新
            // =========================
            if (decFirstIndex >= 0)
                decFile[decFirstIndex] = newLine1;

            if (decSecondIndex >= 0)
                decFile[decSecondIndex] = newLine2;

            // =========================
            // Site
            // =========================
            var t1 = (newLine1.start, newLine1.end);
            var t2 = (newLine2.start, newLine2.end);

            if (siteFirstIndex >= 0)
                site.LineList[siteFirstIndex] = t1;

            if (siteSecondIndex >= 0)
                site.LineList[siteSecondIndex] = t2;

            // =========================
            // Road
            // =========================
            if (roadFirstIndex >= 0)
                road.LineList[roadFirstIndex] = t1;

            if (roadSecondIndex >= 0)
                road.LineList[roadSecondIndex] = t2;

            // =========================
            // Building
            // =========================
            if (buildingFirstIndex >= 0)
                building.LineList[buildingFirstIndex] = t1;

            if (buildingSecondIndex >= 0)
                building.LineList[buildingSecondIndex] = t2;
        }

        // 全削除
        public void ClearAll()
        {
            decFile.Clear();

            switch (TargetM)
            {
                case TargetType.Site: // TargetMode から TargetType に変更
                    site.LineList.Clear();
                    break;

                case TargetType.Road: // TargetMode から TargetType に変更
                    road.LineList.Clear();
                    break;

                case TargetType.Building: // TargetMode から TargetType に変更
                    building.LineList.Clear();
                    break;
            }

        }

        // 最後の線を削除
        public void RemoveLastLine()
        {
            if (decFile.Count > 0)
                decFile.RemoveAt(decFile.Count - 1);

            switch (TargetM)
            {
                case TargetType.Site: // TargetMode から TargetType に変更
                    if (site.LineList.Count > 0)
                        site.LineList.RemoveAt(site.LineList.Count - 1);
                    break;

                case TargetType.Road: // TargetMode から TargetType に変更
                    if (road.LineList.Count > 0)
                        road.LineList.RemoveAt(road.LineList.Count - 1);
                    break;

                case TargetType.Building: // TargetMode から TargetType に変更
                    if (building.LineList.Count > 0)
                        building.LineList.RemoveAt(building.LineList.Count - 1);
                    break;
            }

        }

        public void RebuildLayerLists(
              SiteDataList site,
              RoadDataList road,
              BuildingDataList building)
        {
            site.LineList.Clear();
            road.LineList.Clear();
            building.LineList.Clear();

            foreach (var line in decFile)
            {
                switch ((LayerType)line.Layer)
                {
                    case LayerType.Site:
                        site.LineList.Add((line.start, line.end));
                        break;

                    case LayerType.Road:
                        road.LineList.Add((line.start, line.end));
                        break;

                    case LayerType.Building:
                        building.LineList.Add((line.start, line.end));
                        break;
                }

            }
        }
    }
}
