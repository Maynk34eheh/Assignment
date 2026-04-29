using UnityEngine;

// Switches the handle between its normal and pulled positions
public class HandleController : MonoBehaviour
{
    [Header("Handle States")]
    [SerializeField] private GameObject normalHandle;
    [SerializeField] private GameObject pulledHandle;

    private void Awake()
    {
        SetNormal();
    }

    public void SetPulled()
    {
        if (normalHandle != null) normalHandle.SetActive(false);
        if (pulledHandle != null) pulledHandle.SetActive(true);
    }

    public void SetNormal()
    {
        if (normalHandle != null) normalHandle.SetActive(true);
        if (pulledHandle != null) pulledHandle.SetActive(false);
    }
}
