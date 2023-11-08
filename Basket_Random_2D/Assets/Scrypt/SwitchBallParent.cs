using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBallParent : MonoBehaviour
{

    private Player currentPlayer = null;
    private Player previousPlayer = null;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }



    public void switchParent(Player player)
    {
        currentPlayer = player;
        if (previousPlayer == null)
        {
            previousPlayer = currentPlayer;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentPlayer != null && other.gameObject.layer == LayerMask.NameToLayer("player"))
        {
            Debug.Log("Previous player name " + previousPlayer.gameObject.name);
            Debug.Log("Current player name " + currentPlayer.gameObject.name);
            if (previousPlayer != currentPlayer)
            {
                Debug.Log("It is another player");
                previousPlayer.anotherPlayerGetBall();

                previousPlayer = currentPlayer;
            }
        }
    }
}
