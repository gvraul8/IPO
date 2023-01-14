using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Xml;

namespace HITO2_IPO_NUEVO
{
    /// <summary>
    /// Lógica de interacción para Principal.xaml
    /// </summary>
    public partial class Principal : Window
    {
        private MainWindow login;
        List<Ruta> listadoRutas = new List<Ruta>();
        List<PuntoInteres> listadoPuntosInteres = new List<PuntoInteres>();
        List<Excursionista> listadoExcursionistas = new List<Excursionista>();
        List<Guia> listadoGuias = new List<Guia>();
        List<Oferta> listadoOfertas = new List<Oferta>();
        Usuario user;
        private BitmapImage imagCheck = new BitmapImage(new Uri("/Imagenes/check.png", UriKind.Relative)); 
        private BitmapImage imagCross = new BitmapImage(new Uri("/Imagenes/incorrect.png", UriKind.Relative));
        private BitmapImage imagAdd = new BitmapImage(new Uri("/Imagenes/point-of-interest.png", UriKind.Relative));
        private Uri IMAGEN_USUARIO_DEFAULT = (new Uri("https://mymef.org/wp-content/uploads/2016/10/icon-user-default.png", UriKind.Absolute));
        private Uri IMAGEN_RUTA_DEFAULT = (new Uri("https://fotografias.lasexta.com/clipping/cmsimages01/2020/12/14/486AFE7D-A545-4476-91F4-1790FB99CDB0/default.jpg?crop=1300,731,x0,y19&width=1900&height=1069&optimize=low", UriKind.Absolute));
        private Uri IMAGEN_OFERTA_DEFAULT = (new Uri("https://i0.wp.com/rogarsol.com/wp-content/uploads/2017/07/oferta.png?resize=301%2C167", UriKind.Absolute));
        private Uri IMAGEN_PDI_DEFAULT = (new Uri("https://img.freepik.com/fotos-premium/puntos-destino-viaje-mapa-indicado-chinchetas-coloridas_93675-31562.jpg?w=2000", UriKind.Absolute));
        
        static Guia guiaNulo = null;
        static Ruta rutaNula = new Ruta("", "", "", "", DateTime.Today, "", 0, "", 0, guiaNulo);

        private bool impresoNombresRutas = false;
        private bool editandoRuta = false;
        private bool rutaSinGuia = false;

        public Principal(Usuario u)
        {
            user = u;
            InitializeComponent();
            PrintUserData();


            listadoRutas = CargarContenidoRutasXML();
            imprimirNombreRutas(0);
            impresoNombresRutas = true;
            inicializaComponentesRutas();

            listadoExcursionistas = CargarContenidoExcursionistasXML();
            imprimirNombreExcursionistas();
            inicializaComponenentesExcursionistas();

            listadoGuias = CargarContenidoGuiasXML();
            imprimirNombreGuias();
            inicializaComponenentesGuias();

            var random = new Random();
            foreach (Ruta rutaAux in listadoRutas)
            {
                
                int aleatorio = random.Next(0, listadoGuias.Count);
                rutaAux.Guia = listadoGuias[aleatorio];
            }

            listadoOfertas = CargarContenidoOfertasXML();
            imprimirNombreOfertas();
            inicializaComponenentesOfertas();

            listadoPuntosInteres = CargarContenidoPuntosInteresXML();
            inicializaComponenentesPuntosInteres();

        }

        ///////////////////////////////////////////////////////////////////////////
        /// ---------------  DATOS USUARIO -------------------------------------
        //////////////////////////////////////////////////////////////////////////
        void PrintUserData()
        {
            lbNombreUsuario.Content = user.Name.ToString(); ;
            lbApellidosUsuario.Content = user.LastName.ToString();
            lbEmailUsuario.Content=user.Email.ToString();
            lbLastLoginUsuario.Content = user.LastLogin.ToString();
           // lbUltimoAccesoUsuario.Content = user.LastLogin.ToString();

            var fullFilePath = user.ImgUrl.ToString();
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
            bitmap.EndInit();
            imgUsuario.Source = bitmap;
        }

        private void btCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            login= new MainWindow();
            login.Show();
            this.Close();
        }

        private void btn_Ayuda_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/gvraul8/IPO/wiki/AYUDA");
        }

        ///////////////////////////////////////////////////////////////////////////
        /// ----------------------------  PESTAÑA RUTAS 
        ///////////////////////////////////////////////////////////////////////////

        

        private void bt_editars_Click(object sender, RoutedEventArgs e)
        {
            tcPestanas.SelectedIndex = 1;
        }

        private void edicionTextBox(Boolean bloqueado)
        {
            tb_nombre.IsReadOnly = bloqueado;
            tb_origen.IsReadOnly = bloqueado;
            tb_destino.IsReadOnly = bloqueado;
            tb_provincia.IsReadOnly = bloqueado;
            dp_fecha.IsEnabled = !bloqueado;
            tb_dificultad.IsReadOnly = bloqueado;
            tb_plazas.IsReadOnly = bloqueado;

            if (bloqueado == false)
            {
                tb_nombre.Text = "Escriba aqui el nombre de la ruta";
                bt_verGuiaRuta.Content = "Seleccionar guia";
                img_btnConsultarGuia.Source = imagAdd;
                bt_consultarPDis.Content = "Añadir PDI";
                img_btnConsultarPDIs.Source = imagAdd;
                // bt_consultarPDis.IsEnabled = !bloqueado;
                tb_nombre.Focus();
            }
            else
            {
                bt_verGuiaRuta.Content = "Consultar guia";
                bt_consultarPDis.Content = "Consultar PDIs";
            }

        }

        private void cambi_seleccionTipoRuta(object sender, SelectionChangedEventArgs e)
        {
            if (cb_tipoRuta.SelectedIndex == 1)
            {
                imprimirNombreRutas(1);
            }
            else if (cb_tipoRuta.SelectedIndex == 2)
            {
                imprimirNombreRutas(2);
            }
            else
            {
                imprimirNombreRutas(0); 
            }
        }

        private void imprimirNombreRutas(int cb_selected)
        {


            if (cb_selected== 0)
            {
                if (impresoNombresRutas == true)
                {
                    ListBoxRutas.Items.Clear();
                }
                foreach (Ruta ruta in listadoRutas)
                {
                    ListBoxRutas.Items.Add(ruta.Nombre);
                }
            } 
            else
            {
                ListBoxRutas.Items.Clear();
                foreach (Ruta ruta in listadoRutas)
                {
                    if (ruta.Fecha <  DateTime.Now)
                    {
                        if (cb_selected == 1)
                        {
                            ListBoxRutas.Items.Add(ruta.Nombre);
                        }
                    }
                    else
                    {
                        if (cb_selected == 2)
                        {
                            ListBoxRutas.Items.Add(ruta.Nombre);
                        }
                    }
                }
            }
        }

        private List<Ruta> CargarContenidoRutasXML()
        {
            List<Ruta> listado = new List<Ruta>();
            // Cargar contenido de prueba
            XmlDocument doc = new XmlDocument();

            var fichero = Application.GetResourceStream(new Uri("Datos/rutas.xml", UriKind.Relative));
            //var fichero = Application.GetResourceStream(new Uri("bin/Debug/rutas.xml", UriKind.Relative));

            doc.Load(fichero.Stream);

            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                

                Guia guiaAux = null;
                //Ruta(string nombre, string origen, string destino, string provincia, DateTime fecha, string dificultad, int plazasDisponibles, string material, int numRealizaciones)
                var nuevaRuta = new Ruta("", "", "", "", DateTime.Today, "", 0, "", 0, guiaAux);
                nuevaRuta.Nombre = node.Attributes["Nombre"].Value;
                nuevaRuta.Origen = node.Attributes["Origen"].Value;
                nuevaRuta.Destino = node.Attributes["Destino"].Value;
                nuevaRuta.Provincia = node.Attributes["Provincia"].Value;
                nuevaRuta.Fecha = Convert.ToDateTime(node.Attributes["Fecha"].Value);
                nuevaRuta.Dificultad = node.Attributes["Dificultad"].Value;
                nuevaRuta.PlazasDisponibles = Convert.ToInt32(node.Attributes["PlazasDisponibles"].Value);
                nuevaRuta.MaterialNecesario = node.Attributes["MaterialNecesario"].Value;
                nuevaRuta.NumeroDeRealizaciones = Convert.ToInt32(node.Attributes["NumeroDeRealizaciones"].Value);
                nuevaRuta.URL_RUTA = new Uri(node.Attributes["URL_RUTA"].Value, UriKind.Absolute);
                //nuevaRuta.Guia = listadoGuias[aleatorio];
                //nuevaRuta.URL_INTERES = new Uri(node.Attributes["URL_INTERES"].Value, UriKind.Absolute);
                listado.Add(nuevaRuta);
            }
            return listado;
        }

        private void inicializaComponentesRutas()
        {
            tb_nombre.Text = "";
            tb_origen.Text = "";
            tb_destino.Text = "";
            tb_provincia.Text = "";
            tb_dificultad.Text = "";
            tb_plazas.Text = "";
            dp_fecha.Text = "";
            dp_fecha.IsEnabled = true;
            img_ruta.Source = new BitmapImage();


            bt_anadir.IsEnabled = true;
            bt_editar.IsEnabled = false;
            bt_eliminar.IsEnabled = false;

            bt_guardarRuta.IsEnabled = false;  //poner a true cuando se pulse el de añadir
            bt_consultarPDis.IsEnabled = false;
            bt_verGuiaRuta.IsEnabled = false;

            img_bt_guia.Visibility = Visibility.Hidden;
            img_bt_pdis.Visibility = Visibility.Hidden;

            cb_elegirExcursionista.IsEnabled = false;
            cb_elegirExcursionista.SelectedIndex = -1;
            list_excursionistas.Items.Clear();

            img_bt_guardarRuta.Visibility = Visibility.Hidden;
            img_tb_fecha.Visibility = Visibility.Hidden;

            img_tb_plazas.Visibility = Visibility.Hidden;

            chb_apuntarseRuta.IsChecked = false;
            chb_apuntarseRuta.IsEnabled = false;

            tiRutas.IsEnabled = true;
            tiGuia.IsEnabled = true;
            tiExcursionista.IsEnabled = true;
            tiOfertas.IsEnabled = true;

            dp_fecha.Background = Brushes.Transparent;
        }

        private void cambiaModoCasillasRuta(bool soloLectura)
        {
      
            tb_nombre.IsReadOnly = soloLectura;
            tb_nombre.IsReadOnly = soloLectura;
            tb_origen.IsReadOnly = soloLectura;
            tb_destino.IsReadOnly = soloLectura;
            tb_provincia.IsReadOnly = soloLectura;
            tb_dificultad.IsReadOnly = soloLectura;
            tb_plazas.IsReadOnly = soloLectura;
            dp_fecha.IsEnabled = !soloLectura;

            if (!soloLectura)
            {
                if (tb_nombre.Text == String.Empty) //evita que se edite el nombre de la ruta existente
                {
                    tb_nombre.Text = "Escriba aqui el nombre de la nueva ruta";
                }
                bt_verGuiaRuta.Content = "Seleccionar guia";
                bt_consultarPDis.Content = "Añadir PDI";
                bt_verGuiaRuta.IsEnabled = true;
                bt_consultarPDis.IsEnabled = true;
            }
        }

        private void rellenaCasillasRuta(object sender, SelectionChangedEventArgs e)
        {
            

            if (ListBoxRutas.SelectedItem != null)
            {
              
                chb_apuntarseRuta.IsChecked = false;

                int index = buscaRuta(ListBoxRutas.SelectedItem.ToString());

                var rutaAux = listadoRutas[index];

                tb_nombre.Text = rutaAux.Nombre.ToString();
                tb_origen.Text = rutaAux.Origen.ToString();
                tb_destino.Text = rutaAux.Origen.ToString();
                tb_provincia.Text = rutaAux.Provincia.ToString();
                tb_dificultad.Text = rutaAux.Dificultad.ToString();
                tb_plazas.Text = rutaAux.PlazasDisponibles.ToString();
                //tb_material.Text = rutaAux.MaterialNecesario.ToString();
                //tb_realizaciones.Text = rutaAux.NumeroDeRealizaciones.ToString();
                dp_fecha.Text = Convert.ToDateTime(rutaAux.Fecha.ToString()).ToString();

                // https://stackoverflow.com/questions/18435829/showing-image-in-wpf-using-the-url-link-from-database
                var fullFilePath = rutaAux.URL_RUTA.ToString();
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
                bitmap.EndInit();

                img_ruta.Source = bitmap;

   
                bt_anadir.IsEnabled = false;
                bt_editar.IsEnabled = true;
                bt_eliminar.IsEnabled = true;

                bt_consultarPDis.IsEnabled = true;
                bt_verGuiaRuta.IsEnabled = true;


                edicionTextBox(true);
                img_bt_guia.Visibility = Visibility.Hidden;
                img_bt_pdis.Visibility = Visibility.Hidden;

                cb_elegirExcursionista.IsEnabled = true;
                cb_elegirExcursionista.SelectedIndex = -1;
                list_excursionistas.Items.Clear();

                img_bt_guardarRuta.Visibility = Visibility.Hidden;
                img_tb_fecha.Visibility = Visibility.Hidden;

                tiRutas.IsEnabled = true;
                tiGuia.IsEnabled = true;
                tiExcursionista.IsEnabled = true;
                tiOfertas.IsEnabled = true;

                inicializaComponenentesPuntosInteres();

                img_tb_plazas.Visibility = Visibility.Hidden;

                int posicionUsuario = buscaExcursionista(lbNombreUsuario.Content.ToString());
                if (posicionUsuario != -1)
                {
                    foreach(Ruta ruta in listadoExcursionistas[posicionUsuario].RutasFuturas)
                    {
                        if (ruta.Nombre == rutaAux.Nombre)
                        {
                            chb_apuntarseRuta.IsChecked = true;
                        }
                    }
                }
                else
                {
                    posicionUsuario = buscaGuia(lbNombreUsuario.Content.ToString());
                    foreach (Ruta ruta in listadoGuias[posicionUsuario].RutasFuturas)
                    {
                        if (ruta.Nombre == rutaAux.Nombre)
                        {
                            chb_apuntarseRuta.IsChecked = true;
                        }
                    }
                }

                if  (Convert.ToDateTime(dp_fecha.Text) > DateTime.Now)
                {
                    chb_apuntarseRuta.IsEnabled = true;
                    chb_apuntarseRuta.ToolTip = "Sólo puedes apuntarte a rutas que no se hayan realizado aún";
                }

                tb_nombre.IsEnabled = true;

                editandoRuta = false;
            }
            else
            {
                inicializaComponentesRutas();
                List<PuntoInteres> listaAux = new List<PuntoInteres>(listadoPuntosInteres);
                
                foreach (PuntoInteres puntoAux in listadoPuntosInteres)
                {
                    if (buscaRuta(puntoAux.Ruta.Nombre) == -1)
                    {
                        listaAux.Remove(puntoAux);
                    }
                }
                listadoPuntosInteres = listaAux;
            }
        }

        private void lstRutas_SelectionChanged(object sender, MouseButtonEventArgs controlUnderMouse)
        {
            if (controlUnderMouse.GetType() != typeof(ListBoxItem))
            {
                ListBoxRutas.SelectedItem = null;
                chb_apuntarseRuta.IsEnabled = false;
            }
        }

        private void bt_consultarPDis_Click(object sender, RoutedEventArgs e)
        {
            bt_consultarPDis.IsEnabled = true;
            
            tcPestanas.SelectedIndex = 4;

            int index = buscaRuta(tb_nombre.Text);
            if (index != -1)
            {
                var rutaAux = listadoRutas[index];
                imprimirNombrePuntosInteres(rutaAux.Nombre);
            }
            else if (!rutaTieneAlgunPDI(tb_nombre.Text))
            {
                cb_ruta_PDI.Items.Add(tb_nombre.Text);
                tiRutas.IsEnabled = false;
                tiGuia.IsEnabled = false;
                tiExcursionista.IsEnabled = false;
                tiOfertas.IsEnabled = false;
            }
               
            
        }

        private void click_añadir_Ruta(object sender, RoutedEventArgs e)
        {
            inicializaComponentesRutas();
            cambiaModoCasillasRuta(false);
            bt_guardarRuta.IsEnabled = true;

            tiRutas.IsEnabled = false;
            tiGuia.IsEnabled = false;
            tiExcursionista.IsEnabled = false;
            tiOfertas.IsEnabled = false;

            tb_nombre.IsEnabled = true;

            rutaSinGuia = true;

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = IMAGEN_RUTA_DEFAULT;
            bitmap.EndInit();
            img_ruta.Source = bitmap;

        }
        

        private Boolean rutaTieneAlgunPDI(String nombreRuta)
        {
            foreach(PuntoInteres puntoAux in listadoPuntosInteres)
            {
                if (puntoAux.Ruta.Nombre == nombreRuta)
                {
                    return true;
                }
            }
            return false;
        }


        private void clickGuardarRuta(object sender, RoutedEventArgs e)
        {
            img_tb_plazas.Visibility = Visibility.Hidden;
            if (rutaSinGuia)
                rutaNula.Guia = null;

            if (compruebaValidezCasillasRuta())
            {
                rutaNula.Nombre = tb_nombre.Text;
                rutaNula.Origen = tb_origen.Text;
                rutaNula.Destino = tb_destino.Text;
                rutaNula.Provincia = tb_provincia.Text;
                rutaNula.Dificultad = tb_dificultad.Text;
                rutaNula.PlazasDisponibles = int.Parse(tb_plazas.Text);

                foreach(Ruta rutaAux in listadoRutas)
                {
                    if (rutaAux.Nombre == rutaNula.Nombre)
                    {
                        rutaNula.Guia = rutaAux.Guia;
                    }
                }
                if (!editandoRuta && Convert.ToDateTime(dp_fecha.Text) < DateTime.Now)
                {
                    img_tb_fecha.Source = imagCross;
                    img_tb_fecha.Visibility = Visibility.Visible;
                    img_tb_fecha.ToolTip = "La fecha no puede ser anterior a la actual";
                    dp_fecha.Background = Brushes.Red;

                }
                else
                {
                    rutaNula.Fecha = Convert.ToDateTime(dp_fecha.Text);
                }

                if (Convert.ToInt32(tb_plazas.Text) < 4 || Convert.ToInt32(tb_plazas.Text) > 20)
                {
                    img_tb_plazas.Source = imagCross;
                    img_tb_plazas.Visibility = Visibility.Visible;
                    img_tb_plazas.ToolTip = "Las plazas disponibles deben estar entre 4 y 20";
                }
                else if (rutaNula.Guia != null && rutaTieneAlgunPDI(rutaNula.Nombre))
                {
                    img_bt_guardarRuta.Source = imagCheck;
                    img_bt_guardarRuta.Visibility = Visibility.Visible;
                    img_bt_guardarRuta.ToolTip = "Todos los datos requeridos han sido aportados";

                    

                    int posicionRuta = buscaRuta(rutaNula.Nombre);

                    if(posicionRuta != -1)
                    {
                        rutaNula.URL_RUTA = listadoRutas[posicionRuta].URL_RUTA;
                        listadoRutas[posicionRuta] = rutaNula;
                    }
                    else {
                        rutaNula.URL_RUTA = IMAGEN_RUTA_DEFAULT;
                        listadoRutas.Add(rutaNula);
                    }
                    tiRutas.IsEnabled = true;
                    tiGuia.IsEnabled = true;
                    tiExcursionista.IsEnabled = true;
                    tiOfertas.IsEnabled = true;

                    inicializaComponentesRutas();
                    ListBoxRutas.Items.Clear();
                    imprimirNombreRutas(0);

                    if (editandoRuta == true)
                        editandoRuta = false;

                    img_tb_plazas.Visibility = Visibility.Hidden;

                }
                else if (rutaNula.Guia == null && rutaTieneAlgunPDI(rutaNula.Nombre))
                {
                    bt_consultarPDis.Content = "Consultar PDIs";

                    img_bt_guia.Source = imagCross;
                    img_bt_guia.Visibility = Visibility.Visible;
                    img_bt_guia.ToolTip = "Añada un guía nuevo a la ruta en creación";

                    img_bt_pdis.Source = imagCheck;
                    img_bt_pdis.Visibility = Visibility.Visible;
                    img_bt_pdis.ToolTip = "La ruta cuenta con al menos 1 punto de interés";

                    img_tb_plazas.Visibility = Visibility.Hidden;

                }
                else if (rutaNula.Guia != null && !rutaTieneAlgunPDI(rutaNula.Nombre))
                {
                    bt_verGuiaRuta.Content = "Consultar guia";
                    
                    img_bt_guia.Source = imagCheck;
                    img_bt_guia.Visibility = Visibility.Visible;
                    img_bt_guia.ToolTip = "La ruta ya cuenta con un guía asignado";

                    img_bt_pdis.Source = imagCross;
                    img_bt_pdis.Visibility = Visibility.Visible;
                    img_bt_pdis.ToolTip = "Añada al menos un punto de interés nuevo a la ruta en creación";

                    img_tb_plazas.Visibility = Visibility.Hidden;

                }
                else
                {
                    img_bt_guia.Source = imagCross;
                    img_bt_guia.Visibility = Visibility.Visible;
                    img_bt_guia.ToolTip = "Añada un guía nuevo a la ruta en creación";

                    img_bt_pdis.Source = imagCross;
                    img_bt_pdis.Visibility = Visibility.Visible;
                    img_bt_pdis.ToolTip = "Añada al menos un punto de interés nuevo a la ruta en creación";

                    img_tb_plazas.Visibility = Visibility.Hidden;
                }
                
            }

            

        }

        private void cambi_seleccionTipoExcursionistas(object sender, SelectionChangedEventArgs e)
        {
            
            if (cb_elegirExcursionista.SelectedIndex == 0)
            {
                imprimeExcursionistasRealizaronRuta(ListBoxRutas.SelectedItem.ToString());
                imprimeGuiasRealizaronRuta(ListBoxRutas.SelectedItem.ToString());
            }
            else if (cb_elegirExcursionista.SelectedIndex == 1)
            {
                imprimeExcursionistasRealizaranRuta(ListBoxRutas.SelectedItem.ToString());
                imprimeGuiasRealizaranRuta(ListBoxRutas.SelectedItem.ToString());
            }
        }

        private void imprimeExcursionistasRealizaronRuta(String nombreRuta)
        {
            list_excursionistas.Items.Clear();

            foreach (Excursionista excursionistaAux in listadoExcursionistas)
            {
                foreach(Ruta rutaHecha in excursionistaAux.RutasRealizadas)
                {
                    if(rutaHecha.Nombre == nombreRuta)
                    {
                        list_excursionistas.Items.Add(excursionistaAux.Name);
                    }
                }
            }
        }

        private void imprimeExcursionistasRealizaranRuta(String nombreRuta)
        {
            list_excursionistas.Items.Clear();

            foreach (Excursionista excursionistaAux in listadoExcursionistas)
            {
                foreach (Ruta rutaFutura in excursionistaAux.RutasFuturas)
                {
                    if (rutaFutura.Nombre == nombreRuta)
                    {
                        list_excursionistas.Items.Add(excursionistaAux.Name);
                    }
                }
            }
        }

        private void imprimeGuiasRealizaronRuta(String nombreRuta)
        {
            
            foreach (Guia guiaAux in listadoGuias)
            {
                foreach (Ruta rutaHecha in guiaAux.RutasRealizadas)
                {
                    if (rutaHecha.Nombre == nombreRuta)
                    {
                        list_excursionistas.Items.Add(guiaAux.Name);
                    }
                }
            }
        }

        private void imprimeGuiasRealizaranRuta(String nombreRuta)
        {
            foreach (Guia guiaAux in listadoGuias)
            {
                foreach (Ruta rutaHecha in guiaAux.RutasFuturas)
                {
                    if (rutaHecha.Nombre == nombreRuta)
                    {
                        list_excursionistas.Items.Add(guiaAux.Name);
                    }
                }
            }
        }

        private bool compruebaValidezCasillasRuta()
        {
            bool valido = true;

            if (tb_nombre.Text == "" || tb_origen.Text == "" || tb_destino.Text == "" || tb_provincia.Text == "" || tb_dificultad.Text == "" || tb_plazas.Text == "" || dp_fecha.Text == "")
            {
                img_bt_guardarRuta.Visibility = Visibility.Visible;
                img_bt_guardarRuta.Source = imagCross;
                img_bt_guardarRuta.ToolTip = "Debe rellenar todos los campos de esta pestaña para guardar la nueva ruta";
                valido = false;
            }
            else
            {
                img_bt_guardarRuta.Visibility = Visibility.Visible;
                img_bt_guardarRuta.Source = imagCheck;
                img_bt_guardarRuta.ToolTip = "Campos de esta pestaña rellenados correctamente";

            }


            return valido;
        }


        private int buscaRuta(String nombreRuta)
        {
            int posicionRuta = -1;
            int contador = 0;

            foreach(Ruta rutaAux in listadoRutas)
            {
                if (rutaAux.Nombre == nombreRuta)
                {
                    posicionRuta = contador;
                }
                contador++;
            }

            return posicionRuta;
        }

        private void click_consultar_guia(object sender, RoutedEventArgs e)
        {
            img_bt_asignarGuia.Visibility = Visibility.Hidden;

            img_bt_guardarGuia.Visibility = Visibility.Hidden;
            
            tcPestanas.SelectedIndex = 1;
            if (bt_verGuiaRuta.Content == "Seleccionar guia") {
                tiRutas.IsEnabled = false;
                tiGuia.IsEnabled = false;
                tiExcursionista.IsEnabled = false;
                tiOfertas.IsEnabled = false;
                inicializaComponenentesGuias();
            }
            else
            {
                int posicionRuta = buscaRuta(tb_nombre.Text);
                int contador = 0;

                foreach(Guia guiaAux in listadoGuias)
                {
                    if (guiaAux == listadoRutas[posicionRuta].Guia)
                    {
                        ListBoxGuias.SelectedIndex = contador;
                    }
                    contador++;
                }
            }
        }

        private void click_eliminar_ruta(object sender, RoutedEventArgs e)
        {
            int posicionRuta = buscaRuta(tb_nombre.Text);
            listadoRutas.Remove(listadoRutas[posicionRuta]);
            ListBoxRutas.Items.Clear();
            imprimirNombreRutas(0);
            inicializaComponentesRutas();
            List<PuntoInteres> listaAux = new List<PuntoInteres>(listadoPuntosInteres);

            foreach (PuntoInteres puntoAux in listadoPuntosInteres)
            {
                if (buscaRuta(puntoAux.Ruta.Nombre) == -1)
                {
                    listaAux.Remove(puntoAux);
                }
            }
            listadoPuntosInteres = listaAux;
        }

        private void click_editar_ruta(object sender, RoutedEventArgs e)
        {
            cambiaModoCasillasRuta(false);
            bt_guardarRuta.IsEnabled = true;

            tb_nombre.IsEnabled = false;

            editandoRuta = true;
        }









        ///////////////////////////////////////////////////////////////////////////
        /// ----------------------------  PESTAÑA EXCURSIONISTAS
        ///////////////////////////////////////////////////////////////////////////


        private void imprimirNombreExcursionistas()
        {
            ListBoxExcursionistas.Items.Clear();
            foreach (Excursionista excursionista in listadoExcursionistas)
            {
                ListBoxExcursionistas.Items.Add(excursionista.Name);
            }
        }

        private List<Excursionista> CargarContenidoExcursionistasXML()
        {
            
            List<Excursionista> listado = new List<Excursionista>();

            // Cargar contenido de prueba
            XmlDocument doc = new XmlDocument();
            var fichero = Application.GetResourceStream(new Uri("Datos/excursionistas.xml", UriKind.Relative));
            doc.Load(fichero.Stream);
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                Guia guiaAux = null;
                var rutaAux = new Ruta("", "", "", "", DateTime.Today, "", 0, "", 0, guiaAux);
                var nuevoExcursionista = new Excursionista("", "", "", "", "", DateTime.Today, "", 1, true, listadoRutas, listadoRutas);
                nuevoExcursionista.User = node.Attributes["User"].Value;
                nuevoExcursionista.Pass = node.Attributes["Pass"].Value;
                nuevoExcursionista.Name = node.Attributes["Name"].Value;
                nuevoExcursionista.LastName = node.Attributes["LastName"].Value;
                nuevoExcursionista.Email = node.Attributes["Email"].Value;
                nuevoExcursionista.LastLogin = Convert.ToDateTime(node.Attributes["LastLogin"].Value);
                nuevoExcursionista.Telefono = node.Attributes["Phone"].Value;
                nuevoExcursionista.ImgUrl = new Uri(node.Attributes["ImgUrl"].Value);
                nuevoExcursionista.Edad = int.Parse(node.Attributes["Edad"].Value);
                nuevoExcursionista.Notificaciones = Convert.ToBoolean(node.Attributes["Notificaciones"].Value);

                string[] rutasRealizadas = node.Attributes["RutasRealizadas"].Value.Split(',');
                List<Ruta> listadoRutasRealizadas = new List<Ruta>();
                foreach (string nombreRutaRealizadas in rutasRealizadas)
                {
                    for (int i = 0; i < listadoRutas.Count; i++)
                    {
                        if (listadoRutas[i].Nombre == nombreRutaRealizadas)
                        {
                            listadoRutasRealizadas.Add(listadoRutas[i]);
                        }
                    }
                }
                nuevoExcursionista.RutasRealizadas = listadoRutasRealizadas;

                string[] RutasFuturas = node.Attributes["RutasFuturas"].Value.Split(',');
                List<Ruta> listadoRutasFuturas = new List<Ruta>();
                foreach (string nombreRutaFuturas in RutasFuturas)
                {
                    for (int i = 0; i < listadoRutas.Count; i++)
                    {
                        if (listadoRutas[i].Nombre == nombreRutaFuturas)
                        {
                            listadoRutasFuturas.Add(listadoRutas[i]);
                        }
                    }
                }
                nuevoExcursionista.RutasFuturas = listadoRutasFuturas;

                listado.Add(nuevoExcursionista);
            }
            return listado;
        }

        void inicializaComponenentesExcursionistas()
        {
            cb_rutasEx.IsEnabled = false;
            cb_ofertas.IsEnabled = false;
            cb_ofertas.SelectedIndex = -1;
            tb_nombre_excursionista.Text = "";
            tb_edad.Text =  "";
            tb_telefonoexcursionista.Text = "";
            tb_correoexcursionista.Text = "";
            img_excursionista.Source = new BitmapImage();

            bt_anadirExcursionista.IsEnabled = true;
            bt_editarExcursionista.IsEnabled = false;
            bt_eliminarExcursionista.IsEnabled = false;
            lb_rutasrealplaEx.Items.Clear();
            

            cambiaModoCasillasExcursionista(true);
            img_bt_guardarExcursionista.Visibility = Visibility.Hidden;
            img_tb_edadExcursionista.Visibility=Visibility.Hidden;
            bt_guardarEx.IsEnabled = false;

            cb_rutasEx.IsEnabled = false;
            cb_rutasEx.SelectedIndex = -1;

            tb_telefonoexcursionista.Background = Brushes.Transparent;
            tb_correoexcursionista.Background = Brushes.Transparent;
            img_tb_telefonoExcursionista.Visibility = Visibility.Hidden;
            img_tb_correoExcursionista.Visibility = Visibility.Hidden;
            tb_edad.Background = Brushes.Transparent;
        }

        private void cambiaModoCasillasExcursionista(bool soloLectura)
        {
            tb_nombre_excursionista.IsReadOnly = soloLectura;
            tb_edad.IsReadOnly = soloLectura;
            tb_telefonoexcursionista.IsReadOnly = soloLectura;
            tb_correoexcursionista.IsReadOnly = soloLectura;
            cb_ofertas.IsEnabled = !soloLectura;
            

            if (!soloLectura)
            {
                if (tb_nombre_excursionista.Text == String.Empty) {
                    tb_nombre_excursionista.Text = "Escriba aqui el nombre del nuevo excursionista";
                }
                
            }
        }


        private void rellenaCasillasExcursionista(object sender, SelectionChangedEventArgs e)
        {

            if (ListBoxExcursionistas.SelectedItem != null)
            {

                lb_rutasrealplaEx.Items.Clear();
                cb_ofertas.IsEnabled = true;
                cb_rutasEx.IsEnabled = true;

                int index = ListBoxExcursionistas.SelectedIndex;
                var excursionistaAux = listadoExcursionistas[index];

                tb_nombre_excursionista.Text = excursionistaAux.Name;
                tb_edad.Text = excursionistaAux.Edad.ToString();
                tb_telefonoexcursionista.Text = excursionistaAux.Telefono;
                tb_correoexcursionista.Text = excursionistaAux.Email;
                if (excursionistaAux.ImgUrl != null)
                {
                    var fullFilePath = excursionistaAux.ImgUrl.ToString();
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
                    bitmap.EndInit();
                    img_excursionista.Source = bitmap;
                }

                if (excursionistaAux.Notificaciones)
                {
                    cb_ofertas.SelectedIndex = 0;
                }
                else
                {
                    cb_ofertas.SelectedIndex = 1;
                }

                bt_anadirExcursionista.IsEnabled = false;
                bt_editarExcursionista.IsEnabled = true;
                bt_eliminarExcursionista.IsEnabled = true;

                cb_ofertas.IsEnabled = false;
                cambiaModoCasillasExcursionista(true);

                tb_nombre_excursionista.IsEnabled = true;
            }
            else 
            {
                inicializaComponenentesExcursionistas();
            }
        }

        private void lstExcursionistas_SelectionChanged(object sender, MouseButtonEventArgs controlUnderMouse)
        {
            if (controlUnderMouse.GetType() != typeof(ListBoxItem))
            {
                ListBoxExcursionistas.SelectedItem = null;
            }
        }

        private void ComboBoxRutasExcursionistas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_rutasEx.SelectedValue != null)
            {
                string selectedItem = cb_rutasEx.SelectedValue.ToString();

                if (selectedItem.Equals("System.Windows.Controls.ComboBoxItem: Rutas Realizadas"))
                {
                    imprimeRutasRealizarExcursionista();
                }
                else if (selectedItem.Equals("System.Windows.Controls.ComboBoxItem: Rutas Planificadas"))
                {
                    imprimeRutasFuturasExcursionista();
                }
            }
        }

        private void imprimeRutasRealizarExcursionista()
        {
            lb_rutasrealplaEx.Items.Clear();
            int index = ListBoxExcursionistas.SelectedIndex;
            var excursionistaAux = listadoExcursionistas[index];

            foreach (Ruta ruta in excursionistaAux.RutasRealizadas)
            {
                lb_rutasrealplaEx.Items.Add(ruta.Nombre + " (" + ruta.Fecha + ")");
            }
        }

        private void imprimeRutasFuturasExcursionista()
        {
            lb_rutasrealplaEx.Items.Clear();
            int index = ListBoxExcursionistas.SelectedIndex;
            var excursionistaAux = listadoExcursionistas[index];

            foreach (Ruta ruta in excursionistaAux.RutasFuturas)
            {
                lb_rutasrealplaEx.Items.Add(ruta.Nombre + " (" + ruta.Fecha + ")");
            }
        }

        private void click_añadir_Excursionista(object sender, RoutedEventArgs e)
        {
            inicializaComponenentesExcursionistas();
            bt_guardarEx.IsEnabled = true;
            cb_ofertas.IsEnabled = true;
            cambiaModoCasillasExcursionista(false);

            tb_nombre_excursionista.IsEnabled = true;

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = IMAGEN_USUARIO_DEFAULT;
            bitmap.EndInit();
            img_excursionista.Source = bitmap;
        }

        private void compruebaValidezCasillasExcursionista(object sender, TextCompositionEventArgs e)
        {

            if (tb_nombre_excursionista.Text == "" || tb_edad.Text == "" || tb_telefonoexcursionista.Text == "" || tb_correoexcursionista.Text == "")
            {
                img_bt_guardarExcursionista.Visibility = Visibility.Visible;
                img_bt_guardarExcursionista.Source = imagCross;
                img_bt_guardarExcursionista.ToolTip = "Debe rellenar todos los campos de esta pestaña para guardar el nuevo excursionista";

            }
            else
            {
                img_bt_guardarExcursionista.Visibility = Visibility.Visible;
                img_bt_guardarExcursionista.Source = imagCheck;
                img_bt_guardarExcursionista.ToolTip = "Campos de esta pestaña rellenados correctamente";

            }

        }

        private bool compruebaValidezCasillasExcursionista()
        {
            bool valido = true;

            if (tb_nombre_excursionista.Text == "" || tb_edad.Text == "" || tb_telefonoexcursionista.Text == "" || tb_correoexcursionista.Text == "")
            {
                img_bt_guardarExcursionista.Visibility = Visibility.Visible;
                img_bt_guardarExcursionista.Source = imagCross;
                img_bt_guardarExcursionista.ToolTip = "Debe rellenar todos los campos de esta pestaña para guardar el nuevo excursionista";
                valido = false;
            }
            else
            {
                img_bt_guardarExcursionista.Visibility = Visibility.Visible;
                img_bt_guardarExcursionista.Source = imagCheck;
                img_bt_guardarExcursionista.ToolTip = "Campos de esta pestaña rellenados correctamente";

            }


            return valido;
        }

        private void clickGuardarExcursionista(object sender, RoutedEventArgs e)
        {
            //if (string.IsNullOrEmpty(Textbox1.Text))
            List<Ruta> listaRutasAux = new List<Ruta>();
            Excursionista excursionistaAux = new Excursionista("", "", "", "", "", DateTime.Today, "", 1, true, listaRutasAux, listaRutasAux);

            

            if (compruebaValidezCasillasExcursionista()) {
                excursionistaAux.Name = tb_nombre_excursionista.Text;
                excursionistaAux.Edad = int.Parse(tb_edad.Text);
                excursionistaAux.Telefono = tb_telefonoexcursionista.Text;
                excursionistaAux.Email = tb_correoexcursionista.Text;

                if (cb_ofertas.SelectedIndex == 0)
                {
                    excursionistaAux.Notificaciones = true;
                }
                else if (cb_ofertas.SelectedIndex == 1)
                {
                    excursionistaAux.Notificaciones = false;
                }

                int posicionExcursionista = buscaExcursionista(excursionistaAux.Name);

                if (posicionExcursionista == -1) { 
                    excursionistaAux.ImgUrl = IMAGEN_USUARIO_DEFAULT;
                    listadoExcursionistas.Add(excursionistaAux);
                    imprimirNombreExcursionistas();
                    img_bt_guardarExcursionista.Source = imagCheck;
                    img_bt_guardarExcursionista.ToolTip = "Se ha guardado el excursionista correctamente";
                    
                }
                else
                {
                    excursionistaAux.ImgUrl = listadoExcursionistas[posicionExcursionista].ImgUrl;
                    listadoExcursionistas[posicionExcursionista] = excursionistaAux;
                }
                inicializaComponenentesExcursionistas();
                imprimeExcursionistasNotificaciones();
            }
        }

        private void NumericOnlyEdadExcursionista(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = IsTextNumeric(e.Text);
            if (e.Handled == false)
            {
                img_tb_edadExcursionista.Source = imagCheck;
                img_tb_edadExcursionista.Visibility = Visibility.Visible;
                img_tb_edadExcursionista.ToolTip = "Formato adecuado";
                tb_edad.Background = Brushes.Green;
            }
            else
            {
                img_tb_edadExcursionista.Source = imagCross;
                img_tb_edadExcursionista.Visibility = Visibility.Visible;
                img_tb_edadExcursionista.ToolTip = "Debes introducir un formato numérico";
                tb_edad.Background = Brushes.Red;
            }

        }

        private void TelefonoFormatExcursionista(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = IsTextNumeric(e.Text);
            if (e.Handled == false)
            {
                if(tb_telefonoexcursionista.Text.Length == 8)
                {
                    img_tb_telefonoExcursionista.Source = imagCheck;
                    img_tb_telefonoExcursionista.Visibility = Visibility.Visible;
                    img_tb_telefonoExcursionista.ToolTip = "Formato adecuado";
                    tb_telefonoexcursionista.Background = Brushes.Green;
                }
                else
                {
                    img_tb_telefonoExcursionista.Source = imagCross;
                    img_tb_telefonoExcursionista.Visibility = Visibility.Visible;
                    img_tb_telefonoExcursionista.ToolTip = "Debes introducir 9 dígitos";
                    tb_telefonoexcursionista.Background = Brushes.Red;
                }

            }
            else
            {
                img_tb_telefonoExcursionista.Source = imagCross;
                img_tb_telefonoExcursionista.Visibility = Visibility.Visible;
                img_tb_telefonoExcursionista.ToolTip = "Debes introducir un formato numérico";
                tb_telefonoexcursionista.Background = Brushes.Red;
            }

        }


        private void tb_emailExcursionista_TextChanged(object sender, TextCompositionEventArgs e)
        {
            if (e.Handled == false)
            {
                Boolean valido;

                Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{1,4}$");

                valido = regex.IsMatch(tb_correoexcursionista.Text);

                if (valido)
                {
                    img_tb_correoExcursionista.Source = imagCheck;
                    img_tb_correoExcursionista.Visibility = Visibility.Visible;
                    img_tb_correoExcursionista.ToolTip = "Formato adecuado";
                    tb_correoexcursionista.Background = Brushes.Green;
                }
                else
                {
                    img_tb_correoExcursionista.Source = imagCross;
                    img_tb_correoExcursionista.Visibility = Visibility.Visible;
                    img_tb_correoExcursionista.ToolTip = "Debes introducir un email";
                    tb_correoexcursionista.Background = Brushes.Red;
                }
            }
            else
            {
                img_tb_correoExcursionista.Source = imagCross;
                img_tb_correoExcursionista.Visibility = Visibility.Visible;
                img_tb_correoExcursionista.ToolTip = "Debes introducir un formato válido";
                tb_correoexcursionista.Background = Brushes.Red;
            }
        }


        private void TelefonoFormatGuia(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = IsTextNumeric(e.Text);
            if (e.Handled == false)
            {
                if (tb_telefonoguia.Text.Length == 8)
                {
                    img_tb_telefonoGuia.Source = imagCheck;
                    img_tb_telefonoGuia.Visibility = Visibility.Visible;
                    img_tb_telefonoGuia.ToolTip = "Formato adecuado";
                    tb_telefonoguia.Background = Brushes.Green;
                }
                else
                {
                    img_tb_telefonoGuia.Source = imagCross;
                    img_tb_telefonoGuia.Visibility = Visibility.Visible;
                    img_tb_telefonoGuia.ToolTip = "Debes introducir 9 dígitos";
                    tb_telefonoguia.Background = Brushes.Red;
                }

            }
            else
            {
                img_tb_telefonoGuia.Source = imagCross;
                img_tb_telefonoGuia.Visibility = Visibility.Visible;
                img_tb_telefonoGuia.ToolTip = "Debes introducir un formato numérico";
                tb_telefonoguia.Background = Brushes.Red;
            }

        }


        private void emailFormatGuia(object sender, TextCompositionEventArgs e)
        {
            if (e.Handled == false)
            {
                Boolean valido;

                Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{1,4}$");

                valido = regex.IsMatch(tb_correoguia.Text);

                if (valido)
                {
                    img_tb_correoGuia.Source = imagCheck;
                    img_tb_correoGuia.Visibility = Visibility.Visible;
                    img_tb_correoGuia.ToolTip = "Formato adecuado";
                    tb_correoguia.Background = Brushes.Green;
                }
                else
                {
                    img_tb_correoGuia.Source = imagCross;
                    img_tb_correoGuia.Visibility = Visibility.Visible;
                    img_tb_correoGuia.ToolTip = "Debes introducir un email";
                    tb_correoguia.Background = Brushes.Red;
                }
            }
            else
            {
                img_tb_correoGuia.Source = imagCross;
                img_tb_correoGuia.Visibility = Visibility.Visible;
                img_tb_correoGuia.ToolTip = "Debes introducir un formato válido";
                tb_correoguia.Background = Brushes.Red;
            }
        }
        private void click_editar_excursionista(object sender, RoutedEventArgs e)
        {
            cambiaModoCasillasExcursionista(false);
            bt_guardarEx.IsEnabled = true;

            tb_nombre_excursionista.IsEnabled = false;
            

        }

        private int buscaExcursionista(String nombreExcursionista)
        {
            int posicion = -1;
            int contador = 0;
            foreach(Excursionista excursionistaAux in listadoExcursionistas)
            {
                if (excursionistaAux.Name == nombreExcursionista)
                {
                    posicion = contador;
                }
                contador++;
            }
            return posicion;

        }


        private void click_eliminar_excursionista(object sender, RoutedEventArgs e)
        {
            int posicionExcursionista = buscaExcursionista(tb_nombre_excursionista.Text);
            listadoExcursionistas.Remove(listadoExcursionistas[posicionExcursionista]);
            imprimirNombreExcursionistas();
            inicializaComponenentesExcursionistas();

        }














        ///////////////////////////////////////////////////////////////////////////
        /// ----------------------------  PESTAÑA GUIAS
        ///////////////////////////////////////////////////////////////////////////


        private void imprimirNombreGuias()
        {
            ListBoxGuias.Items.Clear();
            foreach (Guia guia in listadoGuias)
            {
                ListBoxGuias.Items.Add(guia.Name);
            }
        }

        private List<Guia> CargarContenidoGuiasXML()
        {
            List<Guia> listado = new List<Guia>();

            // Cargar contenido de prueba
            XmlDocument doc = new XmlDocument();
            var fichero = Application.GetResourceStream(new Uri("Datos/guias.xml", UriKind.Relative));
            doc.Load(fichero.Stream);
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                Guia guiaAux = null;
                var rutaAux = new Ruta("", "", "", "", DateTime.Today, "", 0, "", 0, guiaAux);
                var nuevoGuia = new Guia("", "", "", "", "", DateTime.Today, "", "", "", listadoRutas, listadoRutas);
                nuevoGuia.User = node.Attributes["User"].Value;
                nuevoGuia.Pass = node.Attributes["Pass"].Value;
                nuevoGuia.Name = node.Attributes["Name"].Value;
                nuevoGuia.LastName = node.Attributes["LastName"].Value;
                nuevoGuia.Email = node.Attributes["Email"].Value;
                nuevoGuia.LastLogin = Convert.ToDateTime(node.Attributes["LastLogin"].Value);
                nuevoGuia.Telefono = node.Attributes["Phone"].Value;
                nuevoGuia.ImgUrl = new Uri(node.Attributes["ImgUrl"].Value);
                nuevoGuia.Idiomas = node.Attributes["Idiomas"].Value;
                nuevoGuia.Disponibilidad = node.Attributes["Disponibilidad"].Value;

                string[] rutasRealizadas = node.Attributes["RutasRealizadas"].Value.Split(',');
                List<Ruta> listadoRutasRealizadas = new List<Ruta>();
                foreach (string nombreRutaRealizadas in rutasRealizadas)
                {
                    for (int i = 0; i < listadoRutas.Count; i++)
                    {
                        if (listadoRutas[i].Nombre == nombreRutaRealizadas)
                        {
                            listadoRutasRealizadas.Add(listadoRutas[i]);
                        }
                    }
                }
                nuevoGuia.RutasRealizadas = listadoRutasRealizadas;

                string[] RutasFuturas = node.Attributes["RutasFuturas"].Value.Split(',');
                List<Ruta> listadoRutasFuturas = new List<Ruta>();
                foreach (string nombreRutaFuturas in RutasFuturas)
                {
                    for (int i = 0; i < listadoRutas.Count; i++)
                    {
                        if (listadoRutas[i].Nombre == nombreRutaFuturas)
                        {
                            listadoRutasFuturas.Add(listadoRutas[i]);
                        }
                    }
                }
                nuevoGuia.RutasFuturas = listadoRutasFuturas;

                listado.Add(nuevoGuia);
            }

            return listado;
        }

        void inicializaComponenentesGuias()
        {

            cb_rutasGuias.IsEnabled = false;

            tb_nombreguia.Text = "";
            tb_idiomas.Text = "";
            tb_correoguia.Text = "";
            tb_telefonoguia.Text = "";
            tb_disponibilidad.Text = "";
            img_guia.Source = new BitmapImage();

            bt_anadirGuia.IsEnabled = true;
            bt_editarGuia.IsEnabled = false;
            bt_eliminarGuia.IsEnabled = false;
            lb_rutasrealplaGuias.Items.Clear();

            bt_guardarGuia.IsEnabled = false;
            img_bt_guardarGuia.Visibility = Visibility.Hidden;
            cambiaModoCasillasGuia(true);

            bt_asignarGuia.IsEnabled = false;

            img_bt_asignarGuia.Visibility = Visibility.Hidden;

            tb_telefonoguia.Background = Brushes.Transparent;
            tb_correoguia.Background = Brushes.Transparent;
            img_tb_telefonoGuia.Visibility = Visibility.Hidden;
            img_tb_correoGuia.Visibility = Visibility.Hidden;


        }

        private void cambiaModoCasillasGuia(bool soloLectura)
        {
            tb_nombreguia.IsReadOnly = soloLectura;
            tb_idiomas.IsReadOnly = soloLectura;
            tb_correoguia.IsReadOnly = soloLectura;
            tb_telefonoguia.IsReadOnly = soloLectura;
            tb_disponibilidad.IsReadOnly = soloLectura;

            if (!soloLectura && tb_nombreguia.Text == String.Empty)
            {
                tb_nombreguia.Text = "Escriba aqui el nombre del nuevo guía";
            }
        }

        private void rellenaCasillasGuias(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxGuias.SelectedItem != null)
            {
                lb_rutasrealplaGuias.Items.Clear();
                cb_rutasGuias.IsEnabled = true;

                int index = ListBoxGuias.SelectedIndex;
                var guiaAux = listadoGuias[index];

                tb_nombreguia.Text = guiaAux.Name;
                tb_idiomas.Text = guiaAux.Idiomas;
                tb_telefonoguia.Text = guiaAux.Telefono;
                tb_correoguia.Text = guiaAux.Email;
                tb_disponibilidad.Text = guiaAux.Disponibilidad;

                if (guiaAux.ImgUrl != null)
                {
                    var fullFilePath = guiaAux.ImgUrl.ToString();
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
                    bitmap.EndInit();
                    img_guia.Source = bitmap;
                }
                bt_anadirGuia.IsEnabled = false;
                bt_editarGuia.IsEnabled = true;
                bt_eliminarGuia.IsEnabled = true;

                img_bt_guardarGuia.Visibility = Visibility.Hidden;
                cambiaModoCasillasGuia(true);

                img_bt_asignarGuia.Visibility = Visibility.Hidden;

                if (bt_verGuiaRuta.Content == "Seleccionar guia")
                {
                    bt_asignarGuia.IsEnabled = true;
                }

                tb_nombreguia.IsEnabled = true;
            }
            else
            {
                inicializaComponenentesGuias();
            }

        }

        private void lstGuias_SelectionChanged(object sender, MouseButtonEventArgs controlUnderMouse)
        {
            if (controlUnderMouse.GetType() != typeof(ListBoxItem))
            {
                ListBoxGuias.SelectedItem = null;
            }
        }

        private void ComboBoxRutasGuias_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_rutasGuias.SelectedValue != null) { 
                string selectedItem = cb_rutasGuias.SelectedValue.ToString();

                if (selectedItem.Equals("System.Windows.Controls.ComboBoxItem: Rutas Realizadas"))
                {
                    imprimeRutasRealizarGuia();
                }
                else if (selectedItem.Equals("System.Windows.Controls.ComboBoxItem: Rutas Planificadas"))
                {
                    imprimeRutasFuturasGuia();
                }
            }
        }

        private void imprimeRutasRealizarGuia()
        {
            lb_rutasrealplaGuias.Items.Clear();
            int index = ListBoxGuias.SelectedIndex;
            Guia guiaAux = null;
            if (index != -1)
            {
                guiaAux = listadoGuias[index];
            }
            else
            {
                for (int i = 0; i < listadoGuias.Count; i++)
                {
                    if (listadoGuias[i].Name == tb_nombreguia.Text)
                    {
                        guiaAux = listadoGuias[i];
                    }
                }
            }

            foreach (Ruta ruta in guiaAux.RutasRealizadas)
            {
                lb_rutasrealplaGuias.Items.Add(ruta.Nombre + " (" + ruta.Fecha + ")");
            }
        }

        private void imprimeRutasFuturasGuia()
        {
            lb_rutasrealplaGuias.Items.Clear();
            int index = ListBoxGuias.SelectedIndex;
            Guia guiaAux = null;
            if (index != -1)
            {
                guiaAux = listadoGuias[index];
            }
            else
            {
                for (int i = 0; i < listadoGuias.Count; i++)
                {
                    if (listadoGuias[i].Name == tb_nombreguia.Text)
                    {
                        guiaAux = listadoGuias[i];
                    }
                }
            }

            foreach (Ruta ruta in guiaAux.RutasFuturas)
            {
                lb_rutasrealplaGuias.Items.Add(ruta.Nombre + " (" + ruta.Fecha + ")");
            }
        }

        private void click_añadir_Guia(object sender, RoutedEventArgs e)
        {
            inicializaComponenentesGuias();
            bt_guardarGuia.IsEnabled = true;
            cambiaModoCasillasGuia(false);

            tb_nombreguia.IsEnabled = true;

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = IMAGEN_USUARIO_DEFAULT;
            bitmap.EndInit();
            img_guia.Source = bitmap;
        }

        private void compruebaValidezCasillasGuia(object sender, TextCompositionEventArgs e)
        {
          

            if (tb_nombreguia.Text == "" || tb_idiomas.Text == "" || tb_telefonoguia.Text == "" || tb_disponibilidad.Text == "" || tb_correoguia.Text == "") 
            {
                img_bt_guardarGuia.Visibility = Visibility.Visible;
                img_bt_guardarGuia.Source = imagCross;
                img_bt_guardarGuia.ToolTip = "Debe rellenar todos los campos de esta pestaña para guardar el nuevo guía";
                
            }
            else
            {
                img_bt_guardarGuia.Visibility = Visibility.Visible;
                img_bt_guardarGuia.Source = imagCheck;
                img_bt_guardarGuia.ToolTip = "Campos de esta pestaña rellenados correctamente";

            }

        }

        private bool compruebaValidezCasillasGuia()
        {
            bool valido = true;

            if (tb_nombreguia.Text == "" || tb_idiomas.Text == "" || tb_telefonoguia.Text == "" || tb_disponibilidad.Text == "" || tb_correoguia.Text == "")
            {
                img_bt_guardarGuia.Visibility = Visibility.Visible;
                img_bt_guardarGuia.Source = imagCross;
                img_bt_guardarGuia.ToolTip = "Debe rellenar todos los campos de esta pestaña para guardar el nuevo guía";
                valido = false;
            }
            else
            {
                img_bt_guardarGuia.Visibility = Visibility.Visible;
                img_bt_guardarGuia.Source = imagCheck;
                img_bt_guardarGuia.ToolTip = "Campos de esta pestaña rellenados correctamente";

            }


            return valido;
        }

        private void clickGuardarGuia(object sender, RoutedEventArgs e)
        {
            //if (string.IsNullOrEmpty(Textbox1.Text))

            List<Ruta> listaRutasAux = new List<Ruta>();
            Guia guiaAux = new Guia("", "", "", "", "", DateTime.Today, "", "", "", listaRutasAux, listaRutasAux);


            guiaAux.Name = tb_nombreguia.Text;
            guiaAux.Idiomas = tb_idiomas.Text;
            guiaAux.Telefono = tb_telefonoguia.Text;
            guiaAux.Email = tb_correoguia.Text;
            guiaAux.Disponibilidad = tb_disponibilidad.Text;
            

            if (compruebaValidezCasillasGuia())
            {
                int posicionGuia = buscaGuia(guiaAux.Name);

                if (posicionGuia == -1)
                {
                    guiaAux.ImgUrl = IMAGEN_USUARIO_DEFAULT;
                    listadoGuias.Add(guiaAux);
                    imprimirNombreGuias();
                    img_bt_guia.Source = imagCheck;
                    img_guia.ToolTip = "Se ha asignado un guía a la nueva ruta correctamente";
                }
                else
                {
                    guiaAux.ImgUrl = listadoGuias[posicionGuia].ImgUrl;
                    listadoGuias[posicionGuia] = guiaAux;
                }

                
                inicializaComponenentesGuias();
            }


        }

        private void click_editar_guia(object sender, RoutedEventArgs e)
        {
            cambiaModoCasillasGuia(false);
            bt_guardarGuia.IsEnabled = true;

            tb_nombreguia.IsEnabled = false;


        }

        private int buscaGuia(String nombreGuia)
        {
            int posicion = -1;
            int contador = 0;
            foreach (Guia guiaAux in listadoGuias)
            {
                if (guiaAux.Name == nombreGuia)
                {
                    posicion = contador;
                }
                contador++;
            }
            return posicion;

        }

        private void click_eliminar_guia(object sender, RoutedEventArgs e)
        {
            int posicionGuia = buscaGuia(tb_nombreguia.Text);
            listadoGuias.Remove(listadoGuias[posicionGuia]);
            imprimirNombreGuias();
            inicializaComponenentesGuias();

        }

        private void click_asignar_guia(object sender, RoutedEventArgs e)
        {
            bt_asignarGuia.IsEnabled = false;

            img_bt_asignarGuia.Visibility = Visibility.Visible;
            img_bt_asignarGuia.Source = imagCheck;
            img_bt_asignarGuia.ToolTip = "Guía asignado correctamente";

            img_bt_guia.Source = imagCheck;
            img_bt_guia.Visibility = Visibility.Visible;
            img_bt_guia.ToolTip = "La ruta ya cuenta con un guía asignado";

            int posicionGuia = buscaGuia(tb_nombreguia.Text);
            rutaNula.Guia = listadoGuias[posicionGuia];

            tcPestanas.SelectedIndex = 0;
            img_bt_asignarGuia.Visibility = Visibility.Hidden;

            rutaSinGuia = false;

        }









        ///////////////////////////////////////////////////////////////////////////
        /// ----------------------------  PESTAÑA OFERTAS
        ///////////////////////////////////////////////////////////////////////////


        private void imprimirNombreOfertas()
        {
            ListBoxOfertas.Items.Clear();
            foreach (Oferta oferta in listadoOfertas)
            {
                ListBoxOfertas.Items.Add(oferta.Id);
            }
        }

        private List<Oferta> CargarContenidoOfertasXML()
        {
            List<Oferta> listado = new List<Oferta>();

            // Cargar contenido de prueba
            XmlDocument doc = new XmlDocument();
            var fichero = Application.GetResourceStream(new Uri("Datos/ofertas.xml", UriKind.Relative));
            doc.Load(fichero.Stream);
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                Guia guiaAux = null;
                var rutaAux = new Ruta("", "", "", "", DateTime.Today, "", 0, "", 0, guiaAux);
                var nuevaOferta = new Oferta(0, rutaAux, "");
                nuevaOferta.Id = int.Parse(node.Attributes["Id"].Value);
                nuevaOferta.Descripcion = node.Attributes["Descripcion"].Value;
                nuevaOferta.IMG_OFERTA = new Uri(node.Attributes["IMG_OFERTA"].Value);
                for (int i = 0; i < listadoRutas.Count; i++)
                {
                    if (listadoRutas[i].Nombre == node.Attributes["Ruta"].Value)
                    {
                        nuevaOferta.Ruta = listadoRutas[i];
                    }
                }
                listado.Add(nuevaOferta);
            }
            return listado;
        }

        void inicializaComponenentesOfertas()
        {
            tb_nombre_oferta.Text = "";
            cb_rutaOferta.SelectedIndex = -1;
            tb_descripcionoferta.Text = "";
            img_oferta.Source = new BitmapImage();

            bt_anadirOfeta.IsEnabled = true;
            bt_editarOferta.IsEnabled = false;
            bt_eliminarOferta.IsEnabled = false;


            bt_guardarOferta.IsEnabled = false;
            cb_rutaOferta.IsEnabled = false;

            cb_rutaOferta.Items.Clear();
            foreach (Ruta rutaAux in listadoRutas)
            {
                cb_rutaOferta.Items.Add(rutaAux.Nombre);
            }

            cambiaModoCasillasOferta(true);
            img_bt_guardarOferta.Visibility = Visibility.Hidden;
            img_bt_nombreOferta.Visibility = Visibility.Hidden;

            lb_exc.Items.Clear();
            lb_ofertas.Items.Clear();

            lb_envio_correcto.Visibility = Visibility.Hidden;
            bt_enviarOferta.IsEnabled = false;

            lb_exc.IsEnabled = false;
        }

        private void cambiaModoCasillasOferta(bool soloLectura)
        {
            
            tb_nombre_oferta.IsReadOnly = soloLectura;
            tb_descripcionoferta.IsReadOnly = soloLectura;
            cb_rutaOferta.IsEnabled = !soloLectura;

            if (!soloLectura && tb_nombre_oferta.Text == String.Empty)
            {
                tb_nombre_oferta.Text = "Escriba aqui el nombre de la nueva oferta";
            }
        }

        private void rellenaCasillasOferta(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxOfertas.SelectedItem != null)
            {
               
                int index = ListBoxOfertas.SelectedIndex;
                var ofertaAux = listadoOfertas[index];

                tb_nombre_oferta.Text = ofertaAux.Id.ToString();
                int contadorRutas = 0;
                foreach (Ruta rutaAux in listadoRutas)
                {
                    if (rutaAux.Nombre == ofertaAux.Ruta.Nombre)
                    {
                        cb_rutaOferta.SelectedIndex = contadorRutas;
                    }
                    contadorRutas++;
                }
                
                tb_descripcionoferta.Text = ofertaAux.Descripcion;

                if (ofertaAux.IMG_OFERTA != null)
                {
                    var fullFilePath = ofertaAux.IMG_OFERTA.ToString();
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
                    bitmap.EndInit();
                    img_oferta.Source = bitmap;
                }

                bt_anadirOfeta.IsEnabled = false;
                bt_editarOferta.IsEnabled = true;
                bt_eliminarOferta.IsEnabled = true;

                cb_rutaOferta.IsEnabled = false;
                cambiaModoCasillasOferta(true);
                img_bt_guardarOferta.Visibility = Visibility.Hidden;
                img_bt_nombreOferta.Visibility = Visibility.Hidden;

                imprimeExcursionistasNotificaciones();

                tb_nombre_oferta.IsEnabled = true;

                lb_exc.IsEnabled = true;
            }
            else
            {
                inicializaComponenentesOfertas();
            }

        }

        private void lstOfertas_SelectionChanged(object sender, MouseButtonEventArgs controlUnderMouse)
        {
            if (controlUnderMouse.GetType() != typeof(ListBoxItem))
            {
                ListBoxOfertas.SelectedItem = null;
            }
        }

        private void click_añadir_Oferta(object sender, RoutedEventArgs e)
        {
            inicializaComponenentesOfertas();
            bt_guardarOferta.IsEnabled = true;
            cb_rutaOferta.IsEnabled = true;
            cambiaModoCasillasOferta(false);
            bt_enviarOferta.IsEnabled = false;
            lb_exc.Items.Clear();
            lb_ofertas.Items.Clear();

            tb_nombre_oferta.IsEnabled = true;

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = IMAGEN_OFERTA_DEFAULT;
            bitmap.EndInit();
            img_oferta.Source = bitmap;
        }

         private void clickGuardarOferta(object sender, RoutedEventArgs e)
        {


            Ruta rutaAux = null;
            Oferta ofertaAux = new Oferta(0, rutaAux, "");

            if (compruebaValidezCasillasOferta())
            {
                ofertaAux.Id = int.Parse(tb_nombre_oferta.Text);
                ofertaAux.Descripcion = tb_descripcionoferta.Text;

                String nombreRuta = cb_rutaOferta.SelectedItem.ToString();

                foreach (Ruta rutaAuxx in listadoRutas)
                {
                    if (rutaAuxx.Nombre == nombreRuta)
                    {
                        ofertaAux.Ruta = rutaAuxx;
                    }
                }

                int posicionOferta = buscaOferta(ofertaAux.Id);
                if (posicionOferta == -1)
                {
                    ofertaAux.IMG_OFERTA = IMAGEN_OFERTA_DEFAULT;
                    listadoOfertas.Add(ofertaAux);
                    imprimirNombreOfertas();
                }
                else
                {
                    ofertaAux.IMG_OFERTA = listadoOfertas[posicionOferta].IMG_OFERTA;
                    listadoOfertas[posicionOferta] = ofertaAux;              
                }

                inicializaComponenentesOfertas();

            }
        }

        

        private void compruebaValidezCasillasOferta(object sender, TextCompositionEventArgs e)
        {
            
            if (tb_descripcionoferta.Text == "" || tb_nombre_oferta.Text == "" || cb_rutaOferta.SelectedIndex == -1 )
            {
                img_bt_guardarOferta.Visibility = Visibility.Visible;
                img_bt_guardarOferta.Source = imagCross;
                img_bt_guardarOferta.ToolTip = "Debe rellenar todos los campos de esta pestaña para guardar la nueva oferta";

            }
            else
            {
                img_bt_guardarOferta.Visibility = Visibility.Visible;
                img_bt_guardarOferta.Source = imagCheck;
                img_bt_guardarOferta.ToolTip = "Campos de esta pestaña rellenados correctamente";

            }

        }

        private bool compruebaValidezCasillasOferta()
        {
            bool valido = true;

            if (tb_descripcionoferta.Text == "" || tb_nombre_oferta.Text == "" || cb_rutaOferta.SelectedIndex == -1)
            {
                img_bt_guardarOferta.Visibility = Visibility.Visible;
                img_bt_guardarOferta.Source = imagCross;
                img_bt_guardarOferta.ToolTip = "Debe rellenar todos los campos de esta pestaña para guardar la nueva oferta";
                valido = false;
            }
            else
            {
                img_bt_guardarOferta.Visibility = Visibility.Visible;
                img_bt_guardarOferta.Source = imagCheck;
                img_bt_guardarOferta.ToolTip = "Campos de esta pestaña rellenados correctamente";

            }


            return valido;
        }

        private int buscaOferta(int idOferta)
        {
            int posicion = -1;
            int contador = 0;
            foreach (Oferta ofertaAux in listadoOfertas)
            {
                if (ofertaAux.Id == idOferta)
                {
                    posicion = contador;
                }
                contador++;
            }
            return posicion;

        }

        private void click_editar_oferta(object sender, RoutedEventArgs e)
        {
            bt_enviarOferta.IsEnabled = false;
            lb_exc.Items.Clear();
            lb_ofertas.Items.Clear();

            cambiaModoCasillasOferta(false);
            cb_rutaOferta.IsEnabled = true;
            bt_guardarOferta.IsEnabled = true;

            tb_nombre_oferta.IsEnabled = false;
        }


        private void click_eliminar_oferta(object sender, RoutedEventArgs e)
        {
            int posicionOferta = buscaOferta(int.Parse(tb_nombre_oferta.Text));
            listadoOfertas.Remove(listadoOfertas[posicionOferta]);
            imprimirNombreOfertas();
            inicializaComponenentesOfertas();
        }

        private void NumericOnlyIdOferta(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = IsTextNumeric(e.Text);
            if (e.Handled == false)
            {
                img_bt_nombreOferta.Source = imagCheck;
                img_bt_nombreOferta.Visibility = Visibility.Visible;
                img_bt_nombreOferta.ToolTip = "Formato adecuado";
            }
            else
            {
                img_bt_nombreOferta.Source = imagCross;
                img_bt_nombreOferta.Visibility = Visibility.Visible;
                img_bt_nombreOferta.ToolTip = "Debes introducir un formato numérico";
            }

        }

        private void click_añadir_destinatario_Oferta(object sender, RoutedEventArgs e)
        {
            if (lb_exc.SelectedItem != null)
            {
                lb_ofertas.Items.Add(lb_exc.SelectedItem);
                lb_exc.Items.Remove(lb_exc.SelectedItem);
                bt_enviarOferta.IsEnabled = true;
            }
        }

        private void lstEx_SelectionChanged(object sender, MouseButtonEventArgs controlUnderMouse)
        {
            if (controlUnderMouse.GetType() != typeof(ListBoxItem))
            {
                lb_exc.SelectedItem = null;
            }
        }

        private void click_enviar_Oferta(object sender, RoutedEventArgs e)
        {
            imprimeExcursionistasNotificaciones();
            lb_envio_correcto.Visibility = Visibility.Visible;
            bt_enviarOferta.IsEnabled = false;
        }


        private void imprimeExcursionistasNotificaciones()
        {
            lb_exc.Items.Clear();
            lb_ofertas.Items.Clear();
            foreach (Excursionista excursionistaAux in listadoExcursionistas)
            {
                if (excursionistaAux.Notificaciones)
                {
                    lb_exc.Items.Add(excursionistaAux.Name);
                }
            }
        }

        private void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            lb_envio_correcto.Visibility = Visibility.Hidden;
        }









        ///////////////////////////////////////////////////////////////////////////
        /// ----------------------------  PESTAÑA PDIs
        ///////////////////////////////////////////////////////////////////////////



        private List<PuntoInteres> CargarContenidoPuntosInteresXML()
        {

            List<PuntoInteres> listado = new List<PuntoInteres>();
            // Cargar contenido de prueba
            XmlDocument doc = new XmlDocument();
            var fichero = Application.GetResourceStream(new Uri("Datos/puntosInteres.xml", UriKind.Relative));
            doc.Load(fichero.Stream);
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                //Ruta(string nombre, string origen, string destino, string provincia, DateTime fecha, string dificultad, int plazasDisponibles, string material, int numRealizaciones)
                Guia guiaAux = null;
                var rutaAux = new Ruta("", "", "", "", DateTime.Today, "", 0, "", 0, guiaAux);

                var nuevoPuntoInteres = new PuntoInteres("", "", rutaAux);
                nuevoPuntoInteres.Nombre = node.Attributes["Nombre"].Value;
                nuevoPuntoInteres.Descripcion = node.Attributes["Descripcion"].Value;
                for (int i = 0; i < listadoRutas.Count; i++)
                {
                    if (listadoRutas[i].Nombre == node.Attributes["NombreRuta"].Value)
                    {
                        nuevoPuntoInteres.Ruta = listadoRutas[i];
                    }
                }
                nuevoPuntoInteres.URL_IMAGEN = new Uri(node.Attributes["Imagen"].Value, UriKind.Absolute);

                listado.Add(nuevoPuntoInteres);

            }
            return listado;
        }

        private void imprimirNombrePuntosInteres(string nombreRuta)
        {
            ListBoxPDI.Items.Clear();

            foreach (PuntoInteres puntoInteres in listadoPuntosInteres)
            {
                if (puntoInteres.Ruta.Nombre == nombreRuta) {
                    ListBoxPDI.Items.Add(puntoInteres.Nombre);
                }
                
            }
        }

        private void imprimirTodosNombrePuntosInteres()
        {
            ListBoxPDI.Items.Clear();

            foreach (PuntoInteres puntoInteres in listadoPuntosInteres)
            {
                ListBoxPDI.Items.Add(puntoInteres.Nombre);
            }
        }

        private void inicializaComponenentesPuntosInteres()
        {
            tb_nombre_pdi.Text = "";
            tb_descripcionpdi.Text = "";
            img_pdi.Source = new BitmapImage();

            bt_anadirPdi.IsEnabled = true;
            bt_editarPdi.IsEnabled = false;
            bt_eliminarPdi.IsEnabled = false;

            bt_guardarPDI.IsEnabled = false;

            cb_ruta_PDI.SelectedIndex = -1;
            cb_ruta_PDI.IsEnabled = false;

            cb_ruta_PDI.Items.Clear();
            foreach (Ruta rutaAux in listadoRutas)
            {
                cb_ruta_PDI.Items.Add(rutaAux.Nombre);
            }

            cambiaModoCasillasPDIs(true);
            img_bt_guardarPDI.Visibility = Visibility.Hidden;
            img_bt_guardarPDI.Visibility = Visibility.Hidden;

            bt_mostrarRuta.IsEnabled = false;

        }

        private void cambiaModoCasillasPDIs(bool soloLectura)
        {
            
            tb_nombre_pdi.IsReadOnly = soloLectura;
            tb_descripcionpdi.IsReadOnly = soloLectura;
            cb_ruta_PDI.IsEnabled = !soloLectura;

            if (!soloLectura && tb_nombre_pdi.Text == String.Empty)
            {
                tb_nombre_pdi.Text = "Escriba aqui el nombre del nuevo punto";
            }
        }

        private void rellenaCasillasPuntoInteres(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxPDI.SelectedItem != null)
            {
            
                foreach (PuntoInteres puntoInteres in listadoPuntosInteres)
                {
                    if (puntoInteres.Nombre == ListBoxPDI.SelectedItem.ToString())
                    {
                        tb_nombre_pdi.Text = puntoInteres.Nombre;

                        tb_descripcionpdi.Text = puntoInteres.Descripcion;

                        int contadorRutas = 0;
                        bool encontrado = false;
                        foreach (Ruta rutaAux in listadoRutas)
                        {

                            if (rutaAux.Nombre == tb_nombre.Text)
                            {
                                cb_ruta_PDI.SelectedIndex = contadorRutas;
                                encontrado = true;
                            }
                            contadorRutas++;
                        }
                        if (!encontrado)
                        {
                            cb_ruta_PDI.Items.Add(tb_nombre.Text);
                            cb_ruta_PDI.SelectedIndex = contadorRutas;
                        }

                        if (puntoInteres.URL_IMAGEN != null)
                        {
                            var fullFilePath = puntoInteres.URL_IMAGEN.ToString();
                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
                            bitmap.EndInit();
                            img_pdi.Source = bitmap;
                        }
                    }
                }
                bt_anadirPdi.IsEnabled = false;
                bt_editarPdi.IsEnabled = true;
                bt_eliminarPdi.IsEnabled = true;

                cambiaModoCasillasPDIs(true);
                img_bt_guardarPDI.Visibility = Visibility.Hidden;
                img_bt_guardarPDI.Visibility = Visibility.Hidden;

                bt_mostrarRuta.IsEnabled = true;

                tb_nombre_pdi.IsEnabled = true;

            }
            else
            {
                inicializaComponenentesPuntosInteres();
            }
        }

        private void lstPuntosInteres_SelectionChanged(object sender, MouseButtonEventArgs controlUnderMouse)
        {
            if (controlUnderMouse.GetType() != typeof(ListBoxItem))
            {
                ListBoxPDI.SelectedItem = null;
            }
        }

        private void click_añadir_PDI(object sender, RoutedEventArgs e)
        {
            inicializaComponenentesPuntosInteres();
            bt_guardarPDI.IsEnabled = true;
            cambiaModoCasillasPDIs(false);

            int contadorRutas = 0;
            bool encontrado = false;

            foreach (Ruta rutaAux in listadoRutas)
            {
                
                if (rutaAux.Nombre == tb_nombre.Text)
                {
                    cb_ruta_PDI.SelectedIndex = contadorRutas;
                    encontrado = true;
                }
                 contadorRutas++;  
            }
            if (!encontrado)
            {
                cb_ruta_PDI.Items.Add(tb_nombre.Text);
                cb_ruta_PDI.SelectedIndex = contadorRutas;
            }
            cb_ruta_PDI.IsEnabled = false;

            tb_nombre_pdi.IsEnabled = true;

            
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = IMAGEN_PDI_DEFAULT;
            bitmap.EndInit();
            img_pdi.Source = bitmap;
        }

        private void clickGuardarPDI(object sender, RoutedEventArgs e)
        {
           
            if (compruebaValidezCasillasPDI())
            {
                Guia guiaAux = null;
                Ruta rutaAux = new Ruta(cb_ruta_PDI.SelectedItem.ToString(), "", "", "", DateTime.Today, "", 0, "", 0, guiaAux);

                PuntoInteres puntoInteresAux = new PuntoInteres("", "", rutaAux);


                puntoInteresAux.Nombre = tb_nombre_pdi.Text;
                puntoInteresAux.Descripcion = tb_descripcionpdi.Text;

                foreach (Ruta rutaAuxx in listadoRutas)
                {
                    if (rutaAuxx.Nombre == cb_ruta_PDI.SelectedItem.ToString())
                    {
                        puntoInteresAux.Ruta = rutaAuxx;
                    }
                }

                int posicionPDI = buscaPDI(puntoInteresAux.Nombre);
                
                if (posicionPDI == -1)
                {
                    puntoInteresAux.URL_IMAGEN = IMAGEN_PDI_DEFAULT;
                    listadoPuntosInteres.Add(puntoInteresAux);
                    imprimirTodosNombrePuntosInteres();
                }
                else
                {
                    puntoInteresAux.URL_IMAGEN = listadoPuntosInteres[posicionPDI].URL_IMAGEN;
                    listadoPuntosInteres[posicionPDI] = puntoInteresAux;
                }
                
                inicializaComponenentesPuntosInteres();
                imprimirNombrePuntosInteres(tb_nombre.Text);

                if(bt_consultarPDis.Content == "Añadir PDI")
                {
                    img_bt_pdis.Source = imagCheck;
                    img_bt_pdis.Visibility = Visibility.Visible;
                    img_bt_pdis.ToolTip = "La ruta cuenta con al menos 1 punto de interés";
                }

               
            }
        }


        private bool compruebaValidezCasillasPDI()
        {
            bool valido = true;

            if (tb_descripcionpdi.Text == "" || tb_nombre_pdi.Text == "" || cb_ruta_PDI.SelectedIndex == -1)
            {
                img_bt_guardarPDI.Visibility = Visibility.Visible;
                img_bt_guardarPDI.Source = imagCross;
                img_bt_guardarPDI.ToolTip = "Debe rellenar todos los campos de esta pestaña para guardar el nuevo punto de interés";
                valido = false;
            }
            else
            {
                img_bt_guardarPDI.Visibility = Visibility.Visible;
                img_bt_guardarPDI.Source = imagCheck;
                img_bt_guardarPDI.ToolTip = "Campos de esta pestaña rellenados correctamente";

            }


            return valido;
        }

        private int buscaPDI(String nombrePDI)
        {
            int posicion = -1;
            int contador = 0;
            foreach (PuntoInteres puntoInteresAux in listadoPuntosInteres)
            {
                if (puntoInteresAux.Nombre == nombrePDI)
                {
                    posicion = contador;
                    
                }
                contador++;
            }
            return posicion;

        }

        private void click_editar_PDI(object sender, RoutedEventArgs e)
        {
            bt_guardarPDI.IsEnabled = true;
            cambiaModoCasillasPDIs(false);
            tb_nombre_pdi.IsEnabled = false;
            cb_ruta_PDI.IsEnabled = true;
        }


        private void click_eliminar_PDI(object sender, RoutedEventArgs e)
        {
            int posicionPDI = buscaPDI(tb_nombre_pdi.Text);
            listadoPuntosInteres.Remove(listadoPuntosInteres[posicionPDI]);
            imprimirNombrePuntosInteres(cb_ruta_PDI.SelectedItem.ToString());
            inicializaComponenentesPuntosInteres();
        }

        private void click_verRuta_PDI(object sender, RoutedEventArgs e)
        {
            tcPestanas.SelectedIndex = 0;
        }













        //---------------  CONTROL DE ENTRADAS  -------------------------------


        private void NumericOnly(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = IsTextNumeric(e.Text);
            if (e.Handled == false)
            {
                
                
                img_tb_plazas.Source = imagCheck;
                img_tb_plazas.Visibility = Visibility.Visible;
                img_tb_plazas.ToolTip = "Formato adecuado";
                

            }
            else
            {
                tb_plazas.Text = String.Empty;
                img_tb_plazas.Source=imagCross;
                img_tb_plazas.Visibility = Visibility.Visible;
                img_tb_plazas.ToolTip="Debes introducir un formato numerico";
            }

        }

        private static bool IsTextNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("[^0-9]");
            return reg.IsMatch(str);
        }
        private void tb_origen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                tb_destino.Focus();

            }
        }

        private void tb_destino_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                tb_provincia.Focus();
            }
        }

        private void tb_provincia_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                dp_fecha.Focus();
            } 
        }

        private void dp_fecha_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                tb_dificultad.Focus();
            }
        }

        private void tb_dificultad_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                tb_plazas.Focus();
            }
        }

        private void bt_editar_Click(object sender, RoutedEventArgs e)
        {
            edicionTextBox(true);
            tb_nombre.Focus();
        }

        private void bt_anadir_Click(object sender, RoutedEventArgs e)
        {
            edicionTextBox(false);
            tb_nombre.Focus();
        }
        private void bt_consultarguia_Click(object sender, RoutedEventArgs e)
        {
            tcPestanas.SelectedIndex = 0;
        }


        private void click_checkbox_apuntarse_ruta(object sender, RoutedEventArgs e)
        {
            
            if (chb_apuntarseRuta.IsChecked == true && ListBoxRutas.SelectedIndex != -1)
            {
               
                int posicionRuta = buscaRuta(tb_nombre.Text);

                int posicionUsuario = buscaExcursionista(lbNombreUsuario.Content.ToString());
                if (posicionUsuario != -1)
                {
                    listadoExcursionistas[posicionUsuario].RutasFuturas.Add(listadoRutas[posicionRuta]);
                }
                else
                {
                    posicionUsuario = buscaGuia(lbNombreUsuario.Content.ToString());
                    listadoGuias[posicionUsuario].RutasFuturas.Add(listadoRutas[posicionRuta]);
                }
            }
            else if (chb_apuntarseRuta.IsChecked == false && ListBoxRutas.SelectedIndex != -1)
            {
                
                int posicionRuta = buscaRuta(tb_nombre.Text);
                int contador = 0;
                int posicionBorrar = -1;

                int posicionUsuario = buscaExcursionista(lbNombreUsuario.Content.ToString());
                if (posicionUsuario != -1)
                {
                    foreach(Ruta rutaAux in listadoExcursionistas[posicionUsuario].RutasFuturas)
                    {
                        if(rutaAux.Nombre == tb_nombre.Text)
                        {
                            posicionBorrar = contador;
                            
                        }
                        contador += 1;
                    }
                    listadoExcursionistas[posicionUsuario].RutasFuturas.Remove(listadoExcursionistas[posicionUsuario].RutasFuturas[posicionBorrar]);

                }
                else
                {
                    posicionUsuario = buscaGuia(lbNombreUsuario.Content.ToString());
                    foreach (Ruta rutaAux in listadoGuias[posicionUsuario].RutasFuturas)
                    {
                        if (rutaAux.Nombre == tb_nombre.Text)
                        {
                            posicionBorrar = contador;
                        }
                        contador += 1;
                    }

                    listadoGuias[posicionUsuario].RutasFuturas.Remove(listadoGuias[posicionUsuario].RutasFuturas[posicionBorrar]);
                }

                
            }

            cb_elegirExcursionista.SelectedIndex = -1;
            list_excursionistas.Items.Clear();

            lb_rutasrealplaEx.Items.Clear();
            cb_rutasEx.SelectedIndex = -1;

            cb_rutasGuias.SelectedIndex = -1;
            lb_rutasrealplaGuias.Items.Clear();


        }

        private void inicializa_todas_pestañas()
        {
            inicializaComponentesRutas();
            inicializaComponenentesExcursionistas();
            inicializaComponenentesGuias();
            inicializaComponenentesOfertas();
            inicializaComponenentesPuntosInteres();
        }


    }
}
    

