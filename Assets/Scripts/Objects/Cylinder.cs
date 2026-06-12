using UnityEngine;


public class Cylinder : MonoBehaviour {
    public Vector3 Position = Vector3.zero;
    public Quaternion Rotation = Quaternion.identity;
    public float Height = 2f;
    public float Radius = 0.5f;

    public OBB obb;
    public Capsule capsule;

    void Awake() {
        obb = GetComponent<OBB>() ?? gameObject.AddComponent<OBB>();
        capsule = GetComponent<Capsule>() ?? gameObject.AddComponent<Capsule>();

        UpdateRepresentation();
    }

    void OnValidate() {
        if (Application.isPlaying == false || obb != null && capsule != null) {
            UpdateRepresentation();
        }
    }

    public void UpdateRepresentation() {
        if (obb == null) obb = GetComponent<OBB>();
        if (capsule == null) capsule = GetComponent<Capsule>();
        if (obb == null || capsule == null) return;

        Quaternion Q = Rotation;
        Vector3 axis = Q * Vector3.up;
        float halfHeight = Mathf.Max(0f, Height * 0.5f);

        capsule.a = Position + axis * halfHeight;
        capsule.b = Position - axis * halfHeight;
        capsule.radius = Mathf.Max(0f, Radius);
        capsule.Q = Q;

        obb.center = Position;
        obb.Q = Q;
        obb.x = Q * Vector3.right;
        obb.y = Q * Vector3.up;
        obb.z = Q * Vector3.forward;
        obb.extent = new Vector3(capsule.radius, halfHeight, capsule.radius);
    }
}