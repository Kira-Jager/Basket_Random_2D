using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;

    public bool isJumping = false;
    public bool isMoving = false;
    public bool ballCathed = false;

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
        if (ballCathed && !isJumping)
        {
            setAnimation("drible", true);
        }



        if (ballCathed && isJumping)
        {
            //Invoke("disablePlayerKinematic", .3f);


            //{disablePlayerKinematic();
            //Invoke("activatePlayerKinematic",.2f);}

        }

        if (isMoving)
        {
            setAnimation("run", true);
        }
    }

    private void OnEnable()
    {
        InputController.onJump += jump;
        InputController.onMove += move;
        InputController.onKeyRelease += stopRunning;
        InputController.onThrow += playerThrowBall;


        Ball.ballCatched += playerGetBall;
    }

    private void OnDisable()
    {
        InputController.onJump -= jump;
        InputController.onMove -= move;
        InputController.onKeyRelease -= stopRunning;
        InputController.onThrow -= playerThrowBall;


        Ball.ballCatched -= playerGetBall;

    }

    private void playerThrowBall()
    {
        ballCathed = false;
        disableBoxCollider();


        Invoke("activateBoxCollider",.1f);
        //Debug.Log("Ball throw");

        //{Invoke("disablePlayerKinematic", .3f);}


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

    private void playerGetBall()
    {
        ballCathed = true;
        Debug.Log("Player get ball");
        //activatePlayerKinematic();

    }

    private void activatePlayerKinematic()
    {

        BoxCollider boxCollider = transform.GetChild(0).GetComponent<BoxCollider>();

        boxCollider.enabled = false;

        rb.isKinematic = true;

        Debug.Log("Player kinematic activate");

    }

    private void disablePlayerKinematic()
    {

        BoxCollider boxCollider = transform.GetChild(0).GetComponent<BoxCollider>();
        boxCollider.enabled = true;
        Debug.Log("Player kinematic disable");


        rb.isKinematic = false;
    }

    private void jump()
    {
        if (!isJumping)
        {


            //disablePlayerKinematic();

            isJumping = true;

            Debug.Log("called in jump");
            rb.AddForce(Vector3.up * jumpForce);

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
        //Debug.Log("stopRunning");
        isMoving = false;
        setAnimation("run", false);
    }
}
