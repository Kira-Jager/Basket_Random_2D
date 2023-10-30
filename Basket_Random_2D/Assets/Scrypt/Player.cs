using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;

    public bool isJumping = false;
    public bool isMoving = false;

    private Vector3 newPosition = Vector3.zero;
    private Animator animator;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving)
        {
            setAnimation("drible", true);
        }
        else
        {
            setAnimation("drible", false);
        }
    }

    private void OnEnable()
    {
        InputController.onJump += jump;
        InputController.onMove += move;
        InputController.onKeyRelease += stopRunning;
    }

    private void OnDisable()
    {
        InputController.onJump -= jump;
        InputController.onMove -= move;
        InputController.onKeyRelease -= stopRunning;

    }

    private void jump()
    {
        if (!isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce);
            Debug.Log("called");
            isJumping = true;

            setAnimation("run", false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("ground"))
        {

            isJumping = false;


        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("ground"))
        {
            setAnimation("drible", false);
        }


    }

    private void move(bool isMovingRight)
    {
        isMoving = true;
        setAnimation("run", true);

        float direction = isMovingRight ? 1 : -1;

        Vector3 newPosition = Vector3.right * speed * direction;

        transform.position += newPosition;

        // Flip the character if moving right (true) or left (false)
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.y = direction > 0 ? 90 : -90;
        transform.rotation = Quaternion.Euler(rotation);

    }


    private void setAnimation(string animationName, bool animationState)
    {
        animator.SetBool(animationName, animationState);
    }

    private void stopRunning()
    {
        Debug.Log("stopRunning");
        isMoving = false;
        setAnimation("run", false);
    }
}
