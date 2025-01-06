import pygame

def calcular_tamano_carta(ancho_pantalla, alto_pantalla, proporciones=(0.1, 0.2)):
    """
    Calcula el tamaño de las cartas basado en las proporciones de la pantalla.
    
    :param ancho_pantalla: Ancho de la pantalla.
    :param alto_pantalla: Alto de la pantalla.
    :param proporciones: Tuple (ancho%, alto%) relativo al tamaño de la pantalla.
    :return: Tuple (ancho, alto) del tamaño de las cartas.
    """
    return [ancho_pantalla * proporciones[0], alto_pantalla * proporciones[1]]

def calcular_tamano_espacio(tamano_carta, incremento=0.1):
    """
    Calcula el tamaño de los espacios basándose en el tamaño de las cartas y un incremento porcentual.
    
    :param tamano_carta: Tuple (ancho, alto) del tamaño de las cartas.
    :param incremento: Incremento porcentual (e.g., 0.1 para 10%).
    :return: Tuple (ancho, alto) del tamaño del espacio.
    """
    return [tamano_carta[0] * (1 + incremento), tamano_carta[1] * (1 + incremento)]

def calcular_posiciones(punto_inicial, tamano_carta, desplazamientos):
    """
    Calcula las posiciones centrales de los espacios del tablero, escaladas según el tamaño de las cartas.
    
    :param punto_inicial: Tuple (x, y) del punto inicial.
    :param tamano_carta: Tuple (ancho, alto) del tamaño de las cartas.
    :param desplazamientos: Diccionario con desplazamientos relativos en múltiplos de cartas.
    :return: Diccionario con posiciones centrales calculadas.
    """
    posiciones = {}
    for nombre, (dx, dy) in desplazamientos.items():
        posiciones[nombre] = (
            punto_inicial[0] + dx * tamano_carta[0],
            punto_inicial[1] + dy * tamano_carta[1],
        )
    return posiciones

def dibujar_espacio(pantalla, nombre, pos_central, tamano, color, fuente):
    """
    Dibuja un espacio en el tablero con su etiqueta centrada.
    
    :param pantalla: Superficie de Pygame donde dibujar.
    :param nombre: Nombre del espacio (etiqueta).
    :param pos_central: Tuple (x, y) de la posición central.
    :param tamano: Tuple (ancho, alto) del espacio.
    :param color: Color del marco del espacio.
    :param fuente: Fuente para la etiqueta.
    """
    ancho, alto = tamano
    x_centro, y_centro = pos_central
    x_sup_izq = x_centro - ancho // 2
    y_sup_izq = y_centro - alto // 2

    # Dibujar el marco del espacio
    pygame.draw.rect(pantalla, color, (x_sup_izq, y_sup_izq, ancho, alto), 2)

    # Dibujar la etiqueta
    texto = fuente.render(nombre, True, color)
    pantalla.blit(
        texto, (x_centro - texto.get_width() // 2, y_centro - texto.get_height() // 2)
    )
