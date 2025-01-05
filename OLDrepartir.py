import pygame
import sys
import random

# Inicializar Pygame
pygame.init()

# Configurar las dimensiones de la ventana
ancho, alto = 800, 600
pantalla = pygame.display.set_mode((ancho, alto))
pygame.display.set_caption("Mazo de Cartas")

# Configurar el color de fondo
color_fondo = (34, 139, 34)  # Verde

# Cargar las imágenes de los reversos
ruta_cartas = "Cartas"
reverso_azul = pygame.image.load(f"{ruta_cartas}/ReversoAzul.png")
reverso_rojo = pygame.image.load(f"{ruta_cartas}/ReversoRojo.png")

# Redimensionar las imágenes de las cartas
tamano_carta = (80, 120)
reverso_azul = pygame.transform.scale(reverso_azul, tamano_carta)
reverso_rojo = pygame.transform.scale(reverso_rojo, tamano_carta)

# Crear el mazo combinado (54 cartas de cada color)
mazo_combinado = [("azul", reverso_azul) for _ in range(54)] + [("rojo", reverso_rojo) for _ in range(54)]
random.shuffle(mazo_combinado)

# Espaciado entre cartas superpuestas en el mazo
espaciado_mazo = 0.2

# Inicializar pilas de los jugadores
cartas_jugadores = {"arriba": [], "abajo": [], "izquierda": [], "derecha": []}

# Posiciones de las pilas de los jugadores
posiciones_jugadores = {
    "arriba": (ancho // 1.08 - tamano_carta[0] // 2, alto // 7 - tamano_carta[1] // 2),
    "abajo": (ancho // 12 - tamano_carta[0] // 2, alto * 3 // 3.45 - tamano_carta[1] // 2),
    "izquierda": (ancho // 7 - tamano_carta[1] // 2, alto // 2 - tamano_carta[0] // 2),
    "derecha": (ancho * 6 // 7 - tamano_carta[1] // 2, alto // 2 - tamano_carta[0] // 2),
}


# Espaciado entre cartas en las pilas de los jugadores
espaciado_jugador = 0.2

# Fuente para mostrar la cantidad de cartas
fuente_cartas = pygame.font.Font(None, 24)

# Dibujar el mazo combinado en el centro
def dibujar_mazo():
    x, y = ancho // 2 - tamano_carta[0] // 2, alto // 2 - tamano_carta[1] // 2
    for i, (_, imagen) in enumerate(mazo_combinado):
        pantalla.blit(imagen, (x + i * espaciado_mazo, y))

# Dibujar las pilas de los jugadores con el contador
def dibujar_pilas_jugadores():
    for jugador, cartas in cartas_jugadores.items():
        x, y = posiciones_jugadores[jugador]
        # Dibujar marco del mazo
        pygame.draw.rect(
            pantalla, 
            (255, 255, 255), 
            (x - 10, y - 10, tamano_carta[0] + 20, tamano_carta[1] + 20), 
            2
        )
        # Dibujar las cartas
        for i, (_, imagen) in enumerate(cartas):
            pantalla.blit(imagen, (x + i * espaciado_jugador, y))
        # Mostrar el número de cartas en la esquina inferior derecha de la pila
        if cartas:
            texto_numero = fuente_cartas.render(str(len(cartas)), True, (255, 255, 255))
            pantalla.blit(texto_numero, (x + len(cartas) * espaciado_jugador + 5, y + tamano_carta[1] - 20))

# Dibujar un botón
def dibujar_boton(texto, x, y, ancho, alto, color, color_texto):
    fuente = pygame.font.Font(None, 24)
    pygame.draw.rect(pantalla, color, (x, y, ancho, alto))
    texto_boton = fuente.render(texto, True, color_texto)
    pantalla.blit(texto_boton, (x + (ancho - texto_boton.get_width()) // 2, y + (alto - texto_boton.get_height()) // 2))

# Repartir una carta
def repartir_carta():
    if mazo_combinado:
        carta = mazo_combinado.pop()
        jugadores = list(cartas_jugadores.keys())
        jugador = jugadores[len(mazo_combinado) % 4]
        cartas_jugadores[jugador].append(carta)

# Bucle principal del programa
reloj = pygame.time.Clock()
while True:
    for evento in pygame.event.get():
        if evento.type == pygame.QUIT:
            pygame.quit()
            sys.exit()
        if evento.type == pygame.MOUSEBUTTONDOWN:
            mouse_x, mouse_y = pygame.mouse.get_pos()
            # Detectar si se presionó el botón de repartir
            if ancho // 2 - 50 <= mouse_x <= ancho // 2 + 50 and alto - 50 <= mouse_y <= alto - 20:
                repartir_carta()

    # Rellenar la pantalla con el color de fondo
    pantalla.fill(color_fondo)

    # Dibujar el mazo combinado en el centro
    dibujar_mazo()

    # Dibujar las pilas de los jugadores con los números y marcos
    dibujar_pilas_jugadores()

    # Dibujar el botón de repartir
    dibujar_boton("Repartir", ancho // 2 - 50, alto - 50, 100, 30, (255, 0, 0), (255, 255, 255))

    # Actualizar la pantalla
    pygame.display.flip()
    reloj.tick(60)
