using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private AIController[] enemies;

    public void Spawn(int enemiesNumber)
    {
        for (int i = 0; i < enemiesNumber; i++)
        {
            Vector3 randomPos = Random.insideUnitCircle * 5;
            Vector3 spawnPosition = new Vector3(randomPos.x + transform.position.x, 0, randomPos.y + transform.position.z);
            int randomEnemy = UnityEngine.Random.Range(0, enemies.Length);
            Instantiate(enemies[randomEnemy], spawnPosition, Quaternion.identity);
        }
    }

    public void SpawnAll()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            Vector3 randomPos = Random.insideUnitCircle * 5;
            Vector3 spawnPosition = new Vector3(randomPos.x + transform.position.x, 0, randomPos.y + transform.position.z);
            Instantiate(enemies[i], spawnPosition, Quaternion.identity);
        }
    }
}
