using System.Drawing.Drawing2D;

namespace Tooded_EFCore
{
    public static class Kujundid
    {
        /// <summary>Ristkülik ümarate nurkadega.</summary>
        public static GraphicsPath Umar(Rectangle r, int raadius)
        {
            int d = Math.Max(2, raadius * 2);
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }
    }

    /// <summary>Päisepaneel ühtse taustavärviga (teema värv "Pais").</summary>
    public class GradPanel : Panel
    {
        public Color Algus { get; set; } = Teema.Pais;

        public GradPanel()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using var pintsel = new SolidBrush(Algus);
            e.Graphics.FillRectangle(pintsel, ClientRectangle);
        }
    }

    /// <summary>Kaart ümarate nurkadega ja õhukese äärega.</summary>
    public class KaartPanel : Panel
    {
        private Color _fill = Teema.Paneel;
        public Color Fill { get => _fill; set { _fill = value; Invalidate(); } }
        public int Raadius { get; set; } = 10;
        public Color Piirjoon { get; set; } = Teema.Joon;

        public KaartPanel()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width < 4 || Height < 4) return;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var tee = Kujundid.Umar(new Rectangle(0, 0, Width - 1, Height - 1), Raadius);
            using var pintsel = new SolidBrush(_fill);
            using var pliiats = new Pen(Piirjoon);
            g.FillPath(pintsel, tee);
            g.DrawPath(pliiats, tee);
        }
    }

    /// <summary>Lame ümar nupp: pealkiri + valikuline väiksem alamtekst, hõljumisel värv muutub.</summary>
    public class RoundButton : Button
    {
        private bool _hover;

        public int Raadius { get; set; } = 8;
        public string Alamtekst { get; set; } = "";
        public Color Alus { get; set; } = Teema.Taust;   // värv nurkade taga

        public RoundButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            UseVisualStyleBackColor = false;
            Cursor = Cursors.Hand;
            ForeColor = Color.White;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        }

        protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnEnabledChanged(EventArgs e) { Invalidate(); base.OnEnabledChanged(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width < 8 || Height < 8) return;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.Clear(Alus);

            Color taust = !Enabled ? Teema.Hall : (_hover ? Teema.Hover(BackColor) : BackColor);
            using (var tee = Kujundid.Umar(new Rectangle(0, 0, Width - 1, Height - 1), Raadius))
            using (var pintsel = new SolidBrush(taust))
                g.FillPath(pintsel, tee);

            Color tekstiVarv = Enabled ? ForeColor : Teema.HallTekst;
            using var vorming = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            };
            using var pealiPintsel = new SolidBrush(tekstiVarv);

            if (string.IsNullOrEmpty(Alamtekst))
            {
                g.DrawString(Text, Font, pealiPintsel, new RectangleF(6, 0, Width - 12, Height), vorming);
            }
            else
            {
                g.DrawString(Text, Font, pealiPintsel, new RectangleF(8, Height * 0.14f, Width - 16, Height * 0.46f), vorming);
                using var vaike = new Font(Font.FontFamily, Math.Max(8f, Font.Size - 3f), FontStyle.Regular);
                using var alaPintsel = new SolidBrush(Color.FromArgb(Enabled ? 220 : 255, tekstiVarv));
                g.DrawString(Alamtekst, vaike, alaPintsel, new RectangleF(8, Height * 0.58f, Width - 16, Height * 0.32f), vorming);
            }
        }
    }

    /// <summary>Pealkiri läbipaistval taustal (kasutatakse päistes).</summary>
    public class Pealkiri : Control
    {
        public Pealkiri()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            BackColor = Color.Transparent;
            ForeColor = Color.White;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (string.IsNullOrEmpty(Text)) return;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            using var pintsel = new SolidBrush(ForeColor);
            using var vorming = new StringFormat { FormatFlags = StringFormatFlags.NoWrap };
            e.Graphics.DrawString(Text, Font, pintsel, new PointF(0f, 0f), vorming);
        }
    }
}
