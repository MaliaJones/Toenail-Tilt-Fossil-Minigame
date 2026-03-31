using UnityEngine;

public class Plankton : MonoBehaviour
{
    string spawnDir;
    Vector3 floatDirection;

    void Start() {
        // initalize spawn direction
        if (transform.position.x < -8f) {
            spawnDir = "left";
            // initalize movement angle
            if (transform.position.y < 0) {
                floatDirection = new Vector3(1f, Random.Range(0f, 1f), 0f);
            }
            else {
                floatDirection = new Vector3(1f, Random.Range(-1f, 0f), 0f);
            }
            
        }
        else {
            spawnDir = "right";
            // initalize movement angle
            if (transform.position.y < 0) {
                floatDirection = new Vector3(-1f, Random.Range(0f, 1f), 0f);
            }
            else {
                floatDirection = new Vector3(-1f, Random.Range(-1f, 0f), 0f);
            }
        }
    }

    void Update() {
        // float untill off screen
        transform.position += floatDirection * 0.5f * Time.deltaTime;
        if ((spawnDir == "left" && transform.position.x > 10f) ||
            (spawnDir == "right" && transform.position.x < -10f)) {
            Destroy(gameObject);
        }
        // correct floatDirection if too close to bottom or top
        if (transform.position.y > 2.9f) {
            floatDirection.y -= 0.33f * Time.deltaTime;
        }
        if (transform.position.y < -2.8f) {
            floatDirection.y += 0.33f * Time.deltaTime;
        }
        // continually spin
        transform.Rotate(0f, 0f, 90f*Time.deltaTime);

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player") { 
            PlayerControlls playerScript = collider.gameObject.GetComponent<PlayerControlls>();
            playerScript.IncreaseScore();
            Destroy(gameObject);
        }
    }
}
