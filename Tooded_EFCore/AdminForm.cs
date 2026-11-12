#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore
{
    public record KlientVaade(int Id, string Nimi, string? Epost, string? Telefon, string Kliendikaart, decimal Boonus);

    /// <summary>Administraatori paneel: klientide andmete haldus (tabel Kliendid). Ainult Omanik.</summary>
    public class AdminForm : Form
    {
        private TextBox txtNimi, txtEpost, txtTelefon, txtKaart, txtOtsing;
        private NumericUpDown nudBoonus;
        private Button btnUusKaart, btnLisa, btnUuenda, btnKustuta, btnTuhjenda;
        private DataGridView dgv;
        private bool _laeb;

        public AdminForm()
        { Load += (s_, e_) => Teema.Rakenda(this);
            EhitaUI();
            Load += AdminForm_Load;
        }

        private void EhitaUI()
        {
            Text = "Administraatori paneel – kliendid";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1040, 560);
            MinimumSize = new Size(960, 520);

            int y = 20;
            txtNimi = Rida("Nimi", ref y);
            txtEpost = Rida("E-post", ref y);
            txtTelefon = Rida("Telefon", ref y);

            Controls.Add(new Label { Text = "Kliendikaart", Left = 20, Top = y + 3, AutoSize = true });
            txtKaart = new TextBox { Left = 120, Top = y, Width = 150, MaxLength = 30 };
            btnUusKaart = new Button { Left = 276, Top = y - 1, Width = 80, Height = 27, Text = "Uus kaart" };
            btnUusKaart.Click += (s, e) => UusKaardiNumber();
            Controls.Add(txtKaart);
            Controls.Add(btnUusKaart);
            y += 38;

            Controls.Add(new Label { Text = "Boonus (€)", Left = 20, Top = y + 3, AutoSize = true });
            nudBoonus = new NumericUpDown
            {
                Left = 120, Top = y, Width = 236, DecimalPlaces = 2, Minimum = 0, Maximum = 1000000, Increment = 1
            };
            nudBoonus.KeyPress += (s, e) => { if (e.KeyChar == '-') e.Handled = true; };
            Controls.Add(nudBoonus);
            y += 50;

            btnLisa = new Button { Left = 20, Top = y, Width = 100, Height = 36, Text = "Lisa" };
            btnUuenda = new Button { Left = 126, Top = y, Width = 100, Height = 36, Text = "Uuenda" };
            btnKustuta = new Button { Left = 232, Top = y, Width = 100, Height = 36, Text = "Kustuta" };
            btnTuhjenda = new Button { Left = 20, Top = y + 44, Width = 312, Height = 30, Text = "Tühjenda väljad (uus klient)" };
            btnLisa.Click += btnLisa_Click;
            btnUuenda.Click += btnUuenda_Click;
            btnKustuta.Click += btnKustuta_Click;
            btnTuhjenda.Click += (s, e) => TuhjendaVljad();
            Controls.AddRange(new Control[] { btnLisa, btnUuenda, btnKustuta, btnTuhjenda });

            txtOtsing = new TextBox
            {
                Left = 380, Top = 20, Width = 300, PlaceholderText = "Otsi nime, telefoni või kaardi järgi..."
            };
            txtOtsing.TextChanged += (s, e) => LaeKliendid();
            Controls.Add(txtOtsing);

            dgv = new DataGridView
            {
                Left = 380, Top = 55, Width = 640, Height = 485,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AllowUserToAddRows = false, AllowUserToDeleteRows = false, ReadOnly = true, MultiSelect = false,
                RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = SystemColors.AppWorkspace
            };
            dgv.SelectionChanged += (s, e) => { if (!_laeb) KuvaValitud(); };
            Controls.Add(dgv);
        }

        private TextBox Rida(string silt, ref int y)
        {
            Controls.Add(new Label { Text = silt, Left = 20, Top = y + 3, AutoSize = true });
            var kast = new TextBox { Left = 120, Top = y, Width = 236 };
            Controls.Add(kast);
            y += 38;
            return kast;
        }

        private void AdminForm_Load(object? sender, EventArgs e)
        {
            if (!Seanss.OnOmanik)
            {
                Teade.Viga("Administraatori paneelile pääseb ligi ainult Omanik.");
                Close();
                return;
            }
            LaeKliendid();
        }

        // ------------------------------------------------------------------ andmed

        private void LaeKliendid(int? valiId = null)
        {
            try
            {
                List<KlientVaade> list;
                using (var db = new ToodedContext())
                {
                    var paring = db.Kliendid.AsNoTracking();
                    string s = txtOtsing.Text.Trim();
                    if (s.Length > 0)
                        paring = paring.Where(k => k.Nimi.Contains(s) || k.Kliendikaart.Contains(s)
                                                   || (k.Telefon != null && k.Telefon.Contains(s)));
                    list = paring.OrderBy(k => k.Nimi)
                                 .Select(k => new KlientVaade(k.Id, k.Nimi, k.Epost, k.Telefon, k.Kliendikaart, k.Boonus))
                                 .ToList();
                }

                _laeb = true;
                dgv.DataSource = list;
                _laeb = false;

                dgv.Columns[nameof(KlientVaade.Epost)]!.HeaderText = "E-post";
                dgv.Columns[nameof(KlientVaade.Boonus)]!.DefaultCellStyle.Format = "N2";
                dgv.Columns[nameof(KlientVaade.Id)]!.FillWeight = 30;

                if (valiId != null)
                {
                    foreach (DataGridViewRow rida in dgv.Rows)
                    {
                        if (rida.DataBoundItem is KlientVaade v && v.Id == valiId.Value)
                        {
                            _laeb = true;
                            dgv.CurrentCell = rida.Cells[nameof(KlientVaade.Nimi)];
                            rida.Selected = true;
                            _laeb = false;
                            break;
                        }
                    }
                }
                KuvaValitud();
            }
            catch (Exception ex)
            {
                Teade.Viga("Klientide laadimine ebaõnnestus:\n" + ex.GetBaseException().Message);
            }
        }

        private KlientVaade? Valitud => dgv.CurrentRow?.DataBoundItem as KlientVaade;

        private void KuvaValitud()
        {
            if (Valitud is not { } v) return;
            txtNimi.Text = v.Nimi;
            txtEpost.Text = v.Epost ?? "";
            txtTelefon.Text = v.Telefon ?? "";
            txtKaart.Text = v.Kliendikaart;
            nudBoonus.Value = Math.Min(v.Boonus, nudBoonus.Maximum);
        }

        private void TuhjendaVljad()
        {
            txtNimi.Clear();
            txtEpost.Clear();
            txtTelefon.Clear();
            txtKaart.Clear();
            nudBoonus.Value = 0;
            dgv.ClearSelection();
            txtNimi.Focus();
        }

        // ------------------------------------------------------------------ kliendikaart

        private static string LooUnikaalneKaart(ToodedContext db)
        {
            while (true)
            {
                string kaart = Random.Shared.NextInt64(1_000_000_000L, 9_999_999_999L).ToString();
                if (!db.Kliendid.Any(k => k.Kliendikaart == kaart)) return kaart;
            }
        }

        private void UusKaardiNumber()
        {
            try
            {
                using var db = new ToodedContext();
                txtKaart.Text = LooUnikaalneKaart(db);
            }
            catch (Exception ex)
            {
                Teade.Viga("Kaardi number ei õnnestunud:\n" + ex.GetBaseException().Message);
            }
        }

        // ------------------------------------------------------------------ nupud

        private bool Kontrolli(out string nimi, out string? epost, out string? telefon)
        {
            nimi = txtNimi.Text.Trim();
            epost = string.IsNullOrWhiteSpace(txtEpost.Text) ? null : txtEpost.Text.Trim();
            telefon = string.IsNullOrWhiteSpace(txtTelefon.Text) ? null : txtTelefon.Text.Trim();

            if (nimi.Length == 0)
            {
                Teade.Viga("Sisesta kliendi nimi.");
                txtNimi.Focus();
                return false;
            }
            if (epost != null && (!epost.Contains('@') || !epost.Contains('.')))
            {
                Teade.Viga("E-posti aadress ei ole korrektne.");
                txtEpost.Focus();
                return false;
            }
            nudBoonus.Validate();
            return true;
        }

        private void btnLisa_Click(object? sender, EventArgs e)
        {
            if (!Kontrolli(out var nimi, out var epost, out var telefon)) return;

            try
            {
                using var db = new ToodedContext();
                string kaart = txtKaart.Text.Trim();
                if (kaart.Length == 0) kaart = LooUnikaalneKaart(db);
                if (db.Kliendid.Any(k => k.Kliendikaart == kaart))
                {
                    Teade.Viga("Selline kliendikaart on juba olemas.");
                    return;
                }

                var uus = new Klient { Nimi = nimi, Epost = epost, Telefon = telefon, Kliendikaart = kaart, Boonus = nudBoonus.Value };
                db.Kliendid.Add(uus);
                db.SaveChanges();
                LaeKliendid(uus.Id);
            }
            catch (Exception ex)
            {
                Teade.Viga("Lisamine ebaõnnestus:\n" + ex.GetBaseException().Message);
            }
        }

        private void btnUuenda_Click(object? sender, EventArgs e)
        {
            if (Valitud is not { } v)
            {
                Teade.Viga("Vali tabelist klient.");
                return;
            }
            if (!Kontrolli(out var nimi, out var epost, out var telefon)) return;

            string kaart = txtKaart.Text.Trim();
            if (kaart.Length == 0)
            {
                Teade.Viga("Kliendikaardi number ei tohi olla tühi. Vajuta „Uus kaart”.");
                return;
            }

            try
            {
                using var db = new ToodedContext();
                if (db.Kliendid.Any(k => k.Kliendikaart == kaart && k.Id != v.Id))
                {
                    Teade.Viga("Selline kliendikaart on juba teisel kliendil.");
                    return;
                }

                var klient = db.Kliendid.Find(v.Id);
                if (klient == null)
                {
                    Teade.Viga("Klient on vahepeal kustutatud.");
                    LaeKliendid();
                    return;
                }

                klient.Nimi = nimi;
                klient.Epost = epost;
                klient.Telefon = telefon;
                klient.Kliendikaart = kaart;
                klient.Boonus = nudBoonus.Value;
                db.SaveChanges();
                LaeKliendid(klient.Id);
            }
            catch (Exception ex)
            {
                Teade.Viga("Uuendamine ebaõnnestus:\n" + ex.GetBaseException().Message);
            }
        }

        private void btnKustuta_Click(object? sender, EventArgs e)
        {
            if (Valitud is not { } v)
            {
                Teade.Viga("Vali tabelist klient.");
                return;
            }
            if (!Teade.Kinnita($"Kustutada klient „{v.Nimi}”? Müügiajalugu jääb alles.")) return;

            try
            {
                using var db = new ToodedContext();
                var klient = db.Kliendid.Find(v.Id);
                if (klient != null)
                {
                    db.Kliendid.Remove(klient);
                    db.SaveChanges();
                }
                LaeKliendid();
                TuhjendaVljad();
            }
            catch (Exception ex)
            {
                Teade.Viga("Kustutamine ebaõnnestus:\n" + ex.GetBaseException().Message);
            }
        }
    }
}
