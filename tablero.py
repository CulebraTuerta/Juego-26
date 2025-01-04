import pygame
import sys

# Inicializar Pygame
pygame.init()

# Configurar dimensiones de la ventana
ancho, alto = 800, 600
pantalla = pygame.display.set_mode((ancho, alto))
pygame.display.set_caption("Tablero de Jugador con Posiciones Centrales")

# Configurar el color de fondo
color_fondo = (34, 139, 34)  # Verde oscuro

# Tamaño inicial de los espacios (ancho y alto)
tamano_espacio = [100, 140]  # Ajustable

# Colores
color_blanco = (255, 255, 255)
color_rojo = (255, 0, 0)

# Fuente para etiquetas
fuente = pygame.font.Font(None, 24)

# Configuración de las posiciones centrales de los espacios
altura_tablero=500
pos_inicial=100

posiciones_centrales = {
    "comodines": (pos_inicial, altura_tablero),  # Posición central del espacio de comodines
    "mazo_jugador": (pos_inicial*2 + 10, altura_tablero-20),  # Posición central del mazo del jugador
    "reserva_1": (pos_inicial*3 +20 , altura_tablero),  # Primera reserva del jugador
    "reserva_2": (pos_inicial*4 +30, altura_tablero),  # Segunda reserva
    "reserva_3": (pos_inicial*5 +40, altura_tablero),  # Tercera reserva
    "reserva_4": (pos_inicial*6 +50, altura_tablero),  # Cuarta reserva
}

# Función para dibujar un espacio dado su posición central
def dibujar_espacio(nombre, pos_central, tamano, color):
    # Calcular la esquina superior izquierda desde el centro
    x_centro, y_centro = pos_central
    ancho, alto = tamano
    x_superior_izquierda = x_centro - ancho // 2
    y_superior_izquierda = y_centro - alto // 2

    # Dibujar el marco del espacio
    pygame.draw.rect(pantalla, color, (x_superior_izquierda, y_superior_izquierda, ancho, alto), 2)

    # Etiqueta en el centro
    texto = fuente.render(nombre, True, color_blanco)
    pantalla.blit(
        texto,
        (x_centro - texto.get_width() // 2, y_centro - texto.get_height() // 2),
    )

# Bucle principal del programa
reloj = pygame.time.Clock()
while True:
    for evento in pygame.event.get():
        if evento.type == pygame.QUIT:
            pygame.quit()
            sys.exit()

    # Rellenar la pantalla con el color de fondo
    pantalla.fill(color_fondo)

    # Dibujar cada espacio con su posición central
    for nombre, pos_central in posiciones_centrales.items():
        dibujar_espacio(nombre, pos_central, tamano_espacio, color_blanco)

    # Actualizar la pantalla
    pygame.display.flip()
    reloj.tick(60)
