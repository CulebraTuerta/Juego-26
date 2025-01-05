import sys
import pygame
from jugador import Jugador
from tablero import Tablero

# Inicializar Pygame
pygame.init()

# Configurar dimensiones de la ventana
ancho, alto = 800, 600
pantalla = pygame.display.set_mode((ancho, alto))
pygame.display.set_caption("Tablero de Jugador Modular")

# Configurar el color de fondo y otros parámetros
color_fondo = (34, 139, 34)  # Verde oscuro
tamano_espacio = [100, 140]  # Tamaño de los espacios
fuente = pygame.font.Font(None, 24)

# Crear el jugador
posiciones_centrales = {
    "comodines": (200, 400),
    "mazo_jugador": (400, 400),
    "reserva_1": (600, 400),
    "reserva_2": (700, 400),
    "reserva_3": (800, 400),
    "reserva_4": (900, 400),
}
jugador = Jugador("Jugador 1", posiciones_centrales)

# Crear el tablero
tablero = Tablero(pantalla, tamano_espacio)

# Bucle principal del programa
reloj = pygame.time.Clock()
while True:
    for evento in pygame.event.get():
        if evento.type == pygame.QUIT:
            pygame.quit()
            sys.exit()

    # Rellenar la pantalla con el color de fondo
    pantalla.fill(color_fondo)

    # Dibujar los espacios del tablero
    for nombre, pos_central in posiciones_centrales.items():
        tablero.dibujar_espacio(nombre, pos_central, (255, 255, 255), fuente)

    # Actualizar la pantalla
    pygame.display.flip()
    reloj.tick(60)
