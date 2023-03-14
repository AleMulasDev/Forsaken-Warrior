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
    [SerializeField] private ParticleSystem minibossSpawnEffect;

    private List<AIController> _spawnedEnemies = new List<AIController>();
    private BossProp _propInstance;
    public List<AIController> SpawnEnemies(int enemiesNumber)
    {
        _spawnedEnemies.Clear();

        for (int i = 0; i < enemiesNumber; i++)
        {
            Vector3 randomPos = Random.insideUnitCircle * 5;
            Vector3 spawnPosition = new Vector3(randomPos.x + transform.position.x, 0, randomPos.y + transform.position.z);
            int randomEnemy = UnityEngine.Random.Range(0, enemies.Length);
            _spawnedEnemies.Add(Instantiate(enemies[randomEnemy], spawnPosition, Quaternion.identity));
        }

        return _spawnedEnemies;
    }

    public bool GetShouldSpawnMiniboss()
    {
        return shouldSpawnMiniboss;
    }

    public void SpawnAllEnemies()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            Vector3 randomPos = Random.insideUnitCircle * 5;
            Vector3 spawnPosition = new Vector3(randomPos.x + transform.position.x, 0, randomPos.y + transform.position.z);
            Instantiate(enemies[i], spawnPosition, Quaternion.identity);
        }
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

    public void SpawnMiniboss()
    {
        Destroy(Instantiate(minibossSpawnEffect, transform.position, minibossSpawnEffect.transform.rotation), 3f);
        StartCoroutine(SpawnMinibossCoroutine());
    }

    private IEnumerator SpawnMinibossCoroutine()
    {
        yield return new WaitForSeconds(.25f);
        MinibossController minibossInstance = Instantiate(miniboss, transform.position, miniboss.transform.rotation);
        minibossInstance.SetShouldSpawnEnemies(false);
    }
}
