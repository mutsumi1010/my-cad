using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp17
{
    public static class CentroidCalculator
    {

        public static PointDec CalcCentroidFromSegments(List<(PointDec start, PointDec end)> segments)
        {
            // 頂点リストを作る（順番つき）
            var vertices = BuildOrderedVertices(segments);

            decimal area = 0;
            decimal cx = 0;
            decimal cy = 0;

            for (int i = 0; i < vertices.Count - 1; i++)
            {
                var p1 = vertices[i];
                var p2 = vertices[i + 1];

                decimal cross = p1.x * p2.y - p2.x * p1.y;

                area += cross;
                cx += (p1.x + p2.x) * cross;
                cy += (p1.y + p2.y) * cross;
            }

            area /= 2;

            if (area == 0)
                throw new Exception("面積が0なので重心が求められません");

            cx /= (6 * area);
            cy /= (6 * area);

            return new PointDec(cx, cy);
        }

        private static List<PointDec> BuildOrderedVertices(List<(PointDec start, PointDec end)> segments)
        {
            var vertices = new List<PointDec>();

            var dict = new Dictionary<PointDec, List<PointDec>>();

            foreach (var (s, e) in segments)
            {
                if (!dict.ContainsKey(s)) dict[s] = new List<PointDec>();
                if (!dict.ContainsKey(e)) dict[e] = new List<PointDec>();

                dict[s].Add(e);
                dict[e].Add(s);
            }

            var start = segments[0].start;
            var current = start;
            PointDec prev = default;

            do
            {
                vertices.Add(current);

                var nextList = dict[current];
                PointDec next = nextList[0];

                if (next.Equals(prev) && nextList.Count > 1)
                    next = nextList[1];

                prev = current;
                current = next;

            } while (!current.Equals(start));

            vertices.Add(start); // 閉じる

            return vertices;
        }
    }
}
