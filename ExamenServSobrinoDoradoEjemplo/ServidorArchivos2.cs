using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExamenServSobrinoDoradoEjemplo
{
    class ServidorArchivos
    {
        bool pararServidor = false;

        public string leeArchivo(String nombreArchivo, int nLineas)
        {
            String cadena = "";
            try
            {
                using (StreamReader reader = new StreamReader(Environment.GetEnvironmentVariable("EXAMEN") + "/" + nombreArchivo))
                {
                    String linea = reader.ReadLine();
                    int contLineas = 1;
                    while (linea != null && contLineas <= nLineas)
                    {
                        cadena += linea + Environment.NewLine;
                        contLineas++;
                        linea = reader.ReadLine();
                    }
                }
            }
            catch (Exception)
            {
                cadena = "<ERROR_IO>";
            }
            return cadena;
        }

        public int leePuerto()
        {
            int puerto;
            try
            {
                puerto = Convert.ToInt32(leeArchivo("puerto.txt", 1));
                if (puerto < IPEndPoint.MinPort || puerto > IPEndPoint.MaxPort)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                puerto = 31416;
            }
            return puerto;
        }

        public void guardaPuerto(int numero)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(Environment.GetEnvironmentVariable("EXAMEN") + "/puerto.txt"))
                {
                    writer.WriteLine(numero);
                }
            }
            catch (Exception) { }
        }

        public string listarArchivos()
        {
            string nombres = "";
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(Environment.GetEnvironmentVariable("EXAMEN"));
                FileInfo[] fileInfos = directoryInfo.GetFiles("*.txt");
                if (fileInfos.Length > 0)
                    nombres = Environment.NewLine;
                foreach (FileInfo fileInfo in fileInfos)
                {
                    nombres += fileInfo.Name + Environment.NewLine;
                }
            }
            catch (Exception) { }
            return nombres;
        }

        public void iniciaServidorArchivos()
        {
            int puerto = leePuerto();
            try
            {
                IPEndPoint ie = new IPEndPoint(IPAddress.Any, puerto);
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                s.Bind(ie);
                s.Listen(10);
                Console.WriteLine("Escuchando en el puerto " + puerto);
                while (!pararServidor)
                {
                    Socket sCliente = s.Accept();
                    Thread hilo = new Thread(hiloCliente);
                    hilo.IsBackground = true;
                    hilo.Start(sCliente);
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("El puerto esta ocupado");
            }
            Console.ReadKey();
        }

        public void hiloCliente(object socket)
        {
            bool parar = false;
            string msg = "";
            Socket sCliente = (Socket)socket;
            IPEndPoint ieCliente = (IPEndPoint)sCliente.RemoteEndPoint;
            Console.WriteLine(ieCliente.Address + " " + ieCliente.Port);
            using (NetworkStream ns = new NetworkStream(sCliente))
            {
                using (StreamWriter sw = new StreamWriter(ns))
                {
                    using (StreamReader sr = new StreamReader(ns))
                    {
                        sw.WriteLine("CONEXION ESTABLECIDA");
                        sw.Flush();
                        while (msg != null && !parar)
                        {
                            try
                            {
                                msg = sr.ReadLine();
                                if (msg != null)
                                {
                                    string[] msgSeparado = msg.Split(' ');
                                    switch (msgSeparado[0].ToUpper())
                                    {
                                        case "GET":
                                            if (msgSeparado.Length == 2)
                                            {
                                                try
                                                {
                                                    string[] datos = msgSeparado[1].Split(',');
                                                    string nombreArchivo = datos[0];
                                                    int lineas = Convert.ToInt32(datos[1]);
                                                    sw.WriteLine(leeArchivo(nombreArchivo, lineas));
                                                }
                                                catch (Exception)
                                                {
                                                    sw.WriteLine("Error");
                                                }
                                            }
                                            else
                                            {
                                                sw.WriteLine("Error en la estructura del mensage, GET XXXX");
                                            }
                                            sw.Flush();
                                            break;
                                        case "PORT":
                                            if (msgSeparado.Length == 2)
                                            {
                                                try
                                                {
                                                    int puerto = Convert.ToInt32(msgSeparado[1]);
                                                    guardaPuerto(puerto);
                                                    sw.WriteLine("Guardado correcto");
                                                }
                                                catch (Exception)
                                                {
                                                    sw.WriteLine("Error en el puerto");
                                                }
                                            }
                                            else
                                            {
                                                sw.WriteLine("Error en la estructura del mensage, PORT XXXX");
                                            }
                                            sw.Flush();
                                            break;
                                        case "LIST":
                                            sw.WriteLine(listarArchivos());
                                            sw.Flush();
                                            break;
                                        case "CLOSE":
                                            parar = true;
                                            break;
                                        case "HALT":
                                            parar = true;
                                            pararServidor = true;
                                            Environment.Exit(0);
                                            break;
                                    }
                                }
                            }
                            catch (IOException)
                            {
                                parar = true;
                            }
                        }
                    }
                }
            }
            sCliente.Close();
        }
    }
}
