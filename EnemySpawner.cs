using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.CompilerServices;

public class EnemySpawner : MonoBehaviour
{
    // instance vars
    [SerializeField] private List<GameObject> enemyTypes;
    public bool playerAlive;

    private void Start()
    {
    }

    public void StartGame()
    {
        playerAlive = true;
        StartCoroutine(StartWaitTime());
    }

    void Update()
    {
        
    }

    private void SpawnEnemy(int i) {
        float screenSide;
        float orientation;
        int speedDir;
        // spawn left side of screen
        if (Random.Range(0, 2) == 1) {
            screenSide = -10.5f;
            orientation = 180f;
            speedDir = 1;
        }
        // spawn right side of screen
        else {
            screenSide = 10.5f;
            orientation = -360f;
            speedDir = -1;
        }
        Vector3 spawnPos = new Vector3(screenSide, Random.Range(-1.8f, 3.8f), 0f);
        GameObject newEnemy = Instantiate(enemyTypes[i], spawnPos, Quaternion.identity);
        newEnemy.transform.eulerAngles = new Vector3(0f, orientation, 0f);
        newEnemy.GetComponent<Enemy>().Init(speedDir);
    }

    private IEnumerator StartWaitTime() {
        yield return new WaitForSeconds(5);
        StartCoroutine(SpawnRoutine());
    }
    private IEnumerator SpawnRoutine() {
        while (playerAlive)
        {
            int i = Random.Range(0, enemyTypes.Count);
            SpawnEnemy(i);
            int inbetweenSeconds = Random.Range(5, 16);
            yield return new WaitForSeconds(inbetweenSeconds);
        }
    }

}
