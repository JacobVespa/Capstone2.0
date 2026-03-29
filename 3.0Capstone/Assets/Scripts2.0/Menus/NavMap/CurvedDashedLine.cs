using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class CurvedDashedLine : Graphic
{
    // Set by CaveMap after instantiation
    public Vector2 fromPos;
    public Vector2 toPos;

    [Header("Curve")]
    public float curvature = 0.3f; // 0 = straight, higher = more curve

    [Header("Dash Settings")]
    public float dashLength   = 18f;
    public float gapLength    = 12f;
    public float lineWidth    = 6f;

    [Header("Segments")]
    public int curveResolution = 40; // how many straight segments approximate the curve

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        List<Vector2> points = SampleBezier(curveResolution);
        if (points.Count < 2) return;

        // Build a list of (start, end) pairs for each dash segment
        List<(Vector2 a, Vector2 b)> dashes = GetDashSegments(points);

        foreach (var (a, b) in dashes)
            AddDashQuad(vh, a, b);
    }

    // ── Bezier sampling ───────────────────────────────────────────────────────

    private List<Vector2> SampleBezier(int resolution)
    {
        List<Vector2> pts = new List<Vector2>(resolution + 1);

        Vector2 dir  = (toPos - fromPos).normalized;
        Vector2 perp = new Vector2(-dir.y, dir.x);
        float   dist = Vector2.Distance(fromPos, toPos);

        // Two control points offset in opposite directions — creates the S shape
        // curvature controls how wide the S bows out
        Vector2 control1 = Vector2.Lerp(fromPos, toPos, 0.33f) + perp * (dist * curvature);
        Vector2 control2 = Vector2.Lerp(fromPos, toPos, 0.66f) - perp * (dist * curvature);

        for (int i = 0; i <= resolution; i++)
        {
            float t  = i / (float)resolution;
            float mt = 1f - t;

            // Cubic bezier: B(t) = (1-t)³P0 + 3(1-t)²tP1 + 3(1-t)t²P2 + t³P3
            Vector2 p = mt * mt * mt * fromPos
                    + 3f * mt * mt * t  * control1
                    + 3f * mt * t  * t  * control2
                    + t  * t  * t       * toPos;
            pts.Add(p);
        }

        return pts;
    }

    // ── Dash layout ───────────────────────────────────────────────────────────

    private List<(Vector2, Vector2)> GetDashSegments(List<Vector2> pts)
    {
        var dashes = new List<(Vector2, Vector2)>();

        float accumulated = 0f;   // distance travelled along the curve
        bool  drawing     = true; // true = in a dash, false = in a gap
        float budget      = dashLength;
        Vector2 segStart  = pts[0];

        for (int i = 1; i < pts.Count; i++)
        {
            Vector2 prev    = pts[i - 1];
            Vector2 curr    = pts[i];
            float   segLen  = Vector2.Distance(prev, curr);
            float   walked  = 0f;

            while (walked < segLen)
            {
                float remaining = segLen - walked;
                float step      = Mathf.Min(budget, remaining);
                float t         = (walked + step) / segLen;
                Vector2 point   = Vector2.Lerp(prev, curr, t);

                walked    += step;
                budget    -= step;
                accumulated += step;

                if (budget <= 0f)
                {
                    if (drawing)
                        dashes.Add((segStart, point));

                    drawing  = !drawing;
                    budget   = drawing ? dashLength : gapLength;
                    segStart = point;
                }
            }
        }

        // Close out the last dash if we end mid-dash
        if (drawing && segStart != pts[pts.Count - 1])
            dashes.Add((segStart, pts[pts.Count - 1]));

        return dashes;
    }

    // ── Mesh building ─────────────────────────────────────────────────────────

    private void AddDashQuad(VertexHelper vh, Vector2 a, Vector2 b)
    {
        Vector2 dir     = (b - a).normalized;
        Vector2 perp    = new Vector2(-dir.y, dir.x) * (lineWidth * 0.5f);

        // Add rounded end caps by extending slightly past the endpoints
        Vector2 capExt  = dir * (lineWidth * 0.25f);
        Vector2 p0      = a - capExt + perp;
        Vector2 p1      = a - capExt - perp;
        Vector2 p2      = b + capExt - perp;
        Vector2 p3      = b + capExt + perp;

        int idx = vh.currentVertCount;

        UIVertex vert = new UIVertex();
        vert.color = color;

        vert.position = p0; vh.AddVert(vert);
        vert.position = p1; vh.AddVert(vert);
        vert.position = p2; vh.AddVert(vert);
        vert.position = p3; vh.AddVert(vert);

        vh.AddTriangle(idx,     idx + 1, idx + 2);
        vh.AddTriangle(idx,     idx + 2, idx + 3);
    }

    // Call this after changing fromPos/toPos/color to redraw
    public void Refresh() => SetVerticesDirty();
}