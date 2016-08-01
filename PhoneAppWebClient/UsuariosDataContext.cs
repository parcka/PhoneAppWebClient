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
//Agregar esta librería
using System.Data.Linq;

namespace PhoneAppWebClient
{
    //Que herede de: DataContext
    public class UsuariosDataContext : DataContext
    {
        //Constante que contiene el nombre de la base de datos: UsuariosDB.sdf en el almacenaje virtualizado: Data Source=isostore:/
        public const string ConnectionString = @"Data Source=isostore:/UsuariosDB.sdf";

        //Contructor para realizar la conexión a la tabla: Usuarios de la Base de Datos UsuariosDataContext
        public UsuariosDataContext(string connectionString) 
        : base(connectionString)
        {
        }

        //Definición del contexto de la tabla: Usuarios
        public Table<Usuarios> Usuarios
        {
            //Método que obtiene todos los usuarios
            get
            {
                return this.GetTable<Usuarios>();
            }
        }

    }
}
