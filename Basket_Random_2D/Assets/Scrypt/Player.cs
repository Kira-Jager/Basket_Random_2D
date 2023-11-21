using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{

    public delegate void onePlayerThrowBall();
    public static event onePlayerThrowBall onePlayerThrow;

    //public delegate void playerGetBallAction();
    //public static event playerGetBallAction playerCatchball;
    
    public Transform target;
    public Transform playerHand = null;


    private GameObject ballComponent = null;
    private GameManager gameManager = null;

    private InputController controller;

    private Animator animator;

    private Rigidbody rb;

    private AudioSource dribleAudioSource = null;


    private bool isJumping = false;
    private bool ballCathed = false;
    private bool playerScore = false;
    private bool endGame = false;
    private bool makePlayerReturn = false;

    private float movingDirection = 0;

    private Vector3 playerInitialPosition;
    private Quaternion playerInitialRotation;

    // Start is called before the first frame update
    private void Awake()
    {
        controller = GetComponent<InputController>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
        ballComponent = FindObjectOfType<Ball>().gameObject;
        dribleAudioSource = GetComponent<AudioSource>();

        dribleAudioSource.enabled = false;

        playerInitialPosition = transform.position;
        playerInitialRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (ballCathed == false)
        {
            setAnimation("drible", false);
        }

        /*if (makePlayerReturn)
        {

                move(!controlThrowingDirection());
            
        }*/

    }

    private void calculateTargetHeight()
    {
        gameManager.distancePlayerTarget = distancePlayerTarget();
        if (gameManager.distancePlayerTarget <= gameManager.distanceThreshold)
        {
            float targetMinPositionY = gameManager.targetMinHeightValue; //minValue;//target.position.y  ;
            target.position = new Vector3(target.position.x, targetMinPositionY, target.position.z);
        }
        //else if (distance >= distanceValue)
        else if (gameManager.distancePlayerTarget >= gameManager.distanceThreshold)
        {
            float targetMaxPositionY = gameManager.targetMaxHeightValue;//maxValue; // target.position.y ;
            target.position = new Vector3(target.position.x, targetMaxPositionY, target.position.z);
        }
    }

    public float distancePlayerTarget()
    {
        return Mathf.Abs(transform.position.x - target.position.x);

    }

    private void OnEnable()
    {
        controller.onJump += jump;
        controller.onMove += move;
        controller.OnMoveKeyRelease += stopRunning;
        controller.onThrow += playerThrowBall;

        BasketScore.onePlayerScore += onePlayerScoreEvent;

        GameManager.onEndGame += playeronEndGameEvent;
        //inate Event
        onePlayerThrow += throwActionOnDoublePlayer;

    }

    private void OnDisable()
    {
        controller.onJump -= jump;
        controller.onMove -= move;
        controller.OnMoveKeyRelease -= stopRunning;
        controller.onThrow -= playerThrowBall;

        BasketScore.onePlayerScore -= onePlayerScoreEvent;

        GameManager.onEndGame -= playeronEndGameEvent;

        //inate Event
        onePlayerThrow -= throwActionOnDoublePlayer;

    }

    private void playeronEndGameEvent(string winnerName)
    {
        ballComponent.GetComponent<Ball>().resetBall();
        dribleAudioSource.enabled = false;

        resetPlayer();

        //ballCathed = false;
        endGame = true;
    }

    public bool getBallCath()
    {
        return ballCathed;
    }

    public void anotherPlayerGetBall()
    {
        ballCathed = false;

        Debug.Log(transform.gameObject.name + " Another Player touch the ball");

    }
    private void throwActionOnDoublePlayer()
    {
        ballCathed = false;
        //Debug.Log("DOuble player action");
    }

    public bool getThrowingDirection()
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

         if (ballCathed && getThrowingDirection())
        {
            disableBoxCollider();

            Invoke("activateBoxCollider", .2f);

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
        BoxCollider boxCollider = GetComponent<BoxCollider>();

        boxCollider.enabled = false;
    }
    private void activateBoxCollider()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();

        boxCollider.enabled = true;
    }

    public void jump()
    {
        if (!isJumping && !playerScore)
        {
            if (getThrowingDirection() == false)
            {
                //turn player to face its target
                Vector3 rotation = transform.rotation.eulerAngles;
                rotation.y *= -1;
                movingDirection *= -1;
                transform.rotation = Quaternion.Euler(rotation);
            }

            dribleAudioSource.enabled = false;


            isJumping = true;

            //Invoke("disableJumping", 2f);

            if (ballComponent != null && ballCathed)
            {
                //Debug.Log(ballComponent.gameObject.name + " in jump");

                calculateTargetHeight();

                ballComponent.GetComponent<Ball>().ballJump();

            }

            setAnimation("jumping", true);

            //Debug.Log("called in jump");
            rb.AddForce(Vector3.up * gameManager.jumpForce, ForceMode.Impulse);

            setAnimation("run", false);
            setAnimation("drible", false);


        }
    }

    private void disableJumping()
    {
        //this is for the purpose to correct a bug of player being stack on the ball
        isJumping = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("ground"))
        {
            isJumping = false;
            setAnimation("jumping", false);
        }

        playerGetBall(collision);
    }

    private void playerGetBall(Collision collision)
    {

        //if (collision.gameObject.CompareTag("ball") && !ballCathed)
        if (collision.gameObject.layer == LayerMask.NameToLayer("ball") && !ballCathed && !playerScore && !isJumping)
        {
            ballComponent = collision.gameObject; // Update lastCollision only if it's a valid ball collision

            //Debug.Log("is jumping " + gameObject.name + isJumping);

            ballComponent.GetComponent<Ball>().ballCatch(this);

            ballCathed = true;

            setAnimation("drible", true);

            //audio
            dribleAudioSource.enabled = true;
            dribleAudioSource.Play();

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
        if (!isJumping && !playerScore && !endGame)
        {

            movingDirection = isMovingRight ? 1 : -1;

            // Calculate the new position
            Vector3 newPosition = Vector3.right * gameManager.speed * movingDirection;
            Vector3 nextPosition = transform.position + newPosition;

            // Check if the next position is within the limit box
            if (IsWithinLimit(nextPosition))
            {
                setAnimation("run", true);
                rb.velocity = Vector3.zero;
                transform.position = nextPosition;

                Vector3 rotation = transform.rotation.eulerAngles;
                rotation.y = movingDirection > 0 ? 90 : -90;
                transform.rotation = Quaternion.Euler(rotation);
            }
        }
    }

    private void onePlayerScoreEvent(bool state)
    {
        //Debug.Log("State on Score event" + state);
        playerScore = state;


        if (state)
        {
            Player Scorer = ballComponent.GetComponent<Ball>().getPlayerWhoScore();
            
            //ballComponent.GetComponent<Animator>().enabled = false;

            if (Scorer.gameObject.transform == this.gameObject.transform)
            {
                setAnimation("playerScore", true);
                //Debug.Log(" Scorer anim start by " + this.gameObject.name);
            }
            //if(Scorer.gameObject.transform != this.gameObject.transform)
            else{
                setAnimation("loser", true);
                //Debug.Log(" loser anim start by " + this.gameObject.name);
            }
            Invoke("resetPlayer", 2f);
        }
        
    }

    private void resetPlayer()
    {
        transform.position = playerInitialPosition;
        transform.rotation = playerInitialRotation;

        setAnimation("playerScore", false);
        setAnimation("loser", false);

        ballCathed = false;
    }
    private bool IsWithinLimit(Vector3 position)
    {
        foreach (GameObject limitBound in gameManager.limitBounds)
        {
            float distanceThreshold = 1f;
            float distance = Mathf.Abs(position.x - limitBound.transform.position.x);

            //Debug.Log("distance " + distance);

            if (distance < distanceThreshold)
            {
                return false;
            }
        }
        return true;
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
