using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] Slider slider;
    private Transform cam;

    void Awake()
    {
        //Finds the camera
        cam = GameObject.FindWithTag("MainCamera").transform;
        slider = GetComponent<Slider>();
    }

    public void SetMaxHealth(int maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    public void SetHealth(int enemyHealth)
    {
        slider.value = enemyHealth;
    }
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}