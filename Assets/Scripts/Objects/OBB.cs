using UnityEngine;

public class OBB : MonoBehaviour
{
    private void Awake()
    {
        if (CollisionDetection.obbs == null) CollisionDetection.obbs = new System.Collections.Generic.LinkedList<OBB>();
        CollisionDetection.obbs.AddLast(this);
        if(Q== new Quaternion(0,0,0,0)) Q = Quaternion.identity;
    }
    public OBB(Vector3 center, Quaternion q, Vector3 extent)
    {
        this.center = center;
        Q = q;
        this.extent = extent;
    }
    public Vector3 center;
    public Vector3 x;
    public Vector3 y;
    public Vector3 z;
    public Vector3 extent;
    private Quaternion q;
    public Quaternion Q
    {
        get { return q; }
        set
        {
            q = value;
            x = q * Vector3.right;
            y = q * Vector3.up;
            z = q * Vector3.forward;
        }
    }

    public override string ToString()
    {
        return ("OBB with center " + center + ", extent " + extent + " and rotation "+Q.eulerAngles);
    }

}
