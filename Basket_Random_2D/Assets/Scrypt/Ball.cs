using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private bool ballCathed = false;
    private bool playerJumping = false;

    private Animator animator;
    private Transform target;

    private Rigidbody rb;
    private SphereCollider sphereCollider;


    //public float throwingForce = 10f;
    public float BounceForce = 1f;

    public float _Angle;

    private Player currentPlayer = null;
    private Player previousPlayer = null;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();

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

    /*    private float QuadraticEquation(float a, float b, float c, float sign)
        {
            return (-b + sign * Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);

        }

        private void CalculatePathWithHeight(Vector3 targetPos, Vector3 initialPos, float h, out float v0, out float angle, out float time)
        {
            Vector3 relativeTargetPos = targetPos - initialPos;
            float xt = relativeTargetPos.x;
            float yt = relativeTargetPos.y;
            float g = -Physics.gravity.y;

            float b = Mathf.Sqrt(2 * g * h);
            float a = (-0.5f * g);
            float c = -yt;

            float tplus = QuadraticEquation(a, b, c, 1);
            float tmin = QuadraticEquation(a, b, c, -1);
            time = tplus > tmin ? tplus : tmin;

            angle = Mathf.Atan(b * time / xt);
            v0 = b / Mathf.Sin(angle);
        }

        private void throwBallProjectile()
        {
            rb.isKinematic = false;

            Vector3 initialPosition = transform.position;

            transform.SetParent(null);
            float height = initialPosition.y + initialPosition.magnitude / 2f;

            float v0;
            float time;
            float angle;

            CalculatePathWithHeight(target.position, initialPosition, height, out v0, out angle, out time);

            StartCoroutine(projectileMotion(v0, angle, time, initialPosition));
        }


        private IEnumerator projectileMotion(float v0, float angle, float time, Vector3 initialPosition)
        {
            while (time < 100)
            {
                float x = initialPosition.x + v0 * time * Mathf.Cos(angle);
                float y = initialPosition.y + v0 * time * Mathf.Sin(angle) - (1f / 2f) * -Physics.gravity.y * Mathf.Pow(time, 2);

                transform.position = new Vector3(x, y, 0);
                time += Time.deltaTime;
                yield return null;
            }
        }*/

    private void throwBallProjectile()
    {
        sphereCollider.enabled = true;
        rb.isKinematic = false;

        // Unparent the ball
        transform.SetParent(null);

        // Calculate the direction to the target
        Vector3 targetPosition = target.position;
        Vector3 currentPosition = transform.position;

        // Calculate the relative position to the target, including Y-axis
        Vector3 direction = (targetPosition - currentPosition);

        float angle = Mathf.Atan2(direction.y, direction.x);
        float g = 10f; //Physics.gravity.magnitude ; // acceleration due to gravity
        float R = direction.magnitude; // distance to the target
        float initialVelocity = Mathf.Sqrt((R * g) / Mathf.Abs(Mathf.Sin(2 * angle)));


        float initialVelocityX = initialVelocity * Mathf.Cos(angle);
        float initialVelocityY = initialVelocity * Mathf.Sin(angle);

        Debug.Log("Angle: " + angle);
        Debug.Log("R: " + R);
        Debug.Log("g: " + g);
        Debug.Log("Sin(2*angle): " + Mathf.Sin(2 * angle));

        rb.AddForce(new Vector3(initialVelocityX, initialVelocityY, 0), ForceMode.VelocityChange);
        
        // Enable the animator to allow dribbling
        Invoke("enableAnimator", .5f);
    }

    private void enableAnimator()
    {
        animator.enabled = true;
    }

    public void throwBall(Transform targeted)
    {
            target = targeted;

            setAnimation("drible", false);

            ballCathed = false;
            rb.velocity = Vector3.zero;

            throwBallProjectile();


        playerJumping = false;
        Debug.Log("ball throw");

    }

    public void ballCatch(Player player)
    {
        ballCathed = true;
        rb.isKinematic = true;
        sphereCollider.enabled = false;

        currentPlayer = player;
        if (previousPlayer == null)
        {
            previousPlayer = currentPlayer;
        }
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

        if (currentPlayer != null)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log(" One player cath me; inside ball");

                if (previousPlayer != currentPlayer)
                {
                    previousPlayer.anotherPlayerGetBall();
                    Debug.Log("It is another player");

                    previousPlayer = currentPlayer;
                }
            }

        }

    }

    private void setAnimation(string animationName, bool animationState)
    {
        animator.SetBool(animationName, animationState);
    }

}
