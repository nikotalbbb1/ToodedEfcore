using System.Net;
using System.Net.Mail;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace Tooded_EFCore
{
    /// <summary>Poe ostukorvi vaatamine, PDF-arve loomine ja saatmine e-postiga.</summary>
    public class ArveForm : Form
    {
        private readonly ListBox listOstukorv = new();
        private readonly Label lblKokku = new();
        private readonly TextBox txtEmail = new();
        private readonly Button btnEemalda = new(), btnGenPDF = new(), btnSaadaEmail = new();
        private string? viimanePdf;

        public ArveForm()
        { Load += (s_, e_) => Teema.Rakenda(this);
            Text = "Ostukorv ja arve";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(600, 380);

            listOstukorv.SetBounds(20, 20, 330, 300);
            lblKokku.SetBounds(20, 330, 330, 30);
            lblKokku.Font = new Font(Font, FontStyle.Bold);
            txtEmail.SetBounds(370, 20, 200, 27);
            txtEmail.PlaceholderText = "Saaja e-post";

            SeaNupp(btnEemalda, "Eemalda valitud rida", 70, btnEemalda_Click);
            SeaNupp(btnGenPDF, "Genereeri PDF", 115, btnGenPDF_Click);
            SeaNupp(btnSaadaEmail, "Saada e-postiga", 160, btnSaadaEmail_Click);

            Controls.AddRange(new Control[] { listOstukorv, lblKokku, txtEmail,
                btnEemalda, btnGenPDF, btnSaadaEmail });
            NaitaKorvi();
        }

        private void SeaNupp(Button nupp, string tekst, int y, EventHandler kasitleja)
        {
            nupp.Text = tekst;
            nupp.SetBounds(370, y, 200, 35);
            nupp.Click += kasitleja;
        }

        private void NaitaKorvi()
        {
            listOstukorv.Items.Clear();
            foreach (var rida in Ostukorv.Ridad)
                listOstukorv.Items.Add(rida);
            lblKokku.Text = $"Kokku: {Ostukorv.Kokku:F2} €";
        }

        private void btnEemalda_Click(object? sender, EventArgs e)
        {
            if (listOstukorv.SelectedItem is OstukorviRida rida)
            {
                Ostukorv.Ridad.Remove(rida);
                NaitaKorvi();
            }
        }

        private void btnGenPDF_Click(object? sender, EventArgs e)
        {
            if (Ostukorv.Ridad.Count == 0)
            {
                Teade.Viga("Ostukorv on tühi.");
                return;
            }

            Directory.CreateDirectory(Seaded.ArvedKaust);
            viimanePdf = Path.Combine(Seaded.ArvedKaust, $"Arve_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            using var dokument = new PdfDocument();
            PdfPage leht = dokument.AddPage();
            using (XGraphics gfx = XGraphics.FromPdfPage(leht))
            {
                var pealkiri = new XFont("Arial", 20, XFontStyle.Bold);
                var tavaline = new XFont("Arial", 12);
                var paks = new XFont("Arial", 14, XFontStyle.Bold);

                gfx.DrawString("ARVE", pealkiri, XBrushes.Black,
                    new XRect(0, 30, leht.Width, 30), XStringFormats.TopCenter);
                gfx.DrawString($"Kuupäev: {DateTime.Now:dd.MM.yyyy}", tavaline, XBrushes.Black, 40, 90);

                double y = 130;
                foreach (var rida in Ostukorv.Ridad)
                {
                    gfx.DrawString(rida.ToString(), tavaline, XBrushes.Black, 40, y);
                    y += 22;
                }
                gfx.DrawString($"Kokku: {Ostukorv.Kokku:F2} EUR", paks, XBrushes.Black, 40, y + 20);
            }
            dokument.Save(viimanePdf);
            Teade.Info($"PDF on loodud:\n{viimanePdf}");
        }

        private void btnSaadaEmail_Click(object? sender, EventArgs e)
        {
            if (viimanePdf == null || !File.Exists(viimanePdf))
            {
                Teade.Viga("Genereeri enne PDF.");
                return;
            }
            string saaja = txtEmail.Text.Trim();
            if (!MailAddress.TryCreate(saaja, out _))
            {
                Teade.Viga("Sisesta korrektne e-posti aadress.");
                return;
            }

            var seaded = LaeEpostiSeaded();
            if (seaded == null) return;
            string saatja = seaded.Value.Saatja;
            string rakenduseParool = seaded.Value.Parool;

            try
            {
                using var kiri = new MailMessage(saatja, saaja, "Arve", "Arve on manuses.");
                kiri.Attachments.Add(new Attachment(viimanePdf));

                using var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(saatja, rakenduseParool),
                    EnableSsl = true
                };
                smtp.Send(kiri);

                Teade.Info("Arve saadetud!");
                Ostukorv.Tyhjenda();
                NaitaKorvi();
            }
            catch (Exception ex)
            {
                Teade.Viga("Viga: " + ex.Message);
            }
        }

        /// <summary>
        /// Loeb Gmaili aadressi ja rakenduse parooli failist %LOCALAPPDATA%\ToodedApp\epost.txt
        /// (1. rida: aadress, 2. rida: 16-täheline rakenduse parool). Parooli ei hoita koodis ega GitHubis.
        /// </summary>
        private static (string Saatja, string Parool)? LaeEpostiSeaded()
        {
            string tee = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                      "ToodedApp", "epost.txt");
            try
            {
                if (File.Exists(tee))
                {
                    var read = File.ReadAllLines(tee).Select(r => r.Trim()).Where(r => r.Length > 0).ToArray();
                    if (read.Length >= 2 && read[0].Contains('@') && !read[0].StartsWith("sinu.")) return (read[0], read[1]);
                }
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(tee)!);
                    File.WriteAllText(tee, "sinu.konto@gmail.com\r\nxxxx xxxx xxxx xxxx\r\n");
                }
            }
            catch { /* kuvame allolevas teates */ }

            Teade.Viga("E-posti seaded puuduvad.\n\nAvage fail:\n" + tee +
                       "\n\nKirjutage 1. reale oma Gmaili aadress ja 2. reale rakenduse parool, salvestage ja proovige uuesti.");
            return null;
        }
    }
}
