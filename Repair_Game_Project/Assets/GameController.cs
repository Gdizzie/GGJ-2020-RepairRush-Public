using Lean.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int maxEnemies = 3;
    public int minEnemies = 1;

    public static GameController s_Instance;

    private List<WheelDriveCustom> enemies = new List<WheelDriveCustom>();

    public GameData gameData;

    public StressReceiver stressReceiver;

    private int enemySpawnQueueCount = 0;

    private int totalTracksSpawned = 0;

    public static PlayerController s_playerController;
    public static void setPlayerController(PlayerController controller) { s_playerController = controller; }

    // Start is called before the first frame update
    void Awake()
    {
        if (s_Instance == null)
            s_Instance = this;
        else Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EndGame()
    {
        Debug.Log("Game Ended");
    }

    public void EnemySpawned(WheelDriveCustom enemy)
    {
        enemies.Add(enemy);
    }

    public void EnemyKilled(WheelDriveCustom enemy)
    {
        enemies.Remove(enemy);
    }

    public void TrackSpawned(TrackPiece trackPiece)
    {
        //Debug.Log("Track Spawned");
        totalTracksSpawned += 1;
        
        if(totalTracksSpawned <= 1)
            return;
        
        if (enemies.Count + enemySpawnQueueCount < minEnemies)
        {
            StartCoroutine(SpawnEnemy(maxEnemies - enemies.Count, trackPiece));
            return;
        }

        if (enemies.Count + enemySpawnQueueCount < maxEnemies)
            //if (UnityEngine.Random.Range(0, 1) == 1)
            StartCoroutine(SpawnEnemy(1, trackPiece));

    }

    private IEnumerator SpawnEnemy(int amount, TrackPiece trackPiece, float spawnDelay = 1f)
    {
        //Debug.Log("Spawn Enemy");
        enemySpawnQueueCount += amount;
        for (int i = 0; i < amount; i++)
        {
            yield return new WaitForEndOfFrame();
            if (i > trackPiece.enemySpawnPoints.Count)
                break;

            Transform spawnPoint = trackPiece.enemySpawnPoints[i];
            WheelDriveCustom newEnemy = LeanPool.Spawn(gameData.enemyPrefab, trackPiece.enemySpawnPoints[i].position,
                trackPiece.enemySpawnPoints[i].rotation);
            enemies.Add(newEnemy);
            trackPiece.QueueEnemy(spawnPoint, newEnemy);
            enemySpawnQueueCount--;
        }
    }
}
