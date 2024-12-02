using UnityEngine;

public class PavmentScript : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    [SerializeField] private Color hoverColor = new Color(1f, 0f, 0f, 0.5f); // Colore e alpha da usare quando il mouse è sopra
    [SerializeField] private Color colorFire;
    [SerializeField] private Color hole;
    [SerializeField] private Color rooomObscure;
    [SerializeField] private Color lowOxigen;
    private float originalAlpha;
    private bool hasCondition;
    private Color actualColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // Salva il colore originale
        /* originalAlpha = spriteRenderer.color.a; */// Salva l'alpha originale
        CheckForRoomAbove();
    }

    void CheckForRoomAbove()
    {
        // Controlla se c'è un oggetto con il tag "Room" sopra la posizione del cubo

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Collider2D hitCollider = Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y));
        if (collision != null && collision.CompareTag("Room"))
            collision.gameObject.GetComponent<Rooms>().pavmentScripts.Add(this.gameObject);

    }
    public void RoomOnFire()
    {

        spriteRenderer.color = colorFire;
        actualColor = colorFire;
        hasCondition = true;
    }
    public void OxygenLow()
    {
        spriteRenderer.color = lowOxigen;
        actualColor = lowOxigen;
        hasCondition = true;
    }
    public void RoomHole()
    {

        spriteRenderer.color = hole;
        actualColor = hole;
        hasCondition = true;
    }
    public void RoomObscure()
    {

        spriteRenderer.color = rooomObscure;
        //actualColor = rooomObscure;
        hasCondition = true;
    }

    public void ResetColor()
    {
        Debug.Log("qui");
        spriteRenderer.color = originalColor;
        actualColor = originalColor;
        hasCondition = false;
    }
    public void ResetColorObscure()
    {
        Debug.Log("qui2");
        spriteRenderer.color = actualColor;

    }
    void OnMouseEnter()
    {
        // Cambia il colore e l'alpha quando il mouse passa sopra
        MouseIn();
    }


    public void MouseIn()
    {
        if (GameManager.Instance.conditionType != CONDITIONTYPE.NEBULOSA)
        {

            Color newColor = hoverColor;
            //newColor.a = hoverColor.a;
            spriteRenderer.color = newColor;
        }

    }

    public void MouseOut()
    {
        if (!hasCondition)
        {

            Color returnColor = originalColor;
            //returnColor.a = originalAlpha;
            spriteRenderer.color = returnColor;
        }
        else
        {
            if (GameManager.Instance.conditionType != CONDITIONTYPE.NEBULOSA)
            {

                Color returnColor = actualColor;
                //returnColor.a = originalAlpha;
                spriteRenderer.color = returnColor;
            }

        }

    }
    void OnMouseExit()
    {
        // Torna al colore e alpha originale quando il mouse esce
        MouseOut();
    }
}
