using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;

// https://docs.microsoft.com/en-us/dotnet/api/system.console.setcursorposition?view=net-6.0
namespace Laberinto
{
    [SupportedOSPlatform("windows")]
    class Program
    {

        static void Main(string[] args)
        {
            Random random = new Random();
            Console.Title = "Laberinto Generador con método Backtracking Recursivo";


            Console.CursorVisible = false;

            Laberinto laberinto;

            do
            {

                int alto;
                do
                {
                    alto = random.Next(7, 32);
                } while (alto % 2 == 0);


                int ancho;
                do
                {
                    ancho = random.Next(11, 56);
                } while (ancho % 2 == 0);

                laberinto = new(alto, ancho);
                laberinto.CrearPasillo(0, 0, 0, 0);
                laberinto.DefinirSalida();
                laberinto.Dibujar();

            } while (laberinto.Jugar());
        }
    }

    class Laberinto
    {
        public Celda[,] Celdas { get; set; }
        public int Renglones { get; private set; }
        public int Columnas { get; private set; }

        public Player player = new();

        public Laberinto(int renglones, int columnas)
        {
            Renglones = renglones;
            Columnas = columnas;

            Celdas = new Celda[renglones, columnas];

            for (int r = 0; r < renglones; r++)
            {
                for (int c = 0; c < columnas; c++)
                {
                    Celdas[r, c] = new Celda();
                }
            }
        }

        // metodo que implementa Backtracking recursivo
        public bool CrearPasillo(int renglon, int columna, int renglonPrev, int columnaPrev)
        {
            Random random = new();

            if (
                renglon < 0 || renglon >= Renglones ||
                columna < 0 || columna >= Columnas ||
                Celdas[renglon, columna].Visitado)
            {
                return false;
            }
            else
            {
                Celdas[renglon, columna].Visitado = true;


                if (columna > columnaPrev) Celdas[renglonPrev, columnaPrev].MuroDerecho = false;
                if (columna < columnaPrev) Celdas[renglon, columna].MuroDerecho = false;

                if (renglon > renglonPrev) Celdas[renglon, columna].MuroArriba = false;
                if (renglon < renglonPrev) Celdas[renglonPrev, columnaPrev].MuroArriba = false;


                Direccion[] direccion = new Direccion[4];
                direccion[0] = new Direccion(-1, 0);// Arriba
                direccion[1] = new Direccion(0, 1); // Derecha
                direccion[2] = new Direccion(1, 0); // Abajo
                direccion[3] = new Direccion(0, -1); // Izquierda

                // random, desorden de direcciones;
                random.Shuffle(direccion);

                //// Prueba de las funciones
                //if (
                //    !CrearPasillo(renglon + 1, columna, renglon, columna)
                //    &&
                //    !CrearPasillo(renglon - 1, columna, renglon, columna)
                //    &&
                //    !CrearPasillo(renglon, columna + 1, renglon, columna)
                //    &&
                //    !CrearPasillo(renglon, columna - 1, renglon, columna)
                //    )

                if (
                   !CrearPasillo(renglon + direccion[0].Renglon, columna + direccion[0].Columna, renglon, columna)
                   &&
                   !CrearPasillo(renglon + direccion[1].Renglon, columna + direccion[1].Columna, renglon, columna)
                   &&
                   !CrearPasillo(renglon + direccion[2].Renglon, columna + direccion[2].Columna, renglon, columna)
                   &&
                   !CrearPasillo(renglon + direccion[3].Renglon, columna + direccion[3].Columna, renglon, columna)
                   )
                {
                    return false;

                }

                return true;
            }

        }

        public bool ValidarMovimiento(int renglon, int columna, Movimiento movimiento)
        {
            bool valido = false;

            if (movimiento == Movimiento.Vertical && renglon >= 0 && renglon < Renglones)
            {
                valido = !Celdas[renglon, columna].MuroArriba;
            }

            if (movimiento == Movimiento.Horizontal && columna >= 0 && columna < Columnas)
            {
                valido = !Celdas[renglon, columna].MuroDerecho;
            }

            return valido;
        }

        public void DefinirSalida()
        {
            List<Direccion> direccion = new();
            //direccion.Add(new Direccion(0, 0)); Inicio no entra la posicion
            direccion.Add(new Direccion(0, Columnas - 1)); // esquina superior derecha
            direccion.Add(new Direccion(0, (Columnas / 2))); // centro superior
            direccion.Add(new Direccion(Renglones - 1, Columnas - 1)); // esquina inferior derecha
            direccion.Add(new Direccion((Renglones / 2), Columnas - 1)); // centro derecha
            direccion.Add(new Direccion(Renglones - 1, 0)); // esquina inferior izquierda
            direccion.Add(new Direccion(Renglones - 1, (Columnas / 2))); // centro inferior 
            direccion.Add(new Direccion((Renglones / 2), 0)); // centro izquierdo


            // otra forma de desordenar elementos (doble desordenamiento)
            direccion = direccion.OrderBy(o => Guid.NewGuid()).ThenBy(o => Guid.NewGuid()).ToList();

            // tomo la primera como la posicion para indicar la salida
            Direccion posicion = direccion.First();

            Celdas[posicion.Renglon, posicion.Columna].TipoCelda = TipoCelda.Salida;
        }

        [SupportedOSPlatform("windows")]
        public void Dibujar()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            int width = (Columnas * 2) + 1;
            int height = (Renglones * 2) + 1;

            Console.SetWindowSize(width, height);
            Console.SetBufferSize(width, height);

            string muroHorizontal = "─";
            string muroVertical = "│";
            string cruz = "┼";

            for (int r = 0; r < Renglones; r++)
            {
                // barda arriba
                for (int c = 0; c < Columnas; c++)
                {
                    if (c == 0)
                    {
                        Console.Write(cruz);
                    }

                    if (Celdas[r, c].MuroArriba)
                    {
                        Console.Write($"{muroHorizontal}{cruz}");
                    }
                    else
                    {
                        Console.Write($" {cruz}");
                    }
                }

                Console.Write(Environment.NewLine);

                // Barda y pasillos
                for (int c = 0; c < Columnas; c++)
                {
                    if (c == 0)
                    {
                        Console.Write(muroVertical);
                    }

                    // aqui se decide es la salida o pasillo
                    if (Celdas[r, c].TipoCelda == TipoCelda.Salida)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("S");
                        Console.ForegroundColor = ConsoleColor.Blue;
                    }
                    else
                    {
                        Console.Write(" ");
                    }

                    // aqui se decide si se abre pasillo/muro
                    if (Celdas[r, c].MuroDerecho)
                    {
                        Console.Write(muroVertical);
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }

                Console.Write(Environment.NewLine);
            }

            //barda de abajo
            for (int c = 0; c < Columnas; c++)
            {

                if (c == 0)
                {
                    Console.Write(cruz);

                }

                Console.Write($"{muroHorizontal}{cruz}");
            }
        }

        public bool Jugar()
        {
            bool nuevo = false;
            ConsoleKeyInfo key = new();
            player.Dibujar();

            while (key.Key != ConsoleKey.Escape && key.Key != ConsoleKey.N)
            {
                key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (ValidarMovimiento(player.RenglonActual, player.ColumnaActual, Movimiento.Vertical))
                        {
                            player.MoverRenglon(-1);
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        if (ValidarMovimiento(player.RenglonActual + 1, player.ColumnaActual, Movimiento.Vertical))
                        {
                            player.MoverRenglon(1);
                        }
                        break;

                    case ConsoleKey.RightArrow:
                        if (ValidarMovimiento(player.RenglonActual, player.ColumnaActual, Movimiento.Horizontal))
                        {
                            player.MoverColumna(1);
                        }
                        break;

                    case ConsoleKey.LeftArrow:
                        if (ValidarMovimiento(player.RenglonActual, player.ColumnaActual - 1, Movimiento.Horizontal))
                        {
                            player.MoverColumna(-1);
                        }
                        break;

                    case ConsoleKey.N:
                        nuevo = true;
                        break;

                    case ConsoleKey.Escape:
                        nuevo = false;
                        break;

                }

                player.Dibujar();
            }

            return nuevo;
        }
    }

    class Celda
    {
        public bool MuroDerecho { get; set; } = true;
        public bool MuroArriba { get; set; } = true;
        public bool Visitado { get; set; } = false;
        public TipoCelda TipoCelda { get; set; } = TipoCelda.Pasillo;
    }

    class Direccion
    {
        public int Renglon { get; private set; }
        public int Columna { get; private set; }

        public Direccion(int renglon, int columna)
        {
            Renglon = renglon;
            Columna = columna;
        }
    }

    class Player
    {
        static readonly string emoji = "■";
        static readonly string emojiRastro = "∙";

        public int RenglonActual { get; private set; }
        public int ColumnaActual { get; private set; }

        public int RenglonPrev { get; private set; }
        public int ColumnaPrev { get; private set; }

        public void MoverRenglon(int renglon)
        {
            RenglonPrev = RenglonActual;
            RenglonActual += renglon;
            ColumnaPrev = ColumnaActual;
        }


        public void MoverColumna(int columna)
        {
            ColumnaPrev = ColumnaActual;
            ColumnaActual += columna;
            RenglonPrev = RenglonActual;
        }

        public void Dibujar()
        {
            int renglonScreen;

            if (RenglonActual == 0)
            {
                renglonScreen = 1;
            }
            else
            {
                renglonScreen = (RenglonActual * 2) + 1;
            }

            int renglonScreenPrev;

            if (RenglonPrev == 0)
            {
                renglonScreenPrev = 1;
            }
            else
            {
                renglonScreenPrev = (RenglonPrev * 2) + 1;
            }


            int columnaScreen;

            if (ColumnaActual == 0)
            {
                columnaScreen = 1;
            }
            else
            {
                columnaScreen = (ColumnaActual * 2) + 1;
            }

            int columnaScreenPrev;

            if (ColumnaPrev == 0)
            {
                columnaScreenPrev = 1;
            }
            else
            {
                columnaScreenPrev = (ColumnaPrev * 2) + 1;
            }

            Console.SetCursorPosition(columnaScreenPrev, renglonScreenPrev);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(emojiRastro);


            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(columnaScreen, renglonScreen);
            Console.WriteLine(emoji);
        }
    }

    static class ArrayExt
    {
        public static void Shuffle<T>(this Random random, T[] array)
        {
            int total = array.Length;

            while (total > 1)
            {
                int posicion = random.Next(total--);

                T temporal = array[total];
                array[total] = array[posicion];
                array[posicion] = temporal;
            }
        }
    }

    enum Movimiento
    {
        Horizontal,
        Vertical
    }

    enum TipoCelda
    {
        Pasillo,
        Entrada,
        Salida
    }
}
