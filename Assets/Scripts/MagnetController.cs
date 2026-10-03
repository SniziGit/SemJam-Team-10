using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MagnetController : MonoBehaviour
{
    public ControlType controlType = ControlType.WASD;
    public bool wasPressingInput;

    public MagneticPole pole;

    public MagneticObject thisMagnet;
    public float thisMagneticForce = 2f;// how much this magnet should be attracted or repelled towards other objects

    public float magneticFieldMin = 0.1f; // the min size of the preview
    public float magneticFieldMax = 5f; // the range of the magnetic field

    public float currentMagneticFieldSize = 0f; // the current size of the magnetic field, should be between 0 and magneticFieldRange
    public float fieldExpandSpeed = 2f; // the speed at which the field expands and contracts

    public GameObject previewCircle;// the object that shows the range of the magnetic field, should be a circle with a transparent material

    //initialise magnetic attraction
    Vector3 attractionVector = Vector3.zero;
    //initialise magnetic repulsion
    Vector3 repulsionVector = Vector3.zero;
    // Update is called once per frame
    void Update()
    {
        if(CheckInput() )// when start pressing, create field, when stop pressing apply magnetic force to all magnetic objects in range
        {
            previewCircle.SetActive(true);
            /*
            int direction = 1;

            if (Mathf.Approximately(currentMagneticFieldSize, magneticFieldMax))
            {
                direction = -1;
            }
            else if (Mathf.Approximately(currentMagneticFieldSize, magneticFieldMin))
            {
                direction = 1;
            }
            float size = Mathf.Lerp(magneticFieldMin,magneticFieldMax,Time.deltaTime * fieldExpandSpeed * direction);
            */
            float sine = Mathf.Sin(Time.time * fieldExpandSpeed);
            float size = Mathf.Lerp(magneticFieldMin, magneticFieldMax, (sine + 1f) / 2f);
            currentMagneticFieldSize = size;
            previewCircle.transform.localScale = Vector3.one * currentMagneticFieldSize;
        }

        if(!CheckInput() && wasPressingInput)
        {
            previewCircle.SetActive(false);
            attractionVector = Vector3.zero;
            repulsionVector = Vector3.zero;
            //collect colliders
            Collider2D[] magneticInEnvironment = Physics2D.OverlapCircleAll(transform.position, currentMagneticFieldSize,1 << LayerMask.NameToLayer("Default"));
            foreach (Collider2D col in magneticInEnvironment)
            {
               if(col.gameObject.TryGetComponent<MagneticObject>(out var magneticObject))
               {
                    if(magneticObject == thisMagnet) continue;
                    magneticObject.ApplyMagneticForce(gameObject, pole);
                    Debug.Log($"Added magnetic force to {magneticObject.name}");
                    if(magneticObject.magneticPole != pole)
                    {
                        repulsionVector += magneticObject.transform.position - transform.position;
                    }
                    else
                    {
                        attractionVector += transform.position - magneticObject.transform.position;
                    }
               }
                else
                {
                    Debug.Log($"No magnetic object found on {col.gameObject.name}");
                }
            }
            Vector3 finalVector = Vector3.Normalize(attractionVector + repulsionVector);
            if (thisMagnet) thisMagnet.rb.AddForce(finalVector * thisMagneticForce, ForceMode2D.Impulse);
            // apply magnetic force to all magnetic objects in range
            currentMagneticFieldSize = 0f;
            previewCircle.transform.localScale = Vector3.one * currentMagneticFieldSize;
        }
        wasPressingInput = CheckInput();

    }
   
    

    
    bool CheckInput()
    {
        switch(controlType)
        {
            case ControlType.WASD:
                bool winput = Input.GetKey(KeyCode.W);
                return winput;
            case ControlType.ArrowKeys:
                bool upinput = Input.GetKey(KeyCode.UpArrow);
                return upinput;
            default:
                return false;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, currentMagneticFieldSize);
    }
}
public enum ControlType
{
    WASD, ArrowKeys
}