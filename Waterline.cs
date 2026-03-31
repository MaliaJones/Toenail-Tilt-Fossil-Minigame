using UnityEngine;
using System.Collections;

public class Waterline : MonoBehaviour
{
    [SerializeField] GameObject splashPart;
    private AudioSource audioSrc;


    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }
    void Update()
    {
        
    }

    // TRIGGER
    private void OnTriggerEnter2D(Collider2D collision) {
        float spawnYPos = this.gameObject.transform.position.y;
        if (collision.gameObject.transform.position.x <= -6.5f || collision.gameObject.transform.position.x >= -0.6f) {
            spawnYPos += 0.5f;
        }
        Vector3 spawnPos = new Vector3(collision.gameObject.transform.position.x, spawnYPos, 0f);
        GameObject thisSplash = Instantiate(splashPart, spawnPos, Quaternion.identity);
        audioSrc.time = Random.Range(0, 15);
        audioSrc.Play();
        StartCoroutine(DestroyAfterAnimation(thisSplash, audioSrc));
    }
/*    private void OnTriggerExit2D(Collider2D collision) {
        GameObject thisSplash = Instantiate(splashPart, collision.gameObject.transform.position, Quaternion.identity);
        StartCoroutine(DestroyAfterAnimation(thisSplash));
    }*/
    private IEnumerator DestroyAfterAnimation(GameObject go, AudioSource aud) {
        yield return new WaitForSeconds(0.5f);
        Destroy(go);
        aud.Stop();
    }



}
