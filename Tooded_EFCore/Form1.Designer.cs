#pragma warning disable CS8618
namespace Tooded_EFCore
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            lblNimetus = new Label();
            txtNimetus = new TextBox();
            lblKogus = new Label();
            nudKogus = new NumericUpDown();
            lblHind = new Label();
            nudHind = new NumericUpDown();
            lblKategooria = new Label();
            comboKategooria = new ComboBox();
            picturePilt = new PictureBox();
            btnLisaKategooria = new Button();
            btnKustutaKategooria = new Button();
            btnOtsiFail = new Button();
            btnLisa = new Button();
            btnUuenda = new Button();
            btnKustuta = new Button();
            dataGridViewTooted = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)nudKogus).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudHind).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picturePilt).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewTooted).BeginInit();
            SuspendLayout();
            //
            // lblNimetus
            //
            lblNimetus.AutoSize = true;
            lblNimetus.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            lblNimetus.Location = new Point(19, 25);
            lblNimetus.Name = "lblNimetus";
            lblNimetus.Text = "Toode nimetus";
            //
            // txtNimetus
            //
            txtNimetus.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            txtNimetus.Location = new Point(186, 27);
            txtNimetus.MaxLength = 100;
            txtNimetus.Name = "txtNimetus";
            txtNimetus.Size = new Size(157, 27);
            txtNimetus.TabIndex = 0;
            //
            // lblKogus
            //
            lblKogus.AutoSize = true;
            lblKogus.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            lblKogus.Location = new Point(99, 72);
            lblKogus.Name = "lblKogus";
            lblKogus.Text = "Kogus";
            //
            // nudKogus  (1. ülesanne: ainult arv, negatiivne ei ole lubatud)
            //
            nudKogus.DecimalPlaces = 0;
            nudKogus.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            nudKogus.Location = new Point(186, 74);
            nudKogus.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            nudKogus.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            nudKogus.Name = "nudKogus";
            nudKogus.Size = new Size(157, 27);
            nudKogus.TabIndex = 1;
            nudKogus.KeyPress += NumberUpDown_KeyPress;
            //
            // lblHind
            //
            lblHind.AutoSize = true;
            lblHind.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            lblHind.Location = new Point(120, 120);
            lblHind.Name = "lblHind";
            lblHind.Text = "Hind";
            //
            // nudHind  (1. ülesanne: ainult arv, negatiivne ei ole lubatud)
            //
            nudHind.DecimalPlaces = 2;
            nudHind.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            nudHind.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudHind.Location = new Point(186, 122);
            nudHind.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            nudHind.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            nudHind.Name = "nudHind";
            nudHind.Size = new Size(157, 27);
            nudHind.TabIndex = 2;
            nudHind.KeyPress += NumberUpDown_KeyPress;
            //
            // lblKategooria
            //
            lblKategooria.AutoSize = true;
            lblKategooria.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            lblKategooria.Location = new Point(50, 166);
            lblKategooria.Name = "lblKategooria";
            lblKategooria.Text = "Kategooria";
            //
            // comboKategooria
            //
            comboKategooria.DropDownStyle = ComboBoxStyle.DropDownList;
            comboKategooria.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            comboKategooria.Location = new Point(186, 169);
            comboKategooria.Name = "comboKategooria";
            comboKategooria.Size = new Size(157, 28);
            comboKategooria.TabIndex = 3;
            //
            // picturePilt
            //
            picturePilt.BorderStyle = BorderStyle.FixedSingle;
            picturePilt.Location = new Point(360, 27);
            picturePilt.Name = "picturePilt";
            picturePilt.Size = new Size(245, 170);
            picturePilt.SizeMode = PictureBoxSizeMode.Zoom;
            picturePilt.TabStop = false;
            //
            // btnLisaKategooria
            //
            btnLisaKategooria.Location = new Point(19, 210);
            btnLisaKategooria.Name = "btnLisaKategooria";
            btnLisaKategooria.Size = new Size(92, 48);
            btnLisaKategooria.TabIndex = 4;
            btnLisaKategooria.Text = "Lisa kategooria";
            btnLisaKategooria.UseVisualStyleBackColor = true;
            btnLisaKategooria.Click += btnLisaKategooria_Click;
            //
            // btnKustutaKategooria
            //
            btnKustutaKategooria.Location = new Point(117, 210);
            btnKustutaKategooria.Name = "btnKustutaKategooria";
            btnKustutaKategooria.Size = new Size(93, 48);
            btnKustutaKategooria.TabIndex = 5;
            btnKustutaKategooria.Text = "Kustuta kategooria";
            btnKustutaKategooria.UseVisualStyleBackColor = true;
            btnKustutaKategooria.Click += btnKustutaKategooria_Click;
            //
            // btnOtsiFail
            //
            btnOtsiFail.Location = new Point(221, 210);
            btnOtsiFail.Name = "btnOtsiFail";
            btnOtsiFail.Size = new Size(123, 48);
            btnOtsiFail.TabIndex = 6;
            btnOtsiFail.Text = "Otsi fail";
            btnOtsiFail.UseVisualStyleBackColor = true;
            btnOtsiFail.Click += btnOtsiFail_Click;
            //
            // btnLisa
            //
            btnLisa.Location = new Point(353, 210);
            btnLisa.Name = "btnLisa";
            btnLisa.Size = new Size(123, 48);
            btnLisa.TabIndex = 7;
            btnLisa.Text = "Lisa";
            btnLisa.UseVisualStyleBackColor = true;
            btnLisa.Click += btnLisa_Click;
            //
            // btnUuenda
            //
            btnUuenda.Location = new Point(483, 210);
            btnUuenda.Name = "btnUuenda";
            btnUuenda.Size = new Size(121, 48);
            btnUuenda.TabIndex = 8;
            btnUuenda.Text = "Uuenda";
            btnUuenda.UseVisualStyleBackColor = true;
            btnUuenda.Click += btnUuenda_Click;
            //
            // btnKustuta
            //
            btnKustuta.Location = new Point(613, 210);
            btnKustuta.Name = "btnKustuta";
            btnKustuta.Size = new Size(122, 48);
            btnKustuta.TabIndex = 9;
            btnKustuta.Text = "Kustuta";
            btnKustuta.UseVisualStyleBackColor = true;
            btnKustuta.Click += btnKustuta_Click;
            //
            // dataGridViewTooted
            //
            dataGridViewTooted.AllowUserToAddRows = false;
            dataGridViewTooted.AllowUserToDeleteRows = false;
            dataGridViewTooted.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridViewTooted.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewTooted.BackgroundColor = SystemColors.AppWorkspace;
            dataGridViewTooted.Location = new Point(41, 280);
            dataGridViewTooted.MultiSelect = false;
            dataGridViewTooted.Name = "dataGridViewTooted";
            dataGridViewTooted.ReadOnly = true;
            dataGridViewTooted.RowHeadersVisible = false;
            dataGridViewTooted.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewTooted.Size = new Size(695, 215);
            dataGridViewTooted.TabIndex = 10;
            dataGridViewTooted.DataBindingComplete += dataGridViewTooted_DataBindingComplete;
            dataGridViewTooted.SelectionChanged += dataGridViewTooted_SelectionChanged;
            //
            // Form1
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(780, 520);
            Controls.Add(dataGridViewTooted);
            Controls.Add(btnKustuta);
            Controls.Add(btnUuenda);
            Controls.Add(btnLisa);
            Controls.Add(btnOtsiFail);
            Controls.Add(btnKustutaKategooria);
            Controls.Add(btnLisaKategooria);
            Controls.Add(picturePilt);
            Controls.Add(comboKategooria);
            Controls.Add(lblKategooria);
            Controls.Add(nudHind);
            Controls.Add(lblHind);
            Controls.Add(nudKogus);
            Controls.Add(lblKogus);
            Controls.Add(txtNimetus);
            Controls.Add(lblNimetus);
            MinimumSize = new Size(796, 559);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Tooded";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)nudKogus).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudHind).EndInit();
            ((System.ComponentModel.ISupportInitialize)picturePilt).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewTooted).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblNimetus;
        private TextBox txtNimetus;
        private Label lblKogus;
        private NumericUpDown nudKogus;
        private Label lblHind;
        private NumericUpDown nudHind;
        private Label lblKategooria;
        private ComboBox comboKategooria;
        private PictureBox picturePilt;
        private Button btnLisaKategooria;
        private Button btnKustutaKategooria;
        private Button btnOtsiFail;
        private Button btnLisa;
        private Button btnUuenda;
        private Button btnKustuta;
        private DataGridView dataGridViewTooted;
    }
}
