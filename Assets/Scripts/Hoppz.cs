//using System.Numerics;
using System;
using UnityEngine;


namespace UniHagenGame.Hoppz {
    public class Hoppz : MonoBehaviour {
        public CharacterController controller;
        Capsule myCapsule;
        readonly OBB myOBB;
        public static Hoppz instance;
        readonly float speed = 8f;

        // jump attributes
        float yspeed = -.2f;
        readonly float standardgravity = 0.05f;
        readonly float jumpgravity = 0.01f;
        readonly float jumpInitialspeed = 0.3f;
        readonly float maximumFallspeed = -.2f;
        int jumpTopCounter;
        int jumpTopFreeze = 3;
        float jumpHoldTimer;
        private readonly float minimumJumpHoldTime = 0.03f;

        // double jump attributes
        int jumpsRemaining;
        readonly int maxJumps = 2;
        bool wasGrounded;

        Vector3 distance;
        Vector3 feetDistance;

        // used to give the player a bit of time to still move, even if already over the platform
        public int coyoteTime;
        private void Awake() {
            Application.targetFrameRate = 60;
            int vSync = Math.Clamp((int)Math.Round(Screen.currentResolution.refreshRateRatio.value / 60), 1, 4);
            QualitySettings.vSyncCount = vSync;
            instance = this;
            myCapsule = GetComponent<Capsule>();
            distance = new Vector3(0, transform.localScale.y, 0);
            myCapsule.radius = transform.localScale.x / 2;
            myCapsule.a = transform.position - distance;
            myCapsule.b = transform.position + distance;
            myCapsule.Q = Quaternion.identity;
            //myOBB = GetComponent<OBB>();
            controller = GetComponent<CharacterController>();
            feetDistance = new Vector3(0, -transform.localScale.y / 2, 0);
        }
        // Update is called once per frame
        void Update() {
            float h = GamePadScript.instance.UserX();
            float v = GamePadScript.instance.UserY();
            Vector3 movementVector = CameraScript.instance.transform.TransformVector(new Vector3(h, 0, v));
            movementVector.y = 0;
            movementVector.Normalize();
            movementVector *= Time.deltaTime * speed;

            Vector3 newPosition = transform.position
                + movementVector;

            if (controller.isGrounded) {
                // double jump reset
                if (!wasGrounded) {
                    jumpsRemaining = maxJumps;
                }
                wasGrounded = true;

                groundEffect = Vector3.zero;
                coyoteTime = 7;
                movementVector.y = -.2f;
            }
            else {
                wasGrounded = false;

                if (--coyoteTime < 0) coyoteTime = 0;
            }
            if (GamePadScript.instance.jumpButtonDown()) {
                bool canGroundJump = coyoteTime > 0;
                // double jump check
                bool canAirJump = coyoteTime == 0 && jumpsRemaining > 0;

                if (canGroundJump || canAirJump) {
                    Jump();
                }
            }
            if (coyoteTime == 0) {
                movementVector.y = yspeed;
                AirAcceleration();
            }

            transform.LookAt(newPosition);
            CheckDrivingPlatform();
            //transform.position = newPosition;
            controller.Move(movementVector + pushingVector + groundEffect * Time.deltaTime);
            pushingVector = Vector3.zero;
        }
        void Jump() {
            coyoteTime = 0;
            yspeed = jumpInitialspeed;
            jumpsRemaining--;

            jumpHoldTimer = minimumJumpHoldTime;
        }
        void AirAcceleration() {
            if (jumpHoldTimer > 0) jumpHoldTimer -= Time.deltaTime;

            if (yspeed > 0) {
                yspeed -= jumpgravity;
                if (yspeed <= .01f
                    || (jumpHoldTimer <= 0 && GamePadScript.instance.jumpButtonUp())) {
                    jumpTopCounter = jumpTopFreeze;
                    yspeed = -.01f;
                }
                CollisionFlags hitCeiling = controller.collisionFlags & CollisionFlags.Above;
                if (hitCeiling != 0) {
                    yspeed = -.01f;
                    jumpTopCounter = 0;
                }
            }
            else if (jumpTopCounter > 0) --jumpTopCounter;
            else if (yspeed < 0) {
                yspeed -= standardgravity;
                if (yspeed < maximumFallspeed) yspeed = maximumFallspeed;
            }
            else jumpTopCounter = jumpTopFreeze;
        }
        Vector3 pushingVector;
        public void Push(Vector3 pushingVector) {
            this.pushingVector += pushingVector;
        }
        Vector3 groundEffect;
        void CheckDrivingPlatform() {
            RaycastHit[] hits = Physics.RaycastAll(transform.position + feetDistance / 2, Vector3.down, 1);

            foreach (RaycastHit hit in hits) {
                DrivingPlatform drivingPlatform = hit.collider.GetComponent<DrivingPlatform>();
                if (drivingPlatform != null) {
                    groundEffect = drivingPlatform.delta / Time.fixedDeltaTime;
                    return;
                }
            }
        }

    }
}