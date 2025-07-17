using UnityEngine;

public class ThrowableObject : MonoBehaviour, IInteractable
{
    [Header("References")]
    private Rigidbody rb;
    private bool isHeld = false;

    public string GetInteractionMessage()
    {
        return isHeld ? "" : "Press E to pick up the object";
    }

    public void Interact()
    {
        if (!isHeld)
        {
            PickUp();
        }
    }

    private void PickUp()
    {
        PlayerSingleton.Instance.playerObjectThrowing.SetHeldObject(this);
        isHeld = true;
        rb = GetComponentInParent<Rigidbody>();
        rb.isKinematic = true;
        transform.parent.SetParent(PlayerSingleton.Instance.playerObjectThrowing.handPosition);
        transform.parent.localPosition = Vector3.zero;
        transform.parent.localRotation = Quaternion.identity;
    }

    public void Throw(Vector3 force)
    {
        if (isHeld)
        {
            isHeld = false;
            rb.isKinematic = false;
            transform.parent.SetParent(null);
            rb.AddForce(force, ForceMode.Impulse);
        }
    }
}
