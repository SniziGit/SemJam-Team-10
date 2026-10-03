using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class MagneticObject : MonoBehaviour
{
   public  Rigidbody2D rb;
    public MagneticPole magneticPole;

    public float attractionForce = 10f;

    public float attractionDuration = 1f;

    private Coroutine currentMagnetRoutine;
    private void Awake()
    {
       rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyMagneticForce(GameObject source, MagneticPole pole)
    {
        currentMagnetRoutine = StartCoroutine(ApplyMagneticForceCoroutine(source, pole));
    }
    IEnumerator ApplyMagneticForceCoroutine(GameObject source, MagneticPole pole)
    {
        float elapsedTime = 0f;

        while (elapsedTime < attractionDuration)
        {
            Vector2 diff = source.transform.position - transform.position;
            float magnitude = pole == magneticPole ? -1f : 1f;
            rb.AddForce(diff.normalized * attractionForce * magnitude, ForceMode2D.Force);
            elapsedTime += Time.fixedDeltaTime;
            yield return null;
        }
    }
}
public enum MagneticPole
{
    North,South
}