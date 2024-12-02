using System.Collections;
using UnityEngine;

public class AnimationSailing : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Vector3 NoShipArea;
    [SerializeField] private Vector3 ShipArea;
    [SerializeField] private float cameraStartFov;
    [SerializeField] private float cameraMiddleFov;
    [SerializeField] private float cameraEndPositionFov;
    [SerializeField] private float elapseTimeAnimation; // Time for first LERP phase
    [SerializeField] private float TimeAnimation; // Total animation time
    [SerializeField] private GameObject animationBlock;

    private void OnEnable()
    {
        EventManager.StartBattle += OnSailing;
    }
    private void OnDisable()
    {
        EventManager.StartBattle -= OnSailing;
    }
    private void Start()
    {
        // Optionally, you can initialize the camera FOV here
        _camera.orthographicSize = cameraStartFov;


    }

    IEnumerator CameraAnimation(bool isWhat)
    {
        GameManager.Instance.gamePaused = true;
        // Calculate the time for each phase
        float halfAnimationTime = TimeAnimation / 2f;

        // Phase 1: Lerp from Start FOV to Middle FOV
        float elapsedTime = 0f;
        while (elapsedTime < halfAnimationTime)
        {
            float t = elapsedTime / halfAnimationTime;
            _camera.orthographicSize = Mathf.Lerp(cameraStartFov, cameraMiddleFov, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        animationBlock.SetActive(true);
        yield return new WaitForSeconds(2f);
        animationBlock.SetActive(false);
        _camera.orthographicSize = cameraMiddleFov; // Ensure it ends exactly at Middle FOV
        if (isWhat)
        {
            _camera.transform.position = ShipArea;
            EventManager.SpawnShip?.Invoke();
        }
        else
            _camera.transform.position = ShipArea;
        // Phase 2: Lerp from Middle FOV to End FOV

        elapsedTime = 0f;
        while (elapsedTime < halfAnimationTime)
        {
            float t = elapsedTime / halfAnimationTime;
            _camera.orthographicSize = Mathf.Lerp(cameraMiddleFov, cameraEndPositionFov, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _camera.orthographicSize = cameraEndPositionFov; // Ensure it ends exactly at End FOV
        EventManager.callEvent?.Invoke();
    }

    private void OnSailing(bool isBattle)
    {
        StartCoroutine(CameraAnimation(isBattle));
    }
}
