using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    [SerializeField] float speed = 5.0f;
    [SerializeField] CharacterController controller; 

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(h, 0f, v).normalized;

        if (direction.magnitude >= 0.1f)
        {
            controller.Move(direction * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ItemObject>(out ItemObject itemAdd))
        {
            itemAdd.OnHandlePickupItem();
        }

        if (other.TryGetComponent<TrashBin>(out TrashBin itemRemove))
        {
            itemRemove.OnHandleTrashBin();
        }
    }
}
