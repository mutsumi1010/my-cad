using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static WinFormsApp17.Form1;

namespace WinFormsApp17
{
    public class DimensionMouseHandler
    {
        private readonly DimensionBuilder dimBuilder;
        private readonly DimensionDataList data;
        private readonly Function01 function;
        private readonly Func<PointF, PointDec> screenToWorld;
        private readonly Func<PointDec, PointF> worldToScreen;
        private readonly Action invalidate;

        public DimensionDirection Direction { get; private set; }
            = DimensionDirection.Horizontal;

        private DimStep dimStep = DimStep.Ext1;
        private int dimMode = 0;

        private PointDec? ext1Point;
        private PointDec? ext2Point;

        public PointDec? Ext1Point => ext1Point;
        public PointDec? Ext2Point => ext2Point;

        public DimensionMouseHandler(
            DimensionBuilder builder,
            DimensionDataList file,
            Function01 func,
            Func<PointF, PointDec> s2w,
            Func<PointDec, PointF> w2s,
            Action invalidateAction)
        {
            dimBuilder = builder;
            data = file;
            function = func;
            screenToWorld = s2w;
            worldToScreen = w2s;
            invalidate = invalidateAction;
        }

        //==========================
        // MouseClick
        //==========================
        public void OnMouseClick(MouseEventArgs e)
        {
            PointDec world = screenToWorld(new PointF(e.X, e.Y));

            // --- Ext1 ---
            if (dimStep == DimStep.Ext1)
            {
                dimMode = 1;
                dimBuilder.SetExt1(world);
                ext1Point = world;  // 引出線　の　先頭
                dimStep = DimStep.Ext2;
                invalidate();
                return;
            }

            // --- Ext2 ---
            if (dimStep == DimStep.Ext2)
            {
                dimMode = 2;
                dimBuilder.SetExt2(world);
                ext2Point = world;  //  引出線　の　根元
                dimStep = DimStep.Points;
                invalidate();
                return;
            }

            // --- 寸法点（右クリック） ---
            if (dimStep == DimStep.Points && e.Button == MouseButtons.Right)
            {
                if (!function.TrySnapToEndpoint(world, worldToScreen, out PointDec snapped))
                    return;

                dimBuilder.AddPoint(snapped);

                var dim = dimBuilder.TryBuildNext(Direction);
                if (dim != null)
                {
                    data.DimList.Add(dim);
                    invalidate();
                }
            }
        }

        //==========================
        // Space キー
        //==========================
        public void ToggleDirectionIfAllowed()
        {
            if (dimMode == 1)
            {
                Direction =
                    Direction == DimensionDirection.Horizontal
                    ? DimensionDirection.Vertical
                    : DimensionDirection.Horizontal;
                invalidate();
            }
            if (dimMode == 2)
            {
                Direction =
                    Direction == DimensionDirection.Horizontal
                    ? DimensionDirection.Horizontal
                    : DimensionDirection.Vertical;
                invalidate();

            }
        }
        //==========================
        // Reset
        //==========================
        public void Reset()
        {
            dimBuilder.Reset();
            dimStep = DimStep.Ext1;
            dimMode = 0;
            ext1Point = null;
            ext2Point = null;
            Direction = DimensionDirection.Horizontal;
        }
    }
}

