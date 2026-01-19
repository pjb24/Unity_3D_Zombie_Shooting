using UnityEngine;

public class ZombieSpawn : MonoBehaviour
{
    float spawnTimer;
    public Transform spawnPoint;
    public GameObject zomPrefab;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (spawnTimer >= 0)
        {
            spawnTimer -= Time.deltaTime;
        }

        else if (spawnTimer < 0)
        {
            GameObject _zomprefab = Instantiate(zomPrefab, spawnPoint.position, spawnPoint.rotation);
            spawnTimer = 1f;
        }
    }
}
