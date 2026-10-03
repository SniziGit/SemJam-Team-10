using UnityEngine;
using UnityEngine.Events;

public class EndFlag : MonoBehaviour
{
    public int entryTracker = 0;
    public GameObject[] players;
    public UnityEvent onAllPlayersEntered;
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
            Utility.InvokeAfter(this, ()=>{ onAllPlayersEntered?.Invoke(); },2f);
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
