using WinFormsApp17;

public static class AreaCalculator
{
    /// <summary>
    /// 敷地境界の線分リスト（順不同OK）から面積を返す（decimal）。
    /// 1つの閉ループであること（各頂点の次数が2）を前提にします。
    /// </summary>
    public static decimal CalcAreaFromSegments(IReadOnlyList<(PointDec A, PointDec B)> segments)
    {
        if (segments == null) throw new ArgumentNullException(nameof(segments));
        if (segments.Count < 3) throw new ArgumentException("線分が少なすぎます（最低3辺必要）。");

        // 1) 隣接リスト（各点に繋がる相手点を2つ持つはず）
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

        foreach (var (A, B) in segments)
        {
            if (A.Equals(B)) continue; // ゼロ長は無視
            AddEdge(A, B);
            AddEdge(B, A);
        }

        // 2) 閉ループチェック：各頂点が次数2か
        foreach (var kv in adj)
        {
            if (kv.Value.Count != 2)
                throw new InvalidOperationException($"閉ループになってない可能性：点 {kv.Key} の接続数が {kv.Value.Count} です（2であるべき）。");
        }

        // 3) どれか1点から順に辿って頂点列を作る（同じ点に戻るまで）
        var start = adj.Keys.First();
        var vertices = BuildOrderedLoopVertices(adj, start);

        if (vertices.Count < 4)
            throw new InvalidOperationException("頂点列が短すぎます。");

        // 最後は始点で閉じてる形にしてある（v0 ... v(n-1)=v0）
        // 4) シュー レース公式
        decimal sum = 0m;
        for (int i = 0; i < vertices.Count - 1; i++)
        {
            var p = vertices[i];
            var q = vertices[i + 1];
            sum += (p.x * q.y) - (q.x * p.y);
        }

        // 面積は |sum| / 2
        var area = Math.Abs(sum) / 2m;
        return area;
    }

    /// <summary>
    /// 隣接辞書（各点に2つ隣接）から、start から辿って閉ループ頂点列を作る。
    /// 返り値は最後に始点を入れて閉じた形（v0..v0）。
    /// </summary>
    private static List<PointDec> BuildOrderedLoopVertices(Dictionary<PointDec, List<PointDec>> adj, PointDec start)
    {
        var vertices = new List<PointDec>();

        // start の隣接2点のうち、どっちから行ってもループになる
        var next = adj[start][0];

        PointDec prev = start;
        PointDec cur = next;

        vertices.Add(start);
        vertices.Add(cur);

        // ループを辿る
        int guard = adj.Count + 5; // 無限ループ防止（適当な上限）
        while (guard-- > 0)
        {
            var nbs = adj[cur];
            // 次は「前の点じゃない方」
            var candidate = nbs[0].Equals(prev) ? nbs[1] : nbs[0];

            prev = cur;
            cur = candidate;

            vertices.Add(cur);

            if (cur.Equals(start))
                return vertices; // 閉じた
        }

        throw new InvalidOperationException("ループ追跡に失敗しました（閉じてない/データ不整合の可能性）。");
    }
}

