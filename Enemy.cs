using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public float speed;
    private Vector3 moveSpeed;
    [SerializeField] Vector2 speedRange;
    public float deathX = 10.0f;

    private bool hasAppliedForce = false;

    // must call when instantiating prefab
    public void Init(int dir)
    {
        speed = Random.Range(speedRange.x, speedRange.y)*dir;
        moveSpeed = new Vector3(speed, 0f, 0f);
        deathX *= dir;
        StartCoroutine(move());
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (!hasAppliedForce) {
            CheckPlayerNear();
        }

    }

    private IEnumerator move() {
        while (deathX>0?transform.position.x < deathX:transform.position.x > deathX) {
            transform.position += moveSpeed;
            yield return new WaitForSeconds(0.02f);
        }
        Destroy(gameObject);
    }

    private void CheckPlayerNear() {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 4);
        foreach (Collider2D collider in hits) {
            if (collider.gameObject.name == "Player") {
                Rigidbody2D rb2d = collider.attachedRigidbody;
                float yDiff = collider.gameObject.transform.position.y- transform.position.y;
                Vector2 gustDir = new Vector2(Mathf.Sign(speed), Mathf.Sign(yDiff));
                Vector2 force = new Vector2(gustDir.x*36*Mathf.Abs(speed), gustDir.y*27*Mathf.Abs(speed));
                rb2d.AddForce(force);
                rb2d.AddTorque(gustDir.x);
            }
        }
    }
}
