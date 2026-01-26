using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace WinFormsApp17
{
    public class DxfLoad
    {
        private readonly LineManager lineManager;
        private readonly List<Dimension> dimList;

        public DxfLoad(LineManager lineManager, List<Dimension> dimList)
        {
            this.lineManager = lineManager;
            this.dimList = dimList;
        }

        public void Execute()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "DXFファイルを開く";
                ofd.Filter = "DXFファイル (*.dxf)|*.dxf";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var importDxf = new ImportDxf();

                    // ★ ここだけ変更（戻り値に合わせる）
                    var (lines, dimensions)
                        = importDxf.ImportDxf01(ofd.FileName);

                    // ===== LINE =====
                    lineManager.ClearAll();
                    lineManager.decFile.AddRange(lines);

                    // ===== DIMENSION =====
                    dimList.Clear();
                    dimList.AddRange(dimensions);

                    MessageBox.Show(
                        "DXFファイルを読み込みました。",
                        "完了",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
        }
    }
}
