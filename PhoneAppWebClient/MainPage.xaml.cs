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
//Adicionamos estas librerías
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;
using System.Text;
//librerías para BD
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Text;


namespace PhoneAppWebClient
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }
        
        //Método que maneja el evento del botón búsqueda
        public void botonBusqueda_Click(object sender, RoutedEventArgs e)
        {
            

            crearBD_Click();
            Usuarios userExist = buscarUser_Click();
            if (userExist != null)
            {
                //me muevo a la otra pantalla
                NavigationService.Navigate(new Uri("/Verusuario.xaml?usuario=" + userExist.Usuario + "&clave=" + userExist.Clave + "&nivel=" + userExist.Nivel + "&checkRecordar=" + "null", UriKind.Relative));
            }
            else 
            { 
                //busco en en web service los datos
                buscarUsuario();

            }
            /* Descomente BuscarUsuario() cuando desee usar WebClient  y comente a: buscarUsuario(); */
            //BuscarUsuario();
            /* Descomente buscarUsuario() cuando desee usar HttpWebRequest  y comente a: BuscarUsuario(); */
           
        }


        //Método que maneja el evento Click del botón: botonEliminar un Usuario
        private void botonEliminar_Click(object sender, RoutedEventArgs e)
        {
            //Realiza al conexión con la Base de datos: UsuariosDB.sdf con una instancia de la clase: UsuariosDataContext
            using (UsuariosDataContext Usuariosdb = new UsuariosDataContext(UsuariosDataContext.ConnectionString))
            {
                try
                {
                    //Se realiza el query por el Usuario, equivale a: Select * from Usuarios Where Usuario=txtUsuario.Text; 
                    IQueryable<Usuarios> UsuariosQuery = from Usuarios in Usuariosdb.Usuarios where Usuarios.Usuario == txtUsuario.Text select Usuarios;
                    //Obtiene la primera incedencia de la consulta
                    Usuarios objUsuarios = UsuariosQuery.FirstOrDefault();
                    //Obtenido la primera incidencia o referencia al objeto a eliminar, se elimina de la tabla.
                    //Equivale a: Delete from Usuarios Where Usuario=txtUsuario.Text; 
                    Usuariosdb.Usuarios.DeleteOnSubmit(objUsuarios);
                    //Se realiza un Commit a la tabla de la base de datos.
                    Usuariosdb.SubmitChanges();
                    MessageBox.Show("Usuario Eliminado.");
                }
                catch (Exception error)
                {
                    MessageBox.Show("No puede eliminar, porque la base de datos no se ha creado, o el error es: " + error);
                }
            }
        }

        private Usuarios buscarUser_Click()
        {
            //Realiza al conexión con la Base de datos: UsuariosDB.sdf con una instancia de la clase: UsuariosDataContext
            using (UsuariosDataContext Usuariosdb = new UsuariosDataContext(UsuariosDataContext.ConnectionString))
            {
                try
                {
                    //Se realiza el query por el Usuario, equivale a: Select * from Usuarios Where Usuario=txtUsuario.Text; 
                    IQueryable<Usuarios> UsuariosQuery = from Usuarios in Usuariosdb.Usuarios where Usuarios.Usuario == txtUsuario.Text select Usuarios;
                    //Obtiene la primera incedencia de la consulta
                    Usuarios objUsuarios = UsuariosQuery.FirstOrDefault();
                    if (objUsuarios != null)
                    {
                        MessageBox.Show("Usuario Encontrado en la BD.");
                        return objUsuarios;
                    }
                   
                }
                catch (Exception error)
                {
                    MessageBox.Show("No se pudo buscar el usuaior, error: " + error);
                }
            }

            return null;
        }

        private void crearBD_Click()
        {
            //Realiza al conexión con la Base de datos: UsuariosDB.sdf con una instancia de la clase: UsuariosDataContext
            using (UsuariosDataContext Usuariosdb = new UsuariosDataContext(UsuariosDataContext.ConnectionString))
            {
                //Si no existe la base de datos la creará
                if (Usuariosdb.DatabaseExists() == false)
                {
                    Usuariosdb.CreateDatabase();
                    MessageBox.Show("Creada la base de datos UsuariosDB.sdf");
                }
                else
                {
                    //MessageBox.Show("La base de datos UsuariosDB.sdf ya existe");
                }
            }
        }

        //Método que maneja el btón Limpiar y reiniciamos los parámetros y visibilidad en la pantalla.
        public void botonLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtUsuario.Text = "";
            txtClave.Password = "";
            txtUsuario.Focus();
        }
        /*
         * Usando WebClient
         */

  





        /*****************************************************************************************************/

        /*
         * Usando HttpWebRequest
         */

        //Método: buscarUsuario() que usa HttpWebRequest 
        private void buscarUsuario()
        {
            string url = "http://192.168.1.128/servidor-restful/Despachador.php?servicio=1&usuario=" + txtUsuario.Text + "&clave=" + txtClave.Password;
            var uri = new Uri(url);
            //Crea un objeto: request de la clase: HttpWebRequest dada el uri o url
            var request = (HttpWebRequest)WebRequest.Create(uri);
            //Llama asincrónicamente al método: HandleRequest pasando como parámetro: request
            request.BeginGetResponse(new AsyncCallback(HandleRequest), request);
        }

        //Método: HandleRequest. Maneja el requerimiento asincrónicamente
        private void HandleRequest(IAsyncResult asyncResult)
        {
            var request = asyncResult.AsyncState as HttpWebRequest;
            using (var response = (HttpWebResponse)request.EndGetResponse(asyncResult))
            {
                //Obtiene la tira Json del Microservicio
                string tiraJson = new StreamReader(response.GetResponseStream()).ReadToEnd();
                Dispatcher.BeginInvoke(() =>
                {
                    //Se instancia la clase Usuarios para asignarle la tira Json a cada atributo de la clase Usuarios a través de: DataContractJsonSerializer
                    UsuariosWS objusuarios = new UsuariosWS();
                    MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(tiraJson));
                    DataContractJsonSerializer serializar = new DataContractJsonSerializer(objusuarios.GetType());
                    objusuarios = serializar.ReadObject(ms) as UsuariosWS;
                    //Validamos si encontró al usuario a través del atributo: exito
                    if (objusuarios.exito.CompareTo("1") == 0)
                    {
                        //Llamamos a otra página xaml llamada: Verusuario.xaml, pasando los parámetros: usuario, clave y nivel 
                        NavigationService.Navigate(new Uri("/Verusuario.xaml?usuario=" + objusuarios.usuario + "&clave=" + objusuarios.clave + "&nivel=" + objusuarios.nivel + "&checkRecordar=" + checkRecordar.IsChecked, UriKind.Relative));
                    }
                    else
                    {
                        MessageBox.Show("No existe el usuario.");
                    }
                });
            }
        }
        /*
         * Fin del Uso HttpWebRequest
         */
    }
   
    /*
     * Clase Usuarios que se necesita para La deserialización del objeto enviado por el Responde del micro servicio.
     */
    public class UsuariosWS
    {
        public string exito { get; set; }
        public string usuario { get; set; }
        public string clave { get; set; }
        public string nivel { get; set; }
    }
   
}