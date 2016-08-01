using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
//Agregar estas librerías
using System.Windows.Navigation;
//librerías para BD
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Text;

namespace PhoneAppWebClient
{
    public partial class Verusuario : PhoneApplicationPage
    {
        public Verusuario()
        {
            InitializeComponent();
        }

        //Método que maneja el evento del botón Regresar
        public void botonRegresar_Click(object sender, RoutedEventArgs e)
        {
            //Llama a la página: MainPage.xaml
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void insertarUsuarioBD(String user, String pass, String level)
        {
            //Realiza al conexión con la Base de datos: UsuariosDB.sdf con una instancia de la clase: UsuariosDataContext
            using (UsuariosDataContext Usuariosdb = new UsuariosDataContext(UsuariosDataContext.ConnectionString))
            {
                //Crea el objUsuarios de la clase Usuarios y asinga los valores de las caja de texto a los atributos del objeto
                Usuarios objUsuarios = new Usuarios
                {
                    Usuario = user,
                    Clave = pass,
                    Nivel = level
                };
                try
                {
                    //Inserta el nuevo registro con la instancia de objUsuarios
                    Usuariosdb.Usuarios.InsertOnSubmit(objUsuarios);
                    Usuariosdb.SubmitChanges();
                    MessageBox.Show("Agregado el nuevo Usuario a la BD.");
                }
                catch (Exception error)
                {
                    MessageBox.Show("No puede adicionar, porque la base de datos no se ha creado, o el error es: " + error);
                }
            }
        }


        /*
         * Sobre escribimos el método: OnNavigatedTo
         * El mismo se activa cuando llamamos a través de programación al método: NavigationService.Navigate.
         * O cuando presionamos el botón físico o de hardware del dispositivo de flecha hacia atrás.
         */
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            /*
             * Si presionó el botón físico o de hardware del dispositivo de flecha hacia atrás llamará
             * a la última página en el stack de memoria de aplicaciones del dispositivo.
             */
            if (e.NavigationMode == NavigationMode.Back) return;

            string usuario = "";
            string clave = "";
            string nivel = "";
            string checkRecordar = "";

            //Verifica con el método: QueryString.TryGetValue que las variables enviadas como parámetros existen
            // Y son recibidas en las variables auxiliares de tipo: out
            if (NavigationContext.QueryString.TryGetValue("usuario", out usuario) &&
                NavigationContext.QueryString.TryGetValue("clave", out clave) &&
                NavigationContext.QueryString.TryGetValue("nivel", out nivel) &&
                 NavigationContext.QueryString.TryGetValue("checkRecordar", out checkRecordar)
                )
            {
                //Asignamos a las variables de la vistas de tipo: TextBlock, con la data recibida.
                textBlockDataUsuario.Text = usuario;
                textBlockDataClave.Text = clave;
                textBlockDataNivel.Text = nivel;
                if (checkRecordar.Equals("null")) {

                }
                else if (checkRecordar.Equals("True")) {
                    insertarUsuarioBD(usuario, clave, nivel);
                }
            }
        }
    }
}