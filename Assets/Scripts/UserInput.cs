using System.Linq;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class UserInput : MonoBehaviour
{
    Transform dragTransform;
    Vector3 dragOffset;
    Vector3 posicionInicial;
    Vector3 scaleOriginal;
    private int orderOriginal;
    public GameObject descartePos;
    private int sorterOrdenArrastre = 100;
    private float scaleHover = 1.2f;

    public float separacionY = 0.22f; //esto es para mover las cartas hacia abajo cuado las montemos
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
                Debug.Log($"Padre: {hit.GetComponent<Seleccionable>().padre}");
                //Debug.Log($"Father: {PadreBajoMouse(mouse)}");
            }
        }

        //-------------------------------------------------------------------------
        // cuando hago clic
        //-------------------------------------------------------------------------
        if (Input.GetMouseButtonDown(0))
        {

            //string cartaPadre = PadreBajoMouse(mouse).name;
            var hitCarta = TopCartaBajoMouse(mouse); //el metodo nos retorna un gameobject de donde esta el mouse, y nos da la carta de mas arriba.
            if (hitCarta != null) //si le hicimos clic a una carta
            {
                var sel=hitCarta.GetComponent<Seleccionable>();
                if (sel != null && sel.padre == "CMazo" && sel.faceUp == false) //si es un objeto del mazo jugador y boca abajo, entonces la damos vuelta
                {
                    sel.faceUp = true;
                    dragTransform = null;
                    return;
                }

                //sino, solo la arrastramos (aplica para todas las cartas, menos las del mazo central). 
                else if (sel.padre != "CMazoCentral" && sel.padre != "CMonton") //Y las cartas del monton (ya no se pueden mover) (BUG.007)
                {
                    if(sel.padre=="CEspacio")
                    {
                        //Debug.Log("estoy tomando una carta desde el espacio");
                        //Debug.Log($"sortin order: {sel.GetComponent<SpriteRenderer>().sortingOrder}; contar cartas: {ContarCartas(PadreBajoMouse(mouse).transform)}");
                        if(sel.GetComponent<SpriteRenderer>().sortingOrder == ContarCartas(PadreBajoMouse(mouse).transform)) // BUG.002
                        {
                            Debug.Log("esta carta es la de arriba");
                            dragTransform = hitCarta.transform;
                            posicionInicial = dragTransform.position;
                            scaleOriginal = dragTransform.localScale;
                            dragOffset = dragTransform.position - mouse;
                            orderOriginal = dragTransform.GetComponent<SpriteRenderer>().sortingOrder;
                        }
                        else
                        {
                            Debug.Log("esta cata no es la de mas arriba, no la puedes mover");
                        }

                    }
                    else // todo los otros lugares para tomar cartas
                    {
                        //Debug.Log("estoy tomando otra carta que no es del espacio");
                        dragTransform = hitCarta.transform;
                        posicionInicial = dragTransform.position;
                        scaleOriginal = dragTransform.localScale;
                        dragOffset = dragTransform.position - mouse;
                        orderOriginal = dragTransform.GetComponent<SpriteRenderer>().sortingOrder;
                    }
                          
                }
                
            }

        }

        //-------------------------------------------------------------------------
        //cuando mantengo apretado el clic
        //-------------------------------------------------------------------------
        if (Input.GetMouseButton(0) && dragTransform != null) //Con este metodo hacemos que cuando este presionado, lleve la carta pegada al mouse
        {
            dragTransform.position = mouse + dragOffset;
            dragTransform.localScale = scaleOriginal * scaleHover; //aumentamos un poco el tamaño de la carta al arrastrar
            dragTransform.GetComponent<SpriteRenderer>().sortingOrder = orderOriginal + sorterOrdenArrastre; //aumentamos su sorter order para que se vea sobre todas las cartas del tablero
        }

        //-------------------------------------------------------------------------
        //cando soltamos el clic  (UNDER DEVELOPMENT)
        //-------------------------------------------------------------------------
        if(Input.GetMouseButtonUp(0) && dragTransform != null)
        {
            var destino = PadreBajoMouse(mouse);
            if (destino == null) //no soltamos en un padre valido (basicamente en un espacio vacio)
            {
                dragTransform.position = posicionInicial;
                dragTransform.localScale=scaleOriginal;
                dragTransform = null;
                return;
            }

            string nombreCarta = dragTransform.name;
            string nombreDestino = destino.name;

            //=====================================
            //REGLAS PARA SOLTAR CARTAS
            //=====================================

            //*****************
            //COMODINES
            //*****************
            if (nombreDestino == "Comodines")
            {
                dragTransform.localScale = scaleOriginal;
                dragTransform.GetComponent<SpriteRenderer>().sortingOrder = orderOriginal; //creo que es innecesario pero lo dejare para evitar problemas
                if (nombreCarta.EndsWith("k")||nombreCarta.EndsWith("N")||nombreCarta.EndsWith("R"))
                {
                    int order = SiguienteOrden(destino.transform);
                    Apilar(dragTransform.gameObject, destino.transform, ref order, 0f); //el cero es porque no tenemos que moverlas hacia abajo, solo las apila.
                    VerificarMano();
                }
                else //vuelvo a posicion inicial
                {
                    dragTransform.position = posicionInicial;
                    dragTransform = null;
                    return;
                }
                dragTransform.GetComponent<Seleccionable>().padre = "CComodin";
            }

            //*****************
            //ESPACIOS (de jugador, descarte)   (Aqui esta relacionado el bug: BUG.001, creo)
            //*****************
            else if (nombreDestino.StartsWith("Espacio"))
            {
                dragTransform.localScale = scaleOriginal;
                dragTransform.GetComponent<SpriteRenderer>().sortingOrder = orderOriginal; //creo que es innecesario pero lo dejare para evitar problemas
                if (nombreCarta.EndsWith("k") || nombreCarta.EndsWith("N") || nombreCarta.EndsWith("R") || nombreCarta.EndsWith("a"))
                {
                    //Debug.Log($"No se permite esta carta ({nombreCarta}) aqui");
                    dragTransform.position = posicionInicial;
                    dragTransform = null;
                    return;
                }
                else if (dragTransform.GetComponent<Seleccionable>().padre == "CMazo") //(BUG.006) Se agrego esta linea de codigo para no permitir su colocacion aqui. 
                {
                    //Debug.Log("Esta carta qla viene del mazo jugador, no se puede colocar en espacio");
                    dragTransform.position = posicionInicial;
                    dragTransform = null;
                    return;
                }
                else
                {
                    int order = SiguienteOrden(destino.transform);
                    Apilar(dragTransform.gameObject, destino.transform, ref order, separacionY);
                    juego.TerminarTurno();
                }
                dragTransform.GetComponent<Seleccionable>().padre = "CEspacio";
            }

            //*****************
            //MONTONES (De mesa central)
            //*****************
            else if (nombreDestino.StartsWith("Monton"))
            {
                // Reglas:
                // - Si esta vacio, solo un as puede iniciar
                // - Si no está vacío, la carta debe coincidir con (cantidadHijos + 1).
                // - Comodín se puede usar arriba salvo que el tope también sea comodín.
                dragTransform.localScale = scaleOriginal;
                dragTransform.GetComponent<SpriteRenderer>().sortingOrder = orderOriginal; //creo que es innecesario pero lo dejare para evitar problemas

                int cartasEnDestino = ContarCartas(destino.transform);
                string nombreUltimaCarta = NombreUltimaCarta(destino.transform);

                // Al colocar un AS
                if(nombreCarta.EndsWith("a") && cartasEnDestino ==0)
                {
                    int order = SiguienteOrden(destino.transform);
                    Apilar(dragTransform.gameObject, destino.transform, ref order, 0f);
                    VerificarMano();
                }

                // Al colocar un COMODIN
                else if(nombreCarta.EndsWith("k") || nombreCarta.EndsWith("N") || nombreCarta.EndsWith("R"))
                {
                    //Si la ultima carta es un comodin o un J (para arreglar que ponga un comodin como Q) (BUG.003)
                    if(nombreUltimaCarta.EndsWith("k") || nombreUltimaCarta.EndsWith("N") || nombreUltimaCarta.EndsWith("R")||nombreUltimaCarta.EndsWith("j"))
                    {
                        Debug.Log("No es posible colocar dos comodines seguidos o estas poniendo un comodin como Q");
                        dragTransform.position = posicionInicial;
                        dragTransform = null;
                        return;
                    }
                    else if(cartasEnDestino == 0)
                    {
                        Debug.Log("No es posible un comodin como inicial");
                        dragTransform.position = posicionInicial;
                        dragTransform = null;
                        return;
                    }
                    else
                    {
                        int order = SiguienteOrden(destino.transform);
                        Apilar(dragTransform.gameObject, destino.transform, ref order, 0f);
                        VerificarMano();                        
                    }
                }

                // Al colocar cualquier otra carta
                else
                {
                    dragTransform.localScale = scaleOriginal;
                    dragTransform.GetComponent<SpriteRenderer>().sortingOrder = orderOriginal; //creo que es innecesario pero lo dejare para evitar problemas
                    int valorCarta = ValorNumerico(nombreCarta);
                    
                    if(valorCarta == cartasEnDestino +1 && cartasEnDestino >0)
                    {
                        if(valorCarta==12)
                        {
                            int order = SiguienteOrden(destino.transform);
                            Apilar(dragTransform.gameObject, destino.transform, ref order, 0f);
                            VerificarMano();

                            int ordenDescarte = 0;
                            while(destino.transform.childCount>0) //mientras mi Monton tenga cartas
                            {
                                Transform carta = destino.transform.GetChild(0); // agarro la primera carta
                                if (carta.CompareTag("carta"))
                                {
                                    Debug.Log("Descartando monton");
                                    Apilar(carta.gameObject,descartePos.transform, ref ordenDescarte, 0f);
                                }
                            }
                        }
                        else
                        {
                            int order = SiguienteOrden(destino.transform);
                            Apilar(dragTransform.gameObject, destino.transform, ref order, 0f);
                            VerificarMano();
                        }

                    }
                    else
                    {
                        Debug.Log("No es posible colocar esta carta (valor no valido)");
                        dragTransform.position = posicionInicial;
                        //dragTransform.localScale = scaleOriginal;
                        dragTransform = null;
                        return;
                    }
                }
                dragTransform.GetComponent<Seleccionable>().padre = "CMonton"; //parte del BUG.007
            }

            else
            {
                // otros
                Debug.Log("Lugar no reconocido");
                dragTransform.position = posicionInicial;
                dragTransform.localScale = scaleOriginal;
                dragTransform.GetComponent<SpriteRenderer>().sortingOrder = orderOriginal; //creo que es innecesario pero lo dejare para evitar problemas
                dragTransform = null;
                return;
            }

            // listo
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

    private Collider2D PadreBajoMouse(Vector3 mouse)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(mouse);
        Collider2D padreCollider = null;
        int mejorOrden = int.MinValue; //le ponemos el valor mas bajo para un entero (simplemente para no poner -1)  

        foreach (Collider2D h in hits)
        {
            if (h == null || !h.CompareTag("padre")) continue; //si entre todos los collider que detecta no hay ninguno con tag "padre", entonces dejara padrecollider como null
            var sr = h.GetComponent<SpriteRenderer>(); //esto es para tener informacion de su sortingOrder
            int orden = sr ? sr.sortingOrder : 0; //basicamente aqui digo que orden toma el valor de sr.sortingOrder, si no encuentra ningun sprite render, lo coloca como 0
            if (orden >= mejorOrden)
            {
                mejorOrden = orden;
                padreCollider = h; //con esto cada vez que el orden sea mayor al mejorOrden encontrado, guardaremos el collider como si fuera Top.
            }
        }
        return padreCollider;        
    }

    private int SiguienteOrden(Transform ancla) //con este metodo podemos ver el ancla que necesitemos revisar
                                                //y entregamos de vuelta el orden que deberia tener la carta que se quiere poner
    {
        int max = 0;
        for (int i = 0; i < ancla.childCount; i++) //hago una revision por todos los hijos del ancla
        {
            max = ancla.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder;
        }
        return max += 1;
    }
    private void Apilar(GameObject carta, Transform ancla, ref int orderCursor, float sepY) //metodo llegado y copiado (NO REVISADO)
    {
        if (!carta || !ancla) return;

        // índice visible = cuántas cartas tiene ya el ancla
        int indice = 0;
        for (int i = 0; i < ancla.childCount; i++)
            if (ancla.GetChild(i).CompareTag("carta")) indice++;

        var pos = ancla.position + new Vector3(0f, -sepY * indice, 0f);
        pos.z = carta.transform.position.z; // z se mantiene

        carta.transform.position = pos;
        carta.transform.SetParent(ancla, true);

        var sr = carta.GetComponent<SpriteRenderer>();
        if (sr)
        {
            sr.sortingOrder = orderCursor++;
            var srAncla = ancla.GetComponent<SpriteRenderer>();
            if (srAncla) sr.sortingLayerID = srAncla.sortingLayerID; // hereda layer del ancla (opcional)
        }

        var sel = carta.GetComponent<Seleccionable>();
        if (sel) sel.faceUp = true;
    }

    private int ContarCartas(Transform ancla)
    {
        int cartas = 0;
        for (int i = 0;i < ancla.childCount;i++)
        {
            if (ancla.GetChild(i).CompareTag("carta")) {  cartas++; }
        }
        return cartas;
    }

    private string NombreUltimaCarta(Transform ancla)
    {
        GameObject ultimaCarta = null;
        int mejorOrder = int.MinValue;

        for (int i = 0; i < ancla.childCount; i++)
        {
            var cartaActual = ancla.GetChild(i);
            if (!cartaActual.CompareTag("carta")) continue; // si es el padre lo saltamos
            var sr = cartaActual.GetComponent<SpriteRenderer>(); //esto es para tener informacion de su sortingOrder
            int orden = sr ? sr.sortingOrder : 0; //basicamente aqui digo que orden toma el valor de sr.sortingOrder, si no encuentra ningun sprite render, lo coloca como 0
            if (orden >= mejorOrder)
            {
                mejorOrder = orden;
                ultimaCarta = cartaActual.gameObject; //con esto cada vez que el orden sea mayor al mejorOrden encontrado, guardaremos el gameobject como si fuera la ultima carta.
            }
        }
        return ultimaCarta ? ultimaCarta.name : "";
    }

    private int ValorNumerico(string nombre)
    {
        if (nombre.EndsWith("2")) return 2;
        if (nombre.EndsWith("3")) return 3;
        if (nombre.EndsWith("4")) return 4;
        if (nombre.EndsWith("5")) return 5;
        if (nombre.EndsWith("6")) return 6;
        if (nombre.EndsWith("7")) return 7;
        if (nombre.EndsWith("8")) return 8;
        if (nombre.EndsWith("9")) return 9;
        if (nombre.EndsWith("0")) return 10;
        if (nombre.EndsWith("j")) return 11;
        if (nombre.EndsWith("q")) return 12;
        return -1; //para otros casos
    }

    private void VerificarMano() //BUG.009
    {
        Debug.Log($"Verificando Mano: {juego.cantidadCartasMano - 1}");
        if (juego.cantidadCartasMano-1 <= 0) //colocar en cero para el juego original.
        {
            juego.TerminarTurno();
        }
    }
}
