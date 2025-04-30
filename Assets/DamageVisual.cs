using TMPro;
using UnityEngine;

public class DamageVisual : MonoBehaviour
{
    [Header("Movement and Lifetime")]
    [SerializeField] private float timeUntillDestroy = 0.5f;
    [SerializeField] private float risingSpeed = 5f;
    private float timer;

    [SerializeField] private TextMeshProUGUI damageText;

    public float Timer { get => timer; set => timer = value; }
    public TextMeshProUGUI DamageText { get => damageText; set => damageText = value; }

    void Update()
    {
        float verticalMovement = risingSpeed * Time.deltaTime;
        transform.position += Vector3.up * verticalMovement;
        timer += Time.deltaTime;
        if (timer >= timeUntillDestroy)
        {
            gameObject.SetActive(false);
        }
    }
}
