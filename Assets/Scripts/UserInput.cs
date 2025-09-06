using UnityEngine;

public class UserInput : MonoBehaviour
{
    Transform dragTransform;
    Vector3 dragOffset;
    Vector3 posicionInicial;

    void Update()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0f;

        if (Input.GetMouseButtonDown(0))
        {
            Collider2D hit = Physics2D.OverlapPoint(mouse);
            if (hit != null && hit.CompareTag("carta"))
            {
                dragTransform = hit.transform;
                posicionInicial = dragTransform.position;
                dragOffset = dragTransform.position - mouse;
            }
        }

        if (Input.GetMouseButton(0) && dragTransform != null)
        {
            dragTransform.position = mouse + dragOffset;
        }

        if (Input.GetMouseButtonUp(0) && dragTransform != null)
        {
            Collider2D[] hits = Physics2D.OverlapPointAll(dragTransform.position);
            Collider2D destino = null;
            foreach (var h in hits) { if (h.CompareTag("padre")) { destino = h; break; } }
            if (destino != null)
            {
                dragTransform.SetParent(destino.transform);
                Vector3 p = destino.transform.position;
                p.z = dragTransform.position.z;
                dragTransform.position = p;
            }
            else
            {
                dragTransform.position = posicionInicial;
            }
            dragTransform = null;
        }
    }
}
