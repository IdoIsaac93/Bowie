using UnityEngine;

public class DamageVisualController : MonoBehaviour
{
    private float timer = 0;
    public float timeUntillDestroy = 0.5f;
    public float risingSpeed = 5f;

    void Update()
    {
        float verticalMovement = risingSpeed * Time.deltaTime;
        transform.position += Vector3.up * verticalMovement;
        timer += Time.deltaTime;
        if (timer >= timeUntillDestroy)
        {
            Destroy(gameObject);
        }
    }
}
