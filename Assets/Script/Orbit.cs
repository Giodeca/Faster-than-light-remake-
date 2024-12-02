using UnityEngine;
using UnityEngine.UI;

public class Orbit : MonoBehaviour
{
    public Image imageToOrbit;
    public float speed = 5f;
    public Vector3 direction = Vector3.up;

    private void Update()
    {
        transform.RotateAround(imageToOrbit.rectTransform.position, direction, speed * Time.deltaTime);
    }
}
