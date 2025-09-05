using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class GameController : MonoBehaviour
{
    public Sprite[] spritesCartas;
    public GameObject PrefabCarta;
    public int cantidadDeJugadores = 2;
    public List<string> mazoCentral;
    public float zOffset = 0.03f;
    public float segundos = 0.03f;

    //-------------------------------------------------------------------------------------------------------------------
    //POSICIONES DE LOS GO (LOS BORDES)
    public GameObject[] montonesPos;
    public GameObject mazoCentralPos;
    public GameObject[] mazosJugadoresPos;
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


    void Start()
    {
        mazosJugadores = new List<string>[] { mazoJ1, mazoJ2, mazoJ3, mazoJ4, };
        manosJugadores = new List<string>[] { manoJ1, manoJ2, manoJ3, manoJ4, };
        manosPos = new GameObject[][] { manoJ1Pos, manoJ2Pos, manoJ3Pos, manoJ4Pos };

        IniciarJuego();        
    }

    public void IniciarJuego()
    {
        LimpiarTodo();
        mazoCentral = GenerarMazoCentral();
        Barajar(mazoCentral);
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

    public void Barajar(List<string> mazo)
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
    
    private void MostrarCartas()
    {
        int j = 0;
        int contador = 1;
        //muestro las cartas que quedan en el mazo
        foreach (string carta in mazoCentral)
        {
            GameObject cartita = Instantiate(PrefabCarta, new Vector3(mazoCentralPos.transform.position.x, mazoCentralPos.transform.position.y, mazoCentralPos.transform.position.z - zOffset), Quaternion.identity); //en este caso el transform position es del GO del juego26
            cartita.name = carta; //asi toma el nombre del string carta in mazocentral
            cartita.tag = "carta";
            cartita.GetComponent<Seleccionable>().faceUp = false;
            cartita.GetComponent<Seleccionable>().setPadre("CMazoCentral");
            zOffset = zOffset + 0.03f;
        }
        zOffset = 0.03f; //vuelvo a poner el offset de z en el inicial.

        //muestro cartas de los mazos de jugadores
        for (int i = 0; i < cantidadDeJugadores; i++) //por cada jugador, instanciaremos sus cartas de sus respectivos mazos
        {
            //muestro las cartas de sus mazos
            foreach (string carta in mazosJugadores[i]) //esta lista tiene todos los mazos de los jugadores
            {
                GameObject cartita = Instantiate(PrefabCarta, new Vector3(mazosJugadoresPos[i].transform.position.x, mazosJugadoresPos[i].transform.position.y, mazosJugadoresPos[i].transform.position.z - zOffset), Quaternion.identity); //en este caso el transform position es del GO del juego26
                cartita.name = carta; //asi toma el nombre del string carta in mazojugador[i]
                cartita.tag = "carta";
                cartita.GetComponent<Seleccionable>().setPadre("CMazo");
                if (contador==mazosJugadores[i].Count)
                {
                    cartita.GetComponent<Seleccionable>().faceUp = true;
                }
                else { cartita.GetComponent<Seleccionable>().faceUp = false; }
                zOffset = zOffset + 0.03f;
                contador++;
            }
            zOffset = 0.03f; //lo seteo nuevamente para el nuevo jugador.
            
            //muestro las cartas de sus manos
            foreach (string carta in manosJugadores[i]) //esta lista tiene todos las manos de los jugadores
            {
                GameObject cartita = Instantiate(PrefabCarta, new Vector3(manosPos[i][j].transform.position.x, manosPos[i][j].transform.position.y, manosPos[i][j].transform.position.z - zOffset), Quaternion.identity); //en este caso el transform position es del GO del juego26
                cartita.name = carta; //asi toma el nombre del string carta in mazojugador[i]
                cartita.tag = "carta";
                cartita.GetComponent<Seleccionable>().setPadre("CMano");
                cartita.GetComponent<Seleccionable>().faceUp = true;
                zOffset = zOffset + 0.03f;
                j++;
            }
            zOffset = 0.03f; //lo seteo nuevamente para el nuevo jugador.
            j = 0;
            contador = 1;
        }
    }

    private void RepartirJugadores()
    {
        for (int i = 0;i < cantidadDeJugadores; i++)
        {
            for (int j = 0; j< 20; j++) //con esto reparto 20 cartas a cada jugador segun la cantidad de jugadore
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
    
    public void LimpiarTodo()
    {
        var cartas = GameObject.FindGameObjectsWithTag("carta");
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
        zOffset = 0;
    }
}
