using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Objects {
    public enum PlatformType {
        Solid,
        IgnoreCollisionBelow,
        IgnoreCollisionAbove,
        Reset
    }

    public class ChangingPlatform : MonoBehaviour {
        public Collider solidCollider;
        public Renderer platformRenderer;

        public PlatformType currentType = PlatformType.Solid;
        public float autoChangeIntervalSeconds = 0f;
        public PlatformType[] cycleTypes;

        public Transform resetPositionMarker;


        public Color colorSolid = Color.yellow;
        public Color colorIgnoreBelow = Color.green;
        public Color colorIgnoreAbove = Color.blue;
        public Color colorReset = Color.red;

        private int currentCycleIndex = 0;

        public void Start() {
            SetPlatformType(currentType);

            if (autoChangeIntervalSeconds > 0 && cycleTypes != null && cycleTypes.Length > 0) {
                currentCycleIndex = System.Array.IndexOf(cycleTypes, currentType);
                if (currentCycleIndex < 0) currentCycleIndex = 0;

                StartCoroutine(AutoChangeRoutine());
            }
        }
        private IEnumerator AutoChangeRoutine() {
            WaitForSeconds wait = new(autoChangeIntervalSeconds);

            while (true) {
                yield return wait;

                currentCycleIndex = (currentCycleIndex + 1) % cycleTypes.Length;
                SetPlatformType(cycleTypes[currentCycleIndex]);
            }
        }

        public void SetPlatformType(PlatformType newType) {
            currentType = newType;
            UpdateAppearance();
        }

        private void UpdateAppearance() {
            if (platformRenderer == null) return;

            switch (currentType) {
                case PlatformType.Solid:
                    platformRenderer.material.color = colorSolid;
                    break;
                case PlatformType.IgnoreCollisionBelow:
                    platformRenderer.material.color = colorIgnoreBelow;
                    break;
                case PlatformType.IgnoreCollisionAbove:
                    platformRenderer.material.color = colorIgnoreAbove;
                    break;
                case PlatformType.Reset:
                    platformRenderer.material.color = colorReset;
                    break;
            }
        }

        public void HandleTopTriggerEnter(Collider playerCollider) {
            if (currentType == PlatformType.IgnoreCollisionAbove) {
                Physics.IgnoreCollision(playerCollider, solidCollider, true);
            }
            else if (currentType == PlatformType.Reset) {
                ResetPlayer(playerCollider);
            }
        }

        public void HandleTopTriggerExit(Collider playerCollider) {
            if (currentType == PlatformType.IgnoreCollisionAbove) {
                Physics.IgnoreCollision(playerCollider, solidCollider, false);
            }
        }

        public void HandleBottomTriggerEnter(Collider playerCollider) {
            if (currentType == PlatformType.IgnoreCollisionBelow) {
                Physics.IgnoreCollision(playerCollider, solidCollider, true);
            }
            else if (currentType == PlatformType.Reset) {
                ResetPlayer(playerCollider);
            }
        }

        public void HandleBottomTriggerExit(Collider playerCollider) {
            if (currentType == PlatformType.IgnoreCollisionBelow) {
                Physics.IgnoreCollision(playerCollider, solidCollider, false);
            }
        }

        private void ResetPlayer(Collider playerCollider) {
            if (resetPositionMarker == null) {
                Debug.LogWarning("No Reset Position Marker assigned to ChangingPlatform!");
                return;
            }

            CharacterController cc = playerCollider.GetComponent<CharacterController>();
            if (cc != null) {
                // Unity Quirk: You must disable the CharacterController to teleport it
                cc.enabled = false;
                playerCollider.transform.position = resetPositionMarker.position;
                cc.enabled = true;
            }
        }
    }
}
