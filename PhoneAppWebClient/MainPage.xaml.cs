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
            /* Descomente BuscarUsuario() cuando desee usar WebClient  y comente a: buscarUsuario(); */
            //BuscarUsuario();
            /* Descomente buscarUsuario() cuando desee usar HttpWebRequest  y comente a: BuscarUsuario(); */
            buscarUsuario();
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

        //Método: BuscarUsuario() que usa WebClient 
        private void BuscarUsuario()
        {
            string url = "http://192.168.1.100/servidor-restful/Despachador.php?servicio=1&usuario="+ txtUsuario.Text + "&clave=" + txtClave.Password;
            var uri = new Uri(url);
            //Crea un objeto: cliente de la clase: WebClient dada el uri o url
            var cliente = new WebClient();
            //Si es recibida totalmente la tira Json comenzará su ejecución las instrucciones despues del indicador: =>
            cliente.DownloadStringCompleted += (sender, e) =>
            {
                //Obtiene la tira Json del Microservicio
                var tiraJson = e.Result;
                Dispatcher.BeginInvoke(() =>
                {
                    //Se instancia la clase Usuarios para asignarle la tira Json a cada atributo de la clase Usuarios a través de: DataContractJsonSerializer
                    Usuarios objusuarios = new Usuarios();
                    MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(System.Net.HttpUtility.UrlDecode(tiraJson)));
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(objusuarios.GetType());
                    objusuarios = ser.ReadObject(ms) as Usuarios;
                    //Validamos si encontró al usuario a través del atributo: exito
                    if (objusuarios.exito.CompareTo("1")==0)
                    {
                        //Llamamos a otra página xaml llamada: Verusuario.xaml, pasando los parámetros: usuario, clave y nivel 
                        NavigationService.Navigate(new Uri("/Verusuario.xaml?usuario=" + objusuarios.usuario + "&clave=" + objusuarios.clave + "&nivel=" + objusuarios.nivel, UriKind.Relative));
                        
                    }
                    else
                    {
                        MessageBox.Show("No existe el usuario.");
                    }
                });
            };
            /*
             * Se dispara la descarga o llamada asincrónicamente del uri o url. 
             * Si es completado o llega la tira Json se disparará el método: DownloadStringCompleted
             */
            cliente.DownloadStringAsync(uri);
        }
        /*
         * Fin del Uso WebClient
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
                    Usuarios objusuarios = new Usuarios();
                    MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(tiraJson));
                    DataContractJsonSerializer serializar = new DataContractJsonSerializer(objusuarios.GetType());
                    objusuarios = serializar.ReadObject(ms) as Usuarios;
                    //Validamos si encontró al usuario a través del atributo: exito
                    if (objusuarios.exito.CompareTo("1") == 0)
                    {
                        //Llamamos a otra página xaml llamada: Verusuario.xaml, pasando los parámetros: usuario, clave y nivel 
                        NavigationService.Navigate(new Uri("/Verusuario.xaml?usuario=" + objusuarios.usuario + "&clave=" + objusuarios.clave + "&nivel=" + objusuarios.nivel, UriKind.Relative));
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
    public class Usuarios
    {
        public string exito { get; set; }
        public string usuario { get; set; }
        public string clave { get; set; }
        public string nivel { get; set; }
    }
   
}