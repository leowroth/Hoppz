using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Objects {
    internal class PlatformTrigger : MonoBehaviour {
        public ChangingPlatform parentPlatform;

        public bool isTopTrigger;

        private void OnTriggerEnter(Collider other) {
            if(other.CompareTag("Player")) {
                if (isTopTrigger) {
                    parentPlatform.HandleTopTriggerEnter(other);
                }
                else {
                    parentPlatform.HandleBottomTriggerEnter(other);
                }
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.CompareTag("Player")) {
                if (isTopTrigger) {
                    parentPlatform.HandleTopTriggerExit(other);
                }
                else {
                    parentPlatform.HandleBottomTriggerExit(other);
                }
            }
        }
    }
}
