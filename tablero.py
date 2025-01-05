import pygame

class Tablero:
    def __init__(self, pantalla, tamano_espacio):
        """
        Inicializa el tablero con una pantalla y el tamaño de los espacios.
        
        :param pantalla: Pantalla de Pygame donde se dibujará el tablero.
        :param tamano_espacio: Tamaño de los espacios del tablero.
        """
        self.pantalla = pantalla
        self.tamano_espacio = tamano_espacio

    def dibujar_espacio(self, nombre, pos_central, color, fuente):
        """
        Dibuja un espacio en el tablero desde su posición central.
        
        :param nombre: Nombre del espacio (etiqueta).
        :param pos_central: Posición central del espacio (x, y).
        :param color: Color del borde del espacio.
        :param fuente: Fuente para la etiqueta del espacio.
        """
        ancho, alto = self.tamano_espacio
        x_centro, y_centro = pos_central
        x_sup_izq = x_centro - ancho // 2
        y_sup_izq = y_centro - alto // 2

        # Dibujar el marco del espacio
        pygame.draw.rect(self.pantalla, color, (x_sup_izq, y_sup_izq, ancho, alto), 2)

        # Dibujar la etiqueta
        texto = fuente.render(nombre, True, (255, 255, 255))
        self.pantalla.blit(
            texto, (x_centro - texto.get_width() // 2, y_centro - texto.get_height() // 2)
        )
