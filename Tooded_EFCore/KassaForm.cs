#pragma warning disable CS8618
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore
{
    public class OstuRida
    {
        public int ToodeId { get; set; }
        public string Nimetus { get; set; } = "";
        public decimal Hind { get; set; }
        public int Kogus { get; set; }
        public decimal Summa => Hind * Kogus;
    }

    /// <summary>
    /// Kassa: visuaalne toodete ruudustik (pildid), ostukorv, kliendikaart/boonus ja PDF-tšekk.
    /// Ligipääs: Ostja ja Omanik.
    /// </summary>
    public class KassaForm : Form
    {
        private readonly BindingList<OstuRida> _korv = new();
        private Klient? _klient;
        private bool _valmis;

        private Panel pnlTop, pnlVasak, pnlParem, pnlFilter;
        private Label lblKasutaja, lblKlient, lblVahe, lblBoonusKasutatud, lblKokku, lblBoonusSaadud;
        private Button btnValja, btnPlus, btnMiinus, btnEemalda, btnTuhjenda, btnLeia, btnMaksa;
        private TextBox txtOtsing, txtKaart;
        private ComboBox comboKat;
        private FlowLayoutPanel flowToted;
        private DataGridView dgvOstukorv;
        private CheckBox chkBoonus;

        public KassaForm()
        { Load += (s_, e_) => Teema.Rakenda(this);
            EhitaUI();
            Load += KassaForm_Load;
            FormClosed += (s, e) => TuhjendaTiled();
        }

        // ------------------------------------------------------------------ UI

        private void EhitaUI()
        {
            Text = "Kassa";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1180, 720);
            MinimumSize = new Size(1000, 660);

            // ülariba
            pnlTop = new GradPanel { Dock = DockStyle.Top, Height = 56 };
            lblKasutaja = new Label
            {
                Dock = DockStyle.Fill, ForeColor = Color.White, BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold), Padding = new Padding(12, 0, 0, 0)
            };
            btnValja = new Button
            {
                Dock = DockStyle.Right, Width = 130, FlatStyle = FlatStyle.Flat, ForeColor = Color.White, BackColor = Teema.Punane, Cursor = Cursors.Hand,
                Text = Seanss.OnOmanik ? "Sulge" : "Logi välja"
            };
            btnValja.Click += btnValja_Click;
            btnValja.FlatAppearance.BorderSize = 0;
            pnlTop.Controls.Add(lblKasutaja);
            pnlTop.Controls.Add(btnValja);

            // vasak pool: filter + toodete ruudustik (6. ülesanne)
            pnlVasak = new Panel { Dock = DockStyle.Fill };
            flowToted = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill, AutoScroll = true, BackColor = Teema.Taust, Padding = new Padding(6)
            };
            pnlFilter = new Panel { Dock = DockStyle.Top, Height = 46 };
            txtOtsing = new TextBox { Left = 10, Top = 10, Width = 260, PlaceholderText = "Otsi toodet..." };
            comboKat = new ComboBox { Left = 280, Top = 10, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
            txtOtsing.TextChanged += (s, e) => { if (_valmis) LaeTooted(); };
            comboKat.SelectedIndexChanged += (s, e) => { if (_valmis) LaeTooted(); };
            pnlFilter.Controls.Add(txtOtsing);
            pnlFilter.Controls.Add(comboKat);
            pnlVasak.Controls.Add(flowToted);
            pnlVasak.Controls.Add(pnlFilter);

            // parem pool: ostukorv
            pnlParem = new Panel { Dock = DockStyle.Right, Width = 440, BackColor = Teema.Paneel };

            dgvOstukorv = new DataGridView
            {
                Left = 10, Top = 10, Width = 420, Height = 280,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                AutoGenerateColumns = false, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                ReadOnly = true, MultiSelect = false, RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Teema.Paneel
            };
            var paremale = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight };
            var hinnaStiil = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N2" };
            dgvOstukorv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(OstuRida.Nimetus), HeaderText = "Toode", FillWeight = 150 });
            dgvOstukorv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(OstuRida.Hind), HeaderText = "Hind", FillWeight = 60, DefaultCellStyle = hinnaStiil });
            dgvOstukorv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(OstuRida.Kogus), HeaderText = "Kogus", FillWeight = 50, DefaultCellStyle = paremale });
            dgvOstukorv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(OstuRida.Summa), HeaderText = "Summa", FillWeight = 70, DefaultCellStyle = hinnaStiil });
            dgvOstukorv.DataSource = _korv;

            btnPlus = new Button { Left = 10, Top = 298, Width = 80, Height = 32, Text = "+" };
            btnMiinus = new Button { Left = 96, Top = 298, Width = 80, Height = 32, Text = "−" };
            btnEemalda = new Button { Left = 182, Top = 298, Width = 110, Height = 32, Text = "Eemalda" };
            btnTuhjenda = new Button { Left = 298, Top = 298, Width = 132, Height = 32, Text = "Tühjenda korv" };
            btnPlus.Click += btnPlus_Click;
            btnMiinus.Click += btnMiinus_Click;
            btnEemalda.Click += (s, e) => { if (ValitudRida() is { } r) { _korv.Remove(r); UuendaSummad(); } };
            btnTuhjenda.Click += (s, e) => { _korv.Clear(); UuendaSummad(); };

            var lblKaart = new Label { Left = 10, Top = 349, AutoSize = true, Text = "Kliendikaart:" };
            txtKaart = new TextBox { Left = 100, Top = 345, Width = 190 };
            btnLeia = new Button { Left = 298, Top = 343, Width = 132, Height = 28, Text = "Leia klient" };
            btnLeia.Click += (s, e) => LeiaKlient();
            txtKaart.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; LeiaKlient(); }
            };
            lblKlient = new Label { Left = 10, Top = 380, Width = 420, Height = 22, Text = "Klient: —" };
            chkBoonus = new CheckBox { Left = 10, Top = 406, Width = 300, Text = "Kasuta boonust", Enabled = false };
            chkBoonus.CheckedChanged += (s, e) => UuendaSummad();

            lblVahe = new Label { Left = 10, Top = 445, Width = 420, Height = 22, Font = new Font("Segoe UI", 10F) };
            lblBoonusKasutatud = new Label { Left = 10, Top = 470, Width = 420, Height = 22, Font = new Font("Segoe UI", 10F) };
            lblKokku = new Label { Left = 10, Top = 497, Width = 420, Height = 36, Font = new Font("Segoe UI", 18F, FontStyle.Bold) };
            lblBoonusSaadud = new Label { Left = 10, Top = 537, Width = 420, Height = 22, Font = new Font("Segoe UI", 10F), ForeColor = Teema.Roheline };

            btnMaksa = new Button
            {
                Left = 10, Top = 575, Width = 420, Height = 56, Text = "Maksa ja loo tšekk",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold), BackColor = Teema.Roheline, ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnMaksa.Click += btnMaksa_Click;

            pnlParem.Controls.AddRange(new Control[]
            {
                dgvOstukorv, btnPlus, btnMiinus, btnEemalda, btnTuhjenda,
                lblKaart, txtKaart, btnLeia, lblKlient, chkBoonus,
                lblVahe, lblBoonusKasutatud, lblKokku, lblBoonusSaadud, btnMaksa
            });

            Controls.Add(pnlVasak);
            Controls.Add(pnlParem);
            Controls.Add(pnlTop);
        }

        private void KassaForm_Load(object? sender, EventArgs e)
        {
            if (!Seanss.OnSisse)
            {
                Teade.Viga("Palun logi kõigepealt sisse.");
                Close();
                return;
            }

            lblKasutaja.Text = $"Kassa  |  {Seanss.Nimi} ({Seanss.Kasutaja?.Roll})";

            try
            {
                using (var db = new ToodedContext())
                {
                    var kategooriad = new List<Kategooria> { new Kategooria { Id = 0, Kategooria_nimetus = "Kõik kategooriad" } };
                    kategooriad.AddRange(db.Kategooriatabel.AsNoTracking().OrderBy(k => k.Kategooria_nimetus));
                    comboKat.DisplayMember = nameof(Kategooria.Kategooria_nimetus);
                    comboKat.ValueMember = nameof(Kategooria.Id);
                    comboKat.DataSource = kategooriad;
                }
                _valmis = true;
                LaeTooted();
                UuendaSummad();
            }
            catch (Exception ex)
            {
                Teade.Viga("Andmete laadimine ebaõnnestus:\n" + ex.GetBaseException().Message);
            }
        }

        private void btnValja_Click(object? sender, EventArgs e)
        {
            if (_korv.Count > 0 && !Teade.Kinnita("Ostukorv ei ole tühi. Kas sulgeda ikkagi?")) return;
            if (!Seanss.OnOmanik) Seanss.Kasutaja = null;     // Ostja: logime välja, Omanik naaseb menüüsse
            Close();
        }

        // ------------------------------------------------------------------ toodete ruudustik

        private void LaeTooted()
        {
            List<Toode> tooted;
            using (var db = new ToodedContext())
            {
                var paring = db.Toodetabel.AsNoTracking();
                string otsing = txtOtsing.Text.Trim();
                if (otsing.Length > 0)
                    paring = paring.Where(t => t.Toodenimetus.Contains(otsing));
                if (comboKat.SelectedValue is int katId && katId > 0)
                    paring = paring.Where(t => t.KategooriaId == katId);
                tooted = paring.OrderBy(t => t.Toodenimetus).ToList();
            }

            flowToted.SuspendLayout();
            TuhjendaTiled();
            foreach (var t in tooted)
                flowToted.Controls.Add(LooTile(t));
            flowToted.ResumeLayout();
        }

        private void TuhjendaTiled()
        {
            foreach (Control tile in flowToted.Controls.Cast<Control>().ToList())
            {
                flowToted.Controls.Remove(tile);
                foreach (var pb in tile.Controls.OfType<PictureBox>())
                {
                    pb.Image?.Dispose();
                    pb.Image = null;
                }
                tile.Dispose();
            }
        }

        private Control LooTile(Toode t)
        {
            bool otsas = t.Kogus <= 0;
            int id = t.Id;

            var tile = new KaartPanel
            {
                Width = 150, Height = 185, Margin = new Padding(8), Padding = new Padding(6),
                Fill = otsas ? Teema.Taust : Teema.Paneel,
                Cursor = otsas ? Cursors.No : Cursors.Hand
            };
            var info = new Label
            {
                Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter,
                Text = otsas ? $"{t.Hind:N2} €\nOTSAS" : $"{t.Hind:N2} €\nLaos: {t.Kogus}",
                ForeColor = otsas || t.Kogus <= Seaded.MadalLaoseis ? Teema.Punane : Teema.Tume
            };
            var nimi = new Label
            {
                Dock = DockStyle.Top, Height = 24, TextAlign = ContentAlignment.MiddleCenter,
                Text = t.Toodenimetus, AutoEllipsis = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            var pilt = new PictureBox
            {
                Dock = DockStyle.Top, Height = 110, SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Teema.Paneel2, Image = PildiHoidla.Lae(t.Pilt)
            };

            // Fill-kontroll lisatakse esimesena, Top-kontrollid pärast seda (WinForms dokkimise järjekord)
            tile.Controls.Add(info);
            tile.Controls.Add(nimi);
            tile.Controls.Add(pilt);

            foreach (Control c in new Control[] { tile, info, nimi, pilt })
                c.Click += (s, e) => LisaKorvi(id);

            return tile;
        }

        // ------------------------------------------------------------------ ostukorv

        private OstuRida? ValitudRida() => dgvOstukorv.CurrentRow?.DataBoundItem as OstuRida;

        /// <summary>Lisab ostukorvi 1 ühiku. 5. ülesanne: rohkem kui laos on, lisada ei saa.</summary>
        private void LisaKorvi(int toodeId)
        {
            try
            {
                using var db = new ToodedContext();
                var t = db.Toodetabel.AsNoTracking().FirstOrDefault(x => x.Id == toodeId);
                if (t == null)
                {
                    Teade.Viga("Toodet ei leitud (võib-olla kustutati).");
                    LaeTooted();
                    return;
                }

                var rida = _korv.FirstOrDefault(r => r.ToodeId == toodeId);
                int uusKogus = (rida?.Kogus ?? 0) + 1;

                if (t.Kogus <= 0)
                {
                    Teade.Viga($"Toode „{t.Toodenimetus}” on otsas.");
                    LaeTooted();
                    return;
                }
                if (uusKogus > t.Kogus)
                {
                    Teade.Viga($"Laos on ainult {t.Kogus} ühikut toodet „{t.Toodenimetus}”.");
                    return;
                }

                if (rida == null)
                {
                    _korv.Add(new OstuRida { ToodeId = t.Id, Nimetus = t.Toodenimetus, Hind = t.Hind, Kogus = 1 });
                }
                else
                {
                    rida.Kogus = uusKogus;
                    rida.Hind = t.Hind;
                    _korv.ResetItem(_korv.IndexOf(rida));
                }
                UuendaSummad();
            }
            catch (Exception ex)
            {
                Teade.Viga("Viga ostukorvi lisamisel:\n" + ex.GetBaseException().Message);
            }
        }

        private void btnPlus_Click(object? sender, EventArgs e)
        {
            if (ValitudRida() is { } r) LisaKorvi(r.ToodeId);
        }

        private void btnMiinus_Click(object? sender, EventArgs e)
        {
            if (ValitudRida() is not { } r) return;
            if (r.Kogus > 1)
            {
                r.Kogus--;
                _korv.ResetItem(_korv.IndexOf(r));
            }
            else
            {
                _korv.Remove(r);
            }
            UuendaSummad();
        }

        // ------------------------------------------------------------------ klient ja summad

        private void LeiaKlient()
        {
            string kaart = txtKaart.Text.Trim();
            if (kaart.Length == 0)
            {
                _klient = null;
            }
            else
            {
                try
                {
                    using var db = new ToodedContext();
                    _klient = db.Kliendid.AsNoTracking().FirstOrDefault(k => k.Kliendikaart == kaart);
                }
                catch (Exception ex)
                {
                    Teade.Viga("Kliendi otsing ebaõnnestus:\n" + ex.GetBaseException().Message);
                    return;
                }
            }

            if (_klient == null)
            {
                lblKlient.Text = kaart.Length == 0 ? "Klient: —" : "Klient: sellist kliendikaarti ei leitud";
                chkBoonus.Checked = false;
                chkBoonus.Enabled = false;
            }
            else
            {
                lblKlient.Text = $"Klient: {_klient.Nimi}   |   Boonus: {_klient.Boonus:N2} €";
                chkBoonus.Enabled = _klient.Boonus > 0;
                if (!chkBoonus.Enabled) chkBoonus.Checked = false;
            }
            UuendaSummad();
        }

        private decimal Vahesumma => _korv.Sum(r => r.Summa);

        private decimal KasutatavBoonus =>
            _klient != null && chkBoonus.Checked ? Math.Min(_klient.Boonus, Vahesumma) : 0m;

        private void UuendaSummad()
        {
            decimal vahe = Vahesumma;
            decimal kasutatud = KasutatavBoonus;
            decimal kokku = vahe - kasutatud;
            decimal saadud = _klient != null ? Math.Round(kokku * Seaded.BoonusProtsent, 2) : 0m;

            lblVahe.Text = $"Vahesumma: {vahe:N2} €";
            lblBoonusKasutatud.Text = kasutatud > 0 ? $"Boonus kasutatud: -{kasutatud:N2} €" : "";
            lblKokku.Text = $"KOKKU: {kokku:N2} €";
            lblBoonusSaadud.Text = _klient != null ? $"Boonust saad juurde: +{saadud:N2} €" : "";
            btnMaksa.Enabled = _korv.Count > 0;
        }

        // ------------------------------------------------------------------ maksmine

        private void btnMaksa_Click(object? sender, EventArgs e)
        {
            if (_korv.Count == 0)
            {
                Teade.Viga("Ostukorv on tühi.");
                return;
            }

            string? pdfTee = null;
            try
            {
                using var db = new ToodedContext();
                using var tx = db.Database.BeginTransaction();

                // Värske laoseis andmebaasist (keegi teine võis vahepeal müüa)
                var idd = _korv.Select(r => r.ToodeId).ToList();
                var tooded = db.Toodetabel.Where(t => idd.Contains(t.Id)).ToDictionary(t => t.Id);

                foreach (var r in _korv)
                {
                    if (!tooded.TryGetValue(r.ToodeId, out var t))
                    {
                        Teade.Viga($"Toodet „{r.Nimetus}” ei ole enam olemas. Eemalda see ostukorvist.");
                        return;
                    }
                    if (r.Kogus > t.Kogus)
                    {
                        Teade.Viga($"Toodet „{t.Toodenimetus}” on laos ainult {t.Kogus}, ostukorvis on {r.Kogus}.\nParanda kogus.");
                        LaeTooted();
                        return;
                    }
                }

                Klient? klient = _klient != null ? db.Kliendid.Find(_klient.Id) : null;

                decimal vahe = _korv.Sum(r => r.Kogus * tooded[r.ToodeId].Hind);
                decimal kasutatud = klient != null && chkBoonus.Checked ? Math.Min(klient.Boonus, vahe) : 0m;
                decimal kokku = vahe - kasutatud;
                decimal saadud = klient != null ? Math.Round(kokku * Seaded.BoonusProtsent, 2) : 0m;

                var muuk = new Muuk
                {
                    Kuupaev = DateTime.Now,
                    KasutajaId = Seanss.Kasutaja!.Id,
                    KlientId = klient?.Id,
                    Vahesumma = vahe,
                    BoonusKasutatud = kasutatud,
                    Kokku = kokku,
                    BoonusSaadud = saadud
                };

                foreach (var r in _korv)
                {
                    var t = tooded[r.ToodeId];
                    t.Kogus -= r.Kogus;                                    // 5. ülesanne: laoseis väheneb
                    muuk.Read.Add(new MuugiRida { ToodeId = t.Id, Nimetus = t.Toodenimetus, Kogus = r.Kogus, Hind = t.Hind });
                }

                if (klient != null)
                    klient.Boonus = klient.Boonus - kasutatud + saadud;

                db.Muugid.Add(muuk);
                db.SaveChanges();                                          // nüüd on muuk.Id olemas

                muuk.PdfFail = TsekiPdf.Loo(muuk, Seanss.Nimi, klient?.Nimi);
                db.SaveChanges();
                tx.Commit();                                               // kui PDF luua ei õnnestu, tühistatakse kogu müük

                pdfTee = Path.Combine(Seaded.ArvedKaust, muuk.PdfFail);
            }
            catch (Exception ex)
            {
                Teade.Viga("Müügi salvestamine ebaõnnestus:\n" + ex.GetBaseException().Message);
                return;
            }

            _korv.Clear();
            chkBoonus.Checked = false;
            if (_klient != null) LeiaKlient();      // värske boonus
            UuendaSummad();
            LaeTooted();                            // värske laoseis ruudustikus

            Teade.Info("Müük on salvestatud.\nTšekk: " + pdfTee);
            Teade.AvaFail(pdfTee!);
        }
    }
}
