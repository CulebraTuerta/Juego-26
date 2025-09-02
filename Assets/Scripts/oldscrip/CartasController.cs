using UnityEngine;
using System.Collections.Generic;

public class CartasController : MonoBehaviour  // script para comportamiento de las cartas, tiene que ponerse en cada carta existente o creada
{
    public PlayerDeckManager jugador;
    public MesaCentral mesa;

    public int manoSlot = -1; // ya que mano tiene 6 espacios disponibles

    private SpriteRenderer sr; //sprite de este script
    private GameObject dragGO;
    private SpriteRenderer dragSR;
    private Vector3 dragOffset;
    private int dragID = -1;

    private float separacionY = 0.20f;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnMouseEnter()
    {
        Debug.Log($"Hover: {name}; Tag: {gameObject.tag}; sprite: {sr.sprite.name}; ID: {GetIdByTag(gameObject.tag)}");
    }

    private int GetIdByTag(string tagName) //recordar que estamos sacando los id desde las listas del jugador, por eso accedemos asi. 
    {
        if (tagName == "CMano")
        {
            if (name == "Mano1") { manoSlot = 0; }
            else if (name == "Mano2") { manoSlot = 1; }
            else if (name == "Mano3") { manoSlot = 2; }
            else if (name == "Mano4") { manoSlot = 3; }
            else if (name == "Mano5") { manoSlot = 4; }
            else if (name == "Mano6") { manoSlot = 5; }

            if (jugador == null || manoSlot < 0 || manoSlot >= jugador.mano.Count) return -1; //metodo para que si hay algun error te de un -1
            return jugador.mano[manoSlot];
        }

        if (tagName == "CComodin")
        {
            return jugador.comodines[0];
        }
        if (tagName == "CEspacio")
        {
            if (name == "Espacio1") { return jugador.espacio_1[0]; }
            else if (name == "Espacio2") { return jugador.espacio_2[0]; }
        }

        return -1;
    }

    private void OnMouseDown() //para cuando hace clic, necesitamos copiar la carta
    {
        //crear una copia de la carta segun el id que identifique...
        dragGO = new GameObject("Carta_Drag");
        dragID = GetIdByTag(tag);
        dragSR = dragGO.AddComponent<SpriteRenderer>(); //con esto al nuevo objeto le agregamos un sprite render, que tiene que ser igual al que esta haciendo clic.
        dragSR.sprite = sr.sprite; //el sr es el render del objeto que tiene este script.
        dragSR.sortingLayerID = sr.sortingLayerID;
        dragSR.sortingOrder = 100; //Ponemos la carta bien arriba, para que no tope con nada.

        // Posición inicial y offset del mouse
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0f;
        dragGO.transform.position = transform.position; //copiamos la posicion del sprite render a este nuevo dragGO.
        dragOffset = dragGO.transform.position - mouse; //el offset sera con relacion al mouse

        // Esconder el sprite original mientras arrastro
        sr.enabled = false;
    }

    private void OnMouseDrag() //para cuando estamos llevando la carta a otro lado
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0f;
        dragGO.transform.position = mouse + dragOffset;
    }

    private void OnMouseUp() //esta parte del codigo hace toda la diferencia ya que aqui vemos como la carta interactua con el resto de la mesa,
                             //aqui se pondran las restricciones
    {
        if (dragGO == null) return; //si no hay un gameobject, gg

        // Buscar Receptor bajo el mouse
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0f;
        var hits = Physics2D.OverlapPointAll(mouse);

        Transform receptor = null; //receptor es el go donde tengo que poner las cartas. 
        int mejorOrden = int.MinValue;

        foreach (var h in hits)
        {
            if (!h) continue;

            var srHit = h.GetComponent<SpriteRenderer>();
            int orden = srHit ? srHit.sortingOrder : 0;
            if (orden >= mejorOrden)
            {
                mejorOrden = orden;
                receptor = h.transform; //esto me da el NAME del GO donde me esto posicionando... con esto tengo que jugar. 
            }
            Debug.Log($"receptor: {receptor}; tag: {receptor.gameObject.tag}");
        }

        Transform slot = RaizSlot(receptor);
        if (slot == null)
        {
            Destroy(dragGO);
            if (sr != null && sr.sprite != null) sr.enabled = true;
            return;
        }

        if (receptor != null)
        {
            string receptorTag = receptor.gameObject.tag; //sacamos el tag del receptor. 
            bool dropOk= false; //esto es para saber que si le achuntamos a un slot donde descargar si no volvemos a origen

            // filtramos aqui donde esta el receptor, porque luego cada cosa tiene su metodo para agregar las cartas. 
            if (receptorTag == "CComodin")
            {
                jugador.comodines.Add(dragID); //con esto se esta agregando a la lista!!! confirmado!!!
                CrearCartaEnReceptor(slot, dragSR.sprite,receptorTag); //metodo para crear la carta visualmente. le damos el receptor actual y le pasamos el sprite del drag
                LimpiarOrigen(); //ojo que no copiamos el receptorTag, porque queremos limpiar origen, apagamos el sprite
                dropOk = true;
            }
            //else if (receptorTag == "CEspacio")
            //{
            //    string nombreEspacio = slot.name;
            //    if (nombreEspacio == "Espacio1") { jugador.espacio_1.Add(dragID);}
            //    else if(nombreEspacio == "Espacio2") { jugador.espacio_2.Add(dragID);}
            //    else if(nombreEspacio == "Espacio3") { jugador.espacio_3.Add(dragID);}
            //    else if(nombreEspacio == "Espacio4") { jugador.espacio_4.Add(dragID);}

            //    CrearCartaEnReceptor(slot, dragSR.sprite); //metodo para crear la carta visualmente. le damos el receptor actual y le pasamos el sprite del drag
            //    LimpiarOrigen(); //ojo que no copiamos el receptorTag, porque queremos limpiar origen, apagamos el sprite
            //    dropOk = true;
            //}
            //else if (receptorTag == "CMonton")
            //{
            //    string nombreMonton = slot.name;
            //    if (nombreMonton == "Monton1") { mesa.monton_1.Add(dragID); }
            //    else if (nombreMonton == "Monton2") { mesa.monton_2.Add(dragID); }
            //    else if (nombreMonton == "Monton3") { mesa.monton_3.Add(dragID); }
            //    else if (nombreMonton == "Monton4") { mesa.monton_4.Add(dragID); }

            //    CrearCartaEnReceptor(slot, dragSR.sprite); //metodo para crear la carta visualmente. le damos el receptor actual y le pasamos el sprite del drag
            //    LimpiarOrigen(); //ojo que no copiamos el receptorTag, porque queremos limpiar origen, apagamos el sprite
            //    dropOk = true;
            //}

            Destroy(dragGO); //esto rompe todo el gameobject del drag y lo que conlleva
            if (!dropOk)
            {
                // drop no valido: volver el origen
                if (sr != null && sr.sprite != null) sr.enabled = true;
            }
        }
        else
        {
            Destroy(dragGO); //lo destruimos si no hay receptor
            if (sr != null && sr.sprite != null) sr.enabled = true; //activamos el sprite del origen
        }

    }
    private void CrearCartaEnReceptor(Transform receptor, Sprite sprite, string tagReceptor) // crea la carta visual como hija del receptor y la apila

    {
        if (receptor == null || sprite == null) return;

        int indice = ContarCartasVisuales(receptor);
        Vector3 pos = receptor.position + new Vector3(0f, -separacionY * indice, 0f);

        GameObject carta = new GameObject("CartaDrop_" + indice);
        var cSR = carta.AddComponent<SpriteRenderer>();
        cSR.sprite = sprite;

        // posicion en mundo, luego parent
        carta.transform.position = pos;
        carta.transform.rotation = Quaternion.identity;
        carta.transform.SetParent(receptor, true);

        // sorting por encima de lo existente
        var recSR = receptor.GetComponent<SpriteRenderer>();
        cSR.sortingLayerID = (recSR != null) ? recSR.sortingLayerID : (sr != null ? sr.sortingLayerID : 0);
        cSR.sortingOrder = SiguienteOrden(receptor);

        // collider para poder mover esta carta despues si quieres
        carta.AddComponent<BoxCollider2D>();
        carta.tag = tagReceptor;
    }

    private void LimpiarOrigen()
    {
        if (tag == "CMano") //el tag es el del origen asi que esta bien ponerlo solo como tag
        {
            // quita el id de la mano (tu metodo elimina por valor)
            jugador.mano.Remove(dragID);
            sr.sprite = null;
            sr.enabled = false;
        }
        if (tag == "CComodin")
        {
            jugador.comodines.Remove(dragID);
            //jugador.QuitarComodin(dragID);   //no se cual de los dos funciona... parecen que son iguales...
            sr.sprite = null;
            sr.enabled = false;

        }
        if(tag == "CEspacio")
        {
            if(name == "Espacio1") { jugador.espacio_1.Remove(dragID); }
            else if(name == "Espacio2") { jugador.espacio_2.Remove(dragID); }
            else if (name == "Espacio3") { jugador.espacio_3.Remove(dragID); }
            else if (name == "Espacio4") { jugador.espacio_4.Remove(dragID); }
            sr.sprite = null;
            sr.enabled = false;
        }
                
    }

    private int ContarCartasVisuales(Transform t)
    {
        int c = 0;
        for (int i = 0; i < t.childCount; i++)
            if (t.GetChild(i).GetComponent<SpriteRenderer>() != null)
                c++;
        return c;
    }
    private int SiguienteOrden(Transform t)
    {
        int maxOrder = 0;
        var pSR = t.GetComponent<SpriteRenderer>();
        if (pSR != null) maxOrder = pSR.sortingOrder;

        for (int i = 0; i < t.childCount; i++)
        {
            var s = t.GetChild(i).GetComponent<SpriteRenderer>();
            if (s != null && s.sortingOrder > maxOrder) maxOrder = s.sortingOrder;
        }
        return maxOrder + 1;
    }
    private bool EsSlot(Transform t)
    {
        if (t == null) return false;
        string tg = t.gameObject.tag;
        return tg == "CComodin" || tg == "CEspacio" || tg == "CMonton";
    }

    private Transform RaizSlot(Transform t)
    {
        while (t != null && !EsSlot(t))
            t = t.parent;
        return t; // null si no encontró un slot con esos tags
    }
}
