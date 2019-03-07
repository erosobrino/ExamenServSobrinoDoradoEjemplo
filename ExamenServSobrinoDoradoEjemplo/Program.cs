using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamenServSobrinoDoradoEjemplo
{
    class Program
    {
        static void Main(string[] args)
        {
            ServidorArchivos servidor = new ServidorArchivos();
            servidor.iniciaServidorArchivos();
        }
    }
}
