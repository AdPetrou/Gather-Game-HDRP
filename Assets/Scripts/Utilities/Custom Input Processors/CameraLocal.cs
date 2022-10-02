using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class CameraLocal : InputProcessor<Vector3>
{
#if UNITY_EDITOR
    static CameraLocal()
    {
        Initialize();
    }
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        InputSystem.RegisterProcessor<CameraLocal>();
    }

    public override Vector3 Process(Vector3 value, InputControl control)
    {
        Vector3 returnValue = Vector3.zero;
        int x = (int)value.x; int z = (int)value.z;
        Transform cameraTransform = Camera.main.transform.parent;

        switch (x)
        {
            case -1:
                returnValue += -cameraTransform.right;
                break;
            case 0:
                break;
            case 1:
                returnValue += cameraTransform.right;
                break;
        }
        switch (z)
        {
            case -1:
                returnValue += -cameraTransform.forward;
                break;
            case 0:
                break;
            case 1:
                returnValue += cameraTransform.forward;
                break;
        }

        return Vector3.Normalize(returnValue);
    }
}
