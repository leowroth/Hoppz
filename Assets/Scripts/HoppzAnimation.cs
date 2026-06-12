using UnityEngine;

public class HoppzAnimation : MonoBehaviour
{
    float currentInclination;
    float maxInclination=20;

    // Update is called once per frame
    void Update()
    {
        if (currentInclination < 20 && (GamePadScript.instance.UserX() != 0 || GamePadScript.instance.UserY() != 0))
            currentInclination+=2;
        if (currentInclination > 0 && GamePadScript.instance.UserX() == 0 && GamePadScript.instance.UserY() == 0)
            currentInclination-=2;
        Vector3 currentRotation = transform.localRotation.eulerAngles;
        currentRotation.x = -currentInclination;
        transform.localRotation = Quaternion.Euler(currentRotation);
    }
}
