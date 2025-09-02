using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using System;

public class UpdateSprite : MonoBehaviour
{
    //este scrip estara en cada una de las cartas, por ende cada carta tendra un name asociado

    public Sprite cardFace;
    public Sprite cardBack;

    private Seleccionable seleccionable;
    private SpriteRenderer spriteRenderer;
    public GameController gameController;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); //le damos el componente de spriterenderer
        seleccionable = GetComponent<Seleccionable>(); //agregamos el componente de seleccionable, que es un script.
    }
    void Start()
    {
        //buscaremos el sprite de cardface para guardarlo en esta carta instanciada
        gameController = FindFirstObjectByType<GameController>(); //con esto le decimos que gamecontroller aqui lo considere encontrando el primer GO llamado gamecontroller.
        string nombre = gameObject.name; //este es el nombre del GO instanciado
        cardFace = BuscarSpritePorNombre(gameController.spritesCartas, nombre); //ojo, los sprites tienen que tener el mismo nombre que las cartas.
    }

    void Update()
    {
        if (seleccionable.faceUp == true) { spriteRenderer.sprite = cardFace; }
        else { spriteRenderer.sprite = cardBack; }
    }

    private Sprite BuscarSpritePorNombre(Sprite[] sprites, string nombre)
    {
        foreach (var s in sprites) { if(s.name == nombre) { return s; } }
        return null;
    }
}
