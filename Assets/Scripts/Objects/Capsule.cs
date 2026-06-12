 using System.Net;
using UnityEngine;

public class Capsule : MonoBehaviour
{

    private void Awake()
    {
        if (CollisionDetection.capsules == null) CollisionDetection.capsules = new System.Collections.Generic.LinkedList<Capsule>();
        CollisionDetection.capsules.AddLast(this);
    }
    public Vector3 a;
    public Vector3 b;
    public float radius;
    public Quaternion Q;
    public override string ToString()
    {
        return ("Capsule with endpoint a " + a + ", endpoint b " + b + ", rotation " + Q.eulerAngles + " and radius " + radius);
    }
}
