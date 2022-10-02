using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class PlayerLocal : InputProcessor<Vector3>
{
#if UNITY_EDITOR
    static PlayerLocal()
    {
        Initialize();
    }
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        InputSystem.RegisterProcessor<PlayerLocal>();
    }

    public override Vector3 Process(Vector3 value, InputControl control)
    {
        Vector3 returnValue = Vector3.zero;
        int x = (int)value.x; int z = (int)value.z;
        Transform localConvert = Transform.FindObjectOfType<PlayerInput>().transform;
        bool isX = true; bool isY = true;

        switch (x)
        {
            case -1:
                returnValue += -localConvert.right;
                break;
            case 0:
                isX = false;
                break;
            case 1:
                returnValue += localConvert.right;
                break;
        }
        switch (z)
        {
            case -1:
                returnValue += -localConvert.forward;
                break;
            case 0:
                isY = false;
                break;
            case 1:
                returnValue += localConvert.forward;
                break;
        }

        if (isX && isY)
            returnValue /= 1.4f;

        return returnValue;
    }
}
