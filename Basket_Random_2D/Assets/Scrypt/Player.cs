using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;

    public bool isJumping = false;
    public bool isMoving = false;
    public bool ballCathed = false;

    private Collision lastCollision = null;

    public Transform target;

    private InputController controller;

    private Vector3 newPosition = Vector3.zero;
    private Animator animator;

    public delegate void OnJumpKeyAction();
    public event OnJumpKeyAction onJumpKeyPressed;

    public delegate void onJumpReleaseAction(Transform targeted);
    public event onJumpReleaseAction onThrowKey;

    public delegate void OnBallCatched();
    public event OnBallCatched playerCatchball;
    
    public delegate void onePlayerThrowBall();
    public static event OnBallCatched onePlayerThrow;

    Rigidbody rb;
    // Start is called before the first frame update
    private void Awake()
    {
        controller = GetComponent<InputController>();

    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ballCathed && !isJumping)
        {
            setAnimation("drible", true);
        }

    }

    private void OnEnable()
    {
        controller.onJump += jump;
        controller.onMove += move;
        controller.onKeyRelease += stopRunning;
        controller.onThrow += playerThrowBall;

    }

    private void OnDisable()
    {
        controller.onJump -= jump;
        controller.onMove -= move;
        controller.onKeyRelease -= stopRunning;
        controller.onThrow -= playerThrowBall;


    }

    private void playerThrowBall()
    {

        if (ballCathed)
        {
            disableBoxCollider();

            Invoke("activateBoxCollider", .1f);

            onThrowKey?.Invoke(target);

            onePlayerThrow?.Invoke();
        }
        ballCathed = false;

        if(lastCollision != null && target != null)
        {
            Debug.Log(lastCollision.gameObject.name +" in throw ball");
            lastCollision.gameObject.GetComponent<Ball>().throwBall(target);
        }


    }

    private void disableBoxCollider()
    {
        BoxCollider boxCollider = transform.GetChild(0).GetComponent<BoxCollider>();

        boxCollider.enabled = false;
    }
    private void activateBoxCollider()
    {
        BoxCollider boxCollider = transform.GetChild(0).GetComponent<BoxCollider>();

        boxCollider.enabled = true;
    }



    private void jump()
    {
        if (!isJumping)
        {

            isJumping = true;

            onJumpKeyPressed?.Invoke();

            if (lastCollision != null)
            {
                Debug.Log(lastCollision.gameObject.name + " in jump");

                lastCollision.gameObject.GetComponent<Ball>().ballJump();
            }

            Debug.Log("called in jump");
            rb.AddForce(Vector3.up * jumpForce  , ForceMode.Impulse);

            //rb.velocity = Vector3.up * jumpForce * Time.deltaTime;
            setAnimation("run", false);
            setAnimation("drible", false);

           

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("ground"))
        {
            isJumping = false;
        }


        //lastcollision variable is not working correctly 
        if (collision.gameObject.CompareTag("ball") && !ballCathed)
        {
            lastCollision = collision; // Update lastCollision only if it's a valid ball collision
            playerGetBall(collision);
        }
    }

    private void playerGetBall(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("ball") && !ballCathed)
        {
            lastCollision = collision;

            ballCathed = true;

            //rb.isKinematic = true;

            playerCatchball?.Invoke();

            collision.gameObject.GetComponent<Ball>().ballCatch();

            Debug.Log("Player get ball");


            //Debug.Log(lastCollision.gameObject.name);
            collision.gameObject.transform.SetParent(transform);


        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("ground"))
        {
            setAnimation("drible", false);
        }

     /*   if (collision.gameObject.CompareTag("ball"))
        {
            ballCathed = false;
        }*/
    }

    private void move(bool isMovingRight)
    {
        isMoving = true;

        if (!isJumping)
        {
            setAnimation("run", true);
        }
        //setAnimation("run", true);

        float direction = isMovingRight ? 1 : -1;

        Vector3 newPosition = Vector3.right * speed * direction ;

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
        //Debug.Log("stopRunning");
        isMoving = false;
        setAnimation("run", false);
    }
}
