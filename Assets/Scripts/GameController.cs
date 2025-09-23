using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class GameController : MonoBehaviour
{
    public Sprite[] spritesCartas;
    public GameObject PrefabCarta;
    public int cantidadDeJugadores = 2;
    public int jugadorActual = 0; //siempre parte el jugador 1
    public List<string> mazoCentral;
    public float zOffset = 0.03f;
    public float segundos = 0.03f;
    public int cantidadCartasMano = 6;

    //-------------------------------------------------------------------------------------------------------------------
    //POSICIONES DE LOS GO (LOS BORDES)
    public GameObject[] montonesPos;
    public GameObject mazoCentralPos;
    public GameObject descartePos;
    public GameObject[] mazosJugadoresPos;
    public GameObject[] comodinesJugadoresPos; //posiciones de los comodines de los jugadores 
    public GameObject[] manoJ1Pos; //todas las posiciones de la mano del jugador1
    public GameObject[] manoJ2Pos;
    public GameObject[] manoJ3Pos;
    public GameObject[] manoJ4Pos;
    //falta incluir posiciones del tablero del jugador(es): comodines, mazo, espacios.
    //-------------------------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------------------------
    //ARREGLO DE LISTAS
    public List<string>[] mazosJugadores;
    public List<string>[] manosJugadores; //lista de todas las manos de los jugadores
    public GameObject[][] manosPos;
    //falta incluir los otros jugadores y espacios
    //-------------------------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------------------------
    //LISTA DE CADA UNO DE LOS ESPACIOS INTERACTUABLES
    public List<string> mazoJ1 = new List<string>();
    public List<string> mazoJ2 = new List<string>();
    public List<string> mazoJ3 = new List<string>();
    public List<string> mazoJ4 = new List<string>();
    public List<string> manoJ1 = new List<string>(); //lista de la mano del jugador 1
    public List<string> manoJ2 = new List<string>();
    public List<string> manoJ3 = new List<string>();
    public List<string> manoJ4 = new List<string>();
    public TextMeshPro textoJugadorActual;
    //public TextMeshPro textoComodinRemplazaM1;

    void Start()
    {
        mazosJugadores = new List<string>[] { mazoJ1, mazoJ2, mazoJ3, mazoJ4, };
        manosJugadores = new List<string>[] { manoJ1, manoJ2, manoJ3, manoJ4, };
        manosPos = new GameObject[][] { manoJ1Pos, manoJ2Pos, manoJ3Pos, manoJ4Pos };
        IniciarJuego();        
    }

    private void Update()
    {
        textoJugadorActual.text = "Jugador Actual: J" + (jugadorActual+1).ToString();
        cantidadCartasMano = ContarCartasMano();
        VerificarMazoCentral();
    }

    public void IniciarJuego()
    {
        LimpiarTodo();
        mazoCentral = GenerarMazoCentral();
        Mezclar(mazoCentral);
        RepartirJugadores();
        MostrarCartas();
    }

    public List<string> GenerarMazoCentral() 
    {
        string[] pintas = new string[] { "t", "d", "c", "p" };
        string[] valores = new string[] { "2", "3", "4", "5", "6", "7", "8", "9", "10", "a", "j", "q", "k" };
        List<string> nuevoMazo = new List<string>();
        
        int decks = 3;
        if (cantidadDeJugadores < 4) //si es 4 entonces va a generar 3 decks
        {
            decks = 2;
            //decks = 1; //SOLO PARA USO DE DEBUGEOOOOO BORRAR DESPUES 
        }
        for (int d = 0; d < decks; d++)
        {
            foreach (var p in pintas)
            {
                foreach (var r in valores)
                { 
                    nuevoMazo.Add(p + r);
                }
            }
            nuevoMazo.Add("jokerR");
            nuevoMazo.Add("jokerN");
        }        
        return nuevoMazo;
    }

    public void Mezclar(List<string> mazo)
    {
        //aqui las mezcla despues de crear
        System.Random random = new System.Random();
        int n = mazo.Count;
        while (n > 1)
        {
            int k = random.Next(n);
            n--;
            string temp = mazo[k];
            mazo[k] = mazo[n];
            mazo[n] = temp;
        }
    }

    private void RepartirJugadores()
    {
        int cantidadaARepartir = 20; //COLOCAR EN 20 PARA JUEGO OFICIAL
        for (int i = 0; i < cantidadDeJugadores; i++)
        {
            for (int j = 0; j < cantidadaARepartir; j++) //con esto reparto 20 cartas a cada jugador segun la cantidad de jugadore
            {
                mazosJugadores[i].Add(mazoCentral.Last<string>());
                mazoCentral.RemoveAt(mazoCentral.Count - 1);
            }
            for (int k = 0; k < 6; k++) //y las 6 cartas de la mano de cada jugador
            {
                manosJugadores[i].Add(mazoCentral.Last<string>());
                mazoCentral.RemoveAt(mazoCentral.Count - 1);
            }
        }
    }

    private void MostrarCartas() //realmente es un metodo para COLOLAR las cartas en la mesa de juego
    {
        //------------------------------------------------------------------
        //MOSTRAR CARTAS EN EL MAZO CENTRAL
        int ordenMazoCentral = 0; //chatgpt me dio un metodo medio raro, pero finalmente decia que lo dejaba en cero al iniciar... 
        foreach (string carta in mazoCentral)
        {
            GameObject cartita = Instantiate(PrefabCarta, mazoCentralPos.transform.position, Quaternion.identity);
            cartita.name = carta;
            cartita.tag = "carta";
            cartita.GetComponent<Seleccionable>().faceUp = false;
            cartita.GetComponent<Seleccionable>().setPadre("CMazoCentral");
            ApilarEnAncla(cartita,mazoCentralPos.transform,ref ordenMazoCentral); //con este nuevo metodo estamos haciendo lo de colocar en orden en el mazo.
            //OBSERVACION, el ref dentro de aplilarEnAncla hace subir el valor de donde se esta evaluando( el metodo aumentara el ordenMazoCentral simplemente por estar referenciado)
        }

        //------------------------------------------------------------------
        //MOSTRAR CARTAS EN LOS MAZOS JUGADORES  (solo la ultima ira boca arriba)
        for (int i = 0; i < cantidadDeJugadores; i++) //por cada jugador, instanciaremos sus cartas de sus respectivos mazos
        {
            int ordenMazoJ = 0;
            foreach (string carta in mazosJugadores[i])
            {
                GameObject cartita = Instantiate(PrefabCarta, mazosJugadoresPos[i].transform.position, Quaternion.identity);
                cartita.name = carta;
                cartita.tag = "carta";
                cartita.GetComponent<Seleccionable>().setPadre("CMazo");
                if (ordenMazoJ == 19)
                {
                    cartita.GetComponent<Seleccionable>().faceUp = true;
                }
                else { cartita.GetComponent<Seleccionable>().faceUp = false; }
                ApilarEnAncla(cartita, mazosJugadoresPos[i].transform, ref ordenMazoJ);
            }
        }
        //------------------------------------------------------------------
        //MOSTRAR CARTAS EN LAS MANOS JUGADORES
        for (int i = 0; i < cantidadDeJugadores; i++) //por cada jugador, instanciaremos sus cartas de sus manos (6 cartas)
        {
            int j = 0; //cada que inicia la repartida de mano de cada jugador, J tiene que ser 0
            //Debug.Log($"Recorrido de jugador{i+1},con i={i} y j={j}");
            foreach (string carta in manosJugadores[i]) //en el J1 (i=0), en este caso cada carta en ManoJ1 (es una lista de cartas string)
            {
                GameObject cartita = Instantiate(PrefabCarta, manosPos[i][j].transform.position, Quaternion.identity); //en este caso el transform position es del GO del juego26
                cartita.name = carta; //asi toma el nombre del string carta in manosjugador[i]
                cartita.tag = "carta";
                cartita.GetComponent<Seleccionable>().setPadre("CMano");
                cartita.GetComponent<Seleccionable>().faceUp = true; //todas visibles
                int ordenCartaMano = 0; //con esto hacemos que las manos siempre esten en el orden CERO. 
                //Debug.Log($"Apilando en manosPos[{i}][{j}]");
                ApilarEnAncla(cartita, manosPos[i][j].transform, ref ordenCartaMano);
                j++;
            }
        }
    }

    private void ApilarEnAncla(GameObject cartaGO, Transform ancla, ref int orderCursor)
    {
        //con esto hacemos hijo y lo posicionamos en el padre(ancla)
        cartaGO.transform.SetParent(ancla,true);
        cartaGO.transform.position = ancla.position;

        //subimos el orden para la siguiente carta
        var sr = cartaGO.GetComponent<SpriteRenderer>(); //obtenemos el spriterenderer de la carta
        orderCursor += 1; //sumamos uno a la variable en referencia.
        sr.sortingOrder = orderCursor; //con esto le seteamos al sprite que se ponga en el orden determinado (el anterior +1)
        sr.sortingLayerID = ancla.GetComponent<SpriteRenderer>().sortingLayerID; //esto no se si esta haciendo mucho... ELIMINAR O COMENTAR MAS ADELANTE
    }

    public bool RellenarMazoCentral()
    {
        Debug.Log("Rellenando Mazo Central");
        if (descartePos == null || mazoCentralPos == null) return false;

        // 1) Recolectar TODAS las cartas actualmente en descarte
        var cartas = new List<Transform>();
        for (int i = 0; i < descartePos.transform.childCount; i++)
        {
            Transform ch = descartePos.transform.GetChild(i);
            if (ch.CompareTag("carta")) cartas.Add(ch);
        }
        if (cartas.Count == 0)
        {
            Debug.Log("No hay cartas en Descarte");
            return false; // nada que rellene desde el descarte
        }

        // 2) Mezclar (Fisher–Yates)
        System.Random rng = new System.Random(System.Environment.TickCount ^ GetInstanceID());
        for (int i = cartas.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            var tmp = cartas[i];
            cartas[i] = cartas[j];
            cartas[j] = tmp;
        }

        // 3) Mover al mazo central con orden desde 0
        var srPadre = mazoCentralPos.GetComponent<SpriteRenderer>();
        int order = 1; //para que parta en la primera capa (BUG.005 antes estaba en order = 0)

        for (int i = 0; i < cartas.Count; i++)
        {
            Transform c = cartas[i];

            // parent y posicion
            c.SetParent(mazoCentralPos.transform, true);
            Vector3 p = mazoCentralPos.transform.position;
            p.z = c.position.z; // da igual si usas sortingOrder, pero lo conservamos
            c.position = p;

            // boca abajo y marcar padre
            var sel = c.GetComponent<Seleccionable>();
            if (sel != null)
            {
                sel.faceUp = false;
                sel.setPadre("CMazoCentral");
            }

            // sorting arriba del mazo (0..N-1)
            var sr = c.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                if (srPadre != null) sr.sortingLayerID = srPadre.sortingLayerID;
                sr.sortingOrder = order++;
            }
        }
        return true;
    }

    public void TerminarTurno()
    {
        SiguienteJugador(jugadorActual); //cambiar al siguiente jugador
        CompletarMano(); //rellenar la mano de este jugador
    }

    public void CompletarMano()
    {
        //Debug.Log("Se inicia Completado de mano");
        for (int i = 0;i<6;i++) //recorremos los 6 slots de la mano del jugadorActual
        {
            //Debug.Log($"Revision de mano{i}");
            //int sortingOrder = 0;
            Transform slot = manosPos[jugadorActual][i].transform; //establecemos slot como una transform de la posicion de la mano del jugador actual (recorriendo todos sus slots)

            // si tiene una carta (devolverá false y eso negado es true, por ende si tiene una carta entraremos aqui),yyyyy continuaremos con el proximo punto i
            if (!SlotEstaVacio(slot)) { continue; }

            //mientras el slot este vacio, sacamos cartas del mazo central y las ponemos en la mano, 
            //si son comodines, no nos saca de este slot
            while (SlotEstaVacio(slot)) 
            {
                //Debug.Log($"SlotEstaVacio es {SlotEstaVacio(slot)}, por ende entramos al while");
                GameObject cartaTop = TomarTopMazoCentral();
                //Debug.Log($"cartaTop = {cartaTop}");
                if (cartaTop == null)
                {
                    //Debug.Log($"Entramos a cartaTop igual a Null");
                    Debug.Log("No hay mas cartas en el mazo central");
                    if(!RellenarMazoCentral())
                    {
                        Debug.Log("Tampoco hay cartas en el descarte. Se corta el relleno");
                        //GAMEOVER!!!!
                        break;
                    }
                    continue; //continuamos con el procedimiento...
                }

                // Si la carta top es un comodin
                if(cartaTop.name.EndsWith("k")|| cartaTop.name.EndsWith("R")|| cartaTop.name.EndsWith("N"))
                {
                    //Debug.Log($"La carta top es comodin, por ende entregamos en espacios comodines");
                    int comodinOrder = SiguienteOrden(comodinesJugadoresPos[jugadorActual].transform);
                    cartaTop.GetComponent<Seleccionable>().faceUp = true; //primero damos vuelta su cara y luego la posicionamos
                    cartaTop.GetComponent<Seleccionable>().setPadre("CComodin"); //aun no se si el padre implica algo, el nombre... quizas borrar (REVISAR)
                    ApilarEnAncla(cartaTop, comodinesJugadoresPos[jugadorActual].transform, ref comodinOrder); //NO SE SI ESTO FUNCIONARA! DEBUGEAR.
                    continue;
                }

                //Debug.Log($"la carta top es entregada al slot {i}");
                int slotOrder = SiguienteOrden(slot);
                cartaTop.GetComponent<Seleccionable>().faceUp = true; //primero damos vuelta su cara y luego la posicionamos
                cartaTop.GetComponent<Seleccionable>().setPadre("CMano"); //aun no se si el padre implica algo, el nombre... quizas borrar (REVISAR)
                ApilarEnAncla(cartaTop, slot, ref slotOrder); //NO SE SI ESTO FUNCIONARA! DEBUGEAR.
                break;
            }
        }
    }

    private int SiguienteOrden(Transform ancla) //con este metodo podemos ver el ancla que necesitemos revisar
                                                //y entregamos de vuelta el orden que deberia tener la carta que se quiere poner
    {
        int max = 0;
        for(int i= 0;i<ancla.childCount;i++) //hago una revision por todos los hijos del ancla
        {
            max = ancla.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder;
        }
        return max += 1;
    }

    //private GameObject TomarTopMazoCentral()
    //{
    //    Debug.Log($"Iniciamos TomarTopMazoCentral");
    //    Collider2D[] hits = Physics2D.OverlapPointAll(mazoCentralPos.transform.position);
    //    GameObject top = null;
    //    Debug.Log($"Top aqui deberia estar NULL: {top}");
    //    int mejorOrden = int.MinValue; //le ponemos el valor mas bajo para un entero (simplemente para no poner -1)  

    //    foreach(Collider2D h in hits)
    //    {
    //        if (h == null || !h.CompareTag("carta")) continue; //si entre todos los collider que detecta no hay ninguno con tag "carta", entonces dejara top como null
    //        var sr = h.GetComponent<SpriteRenderer>(); //esto es para tener informacion de su sortingOrder

    //        //esta mierda me esta dando errores al final del repartijo de cartas...
    //        int orden = sr ? sr.sortingOrder : 0; //basicamente aqui digo que orden toma el valor de sr.sortingOrder, si no encuentra ningun sprite render, lo coloca como 0
    //        //fin de la cosa rara

    //        Debug.Log($"orden es {orden}, mejorOrden es {mejorOrden}");
    //        if (orden >= mejorOrden)
    //        {
    //            Debug.Log($"Top al entrar es {top}");
    //            Debug.Log($"orden es mayor o igual a mejor orden");
    //            mejorOrden = orden;
    //            Debug.Log($"mejorOrden ahora es ={orden}");
    //            top = h.gameObject; //con esto cada vez que el orden sea mayor al mejorOrden encontrado, guardaremos el collider como si fuera Top.
    //            Debug.Log($"top es {top}");
    //        }
    //    }
    //    Debug.Log($"devuelve top = {top}");
    //    return top;
    //}

    private GameObject TomarTopMazoCentral() //este metodo arreglo el (BUG.004)
    {
        Vector2 centro = mazoCentralPos.transform.position;
        float radio = 0.2f; // ajusta al tamaño de tus cartas

        var hits = Physics2D.OverlapCircleAll(centro, radio);

        GameObject top = null;
        int mejorOrden = int.MinValue;

        foreach (var h in hits)
        {
            if (h == null || !h.CompareTag("carta")) continue;

            var sel = h.GetComponent<Seleccionable>();
            if (sel == null || sel.padre != "CMazoCentral") continue; // <- clave

            var sr = h.GetComponent<SpriteRenderer>();
            int orden = (sr != null) ? sr.sortingOrder : 0;

            if (orden >= mejorOrden)
            {
                mejorOrden = orden;
                top = h.gameObject;
            }
        }
        return top;
    }


    private bool SlotEstaVacio(Transform slotito) //si el slot esta vacio devuelve True. 
    {
        var hits = Physics2D.OverlapPointAll(slotito.position);
        foreach (var h in hits)
            if (h != null && h.CompareTag("carta"))
                return false;
        return true;
    }
    private void SiguienteJugador(int JugadorActual)
    {
        if (JugadorActual < cantidadDeJugadores - 1) { jugadorActual++; }
        else { jugadorActual = 0; }
    }

    public void LimpiarTodo()
    {
        var cartas = GameObject.FindGameObjectsWithTag("carta");
        jugadorActual = 0;
        foreach(var item in cartas) { Destroy(item); }
        mazoCentral.Clear();
        mazoJ1.Clear();
        mazoJ2.Clear();
        mazoJ3.Clear();
        mazoJ4.Clear();
        manoJ1.Clear();
        manoJ2.Clear();
        manoJ3.Clear();
        manoJ4.Clear();
    }

    private int ContarCartasMano() //BUG.009
    {
        int total = 0;
        foreach(var slot in manosPos[jugadorActual])
        {
            //if(slotGO ==null) continue;

            if (slot.transform.childCount > 0 && slot.transform.GetChild(slot.transform.childCount - 1).CompareTag("carta"))
            {
                total++;
            }
        }
        return total;
    }

    private void VerificarMazoCentral()  //BUG.008 
    {
        //si el mazo central queda vacio, entonces rellenamos
        if(mazoCentralPos.transform.childCount == 0)
        {
            Debug.Log($"Mazo central vacio, cartas: {mazoCentralPos.transform.childCount}");
            RellenarMazoCentral();
        }
    }
}
