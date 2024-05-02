using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // Include the AI namespace for NavMesh components
using UnityEditor; // Include this for using Editor features like pausing the game

public class ZombieSpawning : MonoBehaviour
{
    [Header("Zombies")]
    public GameObject[] zombies; // List of all zombie prefabs that we will have

    [Header("Spawning Box")]
    public float minX, maxX, yPos, minZ, maxZ;

    [Header("Spawning Stats")]
    public int maxZombies, currentZombies, maxSpawnAttempts;
    public float spawnCooldown;
    public float innerRadius;
    public float outerRadius;

    private float lastSpawnTime;
    private Vector3 lastRaycastOrigin;
    private Vector3 lastRaycastDirection;
    private bool drawLastRaycast = false;

    void Update()
    {
        if (currentZombies < maxZombies && Time.time > lastSpawnTime + spawnCooldown)
        {
            int attempts = 0;
            while (attempts < maxSpawnAttempts && currentZombies < maxZombies)
            {
                if (SpawnZombie())
                {
                    currentZombies++;
                    lastSpawnTime = Time.time;
                    break;
                }
                attempts++;
            }
        }
    }

    public Vector3 SpawnPosition()
    {
        return new Vector3(Random.Range(minX, maxX), yPos, Random.Range(minZ, maxZ));
    }

    public bool GoodDistance(Vector3 enemyLocation)
    {
        float distance = Vector3.Distance(enemyLocation, GameObject.Find("Player").transform.position);
        return innerRadius < distance && distance < outerRadius;
    }

    public bool SpawnZombie()
    {

        Vector3 position = SpawnPosition();
        if (OnGround(position) && CanReachPlayer(position) && GoodDistance(position))
        {
            int randomIndex = Random.Range(0, zombies.Length);
            Instantiate(zombies[randomIndex], position, Quaternion.identity);
            return true;
        }
        return false;
    }

    public bool CanReachPlayer(Vector3 spawnPosition)
    {
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(spawnPosition, GameObject.FindGameObjectWithTag("Player").transform.position, NavMesh.AllAreas, path))
        {
            return path.status == NavMeshPathStatus.PathComplete;
        }
        return false;
    }

    public bool OnGround(Vector3 position)
    {
        RaycastHit hit;
        lastRaycastOrigin = position + Vector3.up;
        lastRaycastDirection = Vector3.down;
        drawLastRaycast = true;

        if (Physics.Raycast(position, lastRaycastDirection, out hit, 10f))
        {
            if (hit.collider.gameObject.CompareTag("Ground"))
            {
                return true;
            }
        }
        return false;
    }
}
