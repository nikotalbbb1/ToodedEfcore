using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore
{
    /// <summary>Rida tabeli jaoks (grid näitab kategooria nime, mitte ainult Id-d).</summary>
    public record ToodeVaade(int Id, string Toodenimetus, int Kogus, decimal Hind, string? Pilt,
                             string Kategooria, int KategooriaId);

    /// <summary>Toodete haldus. Ligipääs ainult rollil Omanik.</summary>
    public partial class Form1 : Form
    {
        private string? _valitudPilt;   // fail, mille kasutaja valis nupuga "Otsi fail" (veel pole Images-kaustas)
        private bool _laeb;             // true, kui vormi täidame programmiliselt (et sündmused ei seguks)

        public Form1()
        { Load += (s_, e_) => Teema.Rakenda(this);
            InitializeComponent();
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            if (!Seanss.OnOmanik)
            {
                Teade.Viga("Toodete vormile pääseb ligi ainult Omanik.");
                Close();
                return;
            }
            LaeKategooriad();
            LaeTooted();
        }

        // ---------------------------------------------------------------- andmete laadimine

        private void LaeKategooriad(int? valiId = null)
        {
            using var db = new ToodedContext();
            var list = db.Kategooriatabel.AsNoTracking().OrderBy(k => k.Kategooria_nimetus).ToList();

            _laeb = true;
            comboKategooria.DataSource = null;
            comboKategooria.DisplayMember = nameof(Kategooria.Kategooria_nimetus);
            comboKategooria.ValueMember = nameof(Kategooria.Id);
            comboKategooria.DataSource = list;
            _laeb = false;

            if (valiId != null && list.Any(k => k.Id == valiId.Value))
                comboKategooria.SelectedValue = valiId.Value;
            else
                comboKategooria.SelectedIndex = list.Count > 0 ? 0 : -1;
        }

        private void LaeTooted(int? valiId = null)
        {
            List<ToodeVaade> read;
            using (var db = new ToodedContext())
            {
                read = db.Toodetabel.AsNoTracking()
                    .OrderBy(t => t.Id)
                    .Select(t => new ToodeVaade(t.Id, t.Toodenimetus, t.Kogus, t.Hind, t.Pilt,
                                                t.Kategooria!.Kategooria_nimetus, t.KategooriaId))
                    .ToList();
            }

            _laeb = true;
            dataGridViewTooted.DataSource = read;
            _laeb = false;

            dataGridViewTooted.Columns[nameof(ToodeVaade.KategooriaId)]!.Visible = false;
            dataGridViewTooted.Columns[nameof(ToodeVaade.Hind)]!.DefaultCellStyle.Format = "N2";

            ValiRida(valiId);
            KuvaValitud();
        }

        private void ValiRida(int? id)
        {
            if (id == null) return;
            foreach (DataGridViewRow rida in dataGridViewTooted.Rows)
            {
                if (rida.DataBoundItem is ToodeVaade v && v.Id == id.Value)
                {
                    _laeb = true;
                    dataGridViewTooted.CurrentCell = rida.Cells[nameof(ToodeVaade.Toodenimetus)];
                    rida.Selected = true;
                    _laeb = false;
                    return;
                }
            }
        }

        // Idee: madala laoseisuga tooted (<= 5) on tabelis punased
        private void dataGridViewTooted_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow rida in dataGridViewTooted.Rows)
            {
                if (rida.DataBoundItem is ToodeVaade v && v.Kogus <= Seaded.MadalLaoseis)
                    rida.DefaultCellStyle.ForeColor = Teema.Punane;
            }
        }

        private void dataGridViewTooted_SelectionChanged(object? sender, EventArgs e)
        {
            if (_laeb) return;
            KuvaValitud();
        }

        /// <summary>Täidab sisestusväljad valitud tabeli rea järgi.</summary>
        private void KuvaValitud()
        {
            // PÕHIPARANDUS: varem jäi eelmise toote pilt meelde ja "Uuenda" pani selle teisele tootele.
            _valitudPilt = null;

            if (dataGridViewTooted.CurrentRow?.DataBoundItem is not ToodeVaade v)
            {
                TuhjendaVorm();
                return;
            }

            _laeb = true;
            txtNimetus.Text = v.Toodenimetus;
            nudKogus.Value = Math.Min(v.Kogus, nudKogus.Maximum);
            nudHind.Value = Math.Min(v.Hind, nudHind.Maximum);
            comboKategooria.SelectedValue = v.KategooriaId;
            NaitaPilti(v.Pilt);
            _laeb = false;
        }

        private void TuhjendaVorm()
        {
            _laeb = true;
            txtNimetus.Clear();
            nudKogus.Value = 0;
            nudHind.Value = 0;
            NaitaPilti(null);
            _laeb = false;
        }

        private void NaitaPilti(string? failinimi)
        {
            var vana = picturePilt.Image;
            picturePilt.Image = failinimi == null ? null : PildiHoidla.Lae(failinimi);
            vana?.Dispose();
        }

        // ---------------------------------------------------------------- sisestuskontroll (1. ülesanne)

        private void NumberUpDown_KeyPress(object? sender, KeyPressEventArgs e)
        {
            // miinusmärki üldse ei lase sisestada
            if (e.KeyChar == '-') e.Handled = true;
        }

        private bool KontrolliSisend(out string nimi, out int kogus, out decimal hind, out int kategooriaId)
        {
            nimi = txtNimetus.Text.Trim();
            kogus = 0;
            hind = 0;
            kategooriaId = 0;

            if (nimi.Length == 0)
            {
                Teade.Viga("Sisesta toote nimetus.");
                txtNimetus.Focus();
                return false;
            }
            if (comboKategooria.SelectedItem is not Kategooria kat)
            {
                Teade.Viga("Vali kategooria (või lisa uus nupuga „Lisa kategooria”).");
                return false;
            }

            nudKogus.Validate();
            nudHind.Validate();

            if (nudKogus.Value < 0)
            {
                Teade.Viga("Kogus ei tohi olla negatiivne.");
                return false;
            }
            if (nudHind.Value <= 0)
            {
                Teade.Viga("Hind peab olema suurem kui 0.");
                nudHind.Focus();
                return false;
            }

            kogus = (int)nudKogus.Value;
            hind = nudHind.Value;
            kategooriaId = kat.Id;
            return true;
        }

        // ---------------------------------------------------------------- nupud: toode

        private void btnOtsiFail_Click(object? sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog { Filter = "Pildid|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Kõik failid|*.*" };
            if (ofd.ShowDialog(this) != DialogResult.OK) return;

            var pilt = PildiHoidla.LaeFailist(ofd.FileName);
            if (pilt == null)
            {
                Teade.Viga("Seda faili ei saa pildina avada.");
                return;
            }

            var vana = picturePilt.Image;
            picturePilt.Image = pilt;
            vana?.Dispose();
            _valitudPilt = ofd.FileName;
        }

        private void btnLisa_Click(object? sender, EventArgs e)
        {
            if (!KontrolliSisend(out var nimi, out var kogus, out var hind, out var katId)) return;

            try
            {
                string? pilt = _valitudPilt != null ? PildiHoidla.Salvesta(_valitudPilt) : null;

                using var db = new ToodedContext();
                var uus = new Toode { Toodenimetus = nimi, Kogus = kogus, Hind = hind, KategooriaId = katId, Pilt = pilt };
                db.Toodetabel.Add(uus);
                db.SaveChanges();

                LaeTooted(uus.Id);
            }
            catch (Exception ex)
            {
                Teade.Viga("Toote lisamine ebaõnnestus:\n" + ex.GetBaseException().Message);
            }
        }

        private void btnUuenda_Click(object? sender, EventArgs e)
        {
            if (dataGridViewTooted.CurrentRow?.DataBoundItem is not ToodeVaade v)
            {
                Teade.Viga("Vali tabelist toode, mida uuendada.");
                return;
            }
            if (!KontrolliSisend(out var nimi, out var kogus, out var hind, out var katId)) return;

            try
            {
                using var db = new ToodedContext();
                var toode = db.Toodetabel.Find(v.Id);
                if (toode == null)
                {
                    Teade.Viga("Toode on vahepeal kustutatud.");
                    LaeTooted();
                    return;
                }

                toode.Toodenimetus = nimi;
                toode.Kogus = kogus;
                toode.Hind = hind;
                toode.KategooriaId = katId;

                // uus pilt ainult siis, kui kasutaja selle just valis; muidu jääb vana pilt alles
                if (_valitudPilt != null)
                    toode.Pilt = PildiHoidla.Salvesta(_valitudPilt);

                db.SaveChanges();
                LaeTooted(toode.Id);
            }
            catch (Exception ex)
            {
                Teade.Viga("Uuendamine ebaõnnestus:\n" + ex.GetBaseException().Message);
            }
        }

        private void btnKustuta_Click(object? sender, EventArgs e)
        {
            if (dataGridViewTooted.CurrentRow?.DataBoundItem is not ToodeVaade v)
            {
                Teade.Viga("Vali tabelist toode, mida kustutada.");
                return;
            }
            if (!Teade.Kinnita($"Kustutada toode „{v.Toodenimetus}”?")) return;

            try
            {
                using var db = new ToodedContext();
                var toode = db.Toodetabel.Find(v.Id);
                if (toode != null)
                {
                    db.Toodetabel.Remove(toode);
                    db.SaveChanges();
                }
                LaeTooted();
            }
            catch (Exception ex)
            {
                Teade.Viga("Kustutamine ebaõnnestus:\n" + ex.GetBaseException().Message);
            }
        }

        // ---------------------------------------------------------------- nupud: kategooria

        private void btnLisaKategooria_Click(object? sender, EventArgs e)
        {
            using var dlg = new KategooriaDialoog();
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                using var db = new ToodedContext();
                if (db.Kategooriatabel.Any(k => k.Kategooria_nimetus == dlg.Nimi))
                {
                    Teade.Viga("Selline kategooria on juba olemas.");
                    return;
                }

                var uus = new Kategooria { Kategooria_nimetus = dlg.Nimi, Kirjeldus = dlg.Kirjeldus };
                db.Kategooriatabel.Add(uus);
                db.SaveChanges();

                LaeKategooriad(uus.Id);     // pilt ja teised väljad jäävad puutumata
            }
            catch (Exception ex)
            {
                Teade.Viga("Kategooria lisamine ebaõnnestus:\n" + ex.GetBaseException().Message);
            }
        }

        private void btnKustutaKategooria_Click(object? sender, EventArgs e)
        {
            if (comboKategooria.SelectedItem is not Kategooria kat)
            {
                Teade.Viga("Vali kategooria, mida kustutada.");
                return;
            }

            try
            {
                using var db = new ToodedContext();
                int tooteid = db.Toodetabel.Count(t => t.KategooriaId == kat.Id);
                if (tooteid > 0)
                {
                    Teade.Viga($"Kategoorias „{kat.Kategooria_nimetus}” on {tooteid} toodet.\n" +
                               "Kustuta need tooted või vii need teise kategooriasse, siis saad kategooria kustutada.");
                    return;
                }
                if (!Teade.Kinnita($"Kustutada kategooria „{kat.Kategooria_nimetus}”?")) return;

                var kustutatav = db.Kategooriatabel.Find(kat.Id);
                if (kustutatav != null)
                {
                    db.Kategooriatabel.Remove(kustutatav);
                    db.SaveChanges();
                }

                int? praeguneToode = (dataGridViewTooted.CurrentRow?.DataBoundItem as ToodeVaade)?.Id;
                LaeKategooriad();
                LaeTooted(praeguneToode);
            }
            catch (Exception ex)
            {
                Teade.Viga("Kategooria kustutamine ebaõnnestus:\n" + ex.GetBaseException().Message);
            }
        }
    }
}
