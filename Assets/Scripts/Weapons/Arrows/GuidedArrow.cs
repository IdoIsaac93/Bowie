using UnityEngine;

public class GuidedArrow : Arrow
{
    public float lifetime = 20f;
    float lifetimeTimer = 0f;

    public float guideAccuracy = 0.1f;

    new void Update()
    {
        //Rotates the arrow according to flight arc
        if (isFlying && rb.linearVelocity.magnitude > 0)
        {
            Quaternion arcRotation = Quaternion.LookRotation(rb.linearVelocity.normalized);
            transform.rotation = arcRotation;

            if (Input.GetKey(KeyCode.Space))
            {
                // Adjust trajectory towards mouse cursor
                Vector3 mousePosition = Input.mousePosition;
                Vector3 mousePositionWorld = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, transform.position.z - Camera.main.transform.position.z));
                Vector3 direction = (mousePositionWorld - transform.position).normalized;
                rb.AddForce(direction * guideAccuracy, ForceMode.Force);
            }
        }

        //Destroy arrow after lifetime
        lifetimeTimer += Time.deltaTime;
        if (lifetimeTimer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
