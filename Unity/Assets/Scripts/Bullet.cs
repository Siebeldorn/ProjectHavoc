using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Space]
    
    [Tooltip("The speed in meters per second")]
    public float velocity = 10f;

    [Tooltip("The amount of seconds until this bullet destroys itself")]
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += transform.forward * velocity * Time.deltaTime;
    }
}
