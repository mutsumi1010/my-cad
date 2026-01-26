using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp17
{
    public class DxfSave
    {
        private readonly LineManager lineManager;
        private readonly List<Dimension> dimList;

        public DxfSave(
            LineManager lineManager,
            List<Dimension> dimList)
        {
            this.lineManager = lineManager;
            this.dimList = dimList;
        }
        public void Execute()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "DXFファイルを保存";
                sfd.Filter = "DXFファイル (*.dxf)|*.dxf";
                //sfd.FileName = "";
                sfd.OverwritePrompt = true;    //上書きしますか？

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var exportDxf = new ExportDxf();
                    exportDxf.ExportDxf01(
                              sfd.FileName,
                              lineManager.decFile, 
                              dimList
                              );

                    MessageBox.Show("DXFファイルとして保存しました。", "完了",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
