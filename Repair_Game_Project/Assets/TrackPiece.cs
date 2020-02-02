using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class TrackPiece : MonoBehaviour
{
    bool m_canDespawn = false;

    static TrackSpawner s_spawner = null;
    public static void setSpawner(TrackSpawner spawner) {
        s_spawner = spawner;
    }
    public static ObstacleSpawner s_obstacleSpawner = null;
    public static void setObstacleSpawner(ObstacleSpawner spawner) {
        s_obstacleSpawner = spawner;
    }
    bool m_hasBeenObstacleSpawn = false;

    public Vector3 obstacleBoxCenter;
    public Vector3 obstacleBoxSize;
    public bool debugDraw = false;

    public List<Transform> enemySpawnPoints = new List<Transform>();

    private bool despawning = false;
    private Dictionary<Transform, WheelDriveCustom> queuedEnemies = new Dictionary<Transform, WheelDriveCustom>();

    void DebugDraw() {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(obstacleBoxCenter, obstacleBoxSize);
    }
    void OnDrawGizmos() {
        if (debugDraw) {
            DebugDraw();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable() {
        //Debug.Log("enable", gameObject);
        despawning = false;
        m_canDespawn = false;
        GameController.s_Instance.TrackSpawned(this);
    }

    public void TurnOnEnemy(Transform spawnPoint)
    {
        if (queuedEnemies.ContainsKey(spawnPoint))
        {
            queuedEnemies[spawnPoint].TurnOnVehicle(2.5f);
            queuedEnemies.Remove(spawnPoint);
        }
    }

    public void QueueEnemy(Transform spawnPoint, WheelDriveCustom enemy)
    {
        queuedEnemies[spawnPoint] = enemy;
    }

    private IEnumerator Despawn()
    {
        yield return new WaitForFixedUpdate();
        LeanPool.Despawn(gameObject.transform.root);
        s_spawner.decreaseTrackCount();
        //Debug.Log("despawn");
    }

    void OnBecameVisible() {
        m_canDespawn = true;
        //Debug.Log("visible");
    }

    void OnBecameInvisible()
    {
        //m_canDespawn = true;
    }

    void OnTriggerExit(Collider other){//OnBecameInvisible() {
        if (other.tag == "Spawn Radius" && transform.position.z < other.transform.position.z) {
            if (!despawning)
            {
                StartCoroutine(Despawn());
                despawning = true;
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Spawn Radius") {
            if (!m_hasBeenObstacleSpawn) {
                m_hasBeenObstacleSpawn = true;
                //Debug.Log("trigger enter");
                s_obstacleSpawner.center = transform.parent.position + obstacleBoxCenter;
                s_obstacleSpawner.size = obstacleBoxSize;
                s_obstacleSpawner.SpawnObstacles(s_obstacleSpawner.maxObstacles, gameObject.transform.root);
            }
        }
    }
}
