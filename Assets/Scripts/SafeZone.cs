using UnityEngine;

/// The place the player must bring creatures to.
public class SafeZone : MonoBehaviour
{
    // OnTriggerStay so it still works if the player is standing inside when picking up.
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameManager.Instance.State != GameState.Playing) return;

        FirstPersonController.Instance.DeliverCarried();   // does nothing if not carrying
    }
}
