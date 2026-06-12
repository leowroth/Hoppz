using UnityEngine;

public class OneWayPlatform : MonoBehaviour {
    public Collider solidCollider;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Physics.IgnoreCollision(
                other,
                solidCollider,
                true
            );
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            Physics.IgnoreCollision(
                other,
                solidCollider,
                false
            );
        }
    }
}