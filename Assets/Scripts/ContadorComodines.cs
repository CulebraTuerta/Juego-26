using UnityEngine;
using TMPro;

public class ContadorComodines : MonoBehaviour
{
    
    public Transform ancla;          // normalmente: este mismo transform
    public TextMeshPro texto;        // TMP en World Space, hijo del ancla
    private Vector3 offset = new Vector3(-0.6f, 0.8f, 0);
    private void Awake()
    {
        texto.gameObject.SetActive(true);
    }

    void Update()
    {
        int count = 0;
        for (int i = 0; i < ancla.childCount; i++)
        {
            count++;
        }

        texto.text = count.ToString();
        texto.transform.position = ancla.position + offset;
        texto.gameObject.SetActive(count > 1); // oculta si es 1 
    }
}

