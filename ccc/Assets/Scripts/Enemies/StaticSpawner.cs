using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class StaticSpawner : MonoBehaviour
{
    public enum SpawnerType
    {
        Sewer,
        Rack,
        Zombie
    }
    // Start is called before the first frame update
    public SpawnerType spawnerType = SpawnerType.Sewer;
    public float spawnTime = 5.0f;
    
    public RatAndThings ratPrefab;
    public RatAndThings cockroachPrefab;
    public GameObject zombie;
    
    private GameObject enemyToSpawn;
    private float timer = 0f;
    
    #region Sewer

    private GameObject[] sewers;
    private float keepOpenTime = 2f;
    private bool openSewer = false;
    
    public int MaxEnemiesInField = 10;
    private int currentNumberOfEnemies = 0;

    #endregion

    void Awake()
    {
        StartCoroutine(Generate());
        SpawnerBehavior(spawnerType);
    }

    // Update is called once per frame
    void Update()
    {
        int n = 0;
        if (spawnerType == SpawnerType.Rack || spawnerType == SpawnerType.Sewer)
        {
            n = FindObjectsOfType<RatAndThings>().Length;    
        }
        else
        {
            n = FindObjectsOfType<ZombieCatEnemy>().Length;
        }
        
        currentNumberOfEnemies = n;
        
        UpdateMaxEnemiesBasedOnPlayerScoreScore();
//        Debug.Log($"MAX ENEMIES: {MaxEnemiesInField}, CURR ENEMIES: {currentNumberOfEnemies}");
        
        if (openSewer)
        {
            timer += Time.deltaTime;
            if (timer >= keepOpenTime)
            {
                openSewer = false;
                if (sewers.Length >= 2)
                {
                    sewers[0].SetActive(true);
                    sewers[1].SetActive(false);    
                }
                timer = 0f;
            }
        }
    }

    void UpdateMaxEnemiesBasedOnPlayerScoreScore()
    {
        int currentScore = PlayerBehaviour.UniquePlayer.Score;
        MaxEnemiesInField = 2 + (currentScore / 1000);
    }

    void SpawnerBehavior(SpawnerType type)
    {
        switch (type)
        {
            case SpawnerType.Sewer:
                SewerBehavior();
                break;
            
            case SpawnerType.Rack:
                RackBehavior();
                break;
            case SpawnerType.Zombie:
                ZombieBehavior();
                break;
        }
    }

    void SewerBehavior()
    {
        int childCount = transform.childCount;
        sewers = new GameObject[childCount];
        for (int i = 0; i < childCount; i++)
            sewers[i] = transform.GetChild(i).gameObject;

        enemyToSpawn = ratPrefab.transform.gameObject;
    }

    void ZombieBehavior()
    {
        int childCount = transform.childCount;
        sewers = new GameObject[childCount];
        Debug.Log(childCount);
        for (int i = 0; i < childCount; i++)
            sewers[i] = transform.GetChild(i).gameObject;

        enemyToSpawn = zombie;
        enemyToSpawn.transform.position = this.transform.position;
    }

    void RackBehavior()
    {
        enemyToSpawn = cockroachPrefab.transform.gameObject;
    }

    private void SpawnEnemy()
    {
        spawnTime = Random.Range(6f, 10f);
        
        switch (spawnerType)
        {
            case SpawnerType.Sewer:
                SpawnRat();
                break;
            
            case SpawnerType.Rack:
                SpawnCockroach();
                break;
            case SpawnerType.Zombie:
                SpawnZombie();
                break;
        }
    }

    private void SpawnCockroach()
    {
        Instantiate(enemyToSpawn, transform.position, Quaternion.identity);
    }

    private void SpawnRat()
    {
        openSewer = true;
        if (sewers.Length >= 2)
        {
            sewers[1].SetActive(true);
            sewers[0].SetActive(false);    
        }
        Instantiate(enemyToSpawn, transform.position, Quaternion.identity);
    }

    private void SpawnZombie()
    {
        openSewer = true;
        if (sewers.Length >= 2)
        {
            sewers[1].SetActive(true);
            sewers[0].SetActive(false);
        }
        Instantiate(enemyToSpawn, transform.position, Quaternion.identity);
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

