using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class TextosMontones : MonoBehaviour
{

    public Transform monton;          
    public TextMeshPro texto;        
    public Vector3 offset = new Vector3(0.2f, -1f, 0);
    private void Awake()
    {
        texto.gameObject.SetActive(false);
    }

    void Reset()
    {
        monton = transform;
        texto = GetComponent<TextMeshPro>();
    }

    void Update()
    {
        //buscamos el hijo con sorting order mas alto
        SpriteRenderer topSR = null;
        Transform topCarta = null;
        
        for (int i = 0; i < monton.childCount; i++)
        {
            var carta = monton.GetChild(i);
            if (!carta.CompareTag("carta")) {continue; }
            
            var sr = carta.GetComponent<SpriteRenderer>();
            if(topSR == null||sr.sortingOrder >= topSR.sortingOrder)
            {
                topSR = sr;
                topCarta = carta;
            }
        }

        // Si no hay carta ocultamos el texto
        if(topCarta == null) { texto.gameObject.SetActive(false); return; }

        // Verificacion de si es comodin
        if(topCarta.name.EndsWith("k")|| topCarta.name.EndsWith("N")|| topCarta.name.EndsWith("R"))
        {
            int numero = topSR.sortingOrder;

            var srAncla = monton.GetComponent<SpriteRenderer>();
            if (srAncla != null)
            {
                numero = topSR.sortingOrder - srAncla.sortingOrder;
            }

            texto.text = "Reemplazando: " + numero;
            texto.transform.position = monton.position + offset; //posicionamos el texto
            texto.gameObject.SetActive(true); //activar el texto
        }
        else
        {
            texto.gameObject.SetActive(false);
        }
    }
}
