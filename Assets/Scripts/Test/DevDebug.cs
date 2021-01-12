using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class DevDebug : MonoBehaviour {
    public GameObject enemy;
    public Transform player;
    public void Restart () {
        SceneManager.LoadScene (0);
    }
    public void AddEnemy () {
        GameObject EnemyGroups = GameObject.Find ("EnemyGroups");
        if (EnemyGroups == null) {
            EnemyGroups = new GameObject ("EnemyGroups");
        }
        Vector3 randomPoint = PhysicsUtils.GetRandomPointInMap (new Vector2 (20f, 60f), 20f, player.position);
        GameObject _enemy = Instantiate<GameObject> (enemy, randomPoint, Quaternion.identity, EnemyGroups.transform);
    }
}