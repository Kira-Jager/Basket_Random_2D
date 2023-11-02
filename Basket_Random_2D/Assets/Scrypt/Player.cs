using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    public Transform target;


    public delegate void OnJumpKeyAction();
    public event OnJumpKeyAction onJumpKeyPressed;

    public delegate void onJumpReleaseAction(Transform targeted);
    public event onJumpReleaseAction onThrowKey;

    public delegate void OnBallCatched();
    public event OnBallCatched playerCatchball;
    
    public delegate void onePlayerThrowBall();
    public static event OnBallCatched onePlayerThrow;

    private bool isJumping = false;
    private bool ballCathed = false;

    private Ball ballComponent = null;

    private InputController controller;

    private Animator animator;

    private Rigidbody rb;
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

        //inate Event
        onePlayerThrow += actionOnDoublePlayer;
    }

    private void OnDisable()
    {
        controller.onJump -= jump;
        controller.onMove -= move;
        controller.onKeyRelease -= stopRunning;
        controller.onThrow -= playerThrowBall;

        //inate Event
        onePlayerThrow -= actionOnDoublePlayer;

    }

    private void actionOnDoublePlayer()
    {
        ballCathed = false;
        Debug.Log("DOuble player action");
    }

    private bool controlThrowingDirection()
    {
        Vector3 playerDirection = transform.forward;
        Vector3 targetDirection = (target.position - transform.position).normalized;

        bool isFacingTarget = false;

        float dotProduct = Vector3.Dot(playerDirection, targetDirection);

        if (dotProduct > 0)
        {
            isFacingTarget = true;
        }
        else
        {
            isFacingTarget = false;
        }

        return isFacingTarget;
    }


    private void playerThrowBall()
    {
        if (ballCathed)
        {
            disableBoxCollider();

            Invoke("activateBoxCollider", .1f);

            onThrowKey?.Invoke(target);

            onePlayerThrow?.Invoke();

            if (ballComponent != null && target != null)
            {
                //Debug.Log(ballComponent.gameObject.name + " in throw ball");
                ballComponent.throwBall(target);

                ballComponent = null;
            }
        }
        ballCathed = false;

        


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

            if (ballComponent != null)
            {
                //Debug.Log(ballComponent.gameObject.name + " in jump");

                ballComponent.ballJump();
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
       
         playerGetBall(collision);
    }

    private void playerGetBall(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("ball") && !ballCathed)
        {
            ballComponent = collision.gameObject.GetComponent<Ball>(); // Update lastCollision only if it's a valid ball collision

            ballCathed = true;

            playerCatchball?.Invoke();

            ballComponent.ballCatch();

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

    }

    private void move(bool isMovingRight)
    {
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
        setAnimation("run", false);
    }
}
