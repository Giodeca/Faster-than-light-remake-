using System.Collections;
using UnityEngine;

public enum ProjectileType
{
    MISSLE, LASER, SUN, METEOR
}
public class Projectile : MonoBehaviour
{
    [SerializeField] private CircleCollider2D circleCollider;
    [SerializeField] private float speed = 5f; // Velocità del proiettile
    [SerializeField] private ProjectileScriptable scriptable;


    private void Awake()
    {
        scriptable = Instantiate(scriptable);
    }

    private void Start()
    {
        SetColor();
    }

    private void SetColor()
    {
        switch (scriptable.type)
        {
            case ProjectileType.MISSLE:
                this.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                break;
            case ProjectileType.LASER:
                this.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
                break;
            case ProjectileType.SUN:
                this.gameObject.GetComponent<SpriteRenderer>().color = Color.magenta;
                break;
            case ProjectileType.METEOR:
                this.gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
                break;
        }


    }
    public IEnumerator MovingObject(Vector2 startPosition, Vector2 positionEnd)
    {
        float distance = Vector2.Distance(startPosition, positionEnd); // Distanza tra punto iniziale e finale
        float elapsedTime = 0;

        // Finché non raggiunge la posizione finale
        while (elapsedTime < distance / speed)
        {
            if (this == null)
            {
                yield break;
            }
            transform.position = Vector2.Lerp(startPosition, positionEnd, elapsedTime / (distance / speed));
            elapsedTime += Time.deltaTime;
            yield return null; // Attende il frame successivo
        }

        // Assicura che la posizione finale venga esattamente raggiunta

        transform.position = positionEnd;
        circleCollider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Pavment"))
        {
            GameManager.Instance.ship.ApplyDamage(scriptable.damage);

            //Debug.Log("Ho colpito qualcosa");

            StopCoroutine("MovingObject");
            if (this != null)
            {
                Destroy(this.gameObject);
            }

        }
        else if (collision.CompareTag("PavmentEnemy"))
        {
            //Debug.Log("Ho colpito qualcosa del nemico");
            GameManager.Instance.enemyActiveShip.ApplyDamage(scriptable.damage);

            StopCoroutine("MovingObject");
            if (this != null)
            {
                Destroy(this.gameObject);
            }
        }
        else if (collision.CompareTag("Shield"))
        {

            collision.gameObject.GetComponent<Shield>().DestroyShield(scriptable.damage);
            scriptable.damage = 0;
            StopCoroutine("MovingObject");

            // Controllo di sicurezza prima di distruggere
            if (this != null)
            {
                Destroy(this.gameObject);
            }
        }
        else if (collision.CompareTag("DeactivateCollider"))
        {

            circleCollider.enabled = false;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Room"))
        {

            collision.gameObject.GetComponent<Rooms>().Broken(collision.gameObject.GetComponent<Rooms>().scriptable.roomHP, collision.gameObject.GetComponent<Rooms>().scriptable, scriptable.damage);

            switch (scriptable.type)
            {
                case ProjectileType.SUN:
                    collision.gameObject.GetComponent<Rooms>().scriptable.roomStatus = RoomStatus.ONFIRE;
                    collision.gameObject.GetComponent<Rooms>().scriptable.roomHP = 30;
                    collision.gameObject.GetComponent<Rooms>().OnFireRoomColor();
                    break;
                case ProjectileType.METEOR:
                    collision.gameObject.GetComponent<Rooms>().scriptable.roomStatus = RoomStatus.HASHOLE;
                    collision.gameObject.GetComponent<Rooms>().scriptable.roomHP = 30;
                    collision.gameObject.GetComponent<Rooms>().OnHoleRoomColor();
                    break;
                case ProjectileType.LASER:
                    collision.gameObject.GetComponent<Rooms>().HitCounter(scriptable.type);
                    break;
                case ProjectileType.MISSLE:
                    collision.gameObject.GetComponent<Rooms>().HitCounter(scriptable.type);
                    break;

            }


            StopCoroutine("MovingObject");

            // Controllo di sicurezza prima di distruggere
            if (this != null)
            {
                Destroy(this.gameObject);
            }
        }
        //else if (collision.gameObject.CompareTag("Shield"))
        //{
        //    Debug.Log("Ho colpito qualcosa Shield");
        //    collision.gameObject.GetComponent<Shield>().DestroyShield(scriptable.damage);
        //    Destroy(this.gameObject);
        //}
    }

}
