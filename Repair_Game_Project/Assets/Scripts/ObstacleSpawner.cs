using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class ObstacleSpawner : MonoBehaviour { 

    public List<GameObject> obstacles;
    public Vector3 center;
    public Vector3 size;
    public int maxObstacles;
    public int currentObstacles;
    public float spawnHeight = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        TrackPiece.setObstacleSpawner(this);
    }

    // Update is called once per frame
    /*void Update()
    {
        if(currentObstacles < maxObstacles)
        {
            SpawnObstacles(maxObstacles);
        }
    }*/
    public void SpawnObstacles(int obstacleAmount, Transform parent, float delay = 1f)
    {
        StartCoroutine(Spawn(obstacleAmount, parent, delay));
    }

    private IEnumerator Spawn(int obstacleAmount, Transform parent, float delay = 1f)
    {
        yield return null;// new WaitForSeconds(delay);

        for (int i = 0; i < obstacleAmount; i++)
        {
            Vector3 pos = transform.position + center + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));

            //Vector3 groundHitPos = new Vector3();

            Ray ray = new Ray(pos, Vector3.down * 5f);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 5f))
                pos = hitInfo.point;

            pos = new Vector3(pos.x, pos.y + spawnHeight, pos.z);

            int choice = Random.Range(0, obstacles.Count);

            GameObject newObstacle = LeanPool.Spawn(obstacles[choice], pos, Quaternion.identity);

            currentObstacles++;

            newObstacle.transform.parent = parent;

            Debug.Log("Spawn Object");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(center,size);
    }
    bool m_debugDraw = false;
    void debugDraw() {
        
    }
}
