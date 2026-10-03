using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class MagneticObject : MonoBehaviour
{
   public  Rigidbody2D rb;
    public MagneticPole magneticPole;

    public float magneticForce = 10f;

    public float attractionDuration = 1f;

    private Coroutine currentMagnetRoutine;
    private void Awake()
    {
       rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }

    public void ApplyMagneticForce(GameObject source,MagneticPole pole)
    {
        if(pole == magneticPole)
        {
            RepelFrom(source);
        }
        else
        {
            AttractTo(source);
        }
    }

    public void AttractTo(GameObject source)
    {
        if (currentMagnetRoutine != null) { StopCoroutine(currentMagnetRoutine); currentMagnetRoutine = null; }
        currentMagnetRoutine = StartCoroutine(ApplyMagneticForceCoroutine(source, MagneticBehaviour.Attract));
    }
    public void RepelFrom(GameObject source)// change to impulse force later
    {
        if (currentMagnetRoutine != null) { StopCoroutine(currentMagnetRoutine); currentMagnetRoutine = null; }
        currentMagnetRoutine = StartCoroutine(ApplyMagneticForceCoroutine(source, MagneticBehaviour.Repel));
    }
    IEnumerator ApplyMagneticForceCoroutine(GameObject source, MagneticBehaviour behaviour)
    {
        float elapsedTime = 0f;

        while (elapsedTime < attractionDuration)
        {
            Vector2 diff = source.transform.position - transform.position;
            float magnitude = behaviour == MagneticBehaviour.Attract ? 1f : -1f;
            rb.AddForce(diff.normalized * magneticForce * magnitude, ForceMode2D.Force);
            elapsedTime += Time.fixedDeltaTime;
            yield return null;
        }
    }
}
public enum MagneticPole
{
    North,South,Metal
}
public enum MagneticBehaviour
{
Attract,Repel}
