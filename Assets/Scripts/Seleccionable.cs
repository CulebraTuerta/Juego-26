using UnityEngine;

public class Seleccionable : MonoBehaviour
{
    public bool faceUp = false;
    public string padre;

    public void setPadre(string nombrePadre)
    {
        padre = nombrePadre;
    }


}
