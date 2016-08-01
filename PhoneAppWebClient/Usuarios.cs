using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
//Agregar estas librerías
using System.Data.Linq.Mapping;
using System.Data.Linq;

namespace PhoneAppWebClient
{
    [Table]
    public class Usuarios
    {
        //Siempre debe haber una columna con la clave primara, se sugiere auto generada
        [Column(IsDbGenerated = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        [Column(CanBeNull = false)]
        public string Usuario
        {
            get;
            set;
        }

        [Column(CanBeNull = false)]
        public string Clave
        {
            get;
            set;
        }

        [Column(CanBeNull = false)]
        public string Nivel
        {
            get;
            set;
        }

    }
}
