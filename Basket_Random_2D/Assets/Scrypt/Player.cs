using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform target;

    private float speed = 5f;
    private float jumpForce = 5f;

    public delegate void onePlayerThrowBall();
    public static event onePlayerThrowBall onePlayerThrow;
    
    public delegate void playerGetBallAction();
    public static event playerGetBallAction playerCatchball;

    private bool isJumping = false;
    private bool ballCathed = false;
    
    private float movingDirection = 0;
    private float distance = 0;
    private float distanceValue = 3f;
    private float minValue = 5f;
    private float maxValue = 9f;

    private GameObject ballComponent = null;
    private GameManager gameManager = null;

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
        gameManager = FindObjectOfType<GameManager>();
        Initialized();

        InvokeRepeating("calculateTargetHeight", 0f, 1.5f);
    }

    private void Initialized()
    {
        speed = gameManager.speed;
        jumpForce = gameManager.jumpForce;

        distanceValue = gameManager.distanceValue;
        minValue = gameManager.minValue;
        maxValue = gameManager.maxValue;
    }

    // Update is called once per frame
    void Update()
    {
        if (ballCathed && !isJumping)
        {
            setAnimation("drible", true);
        }
        if (ballCathed == false)
        {
            setAnimation("drible", false);
        }

        //calculateTargetHeight();

    }

    private void calculateTargetHeight()
    {
        distance = Mathf.Abs(transform.position.x - target.position.x);

        if (distance <= distanceValue)
        {
            float targetMinPositionY = minValue;//target.position.y  ;
            target.position = new Vector3(target.position.x, targetMinPositionY, target.position.z);
        }
        else if (distance >= distanceValue)
        {
            float targetMaxPositionY = maxValue; // target.position.y ;
            target.position = new Vector3(target.position.x, targetMaxPositionY, target.position.z);
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

        Debug.Log(transform.gameObject.name +" Another Player touch the ball");

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

        //.Log("Ball catched state in player throw ball" + ballCathed);

        if (ballCathed && controlThrowingDirection())
        //if ( controlThrowingDirection())
        {
            disableBoxCollider();

            Invoke("activateBoxCollider", .2f);

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

            //Debug.Log("called in jump");
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

        //if (collision.gameObject.CompareTag("ball") && !ballCathed)
        if (collision.gameObject.layer == LayerMask.NameToLayer("ball") && !ballCathed)
        {
            ballComponent = collision.gameObject; // Update lastCollision only if it's a valid ball collision

            collision.gameObject.transform.SetParent(transform);

            collision.gameObject.transform.position = resetBallPosition;

            ballCathed = true;

            //playerCatchball?.Invoke();


            ballComponent.GetComponent<Ball>().ballCatch(this);
            //ballComponent.GetComponent<Ball>().ballCatch();
            //ballComponent.gameObject.transform.GetChild(0).GetComponent<SwitchBallParent>().switchParent(this);
            //ballComponent.GetComponentInChildren<SwitchBallParent>().switchParent(this);

            //Debug.Log("Player get ball");

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
