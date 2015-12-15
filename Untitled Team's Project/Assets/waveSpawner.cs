using UnityEngine;
using System.Collections;

public class waveSpawner : MonoBehaviour {

    public easterEggButtonPlayerMenu easterEgg;

    public enum SpawnState {SPAWNING, WAITING, COUNTING};

    [System.Serializable]

  

    public class Wave
    {
        public string name;
        public Transform enemy;
        
        public int count;
        public float rate;
    }

    public GameObject player;
    public Transform spawnPoint;

    public Transform[] spawnPoints;

    public Wave[] waves;
    private int nextWave = 0;

    public float timeBetweenWaves = 5f;
    public float waveCountdown;

    private float searchCountDown = 1f;

    private SpawnState state = SpawnState.COUNTING;

    void Start()
    {
        waveCountdown = timeBetweenWaves;

        if (spawnPoints.Length == 0)
        {
            Debug.LogError("SPAWN POINT LENGTH");
        }

        

    }

    void Update()
    {
        if (state == SpawnState.WAITING)
        {
            if (EnemyIsAlive() && easterEgg.EasterEgg == true)
            {

            }

            if (!EnemyIsAlive())
            {
                ////wenrowqneroqwnr
                WaveCompleted();
                return;
            }
            else
                return;
        }


        if (waveCountdown <= 0)
        {
            if (state != SpawnState.SPAWNING)
            {
                StartCoroutine(SpawnWave(waves[nextWave]));
            }
        }
        else if (easterEgg.EasterEgg == true) //EASTER EGG CONDITION
        {
            waveCountdown -= Time.deltaTime;

        }

        if (PlayerIsAlive() == false)
        {
            if (easterEgg.EasterEgg == true)
            {
                Instantiate(player, spawnPoint.position, Quaternion.identity);
            }
        }
    }

    void WaveCompleted()
    {
        Debug.Log("Wave Completed!");

        state = SpawnState.COUNTING;
        waveCountdown = timeBetweenWaves;

        if (nextWave + 1 > waves.Length - 1)
        {
            nextWave = 0;
            Debug.Log("ALL WAVES COMPLETE! Looping....");

        }

        nextWave++;
    }

    bool EnemyIsAlive()
    {
        searchCountDown -= Time.deltaTime;

        if (searchCountDown <= 0f)
        {
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }
        }

        return true;
    }

    bool PlayerIsAlive()
    {
        if(GameObject.FindGameObjectWithTag("Player") == null)
            return false;
        return true;
    }

    IEnumerator SpawnWave(Wave _wave)
    {
        state = SpawnState.SPAWNING;

        for (int i = 0; i < _wave.count; i++)
        {
            SpawnEnemy(_wave.enemy);
            yield return new WaitForSeconds(1f / _wave.rate);
        }

            state = SpawnState.WAITING;

        yield break;
    }

    void SpawnEnemy(Transform _enemy)
    {
        
        Transform _sp = spawnPoints[Random.Range(0,spawnPoints.Length)];
        Instantiate(_enemy, _sp.transform.position, Quaternion.Euler(0, 0, 0));
        Debug.Log("Spawning Enemy: " + _enemy.name);
    }

}
