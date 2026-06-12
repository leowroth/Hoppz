using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection {
    public static LinkedList<OBB> obbs;
    public static LinkedList<Sphere> spheres;
    public static LinkedList<Capsule> capsules;
    public static LinkedList<Cylinder> cylinders;

    public static bool collisionDetection(Sphere sphere) {
        if (obbs == null) obbs = new LinkedList<OBB>();
        if (spheres == null) spheres = new LinkedList<Sphere>();
        if (capsules == null) capsules = new LinkedList<Capsule>();
        cylinders ??= new LinkedList<Cylinder>();

        foreach (OBB o in obbs)
            if (collisionSphereOBB(o, sphere)) return true;
        foreach (Sphere otherSphere in spheres)
            if ((sphere != otherSphere) && collisionSphereSphere(sphere, otherSphere)) return true;
        foreach (Capsule capsule in capsules)
            if (collisionSphereCapsule(sphere, capsule)) return true;
        foreach (Cylinder cylinder in cylinders) {
            if (collisionSphereCapsule(sphere, cylinder.capsule)) return true;
        }
        return false;
    }

    public static bool collisionDetection(Capsule capsule) {
        if (obbs == null) obbs = new LinkedList<OBB>();
        if (spheres == null) spheres = new LinkedList<Sphere>();
        if (capsules == null) capsules = new LinkedList<Capsule>();
        cylinders ??= new LinkedList<Cylinder>();

        foreach (OBB o in obbs) {
            if (collisionCapsuleOBB(o, capsule)) return true;
        }
        foreach (Sphere s in spheres)
            if (collisionSphereCapsule(s, capsule)) return true;
        foreach (Capsule c2 in capsules)
            if ((capsule != c2) && collisionCapsuleCapsule(c2, capsule)) return true;
        foreach(Cylinder cylinder in cylinders) {
            if (collisionCapsuleCapsule(cylinder.capsule, capsule)) return true;
        }
        return false;
    }

    public static bool collisionDetection(OBB o) {
        if (obbs == null) obbs = new LinkedList<OBB>();
        if (spheres == null) spheres = new LinkedList<Sphere>();
        if (capsules == null) capsules = new LinkedList<Capsule>();
        foreach (OBB o2 in obbs)
            if ((o != o2) && collisionOBBOBB(o, o2)) return true;
        foreach (Sphere s in spheres)
            if (collisionSphereOBB(o, s)) return true;
        foreach (Capsule c in capsules)
            if (collisionCapsuleOBB(o, c)) return true;
        return false;
    }

    public static bool collisionDetection(Cylinder cylinder) {
        obbs ??= new LinkedList<OBB>();
        spheres ??= new LinkedList<Sphere>();
        capsules ??= new LinkedList<Capsule>();
        foreach (OBB obb in obbs) {
            if (collisionCapsuleOBB(obb, cylinder.capsule)) return true;
        }
        foreach (Sphere sphere in spheres) {
            if (collisionSphereCapsule(sphere, cylinder.capsule)) return true;
        }
        foreach (Capsule capsule in capsules) {
            if ((capsule != cylinder.capsule) && collisionCapsuleCapsule(capsule, cylinder.capsule)) return true;
        }
        return false;
    }

    public static bool collisionOBBCylinder(OBB obb, Cylinder cylinder) {
        return collisionOBBOBB(obb, cylinder.obb) && collisionCapsuleOBB(obb, cylinder.capsule);
    }

    public static bool collisionSphereCylinder(Sphere sphere, Cylinder cylinder) {
        return collisionSphereOBB(cylinder.obb, sphere) && collisionSphereCapsule(sphere, cylinder.capsule);
    }

    public static bool collisionCapsuleCylinder(Capsule capsule, Cylinder cylinder) {
        return collisionCapsuleOBB(cylinder.obb, capsule) && collisionCapsuleCapsule(capsule, cylinder.capsule);
    }

    public static bool collisionCylinderCylinder(Cylinder cylinder, Cylinder otherCylinder) {
        return collisionOBBOBB(cylinder.obb, otherCylinder.obb) && collisionCapsuleCapsule(cylinder.capsule, otherCylinder.capsule);
    }

    public static bool collisionSphereSphere(Vector3 p1, float r1, Vector3 p2, float r2) {
        float sqrDist = (p1 - p2).sqrMagnitude;
        float maxSqrDist = (r1 + r2) * (r1 + r2);
        return sqrDist < maxSqrDist;
    }

    static bool separatingAxisTest(OBB a, OBB b, Vector3 axis) {
        if (axis == Vector3.zero) return false;
        axis = axis.normalized;
        // Projiziere beide Mittelpunkte auf die Achse
        float projectCenterA = Vector3.Dot(a.center, axis);
        float projectCenterB = Vector3.Dot(b.center, axis);
        float maxDist = Mathf.Abs(projectCenterB - projectCenterA);

        // Berechne die projizierten Radien der OBBs:
        float radiusA =
          a.extent.x * Mathf.Abs(Vector3.Dot(a.x, axis))
        + a.extent.y * Mathf.Abs(Vector3.Dot(a.y, axis))
        + a.extent.z * Mathf.Abs(Vector3.Dot(a.z, axis));
        float radiusB =
          b.extent.x * Mathf.Abs(Vector3.Dot(b.x, axis))
        + b.extent.y * Mathf.Abs(Vector3.Dot(b.y, axis))
        + b.extent.z * Mathf.Abs(Vector3.Dot(b.z, axis));

        // Es gibt eine separierende Ebene falls die 
        // beiden Radien kleiner als der Abstand der 
        // projizierten Mittelpunkte ist.
        return radiusA + radiusB < maxDist;
    }
    public static bool collisionOBBOBB(OBB a, OBB b) {
        // Teste die Basisachsen von a:
        if (separatingAxisTest(a, b, a.x)) return false;
        if (separatingAxisTest(a, b, a.y)) return false;
        if (separatingAxisTest(a, b, a.z)) return false;

        // Teste die Basisachsen von b:
        if (separatingAxisTest(a, b, b.x)) return false;
        if (separatingAxisTest(a, b, b.y)) return false;
        if (separatingAxisTest(a, b, b.z)) return false;

        // Kreuzproduktachsen:
        if (separatingAxisTest(a, b, Vector3.Cross(a.x, b.x)) ||
          separatingAxisTest(a, b, Vector3.Cross(a.x, b.y)) ||
          separatingAxisTest(a, b, Vector3.Cross(a.x, b.z)) ||
          separatingAxisTest(a, b, Vector3.Cross(a.y, b.x)) ||
          separatingAxisTest(a, b, Vector3.Cross(a.y, b.y)) ||
          separatingAxisTest(a, b, Vector3.Cross(a.y, b.z)) ||
          separatingAxisTest(a, b, Vector3.Cross(a.z, b.x)) ||
          separatingAxisTest(a, b, Vector3.Cross(a.z, b.y)) ||
          separatingAxisTest(a, b, Vector3.Cross(a.z, b.z)))
            return false;

        // keine Trennebene gefunden, also Kollision:
        return true;
    }

    public static float sqrDistanceLineAABB(Vector3 a, Vector3 b, Vector3 extent) {
        Vector3 ab = b - a;
        // t = 0 und t = 1 für die Randpunkte der Strecke
        float[] candidates = new float[8];
        candidates[0] = 0;
        candidates[1] = 1;

        // Für jede Achse axis den Parameter t so berechnen, 
        // dass s[axis] = (a + t * ab)[axis] == +/-extent.axis,
        // also entlang axis wird der maximale / minimale Wert
        // des AABB angenommen.
        for (int axis = 0; axis < 3; ++axis) {
            // Startwert entlang der Achse axis
            float y0 = a[axis];
            // Steigung entlang der Achse axis
            float m = ab[axis];
            // Zielwert entlang der Achse axis
            float y = extent[axis];

            if (m != 0) {
                float tUpper = (y - y0) / m;
                float tLower = (-y - y0) / m;
                candidates[2 * axis + 2] = tUpper;
                candidates[2 * axis + 2 + 1] = tLower;
            }
        }
        float bestDist2 = float.PositiveInfinity;

        foreach (float t in candidates) {
            // Wenn t nicht in [0,1] liegt, liegt der Kandidat 
            // nicht auf der Strecke.
            if (t < 0 || t > 1) continue;
            // Kandidat für den nächsten Punkt auf der Strecke 
            // zum AABB
            Vector3 s = a + t * ab;
            // Kandidat für den nächsten Punkt auf dem AABB 
            // zur Strecke
            Vector3 clamped = new Vector3(
              clamp(s.x, -extent.x, extent.x),
              clamp(s.y, -extent.y, extent.y),
              clamp(s.z, -extent.z, extent.z)
            );
            // Quadrierte Distanz der Kandidatenpunkte
            float dist2 = (s - clamped).sqrMagnitude;
            // Besser als vorheriger Bestwert?
            if (dist2 < bestDist2)
                bestDist2 = dist2;
        }
        return bestDist2;
    }

    public static float clamp(float x, float lower, float upper) {
        if (x < lower) return lower;
        if (x > upper) return upper;
        return x;
    }
    public static bool collisionCapsuleOBB(OBB a, Capsule b) {
        // Koordinaten der Mittelstrecke relativ 
        // zum OBB-Mittelpunkt
        Vector3 pointA = b.a - a.center;
        Vector3 pointB = b.b - a.center;
        // Koordinaten der Mittelstrecke im 
        // OBB-Koordinatensystem
        pointA = Quaternion.Inverse(a.Q) * pointA;
        pointB = Quaternion.Inverse(a.Q) * pointB;
        // Berechne quadrierten Abstand zwischen dem zentrierten 
        // AABB und der Mittelstrecke der Kapsel
        float distance = sqrDistanceLineAABB(pointA, pointB, a.extent);
        return distance <= b.radius * b.radius;
    }

    public static bool collisionSphereOBB(OBB a, Sphere b) {
        // Koordinaten des Kugelmittelpunkts relativ 
        // zum OBB-Mittelpunkt
        Vector3 centerShifted = b.center - a.center;
        // Koordinaten des Kugelmittelpunkts im 
        // OBB-Koordinatensystem
        Vector3 centerInOBB = Quaternion.Inverse(a.Q) * centerShifted;
        // Ggf. Verschiebung des Mittelpunkts in den oberen 
        // rechten positiven Oktanten des 
        // OBB-Koordinatensystems
        Vector3 absoluteCenter = new Vector3(
          Mathf.Abs(centerInOBB.x),
          Mathf.Abs(centerInOBB.y),
          Mathf.Abs(centerInOBB.z));
        // Berechne komponentenweise den Abstand des 
        // Mittelpunkts vom OBB; 
        // wenn ein Wert kleiner 0 herauskommt, liegt 
        // die Komponente innerhalb der Grenzen des OBB 
        // und muss auf 0 gesetzt werden.
        Vector3 componentDistance = Vector3.Max(
          absoluteCenter - a.extent, Vector3.zero);
        float distance = componentDistance.sqrMagnitude;
        return distance <= b.radius * b.radius;
    }
    static float projectPointOnLine(Vector3 a, Vector3 b, Vector3 p) {
        Vector3 ab = b - a;
        float divisor = Vector3.Dot(ab, ab);
        // Konvention: Projektionsparameter auf Punkt a
        if (divisor == 0f) return 0f;
        return Vector3.Dot(p - a, ab) / divisor;
    }
    static float projectPointOnSegment(Vector3 a, Vector3 b, Vector3 p) {
        return clamp(projectPointOnLine(a, b, p), 0, 1);
    }
    public static float distanceSegments(Capsule a, Capsule b) {
        Vector3 c1SegmentA = a.a;
        Vector3 c1SegmentB = a.b;
        Vector3 c2SegmentA = b.a;
        Vector3 c2SegmentB = b.b;
        // Abfangen des Falls, dass eine der Strecken nur ein Punkt ist
        if (c1SegmentA == c1SegmentB) // c1 ist Punkt
        {
            if (c2SegmentA == c2SegmentB) // Beide Strecken sind Punkte
                return Vector3.Dot(c1SegmentA - c2SegmentA,
                  c1SegmentA - c2SegmentA);
            // Überführen in den Fall, dass c2 Punkt ist; tausche c1, c2
            // wegen c1SegmentA == c1SegmentB ohne temporäre Variable
            c1SegmentA = c2SegmentA;
            c2SegmentA = c1SegmentB;
            c1SegmentB = c2SegmentB;
            c2SegmentB = c2SegmentA;
        }
        if (c2SegmentA == c2SegmentB) // nur c2 ist Punkt
        {
            // Ermittle Projektion q von c2 auf c1:
            Vector3 p1 = c2SegmentA; // der eine Punkt auf c2
            float t = projectPointOnSegment(c1SegmentA, c1SegmentB, p1);
            Vector3 ab = c1SegmentB - c1SegmentA;
            Vector3 q1 = c1SegmentA + t * ab;
            // Gib quadrierten Abstand zwischen p und q zurück
            return Vector3.Dot(q1 - p1, q1 - p1);
        }
        // Allgemeiner Fall: Beide Strecken sind echte Strecken.
        Vector3 m1 = c1SegmentB - c1SegmentA;
        Vector3 m2 = c2SegmentB - c2SegmentA;
        Vector3 ra = c1SegmentA - c2SegmentA;
        float d1 = Vector3.Dot(m1, m1);
        float d2 = Vector3.Dot(m2, m2);
        float e1 = Vector3.Dot(m1, ra);
        float e2 = Vector3.Dot(m2, ra);
        float f = Vector3.Dot(m1, m2);
        float divisor = d1 * d2 - f * f;
        float t1;
        if (divisor == 0) // Parallele Strecken
        {
            // Wähle beliebigen Punkt auf c1
            t1 = 0;
        }
        else // Windschiefe Strecken
        {
            t1 = clamp((f * e2 - e1 * d2) / divisor, 0, 1);
        }
        float t2 = (f * t1 + e2) / d2;
        // Falls t2 in [0,1] liegt, sind wir fertig, sonst müssen
        // wir t2 clampen, auf c1 projizieren und den neuen Wert für
        // t1 ebenfalls clampen
        if (t2 < 0) {
            t2 = 0;
            t1 = clamp(-e1 / d1, 0, 1);
        }
        else if (t2 > 1) {
            t2 = 1;
            t1 = clamp((f - e1) / d1, 0, 1);
        }
        // Berechne Punkte p und q
        Vector3 p = c1SegmentA + t1 * m1;
        Vector3 q = c2SegmentA + t2 * m2;
        // Gib quadrierte Distanz zwischen p und q zurück.
        return Vector3.Dot(q - p, q - p);
    }
    public static bool collisionCapsuleCapsule(Capsule a, Capsule b) {
        float sqrDistance = distanceSegments(a, b);
        return sqrDistance <= (a.radius + b.radius)
          * (a.radius + b.radius
          );
    }
    public static bool collisionSphereCapsule(Sphere sphere, Capsule capsule) {
        Vector3 pointOnCapsule = closestPointSegmentPoint(
          capsule.a, capsule.b, sphere.center);
        float sqrDistance = (pointOnCapsule
          - sphere.center).sqrMagnitude;
        float maxDistance = (capsule.radius + sphere.radius)
          * (capsule.radius + sphere.radius);
        return sqrDistance <= maxDistance;
    }
    static Vector3 closestPointSegmentPoint(Vector3 segmentA, Vector3 segmentB, Vector3 point) {
        Vector3 ab = segmentB - segmentA;
        // Betrachte die Gerade, auf der das Segment 
        // zwischen segmentA und segmentB liegt.
        // Berechne, wie weit man von segmentA nach segmentB laufen 
        // muss, um eine Gerade zu finden, die senkrecht
        // auf der Geraden liegt und durch point läuft.
        float positionOnLine = Vector3.Dot(point - segmentA, ab)
          / Vector3.Dot(ab, ab);
        // Berechne den Punkt auf der Geraden, die wir gerade 
        // charakterisiert haben und clampe ihn auf die Strecke 
        // von segmentA nach segmentB
        return segmentA + clamp(positionOnLine, 0, 1) * ab;
    }
    public static bool collisionSphereSphere(Sphere a, Sphere b) {
        float sqrDist = (a.center - b.center).sqrMagnitude;
        float maxSqrDist = (a.radius + b.radius) * (a.radius + b.radius);
        return sqrDist < maxSqrDist;
    }

}
