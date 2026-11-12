#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore
{
    public record MuukVaade(int Id, DateTime Kuupaev, string Muuja, string Klient, decimal Kokku, string? PdfFail);

    /// <summary>Müügiajalugu (oma idee): müügid perioodi kohta, käive ja tšeki avamine. Ainult Omanik.</summary>
    public class MuugiForm : Form
    {
        private DateTimePicker dtpAlates, dtpKuni;
        private Button btnOtsi, btnAva;
        private DataGridView dgv;
        private Label lblKokku;

        public MuugiForm()
        { Load += (s_, e_) => Teema.Rakenda(this);
            Text = "Müügiajalugu";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(860, 520);
            MinimumSize = new Size(760, 420);

            Controls.Add(new Label { Text = "Alates", Left = 15, Top = 18, AutoSize = true });
            dtpAlates = new DateTimePicker { Left = 70, Top = 14, Width = 120, Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddDays(-30) };
            Controls.Add(dtpAlates);
            Controls.Add(new Label { Text = "Kuni", Left = 205, Top = 18, AutoSize = true });
            dtpKuni = new DateTimePicker { Left = 245, Top = 14, Width = 120, Format = DateTimePickerFormat.Short, Value = DateTime.Today };
            Controls.Add(dtpKuni);

            btnOtsi = new Button { Left = 380, Top = 12, Width = 90, Height = 28, Text = "Näita" };
            btnAva = new Button { Left = 480, Top = 12, Width = 130, Height = 28, Text = "Ava tšekk (PDF)" };
            btnOtsi.Click += (s, e) => Laa();
            btnAva.Click += btnAva_Click;
            Controls.Add(btnOtsi);
            Controls.Add(btnAva);

            dgv = new DataGridView
            {
                Left = 15, Top = 55, Width = 830, Height = 410,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AllowUserToAddRows = false, AllowUserToDeleteRows = false, ReadOnly = true, MultiSelect = false,
                RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = SystemColors.AppWorkspace
            };
            dgv.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) btnAva_Click(s, e); };
            Controls.Add(dgv);

            lblKokku = new Label
            {
                Left = 15, Top = 478, Width = 830, Height = 30, Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold)
            };
            Controls.Add(lblKokku);

            Load += (s, e) =>
            {
                if (!Seanss.OnOmanik)
                {
                    Teade.Viga("Müügiajalugu on nähtav ainult Omanikule.");
                    Close();
                    return;
                }
                Laa();
            };
        }

        private void Laa()
        {
            DateTime alates = dtpAlates.Value.Date;
            DateTime kuni = dtpKuni.Value.Date.AddDays(1);

            try
            {
                List<MuukVaade> list;
                using (var db = new ToodedContext())
                {
                    list = db.Muugid.AsNoTracking()
                        .Where(m => m.Kuupaev >= alates && m.Kuupaev < kuni)
                        .OrderByDescending(m => m.Kuupaev)
                        .Select(m => new MuukVaade(
                            m.Id,
                            m.Kuupaev,
                            m.Kasutaja!.TaisNimi ?? m.Kasutaja.Kasutajanimi,
                            m.Klient != null ? m.Klient.Nimi : "",
                            m.Kokku,
                            m.PdfFail))
                        .ToList();
                }

                dgv.DataSource = list;
                dgv.Columns[nameof(MuukVaade.PdfFail)]!.Visible = false;
                dgv.Columns[nameof(MuukVaade.Kuupaev)]!.DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                dgv.Columns[nameof(MuukVaade.Kokku)]!.DefaultCellStyle.Format = "N2";
                dgv.Columns[nameof(MuukVaade.Id)]!.FillWeight = 30;

                lblKokku.Text = $"Müüke: {list.Count}   |   Käive: {list.Sum(x => x.Kokku):N2} €";
            }
            catch (Exception ex)
            {
                Teade.Viga("Müükide laadimine ebaõnnestus:\n" + ex.GetBaseException().Message);
            }
        }

        private void btnAva_Click(object? sender, EventArgs e)
        {
            if (dgv.CurrentRow?.DataBoundItem is not MuukVaade v || string.IsNullOrEmpty(v.PdfFail))
            {
                Teade.Viga("Vali müük, mille tšekki soovid avada.");
                return;
            }
            string tee = Path.Combine(Seaded.ArvedKaust, v.PdfFail);
            if (!File.Exists(tee))
            {
                Teade.Viga("Tšeki faili ei leitud:\n" + tee);
                return;
            }
            Teade.AvaFail(tee);
        }
    }
}
