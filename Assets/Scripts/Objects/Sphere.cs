using UnityEngine;

public class Sphere : MonoBehaviour
{
    private void Awake()
    {
        if (CollisionDetection.spheres == null) CollisionDetection.spheres = new System.Collections.Generic.LinkedList<Sphere>();
        CollisionDetection.spheres.AddLast(this);
    }
    public Vector3 center;
    public float radius;
    public override string ToString()
    {
        return ("Sphere with center " + center + " and radius " + radius);
    }
}
