using UnityEngine;
using UnityEngine.UI;
public class OrbitUi : MonoBehaviour
{
    public Image imageToOrbit;
    public Image image;
    public float speed = 5f;
    public Vector3 direction = Vector3.up;
    [SerializeField] private float distanceDisappear;
    [SerializeField] private float distanceAppear;

    private void Update()
    {
        // Ruota attorno alla posizione dell'immagine
        transform.RotateAround(imageToOrbit.rectTransform.position, direction, speed * Time.deltaTime);

        // Ottieni la posizione attuale sulla x
        float posX = transform.position.x;
        Debug.Log(posX);
        // Controlla l'alpha in base alla posizione x
        Color color = image.color;
        if (posX <= distanceDisappear)
        {
            Debug.Log("Qui");
            color.a = 0f;
        }
        else if (posX >= distanceAppear)
        {
            color.a = 1f;
        }
        image.color = color;
    }
}
