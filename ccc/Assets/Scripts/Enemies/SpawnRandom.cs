using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnRandom : MonoBehaviour
{
    // Start is called before the first frame update
    // public GameObject trashPrefab;
    public GameObject[] pollutionPrefabs;
    //public Game mainCamera;

    public float spawnTime = 8.0f;
    private Vector2 screenBounds;
    private Vector2 negativeScreenBounds;
    private float halfHeight;
    private float halfWidth;
    private Camera cameraMain;
    
    public int MaxEnemiesInField = 10;
    private int currentNumberOfEnemies = 0;

    void Start()
    {
        cameraMain = Camera.main;
        halfHeight = cameraMain.orthographicSize;
        halfWidth = cameraMain.aspect * halfHeight; //We  need the sizes in world heights/widths not in pixels that is the value given by Screen.width/height
        screenBounds = new Vector2(cameraMain.transform.position.x + halfWidth, cameraMain.transform.position.y + halfHeight);
        StartCoroutine(Generate());
    }

    private void Update()
    {
        var enemies = FindObjectsOfType<Enemy>();
        currentNumberOfEnemies = enemies.Length;
        UpdateMaxEnemiesBasedOnPlayerScoreScore();
    }

    void UpdateMaxEnemiesBasedOnPlayerScoreScore()
    {
        int currentScore = PlayerBehaviour.UniquePlayer.Score;
        MaxEnemiesInField = 10 + (currentScore / 500);
    }

    private void SpawnEnemy()
    {
        Debug.Log(cameraMain.transform.position);
        screenBounds = new Vector2(cameraMain.transform.position.x + halfWidth, cameraMain.transform.position.y + halfHeight);
        negativeScreenBounds = new Vector2(cameraMain.transform.position.x - halfWidth, cameraMain.transform.position.y - halfHeight);
        //Debug.Log("hola");
        //float enemyChosen = Random.Range(0.0f, 2f);
        // GameObject enemy = enemyChosen > 1f ? Instantiate(trashPrefab) : Instantiate(pollutionPrefabs[Random.Range(0, pollutionPrefabs.Length-1)]);
        GameObject enemy = Instantiate(pollutionPrefabs[Random.Range(0, pollutionPrefabs.Length - 1)]);

        float positionToSpawn = Random.Range(0.0f, 2.0f);
        if(positionToSpawn<0.5f)
        {
            enemy.transform.position = new Vector3(screenBounds.x, Random.Range(cameraMain.transform.position.y - halfHeight, screenBounds.y), 2f);
        }
        else if (positionToSpawn >= 0.5f && positionToSpawn < 1.0f)
        {
            enemy.transform.position = new Vector3(cameraMain.transform.position.x - halfWidth, Random.Range(cameraMain.transform.position.y - halfHeight, screenBounds.y), 2f);
        }
        else if (positionToSpawn >= 1.0f && positionToSpawn < 1.5f)
        {
            enemy.transform.position = new Vector3(Random.Range(cameraMain.transform.position.x - halfWidth, screenBounds.x), cameraMain.transform.position.y - halfHeight, 2f);
        }
        else if (positionToSpawn >= 1.5f && positionToSpawn <= 2.0f)
        {
            enemy.transform.position = new Vector3(Random.Range(cameraMain.transform.position.x - halfWidth, screenBounds.x), screenBounds.y, 2f);
        }

        
       // enemy.transform.position = new Vector2(Random.Range(cameraMain.transform.position.x - halfWidth, screenBounds.x), Random.Range(cameraMain.transform.position.y - halfHeight, screenBounds.y));
    }

    IEnumerator Generate()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnTime);
            if (currentNumberOfEnemies < MaxEnemiesInField) SpawnEnemy();
        }
    }
}
