using UnityEngine;

public class MovingHazard : MonoBehaviour
{
    [Tooltip("How far it travels from its starting position (world space)")]
    [SerializeField] private Vector3 moveOffset = new Vector3(6f, 0f, 0f);
    [Tooltip("Back-and-forth trips per second (0.2 = one full trip every 5 seconds)")]
    [SerializeField] private float speed = 0.25f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        float t = Mathf.PingPong(Time.time * speed, 1f);
        transform.position = startPosition + moveOffset * t;
    }
}
