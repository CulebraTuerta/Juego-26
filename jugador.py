class Jugador:
    def __init__(self, nombre, posiciones):
        """
        Inicializa un jugador con su nombre y las posiciones de su tablero.
        
        :param nombre: Nombre del jugador.
        :param posiciones: Diccionario con las posiciones de los espacios del tablero.
        """
        self.nombre = nombre
        self.posiciones = posiciones
        self.cartas = {
            "comodines": [],
            "mazo_jugador": [],
            "reservas": {f"reserva_{i+1}": [] for i in range(4)},
        }

    def agregar_carta(self, espacio, carta):
        """
        Agrega una carta a un espacio específico del tablero del jugador.
        
        :param espacio: Nombre del espacio (e.g., "mazo_jugador" o "reserva_1").
        :param carta: Carta a agregar.
        """
        if espacio in self.cartas:
            self.cartas[espacio].append(carta)
        elif espacio in self.cartas["reservas"]:
            self.cartas["reservas"][espacio].append(carta)
