using UnityEngine;

public class MagnetController : MonoBehaviour
{
    public ControlType controlType = ControlType.WASD;
    public bool wasPressingInput;

    // Update is called once per frame
    void Update()
    {
        if(CheckInput() != wasPressingInput)// when start pressing, create field, when stop pressing apply magnetic force to all magnetic objects in range
        {
            // do something
        }
    }
    bool CheckInput()
    {
        switch(controlType)
        {
            case ControlType.WASD:
                bool winput = Input.GetKey(KeyCode.W);
                wasPressingInput = winput;
                return winput;
            case ControlType.ArrowKeys:
                bool upinput = Input.GetKey(KeyCode.UpArrow);
                wasPressingInput = upinput;
                return upinput;
            default:
                return false;
        }
    }
}
public enum ControlType
{
    WASD, ArrowKeys
}