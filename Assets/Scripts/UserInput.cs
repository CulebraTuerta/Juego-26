using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    //public static UserInput instance;

    string nombreCarta;
    string nombrePadre;

    private Vector3 dragOffset;
    private Transform dragTransform;

    //private void Awake()
    //{
    //    instance = this; //creamos una instancia referida a si misma (es para poder acceder a las opciones de mouse
    //}
    private void Update()
    {
        //OnMouseEnter(); //ESTO SOLO FUNCIONABA PORQUE LO ESTABA LLAMANDO DESDE EL UPDATE, PARA QUE FUNCIONE ESE CALLBACK TIENE QUE ESTAR DENTRO DE UN OBJETO CON COLLIDER
        getMouseClick();
    }
    //private void OnMouseEnter()
    //{
    //    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,-100));
    //    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

    //    if (hit && hit.collider.CompareTag("carta"))
    //    {
    //        nombreCarta = hit.collider.name;
    //        nombrePadre = hit.collider.GetComponent<Seleccionable>().padre;
    //        Debug.Log($"Tocamos: {nombreCarta}, parent: {nombrePadre}");
    //    }

    //}

    public void getMouseClick() //ojo con los metodos, ya que tienen distinto nombre y el cero significa solo una referencia al boton izquierdo del raton
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        // Si se hace Click elegir el objeto bajo el mouse, entonces...
        if (Input.GetMouseButtonDown(0))
        {
            if (hit && hit.collider.CompareTag("carta"))
            {
                dragTransform = hit.collider.gameObject.transform;
                dragOffset = dragTransform.position - mousePosition;
            }
        }

        // Si estamos haciendo clic y el dragTransform no es falso, entonces...
        if (Input.GetMouseButton(0) && dragTransform != null)
        {
            dragTransform.position = mousePosition + dragOffset;
        }

        // Si soltamos
        if (Input.GetMouseButtonUp(0) && dragTransform != null)
        {
            dragTransform = null;
        }

    }



    //private void OnMouseDown()
    //{
    //    Vector3 mouse = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -100));
    //    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
    //    mouse.z = 0f;

    //    dragTransform = hit.collider.GetComponent<GameObject>().transform;
    //    dragOffset = transform.position - mouse;

    //}

    //private void OnMouseDrag()
    //{
    //    var mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    mouse.z = 0f;
    //    transform.position= mouse + dragOffset;
    //}

    //private void OnMouseUp()
    //{
    //    //aqui tengo que aplicar las reglas para ver si las cartas se pueden soltar o no
    //    //tengo que generar la carta arrastrada en el lugar de destino
    //    //tengo que borrar la carta desde origen 
    //    //tengo que borrar la carta de arrastre
    //    //tengo que ver si se corre hacia abajo o no (posicion de carta)
    //}
}
