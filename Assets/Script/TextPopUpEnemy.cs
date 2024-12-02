using TMPro;
using UnityEngine;

public class TextPopUpEnemy : MonoBehaviour
{
    [SerializeField] private TMP_Text textPopUp;
    [SerializeField] private SectorOrbit orb;
    private void Start()
    {
        textPopUp.text = orb.enemyType.ToString();
    }
}
