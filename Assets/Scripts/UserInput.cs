using UnityEngine;

public class UserInput : MonoBehaviour
{
    Transform dragTransform;
    Vector3 dragOffset;
    Vector3 posicionInicial;

    public float separacionY = 0.18f; //esto es para mover las cartas hacia abajo cuado las montemos

    void Update()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0f;

        if (Input.GetMouseButtonDown(0))
        {
            Collider2D hit = Physics2D.OverlapPoint(mouse); //El colider OverlapPoint ve inmediatamente el primer colider que esta bajo esta posicion.
            if (hit != null && hit.CompareTag("carta")) //En este caso veremos que lo primero que esta bajo el mouse son las cartas que tienen que llevar el tag "carta"
            {
                //todo lo que va aqui es para mover la carta 
                dragTransform = hit.transform;
                posicionInicial = dragTransform.position;
                dragOffset = dragTransform.position - mouse;
            }
        }

        if (Input.GetMouseButton(0) && dragTransform != null) //Con este metodo hacemos que cuando este presionado, lleve la carta pegada al mouse
        {
            dragTransform.position = mouse + dragOffset;
        }

        if (Input.GetMouseButtonDown(1))
        {
            Collider2D[] hits = Physics2D.OverlapPointAll(mouse); //este es un arreglo de colliders donde vemos todos los que estan bajo esa posicion
            int cartasYaEnDestino = hits.Length - 1;
            Debug.Log("cartas en la wea " + cartasYaEnDestino);
        }

        if (Input.GetMouseButtonUp(0) && dragTransform != null) //aqui es donde deberiamos ver todas las reglas para cuando soltamos la carta
        {
            Collider2D[] hits = Physics2D.OverlapPointAll(mouse); //este es un arreglo de colliders donde vemos todos los que estan bajo esa posicion
            Collider2D destino = null; //creamos un collider 2d llamado "destino"

            foreach (var h in hits) //con esto buscamos el collider que tenga el tag padre (osea que esta vacio el lugar) y hacemos que destino sea ese padre
            { 
                if (h.CompareTag("padre"))
                { 
                    destino = h;
                    break; 
                } 
            } 

            if (destino != null)
            {
                string idCarta = dragTransform.name;     
                string nombreDestino = destino.name;

                if (nombreDestino == "Comodines") 
                {

                    if (idCarta.EndsWith("k") || idCarta.EndsWith("N") || idCarta.EndsWith("R"))
                    {
                        int cantidadHijos = 0;
                        for (int i = 0; i < destino.transform.childCount; i++)
                            if (destino.transform.GetChild(i).CompareTag("carta"))
                                cantidadHijos++;

                        dragTransform.SetParent(destino.transform, true); // hago que mi arrastre se convierta en hijo de la posicion destino.
                        Vector3 posicionDestino = destino.transform.position; //destino sera copia de la posicion del padre
                        posicionDestino.z = -0.03f * cantidadHijos; //aumentamos la posicion en z hacia arriba visualmente. 
                        dragTransform.position = posicionDestino;                        
                    }
                    else
                    {
                        Debug.Log("No es un comodin");
                        dragTransform.position = posicionInicial; //vuelve a pos inicial
                    }
                }

                else if (nombreDestino.StartsWith("Espacio"))
                {
                    if (!idCarta.EndsWith("k") && !idCarta.EndsWith("N") && !idCarta.EndsWith("R") && !idCarta.EndsWith("a"))
                    {
                        int cantidadHijos = 0;
                        for (int i = 0; i < destino.transform.childCount; i++)
                            if (destino.transform.GetChild(i).CompareTag("carta"))
                                cantidadHijos++;

                        dragTransform.SetParent(destino.transform, true);

                        Vector3 posicionDestino = destino.transform.position + new Vector3(0f, -0.4f * cantidadHijos, 0f);
                        posicionDestino.z = destino.transform.position.z + (-0.03f * cantidadHijos);
                        
                        
                        //------------------------------------
                        // Esto ultimo hace que se pongan las caratas sobre la posicion del padre, y se aplilan hacia abajo, perfecto,
                        // pero ahora necesitamos que considere solo la posicion de la ultima carta, ya que esa sera la unica que se podra hacer clic o 
                        // la unica donde se podra poner una carta encima. 
                        //------------------------------


                        dragTransform.position = posicionDestino;

                    }
                    else
                    {
                        Debug.Log("No es una carta valida (estas poniendo comodines o un as)");
                        dragTransform.position = posicionInicial; //vuelve a pos inicial
                    }
                }


                else
                {
                    dragTransform.position = posicionInicial; //vuelve a pos inicial
                }

            }
            else
            {
                dragTransform.position = posicionInicial; //vuelve a pos inicial 
            }
            dragTransform = null;
        }
    }
}
