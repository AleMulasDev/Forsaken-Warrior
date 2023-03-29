using AmazingAssets.AdvancedDissolve.ExampleScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private AIController[] enemies;
    [SerializeField] private BossProp bossProp;
    [SerializeField] private MinibossController miniboss;
    [SerializeField] private bool shouldSpawnMiniboss;

    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float delayBetweenSpawns;
    [SerializeField] private int timesToSpawn;
    [SerializeField] private int enemiesPerSpawn;
    [SerializeField] private AudioClip spawnAudioClip;

    private AudioSource _audioSource;

    private List<AIController> _spawnedEnemies = new List<AIController>();
    private BossProp _propInstance;

    private Coroutine _spawnCoroutine;

    private int _lastSpawnerIndex = -1;
    private bool _spawned = false;
    private ParticleSystem minibossSpawnEffect;

    private void Start()
    {
        spawnAudioClip = Resources.Load<AudioClip>("EnemyDissolveAudioClip");
        _audioSource = GetComponent<AudioSource>();

        if (shouldSpawnMiniboss)
            minibossSpawnEffect = Resources.Load<ParticleSystem>("MinibossSpawnEffect");
    }

    public List<AIController> SpawnEnemies(int enemiesNumber)
    {
        _spawnedEnemies.Clear();

        for (int i = 0; i < enemiesNumber; i++)
        {
            Vector3 randomPos = Random.insideUnitCircle * 5;
            Vector3 spawnPosition = new Vector3(randomPos.x + transform.position.x, 0, randomPos.y + transform.position.z);
            int randomEnemy = UnityEngine.Random.Range(0, enemies.Length);
            StartCoroutine(PlaySpawnSoundEffectCoroutine());
            _spawnedEnemies.Add(Instantiate(enemies[randomEnemy], spawnPosition, Quaternion.identity));
        }

        return _spawnedEnemies;
    }

    private IEnumerator PlaySpawnSoundEffectCoroutine()
    {
        yield return new WaitForSeconds(0.75f);
        AudioManager.Instance.PlaySoundEffect(_audioSource, spawnAudioClip);
    }

    private void Spawn()
    {
        if (shouldSpawnMiniboss)
            SpawnMiniboss();
        else
            StartCoroutine(SpawnRandomEnemy());
    }

    private IEnumerator SpawnRandomEnemy()
    {
        for (int i = 0; i < timesToSpawn; i++)
        {
            for (int j = 0; j < enemiesPerSpawn; j++)
            {
                int randomEnemy = UnityEngine.Random.Range(0, enemies.Length);
                StartCoroutine(PlaySpawnSoundEffectCoroutine());
                _spawnedEnemies.Add(Instantiate(enemies[randomEnemy], GetRandomSpawnPosition(), Quaternion.identity));
            }
            yield return new WaitForSeconds(delayBetweenSpawns);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        int _spawnerIndex = _lastSpawnerIndex;

        while (_spawnerIndex == _lastSpawnerIndex)
            _spawnerIndex = GetRandomSpawnerIndex();

        _lastSpawnerIndex = _spawnerIndex;
        return spawnPoints[_lastSpawnerIndex].position;
    }

    private int GetRandomSpawnerIndex()
    {
        return Random.Range(0, spawnPoints.Length);
    }

    public bool GetShouldSpawnMiniboss()
    {
        return shouldSpawnMiniboss;
    }

    public List<AIController> SpawnAllEnemies()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            Vector3 randomPos = Random.insideUnitCircle * 5;
            Vector3 spawnPosition = new Vector3(randomPos.x + transform.position.x, 0, randomPos.y + transform.position.z);
            StartCoroutine(PlaySpawnSoundEffectCoroutine());
            _spawnedEnemies.Add(Instantiate(enemies[i], spawnPosition, Quaternion.identity));
        }

        return _spawnedEnemies;
    }

    public void SpawnProp()
    {
        _propInstance = Instantiate(bossProp, transform);
        _propInstance.transform.position = new Vector3(_propInstance.transform.position.x, 2f, _propInstance.transform.position.z);
    }

    public void DestroyProp()
    {
        if (_propInstance != null)
            _propInstance.DestroyProp();
    }

    public MinibossController SpawnMiniboss()
    {
        AudioManager.Instance.PlaySoundEffect(_audioSource, spawnAudioClip);
        Destroy(Instantiate(minibossSpawnEffect, spawnPoints[0].position, minibossSpawnEffect.transform.rotation), 3f);
        return Instantiate(miniboss, spawnPoints[0].position, miniboss.transform.rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_spawned) return;

        if (other.CompareTag("Player") && _spawnCoroutine == null)
        {
            _spawned = true;
            Spawn();
        }
    }
}
