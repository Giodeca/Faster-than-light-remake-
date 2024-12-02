using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Unit", menuName = "UnitCreation")]
public class UnitScriptable : ScriptableObject
{
    public int hpUnit;
    public string unitName;
    public bool isEnemey;
    public bool isDead;
    [SerializeField]
    private float transitionDuration = 0.5f; // Durata della transizione in secondi



    public void SetBarHealth(Slider slider)
    {

        slider.maxValue = hpUnit;
        slider.value = hpUnit; // Imposta il valore iniziale dello slider
    }

    public void Damage(int damage, ref int hp)
    {
        hp -= damage;

        if (hp <= 0)
        {
            isDead = true;
        }
    }



    public IEnumerator UpdateHealthBar(int targetHp, Slider slider)
    {
        float elapsedTime = 0f;
        float startValue = slider.value;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            slider.value = Mathf.Lerp(startValue, targetHp, elapsedTime / transitionDuration);
            yield return null;
        }

        // Assicurati che il valore finale sia esattamente il targetHp
        slider.value = targetHp;
    }
}
