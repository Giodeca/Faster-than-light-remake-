using TMPro;
using UnityEngine;

public class TextPopUp : MonoBehaviour
{
    [SerializeField] private EnergyHandler energyHandler;
    public TMP_Text TEXT;

    private void Start()
    {
        TEXT.text = energyHandler.statToHandle.ToString();
    }
}
