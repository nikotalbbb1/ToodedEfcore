namespace Tooded_EFCore
{
    /// <summary>Väike aken uue kategooria nime ja kirjelduse küsimiseks.</summary>
    public class KategooriaDialoog : Form
    {
        private readonly TextBox _txtNimi = new();
        private readonly TextBox _txtKirjeldus = new();

        public string Nimi => _txtNimi.Text.Trim();
        public string? Kirjeldus => string.IsNullOrWhiteSpace(_txtKirjeldus.Text) ? null : _txtKirjeldus.Text.Trim();

        public KategooriaDialoog()
        { Load += (s_, e_) => Teema.Rakenda(this);
            Text = "Lisa kategooria";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(380, 170);

            var lblNimi = new Label { Text = "Nimetus", Left = 15, Top = 20, AutoSize = true };
            _txtNimi.SetBounds(110, 17, 250, 23);
            _txtNimi.MaxLength = 100;

            var lblKirjeldus = new Label { Text = "Kirjeldus", Left = 15, Top = 60, AutoSize = true };
            _txtKirjeldus.SetBounds(110, 57, 250, 23);

            var btnOk = new Button { Text = "Lisa", Left = 110, Top = 115, Width = 110, Height = 32 };
            var btnCancel = new Button { Text = "Tühista", Left = 250, Top = 115, Width = 110, Height = 32, DialogResult = DialogResult.Cancel };

            btnOk.Click += (s, e) =>
            {
                if (Nimi.Length == 0)
                {
                    Teade.Viga("Sisesta kategooria nimetus.");
                    _txtNimi.Focus();
                    return;
                }
                DialogResult = DialogResult.OK;
            };

            Controls.AddRange(new Control[] { lblNimi, _txtNimi, lblKirjeldus, _txtKirjeldus, btnOk, btnCancel });
            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }
    }
}
