using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] private string deathReason = "Touched a hazard";

    private void OnTriggerEnter(Collider other) => TryKill(other);
    private void OnTriggerStay(Collider other) => TryKill(other);

    private void TryKill(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameManager.Instance.State != GameState.Playing) return;

        FirstPersonController.Instance.Die(deathReason);
    }
}
