using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Ball : MonoBehaviour
{
    // Start is called before the first frame update


    private bool ballCathed = false;
    private bool playerJumping = false;

    private Animator animator;
    private Transform target;

    private Rigidbody rb;

    public float throwingForce = 10f;
    public float BounceForce = 1f;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        //rb.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (ballCathed && !playerJumping)
        {
            setAnimation("drible", true);
        }

    }

    private void throwBallProjectile()
    {
        rb.isKinematic = false;

        // Unparent the ball
        transform.SetParent(null);

        // Calculate the direction to the target
        Vector3 targetPosition = target.position;
        Vector3 currentPosition = transform.position;

        // Calculate the relative position to the target, including Y-axis
        Vector3 direction = targetPosition - currentPosition;

        // Calculate the velocity needed to reach the target
        float initialVelocity = Mathf.Sqrt(throwingForce * Physics.gravity.magnitude / (Mathf.Sin(2 * Mathf.Deg2Rad * Vector3.Angle(direction, Vector3.up))));

        // velocity To reach the target in the direction
        rb.velocity = direction.normalized * initialVelocity;

        // Enable the animator to allow dribbling
        animator.enabled = true;
    }


    public void throwBall(Transform targeted)
    {
        //if (ballCathed && controlThrowingDirection())
        if (ballCathed )
        {
            target = targeted;

            setAnimation("drible", false);

            ballCathed = false;

            throwBallProjectile();

        }
            playerJumping = false;
        Debug.Log("ball throw");

    }

    public void ballCatch()
    {
        ballCathed = true;
        rb.isKinematic = true;
    }

    public void ballJump()
    {
        //Debug.Log("Inside ball Jump");

        playerJumping = true;

        if (ballCathed)
        {
            animator.enabled = false;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("ground")) // Adjust the tag as needed
        {
            // Apply some bounce force when colliding with the ground
            Vector3 bounceForce = Vector3.up * BounceForce;

            rb.AddForce(bounceForce, ForceMode.Impulse);

        }
    }

    private void setAnimation(string animationName, bool animationState)
    {
        animator.SetBool(animationName, animationState);
    }

}
