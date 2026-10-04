using UnityEngine;
using UnityEngine.Events;

public class EndFlag : MonoBehaviour
{
    public int entryTracker = 0;
    public GameObject[] players;
    public UnityEvent onAllPlayersEntered;

    [Header("VFX Settings")]
    [SerializeField] GameObject winVFXPrefab;
    [SerializeField] Transform vfxSpawnPoint;

    [Header("Collision Settings")]
    [SerializeField] float playerCollisionDistance = 1f;

    private bool allPlayersInWinZone = false;
    private bool hasCollided = false;

    private void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            entryTracker++;
        }

        if (entryTracker == players.Length)
        {
            allPlayersInWinZone = true;
            // Play win sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayWinSound();
            }

            Utility.InvokeAfter(this, ()=>{ onAllPlayersEntered?.Invoke(); },2f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            entryTracker--;
            allPlayersInWinZone = false;
            hasCollided = false;
        }
    }

    private void Update()
    {
        // Check for player collision when both are in win zone
        if (allPlayersInWinZone && !hasCollided && players.Length >= 2)
        {
            CheckPlayerCollision();
        }
    }

    private void CheckPlayerCollision()
    {
        if (players[0] == null || players[1] == null) return;

        float distance = Vector2.Distance(players[0].transform.position, players[1].transform.position);

        if (distance <= playerCollisionDistance)
        {
            hasCollided = true;
            // Play player collision sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPlayerCollisionSound();
            }
            // Spawn win VFX on collision
            SpawnWinVFX();
        }
    }

    private void SpawnWinVFX()
    {
        if (winVFXPrefab != null)
        {
            Vector3 spawnPosition = vfxSpawnPoint != null ? vfxSpawnPoint.position : transform.position;
            Instantiate(winVFXPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
