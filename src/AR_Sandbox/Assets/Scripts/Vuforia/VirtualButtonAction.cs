using UnityEngine;
using UnityEngine.Events;
using Vuforia;

/// <summary>
/// Provides a simple, abstract interface for virtual buttons that can be used from the Unity editor.
/// This script must be attached to a Virtual Button.
/// </summary>
public class VirtualButtonAction : MonoBehaviour, IVirtualButtonEventHandler
{
    // onButtonDown preferred over onButtonUp
    public UnityEvent onButtonDown;
    public UnityEvent onButtonUp;

    private VirtualButtonBehaviour virtualButtonBehaviour;

    void Start()
    {
        virtualButtonBehaviour = this.gameObject.GetComponent<VirtualButtonBehaviour>();

        if (virtualButtonBehaviour != null)
        {
            virtualButtonBehaviour.RegisterEventHandler(this);
        }
        else
        {
            throw new System.Exception("No Virtual Button Behaviour found.");
        }
    }

    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        Debug.Log(string.Format("Virtual button {0} pressed.", this.gameObject.name));

        if (onButtonDown != null)
        {
            onButtonDown.Invoke();
        }
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        Debug.Log(string.Format("Virtual button {0} released.", this.gameObject.name));

        if (onButtonUp != null)
        {
            onButtonUp.Invoke();
        }
    }
}
