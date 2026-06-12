using UnityEngine;
using UnityEngine.InputSystem;

public class GamePadScript : MonoBehaviour {
    public static GamePadScript instance;
    void Awake() {
        if (instance != this && instance != null) Destroy(this); else instance = this;
    }
    public float UserX() {
        float h = Input.GetAxis("Horizontal");
        if (Input.GetKey(KeyCode.LeftArrow)) h = -1;
        if (Input.GetKey(KeyCode.RightArrow)) h = 1;

        Vector2 leftStick = Gamepad.current?.leftStick.ReadValue() ?? Vector2.zero;
        if (leftStick.x < -0.5) h = -1;
        if (leftStick.x > 0.5) h = 1;
        return h;
    }
    public float UserY() {
        float v = Input.GetAxis("Vertical");
        if (Input.GetKey(KeyCode.DownArrow)) v = -1;
        if (Input.GetKey(KeyCode.UpArrow)) v = 1;

        Vector2 leftStick = Gamepad.current?.leftStick.ReadValue() ?? Vector2.zero;
        if (leftStick.y < -0.5) v = -1;
        if (leftStick.y > 0.5) v = 1;
        return v;
    }
    public float CameraX() {
        float h = 0;// = Input.GetAxis("Horizontal");
        if (Input.GetKey(KeyCode.Keypad4)) h = -1;
        if (Input.GetKey(KeyCode.Keypad6)) h = 1;

        Vector2 rightStick = Gamepad.current?.rightStick.ReadValue() ?? Vector2.zero;
        if (rightStick.x < -0.5) h = -1;
        if (rightStick.x > 0.5) h = 1;
        return h;
    }
    public float CameraY() {
        float v = 0;// Input.GetAxis("Vertical");
        if (Input.GetKey(KeyCode.Keypad2)) v = -1;
        if (Input.GetKey(KeyCode.Keypad8)) v = 1;

        Vector2 rightStick = Gamepad.current?.rightStick.ReadValue() ?? Vector2.zero;
        if (rightStick.y > 0.5) v = 1;
        if (rightStick.y < -0.5) v = -1;
        return v;
    }
    public float left() {
        if (UserX() < 0) return -UserX();
        return 0;
    }
    public float right() {
        if (UserX() > 0) return UserX();
        return 0;
    }
    public float up() {
        if (UserY() > 0) return UserY();
        return 0;
    }
    public float down() {
        if (UserY() < 0) return -UserY();
        return 0;
    }
    public bool jumpButtonDown() {
        return Input.GetKeyDown(KeyCode.Space)
            || (Gamepad.current?.buttonSouth.wasPressedThisFrame ?? false);
    }
    public bool jumpButtonUp() {
        return Input.GetKeyUp(KeyCode.Space)
            || (Gamepad.current?.buttonSouth.wasPressedThisFrame ?? false);
    }
    public bool jumpButton() {
        return Input.GetKey(KeyCode.Space)
            || (Gamepad.current?.buttonSouth.wasPressedThisFrame ?? false);
    }

}
