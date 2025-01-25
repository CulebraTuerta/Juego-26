import pygame

def calcular_tamano_carta(ancho_pantalla, alto_pantalla, proporciones=(0.9, 0.9)):
    return [ancho_pantalla * proporciones[0], alto_pantalla * proporciones[1]]

def calcular_posiciones(punto_inicial, tamano_carta, desplazamientos):
    posiciones = {}
    for nombre, (dx, dy) in desplazamientos.items():
        posiciones[nombre] = (
            punto_inicial[0] + dx * tamano_carta[0],
            punto_inicial[1] + dy * tamano_carta[1],
        )
    return posiciones


