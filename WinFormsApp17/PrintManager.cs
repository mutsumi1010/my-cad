using System;
using System.Drawing;
using System.Drawing.Printing;

namespace WinFormsApp17
{
    public class PrintManager
    {
        private readonly LineManager lineManager;

        public PrintManager(LineManager lm)
        {
            lineManager = lm;
        }

        public void Execute()
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += Pd_PrintPage;

            PrintDialog dlg = new PrintDialog();
            dlg.Document = pd;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                pd.Print();
        }

        private void Pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            // ◆ 1）背景を白でクリア
            e.Graphics.Clear(Color.White);

            // ◆ 2）プリンタ DPI（PX/MIN）取得
            float dpiX = e.Graphics.DpiX;  // 標準なら 300 や 600 など
            float dpiY = e.Graphics.DpiY;

            // ◆ 3）1mm を何ピクセルで描くか？
            float pxPerMm = dpiX / 25.4f;   // ← これが絶対基準

            // ◆ 4）縮尺（1/100）
            float scale = 1f / 100f;

            // ◆ 5）むつみCAD 内部座標補正（ここだけ調整する）
            // ---------------------------------------------------
            // internalFix の値を後で変えてテストする！
            float internalFix = 1f/3;   // ← 最初は補正なしでテスト
            // ---------------------------------------------------

            // ◆ 6）線を描く
           // foreach (var (start, end) in lineManager.decFile)
            foreach (var line in lineManager.decFile)
            {
                var start = line.start;
                var end = line.end;
                // 内部値 → 印刷ピクセル座標へ変換
                float x1 = (float)start.x * internalFix * scale * pxPerMm;
                float y1 = (float)start.y * internalFix * scale * pxPerMm;
                float x2 = (float)end.x * internalFix * scale * pxPerMm;
                float y2 = (float)end.y * internalFix * scale * pxPerMm;

                e.Graphics.DrawLine(Pens.Black, x1, y1, x2, y2);
            }

            e.HasMorePages = false;
        }
    }
}
