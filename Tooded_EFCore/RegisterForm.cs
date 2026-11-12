namespace Tooded_EFCore
{
    /// <summary>
    /// Kasutajate registreerimine, rollid Ostja ja Omanik.
    /// Turvareegel: Omaniku rolli saab valida ainult esimene kasutaja (kui kasutajaid veel pole)
    /// või sisselogitud Omanik. Muidu saaks igaüks endale Omaniku õigused anda.
    /// </summary>
    public class RegisterForm : Form
    {
        private readonly TextBox _txtNimi = new();
        private readonly TextBox _txtKasutaja = new();
        private readonly TextBox _txtParool = new();
        private readonly TextBox _txtParool2 = new();
        private readonly ComboBox _comboRoll = new();

        public string? LoodudKasutajanimi { get; private set; }

        public RegisterForm()
        { Load += (s_, e_) => Teema.Rakenda(this);
            Text = "Registreeri kasutaja";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(400, 290);

            int y = 20;
            Rida("Täisnimi", _txtNimi, ref y);
            Rida("Kasutajanimi", _txtKasutaja, ref y);
            Rida("Parool", _txtParool, ref y);
            Rida("Korda parooli", _txtParool2, ref y);
            _txtParool.UseSystemPasswordChar = true;
            _txtParool2.UseSystemPasswordChar = true;
            _txtKasutaja.MaxLength = 50;

            var lblRoll = new Label { Text = "Roll", Left = 20, Top = y + 3, AutoSize = true };
            _comboRoll.SetBounds(140, y, 230, 23);
            _comboRoll.DropDownStyle = ComboBoxStyle.DropDownList;
            Controls.Add(lblRoll);
            Controls.Add(_comboRoll);
            y += 50;

            var btnReg = new Button { Text = "Registreeri", Left = 140, Top = y, Width = 110, Height = 32 };
            var btnTyhista = new Button { Text = "Sulge", Left = 260, Top = y, Width = 110, Height = 32, DialogResult = DialogResult.Cancel };
            btnReg.Click += btnRegistreeri_Click;
            Controls.Add(btnReg);
            Controls.Add(btnTyhista);
            AcceptButton = btnReg;
            CancelButton = btnTyhista;

            Load += (s, e) => LaeRollid();
        }

        private void Rida(string silt, TextBox kast, ref int y)
        {
            Controls.Add(new Label { Text = silt, Left = 20, Top = y + 3, AutoSize = true });
            kast.SetBounds(140, y, 230, 23);
            Controls.Add(kast);
            y += 38;
        }

        private bool VoibValidaOmanikku()
        {
            using var db = new ToodedContext();
            return !db.Kasutajad.Any() || Seanss.OnOmanik;
        }

        private void LaeRollid()
        {
            try
            {
                bool esimene;
                using (var db = new ToodedContext()) esimene = !db.Kasutajad.Any();

                _comboRoll.Items.Clear();
                _comboRoll.Items.Add(Rollid.Ostja);
                if (esimene || Seanss.OnOmanik) _comboRoll.Items.Add(Rollid.Omanik);

                _comboRoll.SelectedItem = esimene ? Rollid.Omanik : Rollid.Ostja;
                _comboRoll.Enabled = _comboRoll.Items.Count > 1;
            }
            catch (Exception ex)
            {
                Teade.Viga("Andmebaasi viga:\n" + ex.GetBaseException().Message);
            }
        }

        private void btnRegistreeri_Click(object? sender, EventArgs e)
        {
            string kasutaja = _txtKasutaja.Text.Trim();
            string roll = _comboRoll.SelectedItem as string ?? Rollid.Ostja;

            if (kasutaja.Length < 3) { Teade.Viga("Kasutajanimi peab olema vähemalt 3 märki."); return; }
            if (_txtParool.Text.Length < 6) { Teade.Viga("Parool peab olema vähemalt 6 märki."); return; }
            if (_txtParool.Text != _txtParool2.Text) { Teade.Viga("Paroolid ei kattu."); return; }

            try
            {
                if (roll == Rollid.Omanik && !VoibValidaOmanikku())
                {
                    Teade.Viga("Omaniku rolli saab anda ainult Omanik.");
                    return;
                }

                using var db = new ToodedContext();
                if (db.Kasutajad.Any(k => k.Kasutajanimi == kasutaja))
                {
                    Teade.Viga("Selline kasutajanimi on juba olemas.");
                    return;
                }

                var (hash, sool) = ParoolAbi.Loo(_txtParool.Text);
                db.Kasutajad.Add(new Kasutaja
                {
                    Kasutajanimi = kasutaja,
                    TaisNimi = string.IsNullOrWhiteSpace(_txtNimi.Text) ? null : _txtNimi.Text.Trim(),
                    ParoolHash = hash,
                    Sool = sool,
                    Roll = roll
                });
                db.SaveChanges();

                LoodudKasutajanimi = kasutaja;
                Teade.Info($"Kasutaja „{kasutaja}” on registreeritud rollis {roll}.");
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Teade.Viga("Registreerimine ebaõnnestus:\n" + ex.GetBaseException().Message);
            }
        }
    }
}
