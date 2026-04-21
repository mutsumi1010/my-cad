namespace WinFormsApp17;

//=======================
//  RoadFunction クラス
//=======================
public class RoadFunction
{
    private readonly LineManager lineManager;

    public RoadFunction(LineManager lineManager)
    {
        this.lineManager = lineManager;
    }

    //========================================
    //  反対側道路境界線をつくる
    //========================================
    public bool TryMakeOppositeRoadLine(
        int selectedIndex,
        decimal width1,
        decimal width2,
        out LineEntity oppositeLine)
    {
        oppositeLine = default;

        if (selectedIndex < 0)
            return false;

        if (width1 <= 0 || width2 <= 0)
            return false;

        // 起点を決める
        GetKiten(selectedIndex, out PointDec w1Ktn, out PointDec w2Ktn);

        // 敷地頂点を半時計回りで取得
        var vertices = GetSiteVerticesCCW();
        if (vertices == null)
            return false;

        // 選択された線が何番目の辺か
        int edgeIndex = GetSelectedSiteEdgeIndex(selectedIndex, vertices);
        if (edgeIndex < 0)
            return false;

        // 道路辺の外側法線ベクトル
        var outside = GetOutsideNormal(edgeIndex, vertices);

        // 起点間ベクトル
        decimal dx = w2Ktn.x - w1Ktn.x;
        decimal dy = w2Ktn.y - w1Ktn.y;

        double L = Math.Sqrt((double)(dx * dx + dy * dy));
        if (L == 0.0)
            return false;

        // 幅員差
        decimal dw = width2 - width1;

        // 解なし
        if (Math.Abs(dw) > (decimal)L)
            return false;

        // 起点間の単位ベクトル
        double ex = (double)dx / L;
        double ey = (double)dy / L;

        // その直交ベクトル
        double px = -ey;
        double py = ex;

        // 反対側道路境界線の法線候補を2本つくる
        double a = -(double)dw / L;
        double b = Math.Sqrt(1.0 - a * a);

        // 候補1の法線
        double n1x = a * ex + b * px;
        double n1y = a * ey + b * py;

        // 候補2の法線
        double n2x = a * ex - b * px;
        double n2y = a * ey - b * py;

        // 候補1の接点2点
        PointDec p1a = new PointDec(
            w1Ktn.x + (decimal)(n1x * (double)width1),
            w1Ktn.y + (decimal)(n1y * (double)width1));

        PointDec p1b = new PointDec(
            w2Ktn.x + (decimal)(n1x * (double)width2),
            w2Ktn.y + (decimal)(n1y * (double)width2));

        // 候補2の接点2点
        PointDec p2a = new PointDec(
            w1Ktn.x + (decimal)(n2x * (double)width1),
            w1Ktn.y + (decimal)(n2y * (double)width1));

        PointDec p2b = new PointDec(
            w2Ktn.x + (decimal)(n2x * (double)width2),
            w2Ktn.y + (decimal)(n2y * (double)width2));

        // どちらが敷地の外側か判定
        decimal dot1 =
            (p1a.x - w1Ktn.x) * outside.nx +
            (p1a.y - w1Ktn.y) * outside.ny;

        decimal dot2 =
            (p2a.x - w1Ktn.x) * outside.nx +
            (p2a.y - w1Ktn.y) * outside.ny;

        if (dot1 >= dot2)
        {
            ExtendLine(ref p1a, ref p1b, 5000m);

            oppositeLine = new LineEntity
            {
                start = p1a,
                end = p1b,
                Layer = 2
            };
        }
        else
        {
            ExtendLine(ref p2a, ref p2b, 5000m);

            oppositeLine = new LineEntity
            {
                start = p2a,
                end = p2b,
                Layer = 2
            };
        }

        return true;
    }

    //===============================
    //  起点を決める
    //===============================
    private void GetKiten(int selectedIndex, out PointDec w1Ktn, out PointDec w2Ktn)
    {
        var kyoukaiLine = lineManager.decFile[selectedIndex];
        PointDec a = kyoukaiLine.start;
        PointDec b = kyoukaiLine.end;

        decimal dx = Math.Abs(b.x - a.x);
        decimal dy = Math.Abs(b.y - a.y);

        if (dx >= dy)
        {
            if (a.x <= b.x)
            {
                w1Ktn = a;
                w2Ktn = b;
            }
            else
            {
                w1Ktn = b;
                w2Ktn = a;
            }
        }
        else
        {
            if (a.y <= b.y)
            {
                w1Ktn = a;
                w2Ktn = b;
            }
            else
            {
                w1Ktn = b;
                w2Ktn = a;
            }
        }
    }

    //===============================
    //  敷地頂点を半時計回りに並べる
    //===============================
    private List<PointDec>? GetSiteVerticesCCW()
    {
        var siteLines = lineManager.decFile
            .Where(l => l.Layer == 1)
            .ToList();

        if (siteLines.Count < 3)
            return null;

        var adj = new Dictionary<PointDec, List<PointDec>>();

        void AddEdge(PointDec p, PointDec q)
        {
            if (!adj.TryGetValue(p, out var list))
            {
                list = new List<PointDec>(2);
                adj[p] = list;
            }
            list.Add(q);
        }

        foreach (var line in siteLines)
        {
            if (line.start.Equals(line.end))
                continue;

            AddEdge(line.start, line.end);
            AddEdge(line.end, line.start);
        }

        foreach (var kv in adj)
        {
            if (kv.Value.Count != 2)
                return null;
        }

        var start = adj.Keys.First();
        var closed = BuildOrderedLoopVertices(adj, start);
        var vertices = closed.Take(closed.Count - 1).ToList();

        if (SignedArea2(vertices) < 0m)
            vertices.Reverse();

        return vertices;
    }

    //===============================
    //  頂点を順にたどる
    //===============================
    private List<PointDec> BuildOrderedLoopVertices(
        Dictionary<PointDec, List<PointDec>> adj,
        PointDec start)
    {
        var vertices = new List<PointDec>();

        var next = adj[start][0];

        PointDec prev = start;
        PointDec cur = next;

        vertices.Add(start);
        vertices.Add(cur);

        int guard = adj.Count + 5;
        while (guard-- > 0)
        {
            var nbs = adj[cur];
            var candidate = nbs[0].Equals(prev) ? nbs[1] : nbs[0];

            prev = cur;
            cur = candidate;

            vertices.Add(cur);

            if (cur.Equals(start))
                return vertices;
        }

        throw new InvalidOperationException("ループ追跡に失敗しました。");
    }

    //===============================
    //  符号付き面積の2倍
    //===============================
    private decimal SignedArea2(List<PointDec> vertices)
    {
        decimal sum = 0m;

        for (int i = 0; i < vertices.Count; i++)
        {
            var p = vertices[i];
            var q = vertices[(i + 1) % vertices.Count];
            sum += (p.x * q.y) - (q.x * p.y);
        }

        return sum;
    }

    //========================================
    //  選択された線が、CCW頂点列の何番目の辺か返す
    //========================================
    private int GetSelectedSiteEdgeIndex(int selectedIndex, List<PointDec> vertices)
    {
        if (selectedIndex < 0)
            return -1;

        if (vertices == null || vertices.Count < 2)
            return -1;

        var selectedLine = lineManager.decFile[selectedIndex];
        PointDec a = selectedLine.start;
        PointDec b = selectedLine.end;

        for (int i = 0; i < vertices.Count; i++)
        {
            PointDec p1 = vertices[i];
            PointDec p2 = vertices[(i + 1) % vertices.Count];

            bool sameDir = p1.Equals(a) && p2.Equals(b);
            bool reverseDir = p1.Equals(b) && p2.Equals(a);

            if (sameDir || reverseDir)
                return i;
        }

        return -1;
    }

    //===============================
    //  道路辺の外側法線ベクトル
    //===============================
    private (decimal nx, decimal ny) GetOutsideNormal(int edgeIndex, List<PointDec> vertices)
    {
        PointDec p1 = vertices[edgeIndex];
        PointDec p2 = vertices[(edgeIndex + 1) % vertices.Count];

        decimal dx = p2.x - p1.x;
        decimal dy = p2.y - p1.y;

        decimal nx = dy;
        decimal ny = -dx;

        decimal len = (decimal)Math.Sqrt((double)(nx * nx + ny * ny));

        nx /= len;
        ny /= len;

        return (nx, ny);
    }

    //===============================
    // 線を左右5m延長
    //===============================
    private void ExtendLine(ref PointDec s, ref PointDec e, decimal ext)
    {
        decimal dx = e.x - s.x;
        decimal dy = e.y - s.y;

        double L = Math.Sqrt((double)(dx * dx + dy * dy));
        if (L == 0)
            return;

        decimal ux = (decimal)((double)dx / L);
        decimal uy = (decimal)((double)dy / L);

        s = new PointDec(
            s.x - ux * ext,
            s.y - uy * ext);

        e = new PointDec(
            e.x + ux * ext,
            e.y + uy * ext);
    }

    public bool IsClosedLoop(List<LineEntity> lines)
    {
        var adj = new Dictionary<PointDec, List<PointDec>>();

        void AddEdge(PointDec p, PointDec q)
        {
            if (!adj.TryGetValue(p, out var list))
            {
                list = new List<PointDec>();
                adj[p] = list;
            }
            list.Add(q);
        }

        foreach (var line in lines)
        {
            if (line.start.Equals(line.end))
                continue;

            AddEdge(line.start, line.end);
            AddEdge(line.end, line.start);
        }

        if (adj.Count < 3)
            return false;

        foreach (var kv in adj)
        {
            if (kv.Value.Count != 2)
                return false;
        }

        var start = adj.Keys.First();
        var visited = new HashSet<PointDec>();
        PointDec prev = default;
        PointDec cur = start;

        while (true)
        {
            visited.Add(cur);

            var neighbors = adj[cur];
            PointDec next;

            if (prev.Equals(default(PointDec)))
                next = neighbors[0];
            else
                next = neighbors[0].Equals(prev) ? neighbors[1] : neighbors[0];

            prev = cur;
            cur = next;

            if (cur.Equals(start))
                break;

            if (visited.Contains(cur))
                return false;
        }

        return visited.Count == adj.Count;
    }



}
