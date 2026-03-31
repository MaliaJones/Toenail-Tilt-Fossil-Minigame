using UnityEngine;

public class OceanFloor : MonoBehaviour
{
    // instance vars
    [SerializeField] GameObject player;
    private PlayerControlls playerControlls;
    private Transform playerTransform;

    private bool playerJustDied;
    private bool doCollisionExtraCheck;


    void Start()
    {
        playerControlls = player.GetComponent<PlayerControlls>();
        playerTransform = player.transform;
        ResetVars();
    }

    public void ResetVars() { 
        playerJustDied = false;
        doCollisionExtraCheck = false;
    }

    void Update()
    {
        // continual check for roll into upsidedown position
        if (doCollisionExtraCheck)
        {
            float playerRotation = playerTransform.eulerAngles.z;
            if (playerRotation < 205 && playerRotation > 140)
            {
                playerJustDied = true;
                doCollisionExtraCheck = false;
                playerControlls.KillPlayer();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if first landing is upsidedown position
        if (collision.gameObject.tag == "Player" && playerTransform.eulerAngles.z < 205 && playerTransform.eulerAngles.z > 140 && !playerJustDied)
        {
            playerJustDied = true;
            playerControlls.KillPlayer();
        }
        // extra check: if first landing okay, but rolls into upsiedown position
        else if (collision.gameObject.tag == "Player" && !playerJustDied) {
            doCollisionExtraCheck = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        doCollisionExtraCheck = false;
    }
}
