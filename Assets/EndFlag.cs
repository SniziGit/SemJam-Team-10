using UnityEngine;
using UnityEngine.Events;

public class EndFlag : MonoBehaviour
{
    private int entryTracker = 0;
    private GameObject[] players;
    public UnityEvent onAllPlayersEntered;
    private void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            entryTracker++;
        }

        if (entryTracker == players.Length)
        {
           onAllPlayersEntered?.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            entryTracker--;
        }
    }
}
