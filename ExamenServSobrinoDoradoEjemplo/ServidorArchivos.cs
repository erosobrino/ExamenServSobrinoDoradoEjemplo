using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ExamenServSobrinoDoradoEjemplo
{
    class ServidorArchivos
    {

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
                FileInfo[] fileInfos = directoryInfo.GetFiles(".txt");
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
            IPEndPoint ie = new IPEndPoint(IPAddress.Any, -1);
        }
    }
}
