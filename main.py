import pygame
import sys
from tablero import calcular_tamano_carta, calcular_posiciones

# Inicializar Pygame
pygame.init()

# Configurar dimensiones de la ventana
ancho, alto = 1600, 900
pantalla = pygame.display.set_mode((ancho, alto))
pygame.display.set_caption("Juego 26 - CulebraTuerta")

# Cargar la imagen de fondo
try:
    imagen_fondo = pygame.image.load("fondo_tablero.png")
    imagen_fondo = pygame.transform.scale(imagen_fondo, (ancho, alto))
except pygame.error as e:
    print(f"Error al cargar la imagen de fondo: {e}")
    sys.exit()

# Crear una superficie semitransparente
superficie_transparente = pygame.Surface((ancho, alto))
superficie_transparente.set_alpha(128)  # Ajusta el nivel de transparencia (0-255)
superficie_transparente.fill((0, 0, 0))  # Color negro con transparencia

# Configurar colores y fuente
color_tablero = (0, 210, 0)
fuente = pygame.font.Font(None, 24)

# Calcular tamaño de las cartas (que ahora también será el tamaño del espacio)
tamano_carta = calcular_tamano_carta(ancho, alto, proporciones=(0.07, 0.17))

# Definir puntos iniciales para cada jugador
puntos_iniciales = {
    "jugador_1": (ancho * 0.380, alto * 0.695),
    "jugador_2": (ancho * 0.815, alto * 0.5),
    "jugador_3": (ancho * 0.620, alto * 0.305),
    "jugador_4": (ancho * 0.20, alto * 0.5),
}

dc=1.15 # Distancia cartas
db=0.2 # Distancia borde

dcl=dc+0.1
dbl=db+0.1

# Desplazamientos relativos personalizados para cada jugador
desplazamientos_por_jugador = {
    "jugador_1": {
        "comodines J1": (-dc,db),
        "mazo_jugador J1": (0, 0),
        "reserva_1 J1": (dc,db),
        "reserva_2 J1": (dc*2,db),
        "reserva_3 J1": (dc*3,db),
        "reserva_4 J1": (dc*4,db),
    },
    "jugador_2": {
        "comodines J2": (dbl,2.1),
        "mazo_jugador J2": (0, 1.25),
        "reserva_1 J2": (dbl, 0.40),
        "reserva_2 J2": (dbl, -0.45),
        "reserva_3 J2": (dbl, -1.3),
        "reserva_4 J2": (dbl, -2.15),
    },
    "jugador_3": {
        "comodines J3": (dc,-db),
        "mazo_jugador J3": (0, 0),
        "reserva_1 J3": (-dc, -db),
        "reserva_2 J3": (-dc*2,-db),
        "reserva_3 J3": (-dc*3,-db),
        "reserva_4 J3": (-dc*4,-db),
    },
    "jugador_4": {
        "comodines J4": (-0.2, 1.2),
        "mazo_jugador J4": (0, 0),
        "reserva_1 J4": (-0.2, -1.2),
        "reserva_2 J4": (-0.2, -2.4),
        "reserva_3 J4": (-0.2, -3.6),
        "reserva_4 J4": (-0.2, -4.8),
    },
}

# Calcular posiciones para cada jugador
tableros = {}
for jugador, punto_inicial in puntos_iniciales.items():
    desplazamientos = desplazamientos_por_jugador[jugador]
    tableros[jugador] = calcular_posiciones(punto_inicial, tamano_carta, desplazamientos)

# Función para ajustar dinámicamente el tamaño del espacio según la orientación del jugador
def obtener_tamano_espacio(jugador, tamano_carta):
    """
    Ajusta el tamaño del espacio basado en la orientación del tablero.
    """
    if jugador in ["jugador_2", "jugador_4"]:  # Izquierda o derecha
        return tamano_carta[1], tamano_carta[0]  # Intercambiar ancho y alto
    return tamano_carta  # Superior e inferior permanecen iguales

# Función para dibujar un espacio
def dibujar_espacio(pantalla, nombre, pos_central, tamano, color, fuente):
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

# Bucle principal del programa
reloj = pygame.time.Clock()
while True:
    for evento in pygame.event.get():
        if evento.type == pygame.QUIT:
            pygame.quit()
            sys.exit()

    # Dibujar la imagen de fondo
    pantalla.blit(imagen_fondo, (0, 0))

    # Superponer la superficie semitransparente
    pantalla.blit(superficie_transparente, (0, 0))

    # Dibujar los tableros de los 4 jugadores
    for jugador, posiciones_centrales in tableros.items():
        tamano_espacio = obtener_tamano_espacio(jugador, tamano_carta)
        for nombre, pos_central in posiciones_centrales.items():
            dibujar_espacio(pantalla, nombre, pos_central, tamano_espacio, color_tablero, fuente)

    # Actualizar la pantalla
    pygame.display.flip()
    reloj.tick(60)