using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndTrigger : MonoBehaviour
{
    public string nextScene = "Level2_Castle";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
			if (EnemyManager.Instance != null)
			{
            	if (EnemyManager.Instance.EnemiesCleared() == true)
				{
					SceneManager.LoadScene(nextScene);
				}
			}
        }
    }
}