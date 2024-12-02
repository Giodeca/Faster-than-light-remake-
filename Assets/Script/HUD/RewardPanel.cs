using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RewardPanel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] TMP_Text textReward;


    private void OnEnable()
    {

        EventManager.RewardBattle += OnRewardObtained;
    }
    private void OnDisable()
    {
        EventManager.RewardBattle -= OnRewardObtained;
        textReward.text = null;
    }
    private void Start()
    {

    }


    private void OnRewardObtained(int gold, int fuel, int missle)
    {
        textReward.enabled = true;
        textReward.text = "You Obtained " + "Gold " + gold + " fuel " + fuel + " Missle " + missle;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        this.gameObject.SetActive(false);
    }
}
