using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeckManager : MonoBehaviour
{
    public SpriteRenderer renderMazo;          // SpriteRenderer del Mazo_J*
    public SpriteRenderer[] renderMano = new SpriteRenderer[6]; // Mano_1..Mano_6 //estos no sera mejor dejarlos como game objects? 
    public Sprite spriteMazoVacio;

    // Listas de IDs de cartas
    public List<int> mazo = new List<int>();         // 20 al inicio (virtual)
    public List<int> mano = new List<int>(6);        // 6 visibles
    public List<int> comodines = new List<int>();
    public List<int> espacio_1 = new List<int>();
    public List<int> espacio_2 = new List<int>();
    public List<int> espacio_3 = new List<int>();
    public List<int> espacio_4 = new List<int>();

    //declaro los gameobjects de los montones del centro de mesa. Estos no se modificaran solo quedaran para su uso.
    public GameObject posComodines;
    public GameObject posEspacio1;
    public GameObject posEspacio2;
    public GameObject posEspacio3;
    public GameObject posEspacio4;

    Vector3 posInicialComodines = Vector3.zero;
    Vector3 posInicialE1 = Vector3.zero;
    Vector3 posInicialE2 = Vector3.zero;
    Vector3 posInicialE3 = Vector3.zero;
    Vector3 posInicialE4 = Vector3.zero;

    public void Awake()
    {
        posInicialComodines = posComodines.transform.localPosition;
        posInicialE1 = posEspacio1.transform.localPosition;
        posInicialE2 = posEspacio2.transform.localPosition;
        posInicialE3 = posEspacio3.transform.localPosition;
        posInicialE4 = posEspacio4.transform.localPosition;
    }

    // ----- Limpieza -----
    public void VaciarTodo()
    {
        mazo.Clear();
        mano.Clear();
        comodines.Clear();
        espacio_1.Clear();
        espacio_2.Clear();
        espacio_3.Clear();
        espacio_4.Clear();  

        MostrarCartaTope(null); // limpia el mazo visual

        // apaga los slots de la mano
        for (int i = 0; i < renderMano.Length; i++)
        {
            if (renderMano[i] != null)
            {
                renderMano[i].enabled = false;
                renderMano[i].sprite = null;
            }
        }
    }

    // ----- Mazo (virtual) -----

    public int ObtenerIdTopeMazo() //con esto muestro la ultima carta agregada, mas que nada el id
    {
        if (mazo.Count == 0) return -1;
        return mazo[mazo.Count - 1]; // tope = último agregado
    }

    public void MostrarCartaTope(Sprite sprite) //con esto mostramos la carta del mazo, el mazo de la mano. 
    {
        if (renderMazo == null) return;

        if (sprite != null)
        {
            renderMazo.sprite = sprite;
            renderMazo.enabled = true;
        }
        else
        {
            if (spriteMazoVacio != null)
            {
                renderMazo.sprite = spriteMazoVacio;
                renderMazo.enabled = true;
            }
            else
            {
                renderMazo.enabled = false;
                renderMazo.sprite = null;
            }
        }
    }

    // ----- Mano (visible) -----
    public void MostrarCartaEnMano(int indiceSlot, Sprite sprite)
    {
        if (indiceSlot < 0 || indiceSlot >= renderMano.Length) return;
        var sr = renderMano[indiceSlot];
        if (sr == null) return;

        if (sprite != null)
        {
            sr.sprite = sprite;
            sr.enabled = true;
        }
        else
        {
            sr.enabled = false;
            sr.sprite = null;
        }
    }
}


