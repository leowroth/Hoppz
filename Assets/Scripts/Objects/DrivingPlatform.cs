using UniHagenGame.Hoppz;
using Unity.Mathematics;
using UnityEngine;

public class DrivingPlatform : MonoBehaviour
{
    public Vector3 endpoint;
    Vector3 boxepsilon;
    Vector3 startpoint;
    public float drivingSpeed;
    float direction = 1;
    Vector3 forward;
    float sqrDistance;
    Rigidbody rigidbody;
    LayerMask mask;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        startpoint = transform.position;
        endpoint += startpoint;
        forward = (endpoint - startpoint).normalized;
        sqrDistance = (endpoint - startpoint).sqrMagnitude;
        mask = LayerMask.GetMask("Player");
        boxepsilon = new Vector3(.2f,.2f,.2f);
    }
    public Vector3 delta;
    void FixedUpdate()
    {
        delta = direction * forward * drivingSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + delta;
        float triangleeq = (startpoint - newPosition).sqrMagnitude + (endpoint - newPosition).sqrMagnitude;
        if (triangleeq > sqrDistance) direction *= -1;
        else
        {
            if (Physics.CheckBox(newPosition, transform.localScale / 2f-boxepsilon, transform.rotation, mask))
                Hoppz.instance.Push(delta);
            rigidbody.MovePosition(newPosition);
        }
    }

}
