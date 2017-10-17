using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class TargetTrackingTrigger : MonoBehaviour
{

    public GameObject triggerTarget;
    public string targetFoundMessage, targetLostMessage;

    private bool wasEnabled;
    private MeshRenderer renderer;

    private void Start() {
        renderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (renderer.enabled != wasEnabled)
        {
            if (renderer.enabled) OnEnabled();
            else OnDisabled();
        }

        wasEnabled = renderer.enabled;
    }

    private void OnEnabled()
    {
        if (triggerTarget != null)
        {
            triggerTarget.SendMessage(targetFoundMessage);
        }
    }

    private void OnDisabled()
    {
        if (triggerTarget != null)
        {
            triggerTarget.SendMessage(targetLostMessage);
        }
    }
}
