using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Tooded_EFCore
{
    /// <summary>Loob tšeki PDF-faili kausta "Arved" (exe kõrval). Tagastab failinime.</summary>
    public static class TsekiPdf
    {
        public static string Loo(Muuk muuk, string muuja, string? klient)
        {
            Directory.CreateDirectory(Seaded.ArvedKaust);
            QuestPDF.Settings.License = LicenseType.Community;

            string failinimi = $"Tsekk_{muuk.Id:D5}_{muuk.Kuupaev:yyyyMMdd_HHmmss}.pdf";
            string tee = Path.Combine(Seaded.ArvedKaust, failinimi);

            Document.Create(dokument =>
            {
                dokument.Page(leht =>
                {
                    leht.Size(PageSizes.A5);
                    leht.Margin(25);
                    leht.DefaultTextStyle(x => x.FontSize(10));

                    leht.Header().Column(veerg =>
                    {
                        veerg.Item().Text(Seaded.PoeNimi).FontSize(22).Bold();
                        veerg.Item().Text($"Tšekk nr {muuk.Id:D5}").FontSize(12).SemiBold();
                        veerg.Item().Text($"Kuupäev: {muuk.Kuupaev:dd.MM.yyyy HH:mm}");
                        veerg.Item().Text($"Müüja: {muuja}");
                        if (!string.IsNullOrEmpty(klient))
                            veerg.Item().Text($"Klient: {klient}");
                    });

                    leht.Content().PaddingVertical(10).Column(sisu =>
                    {
                        sisu.Item().Table(tabel =>
                        {
                            tabel.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(5);
                                c.RelativeColumn(1.3f);
                                c.RelativeColumn(2);
                                c.RelativeColumn(2);
                            });

                            tabel.Header(p =>
                            {
                                p.Cell().Element(Pealkiri).Text("Toode");
                                p.Cell().Element(Pealkiri).AlignRight().Text("Kogus");
                                p.Cell().Element(Pealkiri).AlignRight().Text("Hind");
                                p.Cell().Element(Pealkiri).AlignRight().Text("Summa");
                            });

                            foreach (var r in muuk.Read)
                            {
                                tabel.Cell().Element(Lahter).Text(r.Nimetus);
                                tabel.Cell().Element(Lahter).AlignRight().Text(r.Kogus.ToString());
                                tabel.Cell().Element(Lahter).AlignRight().Text($"{r.Hind:N2} €");
                                tabel.Cell().Element(Lahter).AlignRight().Text($"{r.Kogus * r.Hind:N2} €");
                            }
                        });

                        sisu.Item().PaddingTop(12).AlignRight().Column(summa =>
                        {
                            summa.Item().AlignRight().Text($"Vahesumma: {muuk.Vahesumma:N2} €");
                            if (muuk.BoonusKasutatud > 0)
                                summa.Item().AlignRight().Text($"Boonus kasutatud: -{muuk.BoonusKasutatud:N2} €");
                            summa.Item().AlignRight().Text($"KOKKU: {muuk.Kokku:N2} €").FontSize(14).Bold();
                            if (!string.IsNullOrEmpty(klient))
                                summa.Item().AlignRight().Text($"Boonus saadud: +{muuk.BoonusSaadud:N2} €");
                        });
                    });

                    leht.Footer().AlignCenter().Text("Aitäh ostu eest!");
                });
            }).GeneratePdf(tee);

            return failinimi;
        }

        private static IContainer Pealkiri(IContainer c) =>
            c.BorderBottom(1).BorderColor(Colors.Black).PaddingVertical(4).DefaultTextStyle(x => x.SemiBold());

        private static IContainer Lahter(IContainer c) =>
            c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(3);
    }
}
