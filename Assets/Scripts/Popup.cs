using UnityEngine;
using System.Collections;

public class Popup : MonoBehaviour
{

    public Vector3 hiddenPosition;
    private Vector3 initialPosition;

    public float time, delay;
    private bool activated;
    public bool activate = false;

    private void Start()
    {
        initialPosition = transform.localPosition;
        transform.localPosition = hiddenPosition;
    }

    private void Update()
    {
        if (activate)
        {
            activate = false;
            Activate(delay);
        }
    }

    public void Activate(float deactivate = 0f)
    {
        if (activated) return;

        if (deactivate > 0f) Invoke("Deactivate", deactivate + time);

        activated = true;
        iTween.MoveTo(gameObject, iTween.Hash(
                "y", initialPosition.y,
                "time", time,
                "easetype", iTween.EaseType.easeOutBack,
                "islocal", true
            ));
    }

    public void Deactivate()
    {
        if (!activated) return;

        iTween.MoveTo(gameObject, iTween.Hash(
                "y", hiddenPosition.y,
                "time", time,
                "easetype", iTween.EaseType.easeInBack,
                "islocal", true
            ));

        Invoke("SetActivetFalse", time);
    }

    private void SetActivetFalse()
    {
        activated = false;
    }
}
