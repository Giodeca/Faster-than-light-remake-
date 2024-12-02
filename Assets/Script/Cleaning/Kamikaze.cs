using UnityEngine;

public class Kamikaze : MonoBehaviour
{
    private void Awake()
    {
        EventManager.Boom += OnBoom;
    }

    private void OnDisable()
    {
        EventManager.Boom -= OnBoom;
    }
    private void OnBoom()
    {
        Destroy(gameObject);
    }
}
