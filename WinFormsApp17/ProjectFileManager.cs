using System.Drawing;
using System.Text.Json;

namespace WinFormsApp17
{
    public class ProjectFileManager
    {
        private readonly LineManager lineManager;
        private readonly Form5 form5;

        private string? currentFilePath;

        public ProjectFileManager(LineManager lineManager, Form5 form5)
        {
            this.lineManager = lineManager;
            this.form5 = form5;
        }

        // 上書き保存
        public void Save()
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveAs();
                return;
            }

            SaveToFile(currentFilePath);
        }

        // 名前を付けて保存
        public void SaveAs()
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = "MyCADファイル (*.mcad)|*.mcad|JSONファイル (*.json)|*.json";
            sfd.Title = "名前を付けて保存";
            sfd.FileName = "project.mcad";
            sfd.OverwritePrompt = true;
            sfd.AddExtension = true;
            sfd.DefaultExt = "mcad";

            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            currentFilePath = sfd.FileName;

            SaveToFile(currentFilePath);
        }

        private void SaveToFile(string path)
        {
            CadProjectData data = new CadProjectData();

            // 図形データ
            foreach (var line in lineManager.decFile)
            {
                data.Lines.Add(new LineSaveData
                {
                    StartX = line.start.x,
                    StartY = line.start.y,
                    EndX = line.end.x,
                    EndY = line.end.y,
                    Layer = line.Layer
                });
            }

            // Form5の入力データ
            form5.SaveInputDataTo(data);

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(data, options);

            File.WriteAllText(path, json);

            MessageBox.Show("保存しました");
        }

        //===============================
        // 開く
        //==============================
        public void Open()
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "MyCADファイル (*.mcad)|*.mcad|JSONファイル (*.json)|*.json";
            ofd.Title = "プロジェクトを開く";

            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            currentFilePath = ofd.FileName;

            LoadFromFile(currentFilePath);
        }
        private void LoadFromFile(string path)
        {
            string json = File.ReadAllText(path);

            CadProjectData? data =
                JsonSerializer.Deserialize<CadProjectData>(json);

            if (data == null)
            {
                MessageBox.Show("読込失敗");
                return;
            }

            // 線クリア
            lineManager.decFile.Clear();

            // 線復元
            foreach (var line in data.Lines)
            {
                lineManager.decFile.Add(new LineEntity
                {
                    start = new PointDec(line.StartX, line.StartY),
                    end = new PointDec(line.EndX, line.EndY),
                    Layer = line.Layer
                });
            }

            // Form5へ反映
            form5.LoadInputDataFrom(data);

            MessageBox.Show("読込しました");
        }
    }
}
