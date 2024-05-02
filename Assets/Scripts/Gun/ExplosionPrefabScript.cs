using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionPrefabScript : MonoBehaviour
{
    public float timeUntilDead;

    private void Update()
    {
        timeUntilDead -= Time.deltaTime;

        if (timeUntilDead < 0 )
        {
            Destroy(gameObject);
        }
    }
}
