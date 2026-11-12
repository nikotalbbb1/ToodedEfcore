using System.Runtime.InteropServices;

namespace Tooded_EFCore
{
    /// <summary>
    /// Lihtne ja puhas teema. Kolm varianti: Hele, Öö, Mets. Kasutaja valib teema sisselogimisel või menüüs,
    /// valik salvestatakse. Teema.Rakenda(vorm) stiilib vormi elemendid ühtse välimusega.
    /// </summary>
    public static class Teema
    {
        public enum Palett { Hele = 0, Oo = 1, Mets = 2 }
        public static readonly string[] Nimed = { "Hele", "Öö", "Mets" };

        private static Color C(int r, int g, int b) => Color.FromArgb(r, g, b);

        // järjekord: Taust, Paneel, Paneel2, Aksent, AksentHover, Tsuaan, Tume (tekst), Hall, HallTekst, Joon,
        //            Pais, TabelPais, TabelHover
        private static readonly Color[][] _paletid =
        {
            new[] { C(243, 244, 246), C(255, 255, 255), C(248, 250, 252), C(37, 99, 235), C(29, 78, 216), C(13, 148, 136),
                    C(17, 24, 39), C(209, 213, 219), C(107, 114, 128), C(226, 232, 240), C(37, 99, 235), C(51, 65, 85), C(219, 234, 254) },   // Hele
            new[] { C(17, 24, 39), C(31, 41, 55), C(39, 49, 66), C(99, 102, 241), C(79, 70, 229), C(125, 211, 252),
                    C(243, 244, 246), C(55, 65, 81), C(156, 163, 175), C(55, 65, 81), C(49, 46, 129), C(51, 65, 85), C(49, 57, 80) },          // Öö
            new[] { C(240, 247, 243), C(255, 255, 255), C(247, 252, 249), C(22, 163, 74), C(21, 128, 61), C(21, 128, 61),
                    C(20, 38, 28), C(203, 219, 208), C(95, 115, 102), C(209, 226, 215), C(21, 101, 52), C(22, 101, 52), C(220, 252, 231) }    // Mets
        };

        public static readonly Color Punane = C(220, 38, 38);
        public static readonly Color Roheline = C(22, 163, 74);

        public static Color Taust { get; private set; }
        public static Color Paneel { get; private set; }
        public static Color Paneel2 { get; private set; }
        public static Color Aksent { get; private set; }
        public static Color AksentHover { get; private set; }
        public static Color Tsuaan { get; private set; }
        public static Color Tume { get; private set; }
        public static Color Hall { get; private set; }
        public static Color HallTekst { get; private set; }
        public static Color Joon { get; private set; }
        public static Color Pais { get; private set; }
        public static Color TabelPais { get; private set; }
        public static Color TabelHover { get; private set; }
        public static Palett Praegune { get; private set; }

        /// <summary>Seatakse true, kui teema vahetati menüüs: Program avab menüü uue teemaga uuesti.</summary>
        public static bool TeemaVahetatud { get; set; }

        static Teema()
        {
            Vali(LaeSalvestatud(), false);
        }

        private static string FailiTee() => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ToodedApp", "teema.txt");

        private static Palett LaeSalvestatud()
        {
            try
            {
                if (Enum.TryParse(File.ReadAllText(FailiTee()).Trim(), out Palett p) && Enum.IsDefined(typeof(Palett), p)) return p;
            }
            catch { /* faili pole: kasutame vaikimisi teemat */ }
            return Palett.Hele;
        }

        /// <summary>Valib teema. Uued aknad kasutavad seda kohe; valik salvestatakse järgmiseks käivituseks.</summary>
        public static void Vali(Palett p, bool salvesta = true)
        {
            var v = _paletid[(int)p];
            Praegune = p;
            Taust = v[0]; Paneel = v[1]; Paneel2 = v[2]; Aksent = v[3]; AksentHover = v[4]; Tsuaan = v[5];
            Tume = v[6]; Hall = v[7]; HallTekst = v[8]; Joon = v[9]; Pais = v[10]; TabelPais = v[11]; TabelHover = v[12];
            if (!salvesta) return;
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FailiTee())!);
                File.WriteAllText(FailiTee(), p.ToString());
            }
            catch { /* salvestamine ei ole kriitiline */ }
        }

        /// <summary>Silt "TEEMA" ja kolm nuppu. vahetatud() kutsutakse pärast valiku muutmist.</summary>
        public static Control[] TeemaValija(int x, int y, int chipLaius, int vahe, int korgus, Action vahetatud)
        {
            var tulemus = new List<Control>
            {
                new Label
                {
                    Text = "TEEMA", Left = x, Top = y - 22, AutoSize = true, ForeColor = HallTekst,
                    BackColor = Color.Transparent, Font = new Font("Segoe UI", 8.5F, FontStyle.Bold)
                }
            };
            for (int i = 0; i < 3; i++)
            {
                var palett = (Palett)i;
                var chip = Nupp((Praegune == palett ? "● " : "") + Nimed[i], _paletid[i][3],
                                x + i * (chipLaius + vahe), y, chipLaius, korgus, 10F);
                chip.Raadius = 8;
                chip.Click += (s, e) =>
                {
                    if (Praegune == palett) return;
                    Vali(palett);
                    vahetatud();
                };
                tulemus.Add(chip);
            }
            return tulemus.ToArray();
        }

        // ------------------------------------------------------------------

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);

        /// <summary>Tume aknapealkiri tumedas teemas (Windows 10/11); vanemas Windowsis ei tee midagi.</summary>
        public static void TumeTiitel(Form vorm)
        {
            void Pane()
            {
                try { int v = Praegune == Palett.Oo ? 1 : 0; DwmSetWindowAttribute(vorm.Handle, 20, ref v, sizeof(int)); } catch { }
            }
            if (vorm.IsHandleCreated) Pane(); else vorm.HandleCreated += (s, e) => Pane();
        }

        public static void Rakenda(Form vorm)
        {
            TumeTiitel(vorm);
            vorm.BackColor = Taust;
            vorm.ForeColor = Tume;
            Haara(vorm);
        }

        /// <summary>Ümar värviline nupp (menüü, sisselogimine, pood).</summary>
        public static RoundButton Nupp(string tekst, Color vari, int x, int y, int laius, int korgus,
                                       float fondiSuurus = 11F, string alam = "")
        {
            return new RoundButton
            {
                Text = tekst, Alamtekst = alam,
                Left = x, Top = y, Width = laius, Height = korgus,
                BackColor = vari,
                Font = new Font("Segoe UI", fondiSuurus, FontStyle.Bold)
            };
        }

        /// <summary>Värv hõljumisel: heledas teemas veidi tumedam, tumedas heledam.</summary>
        public static Color Hover(Color c) =>
            Praegune == Palett.Oo ? ControlPaint.Light(c, 0.2f) : ControlPaint.Dark(c, 0.1f);

        /// <summary>Väike teavitus: libiseb alt sisse, püsib hetke ja kaob. Asendab "OK"-aknad.</summary>
        public static void Teavita(Form vorm, string tekst)
        {
            if (vorm.IsDisposed) return;
            var fond = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            var mootmed = TextRenderer.MeasureText(tekst, fond);
            var kaart = new KaartPanel { Width = mootmed.Width + 44, Height = 46, Fill = Paneel, Piirjoon = Aksent, Raadius = 10 };
            kaart.Controls.Add(new Label
            {
                Text = tekst, Font = fond, ForeColor = Tume, BackColor = Color.Transparent,
                Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter
            });
            int alla = vorm.ClientSize.Height + 10;
            int siht = vorm.ClientSize.Height - 70;
            kaart.Left = (vorm.ClientSize.Width - kaart.Width) / 2;
            kaart.Top = alla;
            vorm.Controls.Add(kaart);
            kaart.BringToFront();

            int ms = 0;
            var kell = new System.Windows.Forms.Timer { Interval = 15 };
            kell.Tick += (s, e) =>
            {
                ms += 15;
                if (vorm.IsDisposed) { kell.Stop(); kell.Dispose(); return; }
                if (ms < 250) kaart.Top = (int)(alla - (alla - siht) * (ms / 250f));
                else if (ms < 1800) kaart.Top = siht;
                else if (ms < 2050) kaart.Top = (int)(siht + (alla - siht) * ((ms - 1800) / 250f));
                else { kell.Stop(); kell.Dispose(); vorm.Controls.Remove(kaart); kaart.Dispose(); }
            };
            kell.Start();
        }

        // ------------------------------------------------------------------

        private static void Haara(Control c)
        {
            Stiilita(c);
            c.ControlAdded += (s, e) => { if (e.Control != null) Haara(e.Control); };
            foreach (Control laps in c.Controls) Haara(laps);
        }

        private static bool OnHele(Color v) => v.A > 0 && v.GetBrightness() > 0.6f;

        private static void Stiilita(Control c)
        {
            switch (c)
            {
                case RoundButton: case GradPanel: case KaartPanel: case Pealkiri: case Form:
                    return;
                case Button b when b.FlatStyle != FlatStyle.Flat:
                    StiilitaNupp(b); return;
                case Button b2:                       // juba lamedad nupud: ilma äärteta
                    b2.FlatAppearance.BorderSize = 0; return;
                case DataGridView g:
                    StiilitaTabel(g); return;
                case TextBox t:
                    t.BorderStyle = BorderStyle.FixedSingle; t.BackColor = Paneel2; t.ForeColor = Tume; return;
                case ComboBox cb:
                    cb.FlatStyle = FlatStyle.Flat; cb.BackColor = Paneel2; cb.ForeColor = Tume; return;
                case NumericUpDown n:
                    n.BackColor = Paneel2; n.ForeColor = Tume; n.BorderStyle = BorderStyle.FixedSingle; return;
                case ListBox lb:
                    lb.BorderStyle = BorderStyle.FixedSingle; lb.BackColor = Paneel2; lb.ForeColor = Tume; return;
                case DateTimePicker dt:
                    dt.CalendarMonthBackground = Paneel2; dt.CalendarForeColor = Tume; return;
                case GroupBox gb:
                    gb.ForeColor = Tume; gb.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold); return;
                case CheckBox or RadioButton:
                    if (c.ForeColor == SystemColors.ControlText) c.ForeColor = Tume; return;
                case Label l:
                    if (l.ForeColor == SystemColors.ControlText) l.ForeColor = Tume; return;
                case TabPage tp:
                    tp.UseVisualStyleBackColor = false; tp.BackColor = Taust; return;
                case Panel p:
                    if (p.BackColor == SystemColors.Control) p.BackColor = Taust;
                    else if (Praegune == Palett.Oo && OnHele(p.BackColor)) p.BackColor = Paneel;
                    return;
            }
        }

        private static Color NupuVarv(Button b)
        {
            string t = b.Text ?? "";
            if (t.Contains("Kustuta") || t.Contains("Eemalda") || t.Contains("Tühjenda") || t.Contains("Logi välja"))
                return Punane;
            return Aksent;
        }

        private static void StiilitaNupp(Button b)
        {
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.Cursor = Cursors.Hand;
            b.UseVisualStyleBackColor = false;
            Varskenda(b);
            b.SizeChanged += (s, e) => Ymarda(b);
            Ymarda(b);
            b.EnabledChanged += (s, e) => Varskenda(b);
            b.TextChanged += (s, e) => Varskenda(b);
            b.MouseEnter += (s, e) => { if (b.Enabled) b.BackColor = Hover(NupuVarv(b)); };
            b.MouseLeave += (s, e) => Varskenda(b);
        }

        private static void Ymarda(Button b)
        {
            if (b.Width < 4 || b.Height < 4) return;
            using var tee = Kujundid.Umar(new Rectangle(0, 0, b.Width, b.Height), 6);
            b.Region = new Region(tee);
        }

        private static void Varskenda(Button b)
        {
            b.BackColor = b.Enabled ? NupuVarv(b) : Hall;
            b.ForeColor = b.Enabled ? Color.White : HallTekst;
        }

        private static void StiilitaTabel(DataGridView g)
        {
            g.EnableHeadersVisualStyles = false;
            g.BorderStyle = BorderStyle.None;
            g.BackgroundColor = Paneel;
            g.GridColor = Joon;
            g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.ColumnHeadersHeight = 34;
            g.ColumnHeadersDefaultCellStyle.BackColor = TabelPais;
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            g.ColumnHeadersDefaultCellStyle.SelectionBackColor = TabelPais;
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            g.DefaultCellStyle.BackColor = Paneel;
            g.DefaultCellStyle.ForeColor = Tume;
            g.DefaultCellStyle.SelectionBackColor = Aksent;
            g.DefaultCellStyle.SelectionForeColor = Color.White;
            g.AlternatingRowsDefaultCellStyle.BackColor = Paneel2;
            g.RowHeadersVisible = false;
            g.RowTemplate.Height = 28;

            // rea esiletõstmine hiirega
            Color hoverVarv = TabelHover;
            int hr = -1;
            Color vana = Color.Empty;
            g.CellMouseEnter += (s, e) =>
            {
                if (e.RowIndex < 0 || e.RowIndex >= g.Rows.Count) return;
                if (hr >= 0 && hr < g.Rows.Count && g.Rows[hr].DefaultCellStyle.BackColor == hoverVarv)
                    g.Rows[hr].DefaultCellStyle.BackColor = vana;
                hr = e.RowIndex;
                vana = g.Rows[hr].DefaultCellStyle.BackColor;
                g.Rows[hr].DefaultCellStyle.BackColor = hoverVarv;
            };
            g.MouseLeave += (s, e) =>
            {
                if (hr >= 0 && hr < g.Rows.Count && g.Rows[hr].DefaultCellStyle.BackColor == hoverVarv)
                    g.Rows[hr].DefaultCellStyle.BackColor = vana;
                hr = -1;
            };
        }
    }
}
