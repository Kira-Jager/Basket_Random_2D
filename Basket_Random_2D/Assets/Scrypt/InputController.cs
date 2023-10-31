using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public delegate void OnKeyPressedAction();
    public static event OnKeyPressedAction onJump;

    public delegate void OnKeyRelease();
    public static event OnKeyRelease onKeyRelease;

    public delegate void OnMoveAction(bool isMovingRight);
    public static event OnMoveAction onMove;
    
    public delegate void OnThrowAction();
    public static event OnThrowAction onThrow;


    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            onJump?.Invoke();
        }
        
        if (Input.GetKeyUp(KeyCode.W))
        {
            onThrow?.Invoke();
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
        {
            bool isMovingRight = Input.GetKey(KeyCode.D);
            onMove?.Invoke(isMovingRight);

            //Debug.Log("Key Move pressed");
        }

        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A))
        {
            onKeyRelease?.Invoke();
        }

    }
}
