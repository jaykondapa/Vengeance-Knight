using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }
    public int enemyTotal = 10;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void EnemyKilled()
    {
        enemyTotal--;
    }

    public bool EnemiesCleared()
    {
        if (enemyTotal == 0)
        {
            return true;
        }
        return false;
    }
}