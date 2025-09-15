using UnityEngine;
using UnityEngine.Rendering;

public class UserInput : MonoBehaviour
{
    Transform dragTransform;
    Vector3 dragOffset;
    Vector3 posicionInicial;
    //string nombreUltimaCarta = "";

    public float separacionY = 0.18f; //esto es para mover las cartas hacia abajo cuado las montemos
    public GameController juego; //esto no lo estamos usando
    //public int jugadorActual = 0; //todas las acciones seran con el jugador 1.

    void Update()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0f;

        //-------------------------------------------------------------------------
        // Debug clic derecho
        //-------------------------------------------------------------------------
        if (Input.GetMouseButtonDown(1))
        {
            var hit = TopCartaBajoMouse(mouse); //hit es un gameobject
            if (hit != null)
            {
                Debug.Log("Carta: " + hit.name);
            }
        }

        //-------------------------------------------------------------------------
        // cuando hago clic
        //-------------------------------------------------------------------------
        if (Input.GetMouseButtonDown(0))
        {
            var hitCarta = TopCartaBajoMouse(mouse); //el metodo nos retorna un gameobject de donde esta el mouse, y nos da la carta de mas arriba.
            if (hitCarta != null) //si le hicimos clic a una carta
            {
                var sel=hitCarta.GetComponent<Seleccionable>();
                if (sel != null&&sel.padre == "CMazo" && sel.faceUp == false) //si es un objeto del mazo jugador y boca abajo, entonces la damos vuelta
                {
                    sel.faceUp = true;
                    dragTransform = null;
                    return;
                }

                //sino, solo la arrastramos (aplica para todas las cartas, menos las del mazo central). 
                else if(sel.padre!="CMazoCentral")
                {
                    dragTransform = hitCarta.transform;
                    posicionInicial = dragTransform.position;
                    dragOffset = dragTransform.position - mouse;
                }
            }

        }

        //-------------------------------------------------------------------------
        //cuando mantengo apretado el clic
        //-------------------------------------------------------------------------
        if (Input.GetMouseButton(0) && dragTransform != null) //Con este metodo hacemos que cuando este presionado, lleve la carta pegada al mouse
        {
            dragTransform.position = mouse + dragOffset;
        }

        //-------------------------------------------------------------------------
        //cando soltamos el clic  (UNDER DEVELOPMENT)
        //-------------------------------------------------------------------------

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
                        {
                            if (destino.transform.GetChild(i).CompareTag("carta"))
                            {
                                cantidadHijos++;
                            }
                        }

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
                        {
                            if (destino.transform.GetChild(i).CompareTag("carta"))
                            {
                                cantidadHijos++;
                            }
                        }

                        dragTransform.SetParent(destino.transform, true);

                        Vector3 posicionDestino = destino.transform.position + new Vector3(0f, -0.4f * cantidadHijos, 0f);
                        posicionDestino.z = destino.transform.position.z + (-0.03f * cantidadHijos);


                        //------------------------------------
                        // Esto ultimo hace que se pongan las caratas sobre la posicion del padre, y se aplilan hacia abajo, perfecto,
                        // pero ahora necesitamos que considere solo la posicion de la ultima carta, ya que esa sera la unica que se podra hacer clic o 
                        // la unica donde se podra poner una carta encima. 
                        //------------------------------


                        dragTransform.position = posicionDestino;
                        juego.TerminarTurno(); //con esto hacemos cambio de jugador y terminamos el turno, al terminar el otro jugador se rellena mi mano
                    }
                    else
                    {
                        Debug.Log("No es una carta valida (estas poniendo comodines o un as)");
                        dragTransform.position = posicionInicial; //vuelve a pos inicial
                    }
                }

                else if (nombreDestino.StartsWith("Monton"))
                {
                    int numeroCarta = 0;
                    int cantidadHijos = 0;
                    //nombreUltimaCarta = "";

                    if (true)
                    {
                        if (!idCarta.EndsWith("a") && !idCarta.EndsWith("k") && !idCarta.EndsWith("q") && !idCarta.EndsWith("j") && !idCarta.EndsWith("N") && !idCarta.EndsWith("R"))
                        {
                            //con esto paso el numero de la carta a entero
                            numeroCarta = int.Parse(idCarta.Substring(idCarta.Length - 1));
                        }
                        else if (idCarta.EndsWith("0"))
                        {
                            numeroCarta = 10;
                        }
                        else if (idCarta.EndsWith("j"))
                        {
                            numeroCarta = 11;
                        }
                        else if (idCarta.EndsWith("q"))
                        {
                            numeroCarta = 12;
                        }
                    }

                    for (int i = 0; i < destino.transform.childCount; i++)
                    {
                        if (destino.transform.GetChild(i).CompareTag("carta"))
                        {
                            //nombreUltimaCarta = destino.name;
                            cantidadHijos++;
                        }
                    }

                    if (idCarta.EndsWith("a") && cantidadHijos == 0)
                    {
                        Debug.Log("Carta termina con A");
                        dragTransform.SetParent(destino.transform, true); // hago que mi arrastre se convierta en hijo de la posicion destino.
                        Vector3 posicionDestino = destino.transform.position; //destino sera copia de la posicion del padre
                        posicionDestino.z = -0.03f * cantidadHijos; //aumentamos la posicion en z hacia arriba visualmente. 
                        dragTransform.position = posicionDestino;
                    }
                    else if (numeroCarta == cantidadHijos + 1)
                    {
                        Debug.Log("numero carta es igual a la cantidad hijos");
                        dragTransform.SetParent(destino.transform, true); // hago que mi arrastre se convierta en hijo de la posicion destino.
                        Vector3 posicionDestino = destino.transform.position; //destino sera copia de la posicion del padre
                        posicionDestino.z = -0.03f * cantidadHijos; //aumentamos la posicion en z hacia arriba visualmente. 
                        dragTransform.position = posicionDestino;
                    }
                    else if (idCarta.EndsWith("k") || idCarta.EndsWith("N") || idCarta.EndsWith("R"))
                    {
                        Debug.Log("la carta es un comodin");
                        //if (nombreUltimaCarta.EndsWith("k")||nombreUltimaCarta.EndsWith("N") || nombreUltimaCarta.EndsWith("R")) //verificar si no hay otro comodin abajo
                        //{
                        //    Debug.Log("nombre de ultima carta es comodin, no se puede poner un comodin");                            
                        //}
                        //else
                        //{
                        //    Debug.Log("la ultima carta no es comodin asi que podemos usar un comodin");
                        //    dragTransform.SetParent(destino.transform, true); // hago que mi arrastre se convierta en hijo de la posicion destino.
                        //    Vector3 posicionDestino = destino.transform.position; //destino sera copia de la posicion del padre
                        //    posicionDestino.z = -0.03f * cantidadHijos; //aumentamos la posicion en z hacia arriba visualmente. 
                        //    dragTransform.position = posicionDestino;
                        //}
                    }
                    else
                    {
                        Debug.Log("No es una carta valida (estas poniendo algo que no es AS o no cumple con los requisitos)");
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

    //HELPERS
    private GameObject TopCartaBajoMouse(Vector3 mouse)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(mouse);
        GameObject top = null;
        int mejorOrden = int.MinValue; //le ponemos el valor mas bajo para un entero (simplemente para no poner -1)  

        foreach (Collider2D h in hits)
        {
            if (h == null || !h.CompareTag("carta")) continue; //si entre todos los collider que detecta no hay ninguno con tag "carta", entonces dejara top como null
            var sr = h.GetComponent<SpriteRenderer>(); //esto es para tener informacion de su sortingOrder
            int orden = sr ? sr.sortingOrder : 0; //basicamente aqui digo que orden toma el valor de sr.sortingOrder, si no encuentra ningun sprite render, lo coloca como 0
            if (orden >= mejorOrden)
            {
                mejorOrden = orden;
                top = h.gameObject; //con esto cada vez que el orden sea mayor al mejorOrden encontrado, guardaremos el collider como si fuera Top.
            }
        }
        return top;
    }
}
