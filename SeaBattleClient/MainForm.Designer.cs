namespace SeaBattleClient
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gameField1 = new GameFieldLibrary.GameField();
            this.gameField2 = new GameFieldLibrary.GameField();
            this.SuspendLayout();
            // 
            // gameField1
            // 
            this.gameField1.IsMyShips = false;
            this.gameField1.Location = new System.Drawing.Point(13, 13);
            this.gameField1.Name = "gameField1";
            this.gameField1.Size = new System.Drawing.Size(270, 271);
            this.gameField1.TabIndex = 0;
            // 
            // gameField2
            // 
            this.gameField2.Location = new System.Drawing.Point(358, 13);
            this.gameField2.Name = "gameField2";
            this.gameField2.Size = new System.Drawing.Size(294, 271);
            this.gameField2.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(748, 417);
            this.Controls.Add(this.gameField2);
            this.Controls.Add(this.gameField1);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private GameFieldLibrary.GameField gameField1;
        private GameFieldLibrary.GameField gameField2;
    }
}

