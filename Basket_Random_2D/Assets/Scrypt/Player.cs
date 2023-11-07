using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    public Transform target;
    
    public delegate void onePlayerThrowBall();
    public static event onePlayerThrowBall onePlayerThrow;
    
    public delegate void playerGetBallAction();
    public static event playerGetBallAction playerCatchball;

    private bool isJumping = false;
    private bool ballCathed = false;
    
    private float movingDirection = 0;

    private GameObject ballComponent = null;

    private InputController controller;

    private AiController aiController;

    private Animator animator;

    private Rigidbody rb;
    // Start is called before the first frame update
    private void Awake()
    {
        controller = GetComponent<InputController>();
        if(aiController != null)
        {
            aiController = GetComponent<AiController>();
        }
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
        if(ballCathed == false)
        {
            setAnimation("drible", false);

        }

    }

    private void OnEnable()
    {
        controller.onJump += jump;
        controller.onMove += move;
        controller.OnMoveKeyRelease += stopRunning;
        controller.onThrow += playerThrowBall;

        //inate Event
        onePlayerThrow += throwActionOnDoublePlayer;

    }

    private void OnDisable()
    {
        controller.onJump -= jump;
        controller.onMove -= move;
        controller.OnMoveKeyRelease -= stopRunning;
        controller.onThrow -= playerThrowBall;

        //inate Event
        onePlayerThrow -= throwActionOnDoublePlayer;

    }


    public bool getBallCath()
    {
        return ballCathed;
    }

    public void anotherPlayerGetBall()
    {
        ballCathed = false;

        Debug.Log(" Another Player touch the ball");

    }
    private void throwActionOnDoublePlayer()
    {
        ballCathed = false;
        //Debug.Log("DOuble player action");
    }

    private bool controlThrowingDirection()
    {
        Vector3 playerDirection = transform.forward;
        Vector3 targetDirection = (target.position - transform.position).normalized;


        float dotProduct = Vector3.Dot(playerDirection, targetDirection);

        if (dotProduct > 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }


    public void playerThrowBall()
    {

        Debug.Log("Ball catched state in player throw ball" + ballCathed);

        if (ballCathed && controlThrowingDirection())
        //if ( controlThrowingDirection())
        {
            disableBoxCollider();

            Invoke("activateBoxCollider", .1f);

            //onThrowKey?.Invoke(target);

            onePlayerThrow?.Invoke();

            if (ballComponent != null && target != null)
            {
                //Debug.Log(ballComponent.gameObject.name + " in throw ball");
                ballComponent.GetComponent<Ball>().throwBall(target);

                //ballComponent = null;
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



    public void jump()
    {
        if (!isJumping)
        {
            if (controlThrowingDirection() == false && ballCathed)
            {
                Vector3 rotation = transform.rotation.eulerAngles;
                rotation.y *= -1;
                movingDirection *= -1;
                transform.rotation = Quaternion.Euler(rotation);
            }

            isJumping = true;

            //onJumpKeyPressed?.Invoke();

            if (ballComponent != null && ballCathed)
            {
                //Debug.Log(ballComponent.gameObject.name + " in jump");

                Vector3 resetBallPosition = new Vector3(transform.position.x + .5f * movingDirection, transform.position.y + 1f, transform.position.z);
                
                ballComponent.GetComponent<Ball>().ballJump();


                ballComponent.transform.position = resetBallPosition;
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
        Vector3 resetBallPosition = new Vector3(transform.position.x + .5f * movingDirection, transform.position.y, transform.position.z);

        if (collision.gameObject.CompareTag("ball") && !ballCathed)
        {
            ballComponent = collision.gameObject; // Update lastCollision only if it's a valid ball collision

            collision.gameObject.transform.SetParent(transform);

            collision.gameObject.transform.position = resetBallPosition;

            ballCathed = true;

            //playerCatchball?.Invoke();


            ballComponent.GetComponent<Ball>().ballCatch(this);

            Debug.Log("Player get ball");

            //Debug.Log(lastCollision.gameObject.name);

        }

    }


    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("ground"))
        {
            setAnimation("drible", false);
        }

    }

    public void move(bool isMovingRight)
    {
        if (!isJumping)
        {
            setAnimation("run", true);

            movingDirection = isMovingRight ? 1 : -1;

            // Calculate the movement direction
            Vector3 moveDirection = Vector3.right * movingDirection;

            // Apply a force to move the character

            //rb.AddForce( moveDirection * speed, ForceMode.Force);

            rb.MovePosition(rb.position + moveDirection * speed * Time.deltaTime);

            Vector3 rotation = transform.rotation.eulerAngles;
            rotation.y = movingDirection > 0 ? 90 : -90;
            transform.rotation = Quaternion.Euler(rotation);
        }
    }


    private void setAnimation(string animationName, bool animationState)
    {
        animator.SetBool(animationName, animationState);
    }

    public void stopRunning()
    {
        //Debug.Log("stopRunning");
        setAnimation("run", false);
    }
}
