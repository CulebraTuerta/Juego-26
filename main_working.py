import pygame
import sys
import random  # Importar el módulo random
from tablero import calcular_tamano_carta, calcular_posiciones

# Inicializar Pygame
pygame.init()

# Configurar dimensiones de la ventana
ancho, alto = 1600, 900
pantalla = pygame.display.set_mode((ancho, alto))
pygame.display.set_caption("Juego 26 - CulebraTuerta")

# Calcular tamaño de las cartas (que ahora también será el tamaño del espacio)
tamano_carta = calcular_tamano_carta(ancho, alto, proporciones=(0.07, 0.17))

# Cargar imágenes de las cartas
try:
    reverso_carta_azul = pygame.image.load("Cartas/ReversoAzul.png")  # Ruta de la imagen
    reverso_carta_azul = pygame.transform.scale(reverso_carta_azul, tamano_carta)
    reverso_carta_rojo = pygame.image.load("Cartas/ReversoRojo.png")  # Ruta de la imagen
    reverso_carta_rojo = pygame.transform.scale(reverso_carta_rojo, tamano_carta)
except pygame.error as e:
    print(f"Error al cargar la imagen de las cartas: {e}")
    sys.exit()

# Crear el mazo central con dos mazos de 54 cartas cada uno (incluyendo Jokers)
mazo_central = [("rojo", reverso_carta_azul) for _ in range(54)] + [("azul", reverso_carta_rojo) for _ in range(54)]

# Mezclar el mazo central
random.shuffle(mazo_central)

# Configurar colores y fuente
color_fondo = (34, 139, 34)  # Verde oscuro para el fondo
color_bordes = (255, 255, 255)
fuente = pygame.font.Font(None, 24)

# Definir posición del mazo central
posicion_mazo_central = (ancho * 0.5, alto * 0.5)

# Definir posición del mazos centrales secundarios
posiciones_mazos_secundarios = {
    "Mazo Secundario 1": (ancho * 0.34, alto * 0.5),
    "Mazo Secundario 2": (ancho * 0.42, alto * 0.5),
    "Mazo Secundario 3": (ancho * 0.58, alto * 0.5),
    "Mazo Secundario 4": (ancho * 0.66, alto * 0.5),
}

# Definir puntos iniciales para cada jugador
puntos_iniciales = {
    "jugador_1": (ancho * 0.380, alto * 0.695),
    "jugador_2": (ancho * 0.815, alto * 0.5),
    "jugador_3": (ancho * 0.620, alto * 0.305),
    "jugador_4": (ancho * 0.185, alto * 0.5),
}

dc = 1.15  # Distancia cartas
db = 0.2  # Distancia borde
dbl = db + 0.1

# Desplazamientos relativos personalizados para cada jugador
desplazamientos_por_jugador = {
    "jugador_1": {
        "comodines 1": (-dc, db),
        "mazo_jugador 1": (0, 0),
        "reserva_1 1": (dc, db),
        "reserva_2 1": (dc * 2, db),
        "reserva_3 1": (dc * 3, db),
        "reserva_4 1": (dc * 4, db),
    },
    "jugador_2": {
        "comodines 2": (dbl, 2.1),
        "mazo_jugador 2": (0, 1.25),
        "reserva_1 2": (dbl, 0.40),
        "reserva_2 2": (dbl, -0.45),
        "reserva_3 2": (dbl, -1.3),
        "reserva_4 2": (dbl, -2.15),
    },
    "jugador_3": {
        "comodines 3": (dc, -db),
        "mazo_jugador 3": (0, 0),
        "reserva_1 3": (-dc, -db),
        "reserva_2 3": (-dc * 2, -db),
        "reserva_3 3": (-dc * 3, -db),
        "reserva_4 3": (-dc * 4, -db),
    },
    "jugador_4": {
        "comodines 4": (-dbl, -2.15),
        "mazo_jugador 4": (0, -1.3),
        "reserva_1 4": (-dbl, -0.45),
        "reserva_2 4": (-dbl, 0.4),
        "reserva_3 4": (-dbl, 1.25),
        "reserva_4 4": (-dbl, 2.1),
    },
}

# Inicializar los mazos de los jugadores
mazos_jugadores = {
    "jugador_1": [],
    "jugador_2": [],
    "jugador_3": [],
    "jugador_4": []
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
    pantalla.blit(texto, (x_centro - texto.get_width() // 2, y_centro - texto.get_height() // 2))

def dibujar_mazo_central(pantalla, mazo, posicion):
    """
    Dibuja las cartas del mazo central apiladas en la posición indicada.
    """
    x_centro, y_centro = posicion
    for i, (_, imagen) in enumerate(mazo):
        offset = i * 0.1  # Pequeño desplazamiento para simular apilado
        pantalla.blit(imagen, (x_centro - tamano_carta[0] // 2, y_centro - tamano_carta[1] // 2 - offset))

def dibujar_boton_repartir(pantalla, texto, pos, tamano, color, color_texto, fuente):
    """
    Dibuja un botón con texto en la posición indicada.
    """
    x, y = pos
    ancho, alto = tamano
    pygame.draw.rect(pantalla, color, (x, y, ancho, alto))  # Dibuja el rectángulo del botón
    texto_boton = fuente.render(texto, True, color_texto)
    pantalla.blit(texto_boton,(x + (ancho - texto_boton.get_width()) // 2, y + (alto - texto_boton.get_height()) // 2))
    return pygame.Rect(x, y, ancho, alto)  # Devuelve el rectángulo del botón para la detección de clics


def repartir_cartas(mazo_central, mazos_jugadores, jugadores_disponibles):
    """
    Reparte 20 cartas a cada jugador disponible del mazo central.
    """
    for jugador in jugadores_disponibles:
        mazo_jugador = mazos_jugadores[jugador]
        if len(mazo_central) >= 20:  # Verifica que haya suficientes cartas
            for _ in range(20):
                mazo_jugador.append(mazo_central.pop())  # Toma una carta del mazo central

def dibujar_cartas_jugador(pantalla, mazo_jugador, posicion, tamano_carta, girar=False, jugador=None):
    """
    Dibuja las cartas de un jugador apiladas en su posición.
    Si `girar` es True, rota las cartas 90 grados y ajusta la posición.
    El jugador determina si hay un offset especial (e.g., J2, J4).
    """
    x_centro, y_centro = posicion
    for i, (_, imagen) in enumerate(mazo_jugador):
        offset = i * 0.1  # Desplazamiento para simular apilado

        if girar:
            # Rotar la carta y ajustar las coordenadas
            imagen_dibujar = pygame.transform.rotate(imagen, 90)
            nuevo_ancho, nuevo_alto = tamano_carta[1], tamano_carta[0]  # Intercambiar ancho y alto
            
            # Aplicar offset adicional para jugadores 2 y 4
            if jugador == "jugador_2":
                x_offset = -offset  # Desplazamiento horizontal positivo
            elif jugador == "jugador_4":
                x_offset = offset  # Desplazamiento horizontal negativo
            else:
                x_offset = 0  # Sin desplazamiento adicional

            pantalla.blit(imagen_dibujar,(x_centro - nuevo_ancho // 2 + x_offset, y_centro - nuevo_alto // 2))
        else:
            # Dibujar sin rotación
            pantalla.blit(imagen, (x_centro - tamano_carta[0] // 2, y_centro - tamano_carta[1] // 2 - offset))

def dibujar_contador_cartas(pantalla, cantidad, posicion, fuente, color):
    """
    Dibuja un contador con la cantidad de cartas en la posición indicada.
    """
    texto = fuente.render(str(cantidad), True, color)
    x, y = posicion
    pantalla.blit(texto, (x - texto.get_width() // 2, y - texto.get_height() // 2))


# Posición y tamaño del botón
pos_boton_repartir = (ancho * 0.5, alto * 0.05)
tamano_boton_repartir = (120, 40)
color_boton = (0, 128, 255)
color_texto = (255, 255, 255)

# Jugadores disponibles para repartir
jugadores_disponibles = ["jugador_1", "jugador_2", "jugador_3", "jugador_4"]

# Bucle principal del programa
reloj = pygame.time.Clock()
while True:
    for evento in pygame.event.get():
        if evento.type == pygame.QUIT:
            pygame.quit()
            sys.exit()
        elif evento.type == pygame.MOUSEBUTTONDOWN:
            if boton_repartir_rect and boton_repartir_rect.collidepoint(evento.pos):
                repartir_cartas(mazo_central, mazos_jugadores, jugadores_disponibles) 

    # Rellenar la pantalla con el color de fondo
    pantalla.fill(color_fondo)

    # Dibujar el mazo central cartas
    dibujar_mazo_central(pantalla, mazo_central, posicion_mazo_central)

    # Dibujar el mazo central bordes
    dibujar_espacio(pantalla, "Mazo Central", posicion_mazo_central, tamano_carta, color_bordes, fuente)

    # Mostrar el contador de cartas del mazo central
    dibujar_contador_cartas(pantalla, len(mazo_central), (posicion_mazo_central[0], posicion_mazo_central[1] + tamano_carta[1] // 2 + 10),fuente, color_bordes)

    # Dibujar el botón "Repartir"
    boton_repartir_rect = dibujar_boton_repartir(pantalla, "Repartir", pos_boton_repartir, tamano_boton_repartir, color_boton, color_texto, fuente)

    # Dibujar los mazos secundarios
    for nombre, posicion in posiciones_mazos_secundarios.items():
        dibujar_espacio(pantalla, nombre, posicion, tamano_carta, color_bordes, fuente)

    # Dibujar los tableros de los 4 jugadores
    for jugador, posiciones_centrales in tableros.items():
        tamano_espacio = obtener_tamano_espacio(jugador, tamano_carta)
        for nombre, pos_central in posiciones_centrales.items():
            dibujar_espacio(pantalla, nombre, pos_central, tamano_espacio, color_bordes, fuente)

        # Dibujar el contador de cartas en los mazos de los jugadores
        cantidad = len(mazos_jugadores[jugador])
        posicion = tableros[jugador]["mazo_jugador " + jugador[-1]]
        dibujar_contador_cartas(pantalla, cantidad, (posicion[0], posicion[1] + tamano_carta[1] // 2 + 10),fuente, color_bordes)

    # Dibujar las cartas de los jugadores
    for jugador, cartas in mazos_jugadores.items():
        posicion = tableros[jugador]["mazo_jugador " + jugador[-1]]  # Obtener posición del mazo del jugador
        girar = jugador in ["jugador_2", "jugador_4"]  # Determina si las cartas deben girarse
        dibujar_cartas_jugador(pantalla, cartas, posicion, tamano_carta, girar=girar, jugador=jugador)

    # Actualizar la pantalla
    pygame.display.flip()
    reloj.tick(60)