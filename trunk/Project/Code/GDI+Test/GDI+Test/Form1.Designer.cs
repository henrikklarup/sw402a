﻿namespace GDI_Test
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.gameArea = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // gameArea
            // 
            this.gameArea.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gameArea.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gameArea.Location = new System.Drawing.Point(13, 13);
            this.gameArea.Name = "gameArea";
            this.gameArea.Size = new System.Drawing.Size(641, 439);
            this.gameArea.TabIndex = 0;
            this.gameArea.Paint += new System.Windows.Forms.PaintEventHandler(this.gameArea_Paint);
            this.gameArea.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gameArea_MouseDown);
            this.gameArea.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gameArea_MouseMove);
            this.gameArea.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gameArea_MouseUp);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(666, 464);
            this.Controls.Add(this.gameArea);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel gameArea;
        private System.Windows.Forms.Timer timer1;
    }
}

