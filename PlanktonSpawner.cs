using System.Collections;
using UnityEngine;

public class PlanktonSpawner : MonoBehaviour
{
    [SerializeField] GameObject food;
    [SerializeField] Vector2 spawnRate;
    public bool playerAlive;

    private void Start()
    {
        
    }

    public void StartGame()
    {
        playerAlive = true;
        StartCoroutine(SpawningRoutine());
    }

    void Update()
    {
        
    }

    private IEnumerator SpawningRoutine() {
        while (playerAlive) {
            yield return new WaitForSeconds(Random.Range(spawnRate.x, spawnRate.y));
            int randomSide = Random.Range(-1, 1); if (randomSide == 0) { randomSide = 1; }
            Vector3 spawnPos = new Vector3(10*randomSide, Random.Range(-3, 3), 0f);
            Instantiate(food, spawnPos, Quaternion.Euler(0f, 0f, Random.Range(0, 360)));
        }
        // kill the remaining plankton on screen
        Plankton[] remainingPlankton = Object.FindObjectsByType<Plankton>(FindObjectsSortMode.None);
        foreach (Plankton p in remainingPlankton) {
            Destroy(p.gameObject);
        }

    }
}
