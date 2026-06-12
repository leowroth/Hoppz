using UniHagenGame.Hoppz;
using UnityEngine;

public class CameraScript : MonoBehaviour {
    public Vector3 referencePoint;
    public float maxXZSqrDistance;
    float maxXZDistance;
    public float maxYDistance;

    public float cameraAngle;
    public float cameraAngleMin;
    public float cameraAngleMax;
    public float cameraDistance;
    public float cameraDistanceMin;
    public float cameraDistanceMax;
    float cameraZoomPercentage = .5f;
    readonly float manualCameraSpeed = 2;
    public float verticalFollowSpeed = 1f;
    public static CameraScript instance;
    private void Start() {
        instance = this;
        referencePoint = Hoppz.instance.transform.position;
        maxXZDistance = Mathf.Sqrt(maxXZSqrDistance);
        UpdateZoom();
    }

    void LateUpdate() {
        Vector3 userPosition = Hoppz.instance.transform.position;
        Vector3 userToReference = referencePoint - userPosition;
        float x = referencePoint.x;
        float y = referencePoint.y;
        float z = referencePoint.z;

        float minY = userPosition.y - maxYDistance;
        float maxY = userPosition.y + maxYDistance;
        float targetY = Mathf.Clamp(referencePoint.y, minY, maxY);
        y = Mathf.MoveTowards(referencePoint.y, targetY, verticalFollowSpeed * Time.deltaTime);

        Vector2 referenceXZ = new Vector2(userToReference.x, userToReference.z);
        if (referenceXZ.sqrMagnitude > maxXZSqrDistance) {
            referenceXZ = referenceXZ.normalized * maxXZDistance;
            x = userPosition.x + referenceXZ.x;
            z = userPosition.z + referenceXZ.y;
        }

        Vector3 newReference = new(x, y, z);
        transform.position += newReference - referencePoint;
        referencePoint = newReference;

        if (GamePadScript.instance.CameraX() != 0) {
            transform.RotateAround(referencePoint, Vector3.up, GamePadScript.instance.CameraX() * manualCameraSpeed);
        }

        if (GamePadScript.instance.CameraY() != 0) {
            cameraZoomPercentage -= GamePadScript.instance.CameraY() * manualCameraSpeed / 100;
            if (cameraZoomPercentage > 1) cameraZoomPercentage = 1;
            if (cameraZoomPercentage < 0) cameraZoomPercentage = 0;
            UpdateZoom();
        }
        transform.LookAt(referencePoint);
    }
    void UpdateZoom() {
        cameraDistance = cameraZoomPercentage * cameraDistanceMax + (1 - cameraZoomPercentage) * cameraDistanceMin;
        cameraAngle = (1 - cameraZoomPercentage) * cameraAngleMax + cameraZoomPercentage * cameraAngleMin;
        Vector3 cameraToReference = referencePoint - transform.position;
        cameraToReference.y = 0;
        Vector3 horizontal = cameraToReference.normalized;
        Vector3 axis = Vector3.Cross(horizontal, Vector3.up);
        Vector3 tiltedDirection = (Quaternion.AngleAxis(cameraAngle, axis) * horizontal);
        transform.position = referencePoint - tiltedDirection * cameraDistance;
    }
}
