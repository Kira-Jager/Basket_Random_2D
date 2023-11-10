using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class limitBox : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("ball"))
        {
            Debug.Log("ball outside");
           StartCoroutine( ResetBallPosition(other));
        }
    }

    private IEnumerator ResetBallPosition(Collider other)
    {

        yield return new WaitForSeconds(2f);
        other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        other.transform.position = new Vector3(0, 5, 0);
    }

}
