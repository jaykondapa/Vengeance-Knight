using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    private AudioSource audioSource;

    [Header("Audio")]
    public AudioClip bossHeavyAttackSuccess;
    public AudioClip enemyAttackSwing;
    public AudioClip enemyAttackSuccess;
    public AudioClip enemyDeath;
    public AudioClip enemyReceiveHit;
    public AudioClip gameOver;
    public AudioClip playerDeath;

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
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayBossHeavyAttackSuccess()
    {
        audioSource.PlayOneShot(bossHeavyAttackSuccess);
    }
    
    public void PlayEnemyAttackSwing()
    {
        audioSource.PlayOneShot(enemyAttackSwing);
    }
    
    public void PlayEnemyAttackSuccess()
    {
        audioSource.PlayOneShot(enemyAttackSuccess);
    }

    public void PlayEnemyDeath()
    {
        audioSource.PlayOneShot(enemyDeath);
    }

    public void PlayEnemyReceiveHit()
    {
        audioSource.PlayOneShot(enemyReceiveHit);
    }

    public void PlayGameOver()
    {
        audioSource.PlayOneShot(gameOver);
    }
    
    public void PlayPlayerDeath()
    {
        audioSource.PlayOneShot(playerDeath);
    }
}