using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class HandCardProbe : MonoBehaviour
{
    [Header("Destino por Tag")]
    public string receptorTag = "Receptor";    // Objetos válidos para soltar

    [Header("Apilado visual en el receptor")]
    public float separacionY = 0.15f;          // Distancia vertical (en MUNDO) entre cartas apiladas
    public int ordenBase = 0;                  // Offset de sortingOrder sobre el receptor

    [Header("(Opcional) Modelo del jugador")]
    public PlayerDeckManager jugador;          // Para vaciar slot en el modelo
    public int slotIndex = -1;                 // 0..5 si lo conectas

    private SpriteRenderer slotSR;
    private GameObject dragGO;                 // visual en drag
    private SpriteRenderer dragSR;
    private Vector3 dragOffset;

    public MesaCentral mesa;

    private void Awake()
    {
        slotSR = GetComponent<SpriteRenderer>();
    }

    // --- Logs útiles, puedes comentar si ya no los necesitas ---
    private void OnMouseEnter()
    {
        string carta = (slotSR && slotSR.sprite) ? slotSR.sprite.name : "(sin sprite)";
        Debug.Log($"[Hover Mano] Slot '{name}' --> carta: {carta}");
    }

    private void OnMouseDown()
    {
        string carta = (slotSR && slotSR.sprite) ? slotSR.sprite.name : "(sin sprite)";
        Debug.Log($"[MouseDown] Slot '{name}' --> carta: {carta}");

        if (slotSR == null || slotSR.sprite == null) return;

        // Crear visual de arrastre (copia del slot)
        dragGO = new GameObject("Carta_En_Mano_Drag");
        dragSR = dragGO.AddComponent<SpriteRenderer>();
        dragSR.sprite = slotSR.sprite;
        dragSR.sortingLayerID = slotSR.sortingLayerID;
        dragSR.sortingOrder = 1000; // bien arriba

        // Posición inicial y offset del mouse
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0f;
        dragGO.transform.position = transform.position;
        dragOffset = dragGO.transform.position - mouse;

        // Esconder el slot mientras arrastro
        slotSR.enabled = false;
    }

    private void OnMouseDrag()
    {
        if (dragGO == null) return;
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0f;
        dragGO.transform.position = mouse + dragOffset;
    }

    private void OnMouseUp()
    {
        if (dragGO == null) return;

        // Buscar Receptor bajo el mouse
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0f;
        var hits = Physics2D.OverlapPointAll(mouse);

        Transform receptor = null;
        int mejorOrden = int.MinValue;

        foreach (var h in hits)
        {
            if (!h) continue;
            if (!string.IsNullOrEmpty(receptorTag) && !h.CompareTag(receptorTag)) continue;

            var srHit = h.GetComponent<SpriteRenderer>();
            int orden = srHit ? srHit.sortingOrder : 0;
            if (orden >= mejorOrden)
            {
                mejorOrden = orden;
                receptor = h.transform;
            }
        }

        if (receptor != null && slotSR != null && slotSR.sprite != null)
        {
            // Índice de apilado (para el desplazamiento vertical)
            int indice = IndiceApilado(receptor);

            // Crear carta visual
            GameObject cartaGO = new GameObject("CartaDrop_" + indice);
            var sr = cartaGO.AddComponent<SpriteRenderer>();
            sr.sprite = slotSR.sprite;

            // 1) Posicionar en MUNDO con el offset deseado (independiente de la escala del receptor)
            Vector3 worldPos = receptor.position + new Vector3(0f, -separacionY * indice, 0f);
            cartaGO.transform.position = worldPos;
            cartaGO.transform.rotation = Quaternion.identity;

            // 2) Hacer hijo conservando el transform mundial (¡true!)
            cartaGO.transform.SetParent(receptor, true);

            // 3) Agregar a la lista del monton correspondiente
            //mesa.AgregarCartaMonton(4, indice); //esto colapsa todo

            // Orden SIEMPRE por encima (max existente + 1)
            sr.sortingLayerID = (receptor.GetComponent<SpriteRenderer>()?.sortingLayerID) ?? slotSR.sortingLayerID;
            sr.sortingOrder = SiguienteOrden(receptor);

            // Collider por si luego quieres mover esta carta
            cartaGO.AddComponent<BoxCollider2D>();

            // Vaciar slot (y modelo si corresponde)
            //if (jugador != null && slotIndex >= 0) jugador.RemoverCarta(slotIndex);
            slotSR.sprite = null;
            slotSR.enabled = false;

            Destroy(dragGO);
        }
        else
        {
            // Volver al slot
            Destroy(dragGO);
            if (slotSR != null && slotSR.sprite != null) slotSR.enabled = true;
        }
    }

    // ---- Helpers: cuentan hijos y calculan el próximo sortingOrder ----
    private int IndiceApilado(Transform t)
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
        var srPadre = t.GetComponent<SpriteRenderer>();
        if (srPadre != null) maxOrder = srPadre.sortingOrder;

        for (int i = 0; i < t.childCount; i++)
        {
            var sr = t.GetChild(i).GetComponent<SpriteRenderer>();
            if (sr != null && sr.sortingOrder > maxOrder)
                maxOrder = sr.sortingOrder;
        }
        return maxOrder + 1; // siempre arriba de todo
    }

    // (Opcional) Si la quieres usar en otro lado:
    // Cuenta cuántas "cartas" hay ya como hijos directos que tengan SpriteRenderer
    private int ContarCartasVisuales(Transform t)
    {
        int c = 0;
        for (int i = 0; i < t.childCount; i++)
            if (t.GetChild(i).GetComponent<SpriteRenderer>() != null)
                c++;
        return c;
    }
}


