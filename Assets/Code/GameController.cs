using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject Enemy;
    public GameObject Player1;

    private float EnemySpawnIntervalTimer;
    private float EnemySpawnInterval = 1f;
    private Vector3 ScreenBounds;

    // Start is called before the first frame update
    void Start()
    {
        ScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        Instantiate(Player1, Vector2.zero, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        EnemySpawnIntervalTimer -= Time.deltaTime;

        if (EnemySpawnIntervalTimer < 0)
        {
            EnemySpawnIntervalTimer = EnemySpawnInterval;


            var spawnPointX = 0.0f;
            var spawnPointY = 0.0f;
            if (Random.value > 0.5f) //spawn left or right outside the screen
            {
                if (Random.value > 0.5f) spawnPointX = -ScreenBounds.x; else spawnPointX = ScreenBounds.x;
                spawnPointY = Random.Range(-ScreenBounds.y, ScreenBounds.y);
            }
            else //spawn up or down outside the screen
            {
                spawnPointX = Random.Range(-ScreenBounds.x, ScreenBounds.x);
                if (Random.value > 0.5f) spawnPointY = -ScreenBounds.y; else spawnPointY = ScreenBounds.y;
            }


            //var ballCollection = GameObject.FindGameObjectsWithTag("BallCollection")[0];
            var spawnPoint = new Vector3(spawnPointX, spawnPointY );
            Instantiate(Enemy, spawnPoint, Quaternion.identity);
        }
    }
}
