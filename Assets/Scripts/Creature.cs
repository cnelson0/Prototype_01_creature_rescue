using System;
using UnityEngine;

public class Creature : MonoBehaviour
{
    /// Fired when this creature is delivered to the safe zone.
    public static event Action<Creature> Rescued;

    public bool IsCarried { get; private set; }

    private Collider[] colliders;
    private Vector3 homePosition;
    private Quaternion homeRotation;
    private bool delivered;

    private void Awake()
    {
        colliders = GetComponentsInChildren<Collider>();
        homePosition = transform.position;
        homeRotation = transform.rotation;
    }

    // --- Trigger: tells the player a creature is close enough to pick up ---
    private void OnTriggerEnter(Collider other)
    {
        if (IsCarried || !other.CompareTag("Player")) return;
        FirstPersonController.Instance.CreatureInReach = this;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (FirstPersonController.Instance.CreatureInReach == this)
            FirstPersonController.Instance.CreatureInReach = null;
    }

    public void PickUp(FirstPersonController player)
    {
        IsCarried = true;
        SetCollidersEnabled(false);

        transform.SetParent(player.CarryPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        player.SetCarried(this);
    }

    /// Player died while carrying: put the creature back where it started.
    public void ReturnHome()
    {
        IsCarried = false;
        transform.SetParent(null);
        transform.SetPositionAndRotation(homePosition, homeRotation);
        SetCollidersEnabled(true);
    }

    /// Reached the safe zone: counts as rescued.
    public void Deliver()
    {
        if (delivered) return;
        delivered = true;

        GameManager.Instance.RegisterRescue();
        Rescued?.Invoke(this);
        Destroy(gameObject);
    }

    private void SetCollidersEnabled(bool enabled)
    {
        foreach (Collider c in colliders) c.enabled = enabled;
    }
}
