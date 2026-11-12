namespace Tooded_EFCore
{
    public class LoginForm : Form
    {
        private readonly TextBox _txtKasutaja = new();
        private readonly TextBox _txtParool = new();
        private readonly Label _viga = new();

        public LoginForm()
        {
            Text = "Sisselogimine";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(380, 494);
            BackColor = Teema.Taust;
            Teema.TumeTiitel(this);

            var pais = new GradPanel { Dock = DockStyle.Top, Height = 120 };
            pais.Controls.Add(new Pealkiri
            {
                Text = Seaded.PoeNimi, Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                Left = 26, Top = 20, Width = 330, Height = 54
            });
            pais.Controls.Add(new Label
            {
                Text = "Logi sisse, et jätkata.", ForeColor = Color.FromArgb(230, 235, 245),
                BackColor = Color.Transparent, Font = new Font("Segoe UI", 10.5F), Left = 30, Top = 80, AutoSize = true
            });

            var fond = new Font("Segoe UI", 11F);
            var lblK = new Label { Text = "Kasutajanimi", Left = 30, Top = 140, AutoSize = true, ForeColor = Teema.Tume };
            _txtKasutaja.SetBounds(30, 163, 320, 29);
            var lblP = new Label { Text = "Parool", Left = 30, Top = 205, AutoSize = true, ForeColor = Teema.Tume };
            _txtParool.SetBounds(30, 228, 320, 29);
            _txtParool.UseSystemPasswordChar = true;
            foreach (var tb in new[] { _txtKasutaja, _txtParool })
            {
                tb.Font = fond;
                tb.BorderStyle = BorderStyle.FixedSingle;
                tb.BackColor = Teema.Paneel2;
                tb.ForeColor = Teema.Tume;
            }

            var btnLogi = Teema.Nupp("Logi sisse", Teema.Aksent, 30, 285, 320, 44, 12F);
            var btnReg = Teema.Nupp("Registreeru", Teema.Hall, 30, 341, 320, 40, 10.5F);
            btnReg.ForeColor = Teema.Tume;
            btnLogi.Click += btnLogi_Click;
            btnReg.Click += btnRegistreeru_Click;

            _viga.SetBounds(30, 390, 320, 26);
            _viga.ForeColor = Teema.Punane;
            _viga.BackColor = Color.Transparent;
            _viga.TextAlign = ContentAlignment.MiddleCenter;
            _viga.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _txtKasutaja.TextChanged += (s, e) => _viga.Text = "";
            _txtParool.TextChanged += (s, e) => _viga.Text = "";

            Controls.AddRange(new Control[] { pais, lblK, _txtKasutaja, lblP, _txtParool, btnLogi, btnReg, _viga });

            // teemavalija: vahetab värvid ja avab sisselogimise uuesti
            foreach (var c in Teema.TeemaValija(30, 448, 100, 10, 34, () => { DialogResult = DialogResult.Retry; Close(); }))
                Controls.Add(c);

            AcceptButton = btnLogi;
        }

        private void btnLogi_Click(object? sender, EventArgs e)
        {
            string nimi = _txtKasutaja.Text.Trim();
            try
            {
                using var db = new ToodedContext();
                var k = db.Kasutajad.FirstOrDefault(x => x.Kasutajanimi == nimi);
                if (k == null || !ParoolAbi.Kontrolli(_txtParool.Text, k.ParoolHash, k.Sool))
                {
                    _txtParool.Clear();
                    _txtParool.Focus();
                    _viga.Text = "Vale kasutajanimi või parool.";
                    return;
                }
                Seanss.Kasutaja = k;
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Teade.Viga("Sisselogimine ebaõnnestus:\n" + ex.GetBaseException().Message);
            }
        }

        private void btnRegistreeru_Click(object? sender, EventArgs e)
        {
            using var f = new RegisterForm();
            f.ShowDialog(this);
            if (f.LoodudKasutajanimi != null)
            {
                _txtKasutaja.Text = f.LoodudKasutajanimi;
                _txtParool.Clear();
                _txtParool.Focus();
            }
        }
    }
}
