using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MesaCentral : MonoBehaviour
{
    [Header("Jugadores (asigna en Inspector)")]
    public PlayerDeckManager jugador1;
    public PlayerDeckManager jugador2;
    public PlayerDeckManager jugador3;
    public PlayerDeckManager jugador4;

    [Tooltip("Activos en la partida (2 a 4). Si es 4, se suma 1 deck extra (54 cartas).")]
    [Range(2, 4)]
    public int cantidadDeJugadores = 2;

    [Header("Reparto")]
    [SerializeField] private int cartasMazoPorJugador = 20; // 20 por jugador
    [SerializeField] private int cartasManoPorJugador = 6;  // 6 por jugador

    [Header("Cartas (Sprites)")]
    [Tooltip("Arrastra aqui TODOS los sprites: t2..t10, ta, tj, tq, tk, d2.., c2.., p2.., jokerR, jokerN.")]
    public Sprite[] spritesCartas;

    [Header("Montones Centrales")] //va a contener los ID de las cartas que coloquemos aqui
    public List<int> monton_1 = new List<int>(12); 
    public List<int> monton_2 = new List<int>(12);
    public List<int> monton_3 = new List<int>(12);
    public List<int> monton_4 = new List<int>(12);

    //declaro los gameobjects de los montones del centro de mesa. Estos no se modificaran solo quedaran para su uso.
    public GameObject posMonton1;
    public GameObject posMonton2;
    public GameObject posMonton3;
    public GameObject posMonton4;

    Vector3 posInicialM1 = Vector3.zero;
    Vector3 posInicialM2 = Vector3.zero;
    Vector3 posInicialM3 = Vector3.zero;
    Vector3 posInicialM4 = Vector3.zero;

    // Estado interno
    private readonly List<int> mazoCentral = new List<int>(256);      // ids del mazo central
    private readonly List<string> nombresCartas = new List<string>(256); // id -> nombre (puede repetir nombres entre decks)
    private Dictionary<string, Sprite> spritePorNombre;               // nombre -> sprite unico

    public List<int> mazoDescarte = new List<int>(); //creamos un mazo de descartes inicial


    // --------------------------------------------------------
    private void Awake()
    {
        ConstruirMapaSprites();   // nombre -> sprite (una vez)

        // Guardo posiciones iniciales de montones centrales 
        posInicialM1 = posMonton1.transform.localPosition;
        posInicialM2 = posMonton2.transform.localPosition;
        posInicialM3 = posMonton3.transform.localPosition;
        posInicialM4 = posMonton4.transform.localPosition;
    }

    private void Start()
    {
        BotonBarajar();           // inicia nueva partida
    }

    private void Update()
    {
        if (monton_1.Count == 12)
        {
            AlMazoDeDescarte(monton_1);
        }
        if (monton_2.Count == 12)
        {
            AlMazoDeDescarte(monton_2);
        }
        if (monton_3.Count == 12)
        {
            AlMazoDeDescarte(monton_3);
        }
        if (monton_4.Count == 12)
        {
            AlMazoDeDescarte(monton_4);
        }
        if (mazoCentral.Count == 0)
        {
            Debug.Log("Se han acabado las cartas en el mazo central, bajarando las de descarte");
            BarajarDescarte();
        }
    }

    public void AlMazoDeDescarte(List<int> MontonPorDevolver) 
    {
        foreach(int i in MontonPorDevolver){ mazoDescarte.Add(i);} //agregamos las cartas del monton seleccionado al mazo de descarte
        MontonPorDevolver.Clear(); //vaciamos el monton 
    }

    public void BarajarDescarte()
    {
        //convertir todo el mazo de descarte en mazo central, entonces para eso tengo que mezclar los id que tengo en mazo de descarte. 
        // Reconstruir mazo central usando el largo de mazo de descarte
        mazoCentral.Clear();
        foreach(int i in mazoDescarte)
        {
            mazoCentral.Add(i); //con esto agrego todas las cartas del mazo descarte al mazo central
        }
        mazoDescarte.Clear(); //limpiamos el de descarte nuevamente para nuevas cartas. 

        // Mezcla Fisher-Yates
        System.Random rng = new System.Random(unchecked(Environment.TickCount * 31 + GetInstanceID()));
        for (int i = mazoCentral.Count - 1; i > 0; i--)
        {
            int j = rng.Next(0, i + 1);
            int tmp = mazoCentral[i];
            mazoCentral[i] = mazoCentral[j];
            mazoCentral[j] = tmp;
        }
        Debug.Log("Mazo de descarte barajado");
    }

    public Vector3 ObtenerPosInicial(int posInicial)  //si requiero saber la posicion de algunos de los montones, ya los tengo identificados aqui y se pueden consultar desde fuera.
    {
        switch (posInicial)
        { 
            case 1: return posInicialM1;
            case 2: return posInicialM2;
            case 3: return posInicialM3;
            case 4: return posInicialM4;
            default: return Vector3.zero;
        }
    }

    // Llamar desde el boton de UI
    public void BotonBarajar()
    {
        ReiniciarYBarajar();
        RepartirYMostrar();
        //Debug.Log("El mazo central tiene "+mazoCentral.Count+" cartas, juegan "+cantidadDeJugadores+" jugadores");
    }

    public void BotonDebugeo()
    {
        //Debug.Log("la pos inicial de m1 es : " + posInicialM1);
        Debug.Log("El monton 1 tiene: "+monton_1.Count+" cantidad de cartas\n"+
            "El monton 2 tiene: " + monton_2.Count + " cantidad de cartas\n" +
            "El monton 3 tiene: " + monton_3.Count + " cantidad de cartas\n" +
            "El monton 4 tiene: " + monton_4.Count + " cantidad de cartas\n");
    }

    // --------------------------------------------------------
    // Construccion de baraja dinamica

    private void ConstruirNombresCartas()
    {
        nombresCartas.Clear();

        string[] palos = { "t", "d", "c", "p" };  // trebol, diamante, corazon, pica
        string[] rangos = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "a", "j", "q", "k" };

        int decks = 3;

        if (cantidadDeJugadores < 4)
        {
            decks = 2;
        }       

            // Agregar cartas normales: 52 por deck
            for (int d = 0; d < decks; d++)
            {
                foreach (var p in palos)
                    foreach (var r in rangos)
                        nombresCartas.Add(p + r);
            }

        // Jokers: 2 por deck (1 rojo, 1 negro)
        for (int d = 0; d < decks; d++)
        {
            nombresCartas.Add("jokerR");
            nombresCartas.Add("jokerN");
        }
    }

    private void ConstruirMapaSprites()
    {
        spritePorNombre = new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);
        foreach (var sp in spritesCartas)
        {
            if (sp == null) continue;
            string nombre = sp.name.Trim();
            if (!spritePorNombre.ContainsKey(nombre))
                spritePorNombre.Add(nombre, sp);
        }
    }

    // --------------------------------------------------------
    // Barajar

    private void ReiniciarYBarajar()
    {
        // Re-construir nombres segun cantidad de jugadores (para ajustar tamanio total)
        ConstruirNombresCartas();

        // Limpiar jugadores
        foreach (var p in JugadoresActivos())
            p?.VaciarTodo();

        // Reconstruir mazo central usando el largo de nombresCartas
        mazoCentral.Clear();
        for (int i = 0; i < nombresCartas.Count; i++) mazoCentral.Add(i);

        // Mezcla Fisher-Yates
        System.Random rng = new System.Random(unchecked(Environment.TickCount * 31 + GetInstanceID()));
        for (int i = mazoCentral.Count - 1; i > 0; i--)
        {
            int j = rng.Next(0, i + 1);
            int tmp = mazoCentral[i];
            mazoCentral[i] = mazoCentral[j];
            mazoCentral[j] = tmp;
        }

        // Debug:
        // Debug.Log("[MesaCentral] Barajado " + mazoCentral.Count + " cartas. Top: " + NombreDe(mazoCentral[0]));
        
    }

    // --------------------------------------------------------
    // Reparto y visual

    private void RepartirYMostrar()
    {
        var jugadores = JugadoresActivos();
        if (jugadores.Count < 2)
        {
            Debug.LogError("[MesaCentral] Debe haber al menos 2 jugadores asignados.");
            return;
        }

        // Validar referencias no nulas para los primeros N jugadores
        for (int i = 0; i < jugadores.Count; i++)
        {
            if (jugadores[i] == null)
            {
                Debug.LogError("[MesaCentral] Falta asignar PlayerDeckManager para jugador " + (i + 1));
                return;
            }
        }

        int n = jugadores.Count;
        int necesarias = (cartasMazoPorJugador + cartasManoPorJugador) * n;
        if (mazoCentral.Count < necesarias)
        {
            Debug.LogError("[MesaCentral] Cartas insuficientes: necesita " + necesarias + ", hay " + mazoCentral.Count + ".");
            return;
        }

        // 1) Repartir al MAZO (20 cada uno), de forma alternada J1..Jn, J1..Jn...
        for (int k = 0; k < cartasMazoPorJugador; k++)
        {
            for (int i = 0; i < n; i++)
            {
                int id = TomarArriba();
                jugadores[i].mazo.Add(id); 
            }
        }

        // Mostrar tope de cada mazo
        for (int i = 0; i < n; i++)
        {
            int idTop = jugadores[i].ObtenerIdTopeMazo();
            jugadores[i].MostrarCartaTope(SpriteDe(idTop));
        }

        // 2) Repartir a la MANO (6 cada uno), alternado
        for (int h = 0; h < cartasManoPorJugador; h++)
        {
            for (int i = 0; i < n; i++)
            {
                int id = TomarArriba();
                jugadores[i].mano.Add(id);
                jugadores[i].MostrarCartaEnMano(h, SpriteDe(id));
            }
        }
    }

    private List<PlayerDeckManager> JugadoresActivos()
    {
        // Toma los primeros N managers segun cantidadDeJugadores
        var lista = new List<PlayerDeckManager>(4);
        if (cantidadDeJugadores >= 1) lista.Add(jugador1);
        if (cantidadDeJugadores >= 2) lista.Add(jugador2);
        if (cantidadDeJugadores >= 3) lista.Add(jugador3);
        if (cantidadDeJugadores >= 4) lista.Add(jugador4);
        return lista;
    }

    private int TomarArriba()
    {
        int id = mazoCentral[0];
        mazoCentral.RemoveAt(0);
        return id;
    }

    // --------------------------------------------------------
    // Helpers de nombres/sprites

    private string NombreDe(int id)
    {
        if (id < 0 || id >= nombresCartas.Count) return "(none)";
        return nombresCartas[id];
    }

    private Sprite SpriteDe(int id)
    {
        if (id < 0 || id >= nombresCartas.Count) return null;
        string nombre = nombresCartas[id];
        if (spritePorNombre != null && spritePorNombre.TryGetValue(nombre, out var sp)) return sp;

        Debug.LogWarning("[MesaCentral] Falta sprite para '" + nombre + "'.");
        return null;
    }

    //---------------------------------------------------------
    // Agregar cartas a montones

    public void AgregarCartaMonton(int monton, int id) //esto es solo de manera virtual, no con sprites, solo para la lista. 
    {
        //al monton donde se posiciono se agrega a su lista. 
        switch (monton)
        {
            case 1: // Seleccion del Monton 1
                monton_1.Add(id); //agrego el id a la lista
                return;
            case 2: // Seleccion del Monton 2
                monton_2.Add(id); //agrego el id a la lista
                return;
            case 3: // Seleccion del Monton 3
                monton_3.Add(id); //agrego el id a la lista
                return;
            case 4: // Seleccion del Monton 4
                monton_4.Add(id); //agrego el id a la lista
                return;
        }
    }
}
