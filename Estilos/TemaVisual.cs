using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Hotel_Zormat.Estilos
{
    // Tema visual central de la aplicación.
    //
    // Toda la identidad gráfica (colores, tipografías, iconos y helpers de
    // dibujo) vive aquí para que los nueve formularios compartan un mismo
    // lenguaje visual y no se repita código de estilo en cada uno.
    //
    // La paleta no es inventada: los acentos se obtuvieron muestreando los
    // píxeles del logo "Hotel Bisono" (Resources\Logo_Hotel_Bisono.png) y
    // ordenándolos por frecuencia, de modo que la interfaz reproduce los
    // mismos azules, turquesas y dorados del atardecer del logotipo.
    internal static class TemaVisual
    {
        // ------------------------------------------------------------------
        // Paleta
        // ------------------------------------------------------------------
        internal static class Colores
        {
            // Azules — tomados del script "Bisono" y de las olas del logo.
            internal static readonly Color AzulProfundo = Color.FromArgb(0, 47, 85);
            internal static readonly Color AzulMarino = Color.FromArgb(0, 66, 111);
            internal static readonly Color AzulPrimario = Color.FromArgb(0, 96, 144);
            internal static readonly Color AzulBrillante = Color.FromArgb(0, 96, 168);
            internal static readonly Color AzulHover = Color.FromArgb(0, 78, 120);

            // Turquesas — palmeras y cresta de las olas.
            internal static readonly Color TurquesaProfundo = Color.FromArgb(0, 120, 168);
            internal static readonly Color TurquesaPalmera = Color.FromArgb(0, 168, 192);
            internal static readonly Color TurquesaClaro = Color.FromArgb(24, 192, 216);

            // Atardecer y arena — sol poniente y playa del logo.
            internal static readonly Color SolNaranja = Color.FromArgb(255, 168, 96);
            internal static readonly Color SolDurazno = Color.FromArgb(255, 192, 96);
            internal static readonly Color SolAmarillo = Color.FromArgb(255, 240, 168);
            internal static readonly Color ArenaDorada = Color.FromArgb(216, 168, 96);
            internal static readonly Color DoradoTexto = Color.FromArgb(240, 192, 96);

            // Neutros de interfaz.
            internal static readonly Color Blanco = Color.White;
            internal static readonly Color FondoClaro = Color.FromArgb(245, 248, 251);
            internal static readonly Color FondoSecundario = Color.FromArgb(233, 239, 245);
            internal static readonly Color BordeSutil = Color.FromArgb(215, 225, 234);
            internal static readonly Color Texto = Color.FromArgb(34, 64, 90);
            internal static readonly Color TextoSuave = Color.FromArgb(107, 130, 153);

            // Colores semánticos de estado. Se conservan los ya usados por la
            // aplicación para no obligar al personal a reaprender el código
            // de colores del tablero de habitaciones.
            internal static readonly Color Rojo = Color.FromArgb(214, 72, 63);
            internal static readonly Color Verde = Color.FromArgb(46, 158, 82);
            internal static readonly Color Naranja = Color.FromArgb(224, 138, 46);
            internal static readonly Color AzulGris = Color.FromArgb(61, 111, 216);

            internal static readonly Color RojoSuave = Color.FromArgb(247, 222, 220);
            internal static readonly Color VerdeSuave = Color.FromArgb(221, 243, 227);
            internal static readonly Color NaranjaSuave = Color.FromArgb(252, 235, 216);
            internal static readonly Color AzulGrisSuave = Color.FromArgb(223, 231, 250);

            internal static readonly Color Gris = Color.FromArgb(99, 121, 142);

            // Devuelve el color fuerte que corresponde a un estado de habitación.
            internal static Color PorEstado(string estado)
            {
                if (estado == null)
                {
                    return AzulGris;
                }

                switch (estado.Trim().ToUpperInvariant())
                {
                    case "DISPONIBLE":
                        return Verde;
                    case "OCUPADA":
                        return Rojo;
                    case "RESERVADA":
                        return Naranja;
                    default:
                        return AzulGris;
                }
            }

            // Versión clara del color de estado, para pintar filas de tablas.
            internal static Color PorEstadoSuave(string estado)
            {
                if (estado == null)
                {
                    return AzulGrisSuave;
                }

                switch (estado.Trim().ToUpperInvariant())
                {
                    case "DISPONIBLE":
                        return VerdeSuave;
                    case "OCUPADA":
                        return RojoSuave;
                    case "RESERVADA":
                        return NaranjaSuave;
                    default:
                        return AzulGrisSuave;
                }
            }
        }

        // ------------------------------------------------------------------
        // Tipografías
        // ------------------------------------------------------------------
        internal static class Fuentes
        {
            internal static readonly Font TituloHero = new Font("Georgia", 24F, FontStyle.Bold);
            internal static readonly Font TituloForm = new Font("Georgia", 15F, FontStyle.Bold);
            internal static readonly Font TituloSeccion = new Font("Georgia", 12F, FontStyle.Bold);
            internal static readonly Font Cuerpo = new Font("Trebuchet MS", 9.5F);
            internal static readonly Font CuerpoNegrita = new Font("Trebuchet MS", 9.5F, FontStyle.Bold);
            internal static readonly Font Etiqueta = new Font("Trebuchet MS", 9F, FontStyle.Bold);
            internal static readonly Font Pequena = new Font("Trebuchet MS", 8.25F);
            internal static readonly Font Monto = new Font("Consolas", 12F);
            internal static readonly Font MontoTotal = new Font("Consolas", 17F, FontStyle.Bold);

            // Toque decorativo del logo. Monotype Corsiva viene con Windows en
            // la mayoría de instalaciones, pero si faltara, WinForms sustituye
            // la fuente en silencio; por eso se verifica el nombre resuelto y
            // se cae a Georgia cursiva de forma explícita.
            internal static readonly Font Script = CrearConRespaldo(
                "Monotype Corsiva",
                "Georgia",
                17F,
                FontStyle.Italic);

            // Fuente de iconos. El tamaño varía según el contexto, por eso es
            // un método y no una constante.
            internal static Font Icono(float tamano)
            {
                return new Font("Segoe MDL2 Assets", tamano);
            }

            private static Font CrearConRespaldo(
                string preferida,
                string respaldo,
                float tamano,
                FontStyle estilo)
            {
                Font fuente = new Font(preferida, tamano, estilo);
                if (string.Equals(fuente.Name, preferida, StringComparison.OrdinalIgnoreCase))
                {
                    return fuente;
                }

                fuente.Dispose();
                return new Font(respaldo, tamano, estilo);
            }
        }

        // ------------------------------------------------------------------
        // Iconos (Segoe MDL2 Assets)
        //
        // Cada punto de código fue verificado renderizando la fuente y
        // comprobando visualmente la forma resultante; un código equivocado
        // no produce error, sólo dibuja un cuadro vacío.
        // ------------------------------------------------------------------
        internal static class Glifos
        {
            internal const string Habitacion = "";    // edificio
            internal const string Huesped = "";       // persona
            internal const string Huespedes = "";     // grupo de personas
            internal const string Reserva = "";       // calendario
            internal const string CheckIn = "";       // flecha entrando
            internal const string CheckOut = "";      // flecha saliendo
            internal const string Factura = "";       // documento
            internal const string Reportes = "";      // gráfico
            internal const string Bitacora = "";      // reloj de historial
            internal const string CerrarSesion = "";  // botón de encendido
            internal const string Buscar = "";        // lupa
            internal const string Guardar = "";       // disquete
            internal const string Eliminar = "";      // papelera
            internal const string Nuevo = "";         // signo de más
            internal const string Cancelar = "";      // equis
            internal const string Actualizar = "";    // flecha circular
            internal const string Confirmar = "";     // visto bueno
            internal const string VerClave = "";      // ojo abierto
            internal const string OcultarClave = "";  // ojo cerrado
            internal const string Alerta = "";        // triángulo de aviso
            internal const string Filtro = "";        // embudo
            internal const string Administrador = ""; // escudo
        }

        // ------------------------------------------------------------------
        // Logo
        //
        // El PNG mide 1024x1024 y pesa ~1.4 MB, así que se carga una sola vez
        // y se reparte a todos los formularios. Los recortes provienen de
        // medir el contenido real del archivo: el dibujo ocupa X 197..788 e
        // Y 216..687, con una franja vacía en Y 489..493 que separa el
        // emblema (palmeras, sol y olas) del texto "Hotel Bisono".
        // ------------------------------------------------------------------
        private static readonly Rectangle RecorteEmblema = new Rectangle(197, 216, 592, 273);
        private static readonly Rectangle RecorteCompleto = new Rectangle(197, 216, 592, 472);

        private static Bitmap _logoCompleto;
        private static Bitmap _emblema;
        private static Icon _iconoVentana;
        private static bool _iconoCalculado;

        // Logo completo (emblema + texto), recortado al contenido real.
        internal static Bitmap ObtenerLogo()
        {
            if (_logoCompleto == null)
            {
                _logoCompleto = Recortar(RecorteCompleto);
            }

            return _logoCompleto;
        }

        // Sólo el emblema tropical, sin el texto. Se lee mejor en tamaños
        // pequeños, donde el texto del logo sería una mancha ilegible.
        internal static Bitmap ObtenerEmblema()
        {
            if (_emblema == null)
            {
                _emblema = Recortar(RecorteEmblema);
            }

            return _emblema;
        }

        private static Bitmap Recortar(Rectangle area)
        {
            Bitmap original = Properties.Resources.Logo_Hotel_Bisono;
            if (original == null)
            {
                return null;
            }

            Rectangle segura = Rectangle.Intersect(
                area,
                new Rectangle(0, 0, original.Width, original.Height));

            if (segura.Width <= 0 || segura.Height <= 0)
            {
                return null;
            }

            return original.Clone(segura, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }

        // Asigna el icono de ventana a partir del emblema del logo.
        //
        // El icono se construye una única vez: Icon.FromHandle envuelve un
        // HICON nativo que WinForms no libera solo, y esta aplicación abre
        // sus formularios repetidamente en bucle, así que generarlo por cada
        // apertura iría acumulando handles GDI durante toda la sesión.
        internal static void AplicarIconoVentana(Form formulario)
        {
            if (formulario == null)
            {
                return;
            }

            if (!_iconoCalculado)
            {
                _iconoCalculado = true;
                _iconoVentana = ConstruirIcono();
            }

            if (_iconoVentana != null)
            {
                formulario.Icon = _iconoVentana;
            }
        }

        private static Icon ConstruirIcono()
        {
            try
            {
                Bitmap emblema = ObtenerEmblema();
                if (emblema == null)
                {
                    return null;
                }

                const int lado = 64;
                using (Bitmap lienzo = new Bitmap(lado, lado))
                {
                    using (Graphics g = Graphics.FromImage(lienzo))
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.Clear(Color.Transparent);

                        float escala = Math.Min(
                            (float)lado / emblema.Width,
                            (float)lado / emblema.Height);
                        int ancho = (int)(emblema.Width * escala);
                        int alto = (int)(emblema.Height * escala);

                        g.DrawImage(
                            emblema,
                            (lado - ancho) / 2,
                            (lado - alto) / 2,
                            ancho,
                            alto);
                    }

                    IntPtr manejador = lienzo.GetHicon();
                    using (Icon temporal = Icon.FromHandle(manejador))
                    {
                        // Se clona para que el icono sobreviva a la liberación
                        // del handle temporal.
                        return (Icon)temporal.Clone();
                    }
                }
            }
            catch (Exception)
            {
                // Un icono es un detalle cosmético: si algo falla, la
                // aplicación debe seguir abriendo con el icono por defecto.
                return null;
            }
        }

        // ------------------------------------------------------------------
        // Helpers de dibujo GDI+
        // ------------------------------------------------------------------

        // Ruta rectangular con las cuatro esquinas redondeadas.
        internal static GraphicsPath CrearRutaRedondeada(Rectangle area, int radio)
        {
            GraphicsPath ruta = new GraphicsPath();

            if (radio <= 0 || area.Width <= 0 || area.Height <= 0)
            {
                ruta.AddRectangle(area);
                return ruta;
            }

            int diametro = Math.Min(radio * 2, Math.Min(area.Width, area.Height));

            ruta.AddArc(area.X, area.Y, diametro, diametro, 180, 90);
            ruta.AddArc(area.Right - diametro, area.Y, diametro, diametro, 270, 90);
            ruta.AddArc(area.Right - diametro, area.Bottom - diametro, diametro, diametro, 0, 90);
            ruta.AddArc(area.X, area.Bottom - diametro, diametro, diametro, 90, 90);
            ruta.CloseFigure();

            return ruta;
        }

        // Recorta el control con esquinas redondeadas. Se usa en contenedores
        // que tienen hijos: al recortar la región (en lugar de pintar encima)
        // los controles hijos con fondo transparente siguen heredando el color
        // correcto del contenedor.
        internal static void AplicarEsquinasRedondeadas(Control control, int radio)
        {
            if (control == null)
            {
                return;
            }

            ActualizarRegion(control, radio);
            control.Resize += delegate { ActualizarRegion(control, radio); };
        }

        private static void ActualizarRegion(Control control, int radio)
        {
            if (control.Width <= 0 || control.Height <= 0)
            {
                return;
            }

            Region anterior = control.Region;

            using (GraphicsPath ruta = CrearRutaRedondeada(
                new Rectangle(0, 0, control.Width, control.Height),
                radio))
            {
                control.Region = new Region(ruta);
            }

            if (anterior != null)
            {
                anterior.Dispose();
            }
        }

        // Convierte un panel en una tarjeta: fondo redondeado con borde fino.
        //
        // El color exterior debe ser el del contenedor padre para que las
        // esquinas recortadas se fundan con el fondo. Los controles hijos que
        // se coloquen encima deben llevar su BackColor puesto explícitamente
        // al color de relleno, porque en WinForms "transparente" significa
        // "hereda el BackColor del padre", no "muestra lo pintado en el padre".
        internal static void AplicarTarjeta(
            Panel panel,
            Color relleno,
            Color borde,
            Color exterior,
            int radio)
        {
            if (panel == null)
            {
                return;
            }

            panel.BackColor = exterior;
            panel.Paint += delegate (object remitente, PaintEventArgs e)
            {
                Control lienzo = (Control)remitente;
                if (lienzo.Width <= 1 || lienzo.Height <= 1)
                {
                    return;
                }

                Rectangle area = new Rectangle(0, 0, lienzo.Width - 1, lienzo.Height - 1);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using (GraphicsPath ruta = CrearRutaRedondeada(area, radio))
                using (SolidBrush brocha = new SolidBrush(relleno))
                {
                    e.Graphics.FillPath(brocha, ruta);

                    if (borde != Color.Transparent)
                    {
                        using (Pen lapiz = new Pen(borde))
                        {
                            e.Graphics.DrawPath(lapiz, ruta);
                        }
                    }
                }
            };

            panel.Resize += delegate { panel.Invalidate(); };
        }

        // Degradado del atardecer del logo: amarillo pálido, durazno y naranja.
        internal static void PintarAtardecer(Graphics g, Rectangle area)
        {
            if (g == null || area.Width <= 0 || area.Height <= 0)
            {
                return;
            }

            using (LinearGradientBrush brocha = new LinearGradientBrush(
                area,
                Colores.SolAmarillo,
                Colores.SolNaranja,
                LinearGradientMode.Horizontal))
            {
                ColorBlend mezcla = new ColorBlend(3);
                mezcla.Colors = new Color[]
                {
                    Colores.SolAmarillo,
                    Colores.SolDurazno,
                    Colores.SolNaranja
                };
                mezcla.Positions = new float[] { 0f, 0.5f, 1f };
                brocha.InterpolationColors = mezcla;

                g.FillRectangle(brocha, area);
            }
        }

        // Degradado azul profundo, usado en encabezados y barra lateral.
        internal static void PintarAzulProfundo(
            Graphics g,
            Rectangle area,
            LinearGradientMode direccion)
        {
            if (g == null || area.Width <= 0 || area.Height <= 0)
            {
                return;
            }

            using (LinearGradientBrush brocha = new LinearGradientBrush(
                area,
                Colores.AzulProfundo,
                Colores.AzulPrimario,
                direccion))
            {
                g.FillRectangle(brocha, area);
            }
        }

        // Olas del logo, dibujadas con curvas Bézier superpuestas.
        // Se usan como divisor decorativo bajo los encabezados.
        internal static void PintarOlas(Graphics g, Rectangle area)
        {
            if (g == null || area.Width <= 2 || area.Height <= 2)
            {
                return;
            }

            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color[] tonos = new Color[]
            {
                Colores.TurquesaClaro,
                Colores.TurquesaPalmera,
                Colores.AzulBrillante
            };

            float ancho = area.Width;

            for (int capa = 0; capa < tonos.Length; capa++)
            {
                float baseY = area.Top + area.Height * (0.34f + capa * 0.22f);
                float amplitud = area.Height * (0.30f - capa * 0.06f);

                using (GraphicsPath ruta = new GraphicsPath())
                {
                    ruta.AddBezier(
                        area.Left, baseY,
                        area.Left + ancho * 0.25f, baseY - amplitud,
                        area.Left + ancho * 0.45f, baseY + amplitud,
                        area.Left + ancho * 0.65f, baseY);
                    ruta.AddBezier(
                        area.Left + ancho * 0.65f, baseY,
                        area.Left + ancho * 0.80f, baseY - amplitud * 0.8f,
                        area.Left + ancho * 0.92f, baseY + amplitud * 0.6f,
                        area.Right, baseY - amplitud * 0.25f);

                    using (Pen lapiz = new Pen(tonos[capa], 2.4f - capa * 0.5f))
                    {
                        lapiz.StartCap = LineCap.Round;
                        lapiz.EndCap = LineCap.Round;
                        g.DrawPath(lapiz, ruta);
                    }
                }
            }
        }

        // ------------------------------------------------------------------
        // Fábricas de controles reutilizables
        // ------------------------------------------------------------------

        // Encabezado estándar de formulario: franja azul con icono y título,
        // rematada por una fina línea con el degradado del atardecer.
        // Lo comparten ocho de los nueve formularios.
        internal static Panel CrearEncabezado(string titulo, string glifo)
        {
            Panel encabezado = new Panel();
            encabezado.Dock = DockStyle.Top;
            encabezado.Height = 62;
            encabezado.BackColor = Colores.AzulMarino;

            encabezado.Paint += delegate (object remitente, PaintEventArgs e)
            {
                Control lienzo = (Control)remitente;
                Rectangle area = lienzo.ClientRectangle;
                if (area.Width <= 0 || area.Height <= 0)
                {
                    return;
                }

                PintarAzulProfundo(e.Graphics, area, LinearGradientMode.Horizontal);

                Rectangle franja = new Rectangle(0, area.Height - 3, area.Width, 3);
                PintarAtardecer(e.Graphics, franja);
            };

            encabezado.Resize += delegate { encabezado.Invalidate(); };

            Label icono = new Label();
            icono.Text = glifo;
            icono.Font = Fuentes.Icono(19F);
            icono.ForeColor = Colores.DoradoTexto;
            icono.BackColor = Color.Transparent;
            icono.AutoSize = false;
            icono.Size = new Size(38, 38);
            icono.Location = new Point(22, 12);
            icono.TextAlign = ContentAlignment.MiddleCenter;

            Label texto = new Label();
            texto.Text = titulo;
            texto.Font = Fuentes.TituloForm;
            texto.ForeColor = Colores.Blanco;
            texto.BackColor = Color.Transparent;
            texto.AutoSize = false;
            texto.Size = new Size(680, 38);
            texto.Location = new Point(60, 12);
            texto.TextAlign = ContentAlignment.MiddleLeft;

            encabezado.Controls.Add(icono);
            encabezado.Controls.Add(texto);

            return encabezado;
        }

        // Envuelve una caja de texto (o un NumericUpDown) en un panel con
        // borde redondeado. WinForms no permite redondear estos controles
        // directamente, así que se les quita el borde nativo y se dibuja uno
        // propio en el panel contenedor.
        internal static Panel EnvolverCampo(Control campo, Color fondoExterior)
        {
            Panel marco = new Panel();
            marco.BackColor = fondoExterior;
            marco.Padding = new Padding(10, 6, 10, 6);

            TextBoxBase caja = campo as TextBoxBase;
            if (caja != null)
            {
                caja.BorderStyle = BorderStyle.None;
            }

            NumericUpDown numerico = campo as NumericUpDown;
            if (numerico != null)
            {
                numerico.BorderStyle = BorderStyle.None;
            }

            campo.BackColor = Colores.Blanco;
            campo.Font = Fuentes.Cuerpo;
            campo.ForeColor = Colores.Texto;
            campo.Dock = DockStyle.Fill;

            marco.Paint += delegate (object remitente, PaintEventArgs e)
            {
                Control lienzo = (Control)remitente;
                if (lienzo.Width <= 1 || lienzo.Height <= 1)
                {
                    return;
                }

                Rectangle area = new Rectangle(0, 0, lienzo.Width - 1, lienzo.Height - 1);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using (GraphicsPath ruta = CrearRutaRedondeada(area, 8))
                using (SolidBrush brocha = new SolidBrush(Colores.Blanco))
                using (Pen lapiz = new Pen(Colores.BordeSutil))
                {
                    e.Graphics.FillPath(brocha, ruta);
                    e.Graphics.DrawPath(lapiz, ruta);
                }
            };

            marco.Resize += delegate { marco.Invalidate(); };
            marco.Controls.Add(campo);

            return marco;
        }

        // Etiqueta de campo de formulario.
        internal static Label CrearEtiqueta(string texto)
        {
            Label etiqueta = new Label();
            etiqueta.Text = texto;
            etiqueta.Font = Fuentes.Etiqueta;
            etiqueta.ForeColor = Colores.Texto;
            etiqueta.BackColor = Color.Transparent;
            etiqueta.AutoSize = false;
            etiqueta.Dock = DockStyle.Fill;
            etiqueta.TextAlign = ContentAlignment.MiddleLeft;
            return etiqueta;
        }

        // Dibuja un glifo de iconos sobre un mapa de bits transparente.
        //
        // Un control de WinForms sólo admite una tipografía, así que no se
        // puede mezclar en un mismo Text un icono de Segoe MDL2 Assets con
        // texto en Georgia o Trebuchet: el icono saldría como un cuadro vacío
        // porque esas fuentes no cubren el Área de Uso Privado de Unicode.
        // Rasterizarlo permite asignarlo como Image y conservar cada
        // tipografía en su sitio.
        internal static Bitmap RenderizarGlifo(string glifo, float tamano, Color color)
        {
            int lado = (int)Math.Ceiling(tamano * 2.2f);
            Bitmap mapa = new Bitmap(lado, lado);

            using (Graphics g = Graphics.FromImage(mapa))
            using (Font fuente = Fuentes.Icono(tamano))
            using (SolidBrush brocha = new SolidBrush(color))
            using (StringFormat formato = new StringFormat())
            {
                g.Clear(Color.Transparent);
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                formato.Alignment = StringAlignment.Center;
                formato.LineAlignment = StringAlignment.Center;
                g.DrawString(
                    glifo,
                    fuente,
                    brocha,
                    new RectangleF(0, 0, lado, lado),
                    formato);
            }

            return mapa;
        }

        // Título de sección con su icono al lado.
        internal static Label CrearTituloSeccion(string texto, string glifo)
        {
            Label titulo = new Label();
            titulo.Text = texto;
            titulo.Font = Fuentes.TituloSeccion;
            titulo.ForeColor = Colores.AzulMarino;
            titulo.BackColor = Color.Transparent;
            titulo.AutoSize = false;
            titulo.Height = 32;
            titulo.TextAlign = ContentAlignment.MiddleLeft;

            if (!string.IsNullOrEmpty(glifo))
            {
                DibujarGlifoIzquierda(
                    titulo,
                    glifo,
                    13F,
                    Colores.TurquesaProfundo,
                    2);
            }

            return titulo;
        }

        // Distintivo compacto de color, usado para estados y avisos.
        internal static Label CrearDistintivo(string texto, Color fondo, Color frente)
        {
            Label distintivo = new Label();
            distintivo.Text = texto;
            distintivo.Font = Fuentes.CuerpoNegrita;
            distintivo.BackColor = fondo;
            distintivo.ForeColor = frente;
            distintivo.AutoSize = false;
            distintivo.Size = new Size(200, 28);
            distintivo.TextAlign = ContentAlignment.MiddleCenter;
            AplicarEsquinasRedondeadas(distintivo, 14);
            return distintivo;
        }

        // ------------------------------------------------------------------
        // Estilizado de controles existentes
        // ------------------------------------------------------------------

        // Botón redondeado con cambio de color al pasar el ratón.
        //
        // Sólo agrega manejadores de MouseEnter y MouseLeave: nunca toca el
        // evento Click, para no alterar la lógica ya cableada del formulario.
        internal static void EstilizarBoton(
            Button boton,
            Color fondo,
            Color frente,
            Color hover)
        {
            if (boton == null)
            {
                return;
            }

            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.FlatAppearance.MouseOverBackColor = hover;
            boton.FlatAppearance.MouseDownBackColor = hover;
            boton.BackColor = fondo;
            boton.ForeColor = frente;
            boton.Font = Fuentes.CuerpoNegrita;
            boton.UseVisualStyleBackColor = false;
            boton.Cursor = Cursors.Hand;

            if (boton.Height < 34)
            {
                boton.Height = 34;
            }

            RedondearBoton(boton);
            boton.Resize += delegate { RedondearBoton(boton); };

            boton.EnabledChanged += delegate
            {
                boton.BackColor = boton.Enabled ? fondo : Colores.FondoSecundario;
                boton.ForeColor = boton.Enabled ? frente : Colores.TextoSuave;
            };
        }

        private static void RedondearBoton(Button boton)
        {
            if (boton.Width <= 0 || boton.Height <= 0)
            {
                return;
            }

            Region anterior = boton.Region;
            int radio = Math.Min(boton.Height / 2, 18);

            using (GraphicsPath ruta = CrearRutaRedondeada(
                new Rectangle(0, 0, boton.Width, boton.Height),
                radio))
            {
                boton.Region = new Region(ruta);
            }

            if (anterior != null)
            {
                anterior.Dispose();
            }
        }

        // Botón con icono a la izquierda del texto. El icono va como imagen,
        // no concatenado al texto, para que cada uno use su propia fuente.
        internal static void PonerGlifo(Button boton, string glifo, string texto)
        {
            if (boton == null)
            {
                return;
            }

            Color colorActivo = boton.ForeColor;

            boton.Text = "  " + texto;
            boton.Image = RenderizarGlifo(glifo, 12F, colorActivo);
            boton.ImageAlign = ContentAlignment.MiddleCenter;
            boton.TextAlign = ContentAlignment.MiddleCenter;
            boton.TextImageRelation = TextImageRelation.ImageBeforeText;

            // El icono va rasterizado con un color fijo, así que hay que
            // volver a generarlo cuando el botón se habilita o deshabilita;
            // de lo contrario quedaría un icono blanco sobre un fondo gris.
            boton.EnabledChanged += delegate
            {
                Image anterior = boton.Image;
                boton.Image = RenderizarGlifo(
                    glifo,
                    12F,
                    boton.Enabled ? colorActivo : Colores.TextoSuave);

                if (anterior != null)
                {
                    anterior.Dispose();
                }
            };
        }

        // Etiqueta con icono a la izquierda, para avisos y estados.
        internal static void PonerGlifoEtiqueta(Label etiqueta, string glifo, Color color)
        {
            if (etiqueta == null)
            {
                return;
            }

            DibujarGlifoIzquierda(etiqueta, glifo, 11F, color, 10);
        }

        // Coloca un icono a la izquierda de una etiqueta.
        //
        // No se usa la propiedad Image porque Label no expone
        // TextImageRelation (es propia de ButtonBase) y su relleno desplaza
        // por igual al texto y a la imagen, con lo que acabarían superpuestos.
        // Dibujando el icono en el evento Paint y reservando el hueco con el
        // relleno, cada elemento queda en su sitio.
        private static void DibujarGlifoIzquierda(
            Label etiqueta,
            string glifo,
            float tamano,
            Color color,
            int margenIzquierdo)
        {
            Bitmap icono = RenderizarGlifo(glifo, tamano, color);
            etiqueta.Padding = new Padding(
                margenIzquierdo + icono.Width + 6,
                0,
                0,
                0);

            etiqueta.Paint += delegate (object remitente, PaintEventArgs e)
            {
                Control lienzo = (Control)remitente;
                int y = (lienzo.Height - icono.Height) / 2;
                e.Graphics.DrawImage(icono, margenIzquierdo, y);
            };
        }

        // Botón de la barra lateral de navegación: ancho completo, icono a la
        // izquierda y realce al pasar el ratón.
        internal static Button CrearBotonNavegacion(string texto, string glifo)
        {
            Button boton = new Button();
            boton.Text = "    " + texto;
            boton.Font = Fuentes.Cuerpo;
            boton.ForeColor = Colores.Blanco;
            boton.BackColor = Color.Transparent;
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.FlatAppearance.MouseOverBackColor = Colores.AzulPrimario;
            boton.FlatAppearance.MouseDownBackColor = Colores.TurquesaProfundo;
            boton.TextAlign = ContentAlignment.MiddleLeft;
            boton.ImageAlign = ContentAlignment.MiddleLeft;
            boton.TextImageRelation = TextImageRelation.ImageBeforeText;
            boton.Image = RenderizarGlifo(glifo, 13F, Colores.SolDurazno);
            boton.Padding = new Padding(16, 0, 0, 0);
            boton.Size = new Size(214, 44);
            boton.Margin = new Padding(0);
            boton.UseVisualStyleBackColor = false;
            boton.Cursor = Cursors.Hand;
            boton.TabStop = false;
            return boton;
        }

        // Estilo unificado de tabla. Reemplaza el método ConfigurarGrid que
        // estaba duplicado en varios formularios.
        internal static void EstilizarGrid(DataGridView grid)
        {
            if (grid == null)
            {
                return;
            }

            grid.BorderStyle = BorderStyle.None;
            grid.BackgroundColor = Colores.Blanco;
            grid.EnableHeadersVisualStyles = false;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Colores.FondoSecundario;
            grid.RowHeadersVisible = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 38;
            grid.RowTemplate.Height = 30;

            grid.ColumnHeadersDefaultCellStyle.BackColor = Colores.AzulMarino;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Colores.Blanco;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Colores.AzulMarino;
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Colores.Blanco;
            grid.ColumnHeadersDefaultCellStyle.Font = Fuentes.CuerpoNegrita;
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            grid.DefaultCellStyle.BackColor = Colores.Blanco;
            grid.DefaultCellStyle.ForeColor = Colores.Texto;
            grid.DefaultCellStyle.Font = Fuentes.Cuerpo;
            grid.DefaultCellStyle.SelectionBackColor = Colores.SolDurazno;
            grid.DefaultCellStyle.SelectionForeColor = Colores.AzulProfundo;
            grid.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);

            grid.AlternatingRowsDefaultCellStyle.BackColor = Colores.FondoClaro;
            grid.AlternatingRowsDefaultCellStyle.ForeColor = Colores.Texto;
            grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = Colores.SolDurazno;
            grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = Colores.AzulProfundo;
        }

        // Pestañas dibujadas por cuenta propia. WinForms no permite cambiar el
        // color de un TabControl sin asumir su pintado.
        internal static void EstilizarTabControl(TabControl pestanas)
        {
            if (pestanas == null)
            {
                return;
            }

            pestanas.DrawMode = TabDrawMode.OwnerDrawFixed;
            pestanas.SizeMode = TabSizeMode.Fixed;
            pestanas.ItemSize = new Size(170, 34);
            pestanas.Font = Fuentes.CuerpoNegrita;

            pestanas.DrawItem += delegate (object remitente, DrawItemEventArgs e)
            {
                TabControl control = (TabControl)remitente;
                if (e.Index < 0 || e.Index >= control.TabPages.Count)
                {
                    return;
                }

                TabPage pagina = control.TabPages[e.Index];
                bool activa = control.SelectedIndex == e.Index;

                Color fondo = activa ? Colores.AzulMarino : Colores.FondoSecundario;
                Color frente = activa ? Colores.Blanco : Colores.TextoSuave;

                e.Graphics.SmoothingMode = SmoothingMode.None;

                using (SolidBrush brocha = new SolidBrush(fondo))
                {
                    e.Graphics.FillRectangle(brocha, e.Bounds);
                }

                if (activa)
                {
                    Rectangle acento = new Rectangle(
                        e.Bounds.X,
                        e.Bounds.Bottom - 3,
                        e.Bounds.Width,
                        3);
                    PintarAtardecer(e.Graphics, acento);
                }

                using (StringFormat formato = new StringFormat())
                using (SolidBrush brocha = new SolidBrush(frente))
                {
                    formato.Alignment = StringAlignment.Center;
                    formato.LineAlignment = StringAlignment.Center;
                    e.Graphics.DrawString(
                        pagina.Text,
                        control.Font,
                        brocha,
                        e.Bounds,
                        formato);
                }
            };

            // Sin esto, la pestaña que pierde el foco puede quedarse dibujada
            // con el color de seleccionada hasta el siguiente repintado.
            pestanas.SelectedIndexChanged += delegate { pestanas.Invalidate(); };

            foreach (TabPage pagina in pestanas.TabPages)
            {
                pagina.BackColor = Colores.FondoClaro;
                pagina.Font = Fuentes.Cuerpo;
            }
        }

        // Lista desplegable. El borde y la flecha son nativos de Windows y no
        // se pueden redibujar sin asumir el pintado completo del control, algo
        // que aquí no compensa; se ajusta lo que sí es configurable.
        internal static void EstilizarCombo(ComboBox combo)
        {
            if (combo == null)
            {
                return;
            }

            combo.FlatStyle = FlatStyle.Flat;
            combo.Font = Fuentes.Cuerpo;
            combo.BackColor = Colores.Blanco;
            combo.ForeColor = Colores.Texto;
        }

        // Selector de fecha. DateTimePicker tampoco expone BorderStyle, así
        // que sólo se alinean tipografía y colores del calendario.
        internal static void EstilizarFecha(DateTimePicker fecha)
        {
            if (fecha == null)
            {
                return;
            }

            fecha.Font = Fuentes.Cuerpo;
            fecha.CalendarForeColor = Colores.Texto;
            fecha.CalendarMonthBackground = Colores.Blanco;
            fecha.CalendarTitleBackColor = Colores.AzulMarino;
            fecha.CalendarTitleForeColor = Colores.Blanco;
            fecha.CalendarTrailingForeColor = Colores.TextoSuave;
        }

        // Barra horizontal para filtros y acciones secundarias.
        internal static void EstilizarBarra(Control barra)
        {
            if (barra == null)
            {
                return;
            }

            barra.BackColor = Colores.FondoSecundario;
            barra.Font = Fuentes.Cuerpo;
        }

        // Ajustes comunes a todos los formularios de la aplicación.
        internal static void PrepararFormulario(Form formulario, string titulo)
        {
            if (formulario == null)
            {
                return;
            }

            formulario.Text = titulo;
            formulario.BackColor = Colores.FondoClaro;
            formulario.Font = Fuentes.Cuerpo;
            formulario.ForeColor = Colores.Texto;
            AplicarIconoVentana(formulario);
        }
    }
}
