using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;


namespace Tooded_EFCore
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dataGridViewTooted = new DataGridView();
            txtNimetus = new TextBox();
            txtKogus = new TextBox();
            txtHind = new TextBox();
            comboKategooria = new ComboBox();
            picturePilt = new PictureBox();
            btnLisa = new Button();
            btnUuenda = new Button();
            btnKustuta = new Button();
            btnValiPilt = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridViewTooted).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picturePilt).BeginInit();
            SuspendLayout();
            // 
            // dataGridViewTooted
            // 
            dataGridViewTooted.Location = new Point(12, 12);
            dataGridViewTooted.MultiSelect = false;
            dataGridViewTooted.Name = "dataGridViewTooted";
            dataGridViewTooted.ReadOnly = true;
            dataGridViewTooted.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewTooted.Size = new Size(600, 300);
            dataGridViewTooted.TabIndex = 0;
            dataGridViewTooted.CellClick += dataGridViewTooted_CellClick;
            // 
            // txtNimetus
            // 
            txtNimetus.Location = new Point(630, 30);
            txtNimetus.Name = "txtNimetus";
            txtNimetus.PlaceholderText = "Toote nimetus";
            txtNimetus.Size = new Size(200, 23);
            txtNimetus.TabIndex = 1;
            // 
            // txtKogus
            // 
            txtKogus.Location = new Point(630, 70);
            txtKogus.Name = "txtKogus";
            txtKogus.PlaceholderText = "Kogus";
            txtKogus.Size = new Size(200, 23);
            txtKogus.TabIndex = 2;
            // 
            // txtHind
            // 
            txtHind.Location = new Point(630, 110);
            txtHind.Name = "txtHind";
            txtHind.PlaceholderText = "Hind €";
            txtHind.Size = new Size(200, 23);
            txtHind.TabIndex = 3;
            // 
            // comboKategooria
            // 
            comboKategooria.Location = new Point(630, 150);
            comboKategooria.Name = "comboKategooria";
            comboKategooria.Size = new Size(200, 23);
            comboKategooria.TabIndex = 4;
            comboKategooria.SelectedIndexChanged += comboKategooria_SelectedIndexChanged;
            // 
            // picturePilt
            // 
            picturePilt.BorderStyle = BorderStyle.FixedSingle;
            picturePilt.Location = new Point(630, 190);
            picturePilt.Name = "picturePilt";
            picturePilt.Size = new Size(150, 150);
            picturePilt.SizeMode = PictureBoxSizeMode.Zoom;
            picturePilt.TabIndex = 5;
            picturePilt.TabStop = false;
            // 
            // btnLisa
            // 
            btnLisa.Location = new Point(630, 390);
            btnLisa.Name = "btnLisa";
            btnLisa.Size = new Size(75, 23);
            btnLisa.TabIndex = 7;
            btnLisa.Text = "Lisa";
            btnLisa.Click += btnLisa_Click;
            // 
            // btnUuenda
            // 
            btnUuenda.Location = new Point(630, 430);
            btnUuenda.Name = "btnUuenda";
            btnUuenda.Size = new Size(75, 23);
            btnUuenda.TabIndex = 8;
            btnUuenda.Text = "Uuenda";
            btnUuenda.Click += btnUuenda_Click;
            // 
            // btnKustuta
            // 
            btnKustuta.Location = new Point(630, 470);
            btnKustuta.Name = "btnKustuta";
            btnKustuta.Size = new Size(75, 23);
            btnKustuta.TabIndex = 9;
            btnKustuta.Text = "Kustuta";
            btnKustuta.Click += btnKustuta_Click;
            // 
            // btnValiPilt
            // 
            btnValiPilt.Location = new Point(630, 350);
            btnValiPilt.Name = "btnValiPilt";
            btnValiPilt.Size = new Size(75, 23);
            btnValiPilt.TabIndex = 6;
            btnValiPilt.Text = "Vali pilt";
            btnValiPilt.Click += btnValiPilt_Click;
            // 
            // Form2
            // 
            ClientSize = new Size(860, 520);
            Controls.Add(dataGridViewTooted);
            Controls.Add(txtNimetus);
            Controls.Add(txtKogus);
            Controls.Add(txtHind);
            Controls.Add(comboKategooria);
            Controls.Add(picturePilt);
            Controls.Add(btnValiPilt);
            Controls.Add(btnLisa);
            Controls.Add(btnUuenda);
            Controls.Add(btnKustuta);
            Name = "Form2";
            Text = "Toodete haldus";
            ((System.ComponentModel.ISupportInitialize)dataGridViewTooted).EndInit();
            ((System.ComponentModel.ISupportInitialize)picturePilt).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
        private System.Windows.Forms.DataGridView dataGridViewTooted;
        private System.Windows.Forms.TextBox txtNimetus;
        private System.Windows.Forms.TextBox txtKogus;
        private System.Windows.Forms.TextBox txtHind;
        private System.Windows.Forms.ComboBox comboKategooria;
        private System.Windows.Forms.PictureBox picturePilt;
        private System.Windows.Forms.Button btnLisa;
        private System.Windows.Forms.Button btnUuenda;
        private System.Windows.Forms.Button btnKustuta;
        private System.Windows.Forms.Button btnValiPilt;

        
        private void btnValiPilt_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "Pildid|*.jpg;*.png;*.jpeg";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                valitudPilt = ofd.FileName;
                picturePilt.Image = Image.FromFile(valitudPilt);
            }
        }
        private void btnLisa_Click(object sender, EventArgs e)
        {
            var uus = new Toode
            {
                Toodenimetus = txtNimetus.Text,
                Kogus = int.Parse(txtKogus.Text),
                Hind = decimal.Parse(txtHind.Text),
                KategooriaId = (int)comboKategooria.SelectedValue,
                Pilt = Path.GetFileName(valitudPilt)
            };

            // kopeerime pildi projekti kausta
            if (!string.IsNullOrEmpty(valitudPilt))
            {
                File.Copy(valitudPilt, Path.Combine("Images", uus.Pilt), true);
            }

            _db.Toodetabel.Add(uus);
            _db.SaveChanges();
            LaeTooted();
        }
        private void dataGridViewTooted_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            txtNimetus.Text = dataGridViewTooted.Rows[e.RowIndex].Cells["Toodenimetus"].Value.ToString();
            txtKogus.Text = dataGridViewTooted.Rows[e.RowIndex].Cells["Kogus"].Value.ToString();
            txtHind.Text = dataGridViewTooted.Rows[e.RowIndex].Cells["Hind"].Value.ToString();

            string pilt = dataGridViewTooted.Rows[e.RowIndex].Cells["Pilt"].Value?.ToString();

            if (!string.IsNullOrEmpty(pilt) && File.Exists(Path.Combine("Images", pilt)))
                picturePilt.Image = Image.FromFile(Path.Combine("Images", pilt));
        }
        private void btnUuenda_Click(object sender, EventArgs e)
        {
            int id = (int)dataGridViewTooted.SelectedRows[0].Cells["Id"].Value;

            var toode = _db.Toodetabel.Find(id);

            toode.Toodenimetus = txtNimetus.Text;
            toode.Kogus = int.Parse(txtKogus.Text);
            toode.Hind = decimal.Parse(txtHind.Text);
            toode.KategooriaId = (int)comboKategooria.SelectedValue;

            if (valitudPilt != null)
            {
                toode.Pilt = Path.GetFileName(valitudPilt);
                File.Copy(valitudPilt, Path.Combine("Images", toode.Pilt), true);
            }

            _db.SaveChanges();
            LaeTooted();
        }
        private void btnKustuta_Click(object sender, EventArgs e)
        {
            int id = (int)dataGridViewTooted.SelectedRows[0].Cells["Id"].Value;

            var toode = _db.Toodetabel.Find(id);

            _db.Toodetabel.Remove(toode);
            _db.SaveChanges();
            LaeTooted();
        }

        



        #endregion
    }
}