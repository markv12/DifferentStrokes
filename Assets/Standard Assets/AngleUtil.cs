using System;
using UnityEngine;
using Random = UnityEngine.Random;

public static class AngleUtil {
    public static Vector2 PolarToCartesian(double angle, double radius) {
        double angleRad = (Math.PI / 180.0) * (angle - 90);
        double x = radius * Math.Cos(angleRad);
        double y = radius * Math.Sin(angleRad);
        return new Vector2((float)x, (float)y);
    }

    public static Vector2 GetRandomPointInCircle(Vector2 center, float maxRadius, float rimSpawnTendency) {
        var ang = Random.value * 360f;
        Vector2 pos;
        var exp = rimSpawnTendency;
        var radius = maxRadius * (1 - Mathf.Pow(Random.Range(0.1f, 1f), exp));
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }

    public static Vector2 GetRandomPointInEllipse(Vector2 center, float width, float height, float rimSpawnTendency) {
        var r = width * Mathf.Sqrt(Random.value);
        var fi = 2 * Mathf.PI * Random.value;
        var x = center.x + r * Mathf.Cos(fi);
        var y = center.y + height / width * r * Mathf.Sin(fi);
        return new Vector2(x, y);
    }

    public static bool IsPointInEllipse(Vector2 point, Vector2 ellipsePos, float width, float height) {
        var h = ellipsePos.x;
        var k = ellipsePos.y;
        var rxSq = Mathf.Pow(width, 2);
        var rySq = Mathf.Pow(height, 2);
        return (Mathf.Pow(point.x - h, 2) / rxSq + Mathf.Pow(point.y - k, 2) / rySq) <= 1;
    }

    #region draw gizmo 
    public static void DrawEllipse(Vector3 pos, Vector3 forward, Vector3 up, float radiusX, float radiusY, int segments, Color color, float duration = 0, bool dashed = false) {
        var angle = 0f;
        var rot = Quaternion.LookRotation(forward, up);
        var lastPoint = Vector3.zero;
        var thisPoint = Vector3.zero;
        bool Skip(int idx) => dashed && idx % 2 == 0;

        for (var i = 0; i < segments + 1; i++) {
            thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radiusX;
            thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radiusY;

            if (i > 0 && !Skip(i)) {
                Debug.DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color, duration);
            }

            lastPoint = thisPoint;
            angle += 360f / segments;
        }
    }

    public static void DrawX(Vector3 pos, Vector3 forward, Vector3 up, float radiusX, float radiusY, Color color) {
        // var p1 = pos.xy() + new Vector2(radiusX, radiusY);
        // var p2 = pos.xy() + new Vector2(radiusX, radiusY);
        // var p3 = pos.xy() + new Vector2(radiusX, -radiusY);
        // var p4 = pos.xy() + new Vector2(-radiusX, radiusY);
        Debug.DrawLine(pos.xy() + new Vector2(-radiusX, -radiusY), pos.xy() + new Vector2(radiusX, radiusY), color);
        Debug.DrawLine(pos.xy() + new Vector2(radiusX, -radiusY), pos.xy() + new Vector2(-radiusX, radiusY), color);
    }
    #endregion
}
