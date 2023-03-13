using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private AIController[] enemies;
    [SerializeField] private BossProp bossProp;

    private List<AIController> _spawnedEnemies = new List<AIController>();

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
        BossProp prop = Instantiate(bossProp, transform);
        prop.transform.position = new Vector3(prop.transform.position.x, 2f, prop.transform.position.z);
    }
}
