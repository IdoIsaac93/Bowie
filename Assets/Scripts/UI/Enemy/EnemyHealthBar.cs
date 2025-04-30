using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Transform cam;

    void Awake()
    {
        //Finds the camera
        if (cam == null)
        {
            cam = GameObject.FindWithTag("MainCamera").transform;
        }

        slider = GetComponent<Slider>();
    }

    public void SetMaxHealth(int maxHealth)
    {
        slider.maxValue = maxHealth;
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