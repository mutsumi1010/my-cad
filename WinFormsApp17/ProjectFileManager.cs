using System.Drawing;
using System.Text.Json;

namespace WinFormsApp17
{
    public class ProjectFileManager
    {
        private readonly LineManager lineManager;
        private readonly RoadManager roadManager;
        private readonly Form5 form5;

        private string? currentFilePath;

        public ProjectFileManager(
            LineManager lineManager,
            RoadManager roadManager,
            Form5 form5)
        {
            this.lineManager = lineManager;
            this.roadManager = roadManager;
            this.form5 = form5;
        }

        //===============================
        // 上書き保存
        //===============================
        public void Save()
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveAs();
                return;
            }

            SaveToFile(currentFilePath);
        }

        //===============================
        // 名前を付けて保存
        //===============================
        public void SaveAs()
        {
            using SaveFileDialog sfd =
                new SaveFileDialog();

            sfd.Filter =
                "MyCADファイル (*.mcad)|*.mcad|" +
                "JSONファイル (*.json)|*.json";

            sfd.Title =
                "名前を付けて保存";

            sfd.FileName =
                "project.mcad";

            sfd.OverwritePrompt =
                true;

            sfd.AddExtension =
                true;

            sfd.DefaultExt =
                "mcad";

            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            currentFilePath =
                sfd.FileName;

            SaveToFile(currentFilePath);
        }

        //===============================
        // ファイルへ保存
        //===============================
        private void SaveToFile(
            string path)
        {
            CadProjectData data =
                new CadProjectData();

            //===============================
            // decFileを保存
            //===============================
            foreach (LineEntity line
                in lineManager.decFile)
            {
                data.Lines.Add(
                    new LineSaveData
                    {
                        StartX = line.start.x,
                        StartY = line.start.y,
                        EndX = line.end.x,
                        EndY = line.end.y,
                        Layer = line.Layer
                    });
            }

            //===============================
            // roadWidthMapを保存
            //
            // Key   = 敷地境界線のdecFile番号
            // Value = 道路幅員
            //===============================

            foreach (var pair
                      in roadManager.roadWidthMap)
            {
                data.RoadWidthMap[pair.Key] =
                    new RoadWidthSaveData
                    {
                        W1 = pair.Value.w1,
                        W2 = pair.Value.w2
                    };
            }

            //===============================
            // roadVisualMapを保存
            //
            // LineEntityそのものではなく、
            // decFile内の線番号を保存する
            //===============================
            foreach (var pair
                in roadManager.roadVisualMap)
            {
                int roadIndex =
                    pair.Key;

                List<int> lineIndexes =
                    new List<int>();

                foreach (LineEntity roadLine
                    in pair.Value)
                {
                    int lineIndex =
                        lineManager.decFile.IndexOf(
                            roadLine);

                    if (lineIndex < 0)
                    {
                        MessageBox.Show(
                            "RoadVisualMap内の線が、" +
                            "decFileに見つかりません。\n" +
                            "保存を中止しました。");

                        return;
                    }

                    lineIndexes.Add(
                        lineIndex);
                }

                data.RoadVisualMap[roadIndex] =
                    lineIndexes;
            }

            //===============================
            // Form5入力データを保存
            //===============================
            form5.SaveInputDataTo(data);

            JsonSerializerOptions options =
                new JsonSerializerOptions
                {
                    WriteIndented = true
                };

            string json =
                JsonSerializer.Serialize(
                    data,
                    options);

            File.WriteAllText(
                path,
                json);

            MessageBox.Show(
                "保存しました");
        }

        //===============================
        // 開く
        //===============================
        public void Open()
        {
            using OpenFileDialog ofd =
                new OpenFileDialog();

            ofd.Filter =
                "MyCADファイル (*.mcad)|*.mcad|" +
                "JSONファイル (*.json)|*.json";

            ofd.Title =
                "プロジェクトを開く";

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            currentFilePath =
                ofd.FileName;

            LoadFromFile(
                currentFilePath);
        }

        //===============================
        // ファイルから読み込む
        //===============================
        private void LoadFromFile(
            string path)
        {
            string json =
                File.ReadAllText(path);

            CadProjectData? data =
                JsonSerializer.Deserialize
                    <CadProjectData>(json);

            if (data == null)
            {
                MessageBox.Show(
                    "読込失敗");

                return;
            }

            //===============================
            // 既存データをクリア
            //===============================
            lineManager.decFile.Clear();

            roadManager.roadWidthMap.Clear();

            roadManager.roadVisualMap.Clear();

            //===============================
            // decFileを復元
            //===============================
            foreach (LineSaveData line
                in data.Lines)
            {
                lineManager.decFile.Add(
                    new LineEntity
                    {
                        start =
                            new PointDec(
                                line.StartX,
                                line.StartY),

                        end =
                            new PointDec(
                                line.EndX,
                                line.EndY),

                        Layer =
                            line.Layer
                    });
            }

            //===============================
            // roadWidthMapを復元
            //===============================
            if (data.RoadWidthMap != null)
            {
                foreach (var pair
                    in data.RoadWidthMap)
                {
                    int roadIndex =
                        pair.Key;

                    if (roadIndex < 0 ||
                        roadIndex >=
                            lineManager.decFile.Count)
                    {
                        continue;
                    }

                    roadManager.roadWidthMap
                  [roadIndex] =
                   (
                       w1: pair.Value.W1,
                       w2: pair.Value.W2
                   );
                }
            }

            //===============================
            // roadVisualMapを復元
            //
            // 保存されている線番号を使い、
            // decFile内のLineEntityを直接登録する
            //===============================
            if (data.RoadVisualMap != null)
            {
                foreach (var pair
                    in data.RoadVisualMap)
                {
                    int roadIndex =
                        pair.Key;

                    if (roadIndex < 0 ||
                        roadIndex >=
                            lineManager.decFile.Count)
                    {
                        continue;
                    }

                    List<LineEntity> roadLines =
                        new List<LineEntity>();

                    foreach (int lineIndex
                        in pair.Value)
                    {
                        if (lineIndex < 0 ||
                            lineIndex >=
                                lineManager.decFile.Count)
                        {
                            continue;
                        }

                        roadLines.Add(
                            lineManager.decFile
                                [lineIndex]);
                    }

                    roadManager.roadVisualMap
                        [roadIndex] =
                            roadLines;
                }
            }

            //===============================
            // Form5へ反映
            //===============================
            form5.LoadInputDataFrom(data);

            MessageBox.Show(
                "読込しました");
        }
    }
}