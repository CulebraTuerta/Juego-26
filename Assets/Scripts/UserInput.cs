using UnityEngine;

public class UserInput : MonoBehaviour
{
    private void Update()
    {
        OnMouseEnter();
    }
    private void OnMouseEnter()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,-10));
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        //string carta = hit.collider.gameObject.name;

        if (hit)
        {
            Debug.Log($"Tocamos: {hit.collider.gameObject.name}, parent: {hit.collider.gameObject.name}");
        }
    }
}
