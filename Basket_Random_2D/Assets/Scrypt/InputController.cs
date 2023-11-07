using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public delegate void OnJumpKeyPressedAction();
    public event OnJumpKeyPressedAction onJump;

    public delegate void OnMoveKeyReleaseAction();
    public  event OnMoveKeyReleaseAction OnMoveKeyRelease;

    public delegate void OnMoveAction(bool isMovingRight);
    public  event OnMoveAction onMove;
    
    public delegate void OnThrowAction();
    public event OnThrowAction onThrow;

    public KeyCode jumpKey;
    public KeyCode rightMoveKey;
    public KeyCode leftMoveKey;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(jumpKey))
        {
            onJump?.Invoke();
        }
        
        if (Input.GetKeyUp(jumpKey))
        {
            onThrow?.Invoke();
        }

        if (Input.GetKey(rightMoveKey) || Input.GetKey(leftMoveKey))
        {
            bool isMovingRight = Input.GetKey(rightMoveKey);
            onMove?.Invoke(isMovingRight);

            //Debug.Log("Key Move pressed");
        }

        if (Input.GetKeyUp(rightMoveKey) || Input.GetKeyUp(leftMoveKey))
        {
            OnMoveKeyRelease?.Invoke();
        }

    }
}
