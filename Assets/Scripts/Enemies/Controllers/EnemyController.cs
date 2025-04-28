using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] protected float movementSpeed;
    public float MovementSpeed { get { return movementSpeed; } set { movementSpeed = value; } }
}
