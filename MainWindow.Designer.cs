namespace StreamMosaic {
    partial class MainWindow {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent () {
            this.Sources = new System.Windows.Forms.DataGridView();
            this.URL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ResolvedURL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StartOBS = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Sources)).BeginInit();
            this.SuspendLayout();
            // 
            // Sources
            // 
            this.Sources.AllowUserToAddRows = false;
            this.Sources.AllowUserToDeleteRows = false;
            this.Sources.AllowUserToResizeColumns = false;
            this.Sources.AllowUserToResizeRows = false;
            this.Sources.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Sources.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.URL,
            this.ResolvedURL});
            this.Sources.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.Sources.Location = new System.Drawing.Point(12, 12);
            this.Sources.Name = "Sources";
            this.Sources.RowHeadersVisible = false;
            this.Sources.Size = new System.Drawing.Size(770, 325);
            this.Sources.TabIndex = 0;
            // 
            // URL
            // 
            this.URL.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.URL.HeaderText = "Twitch URL";
            this.URL.Name = "URL";
            this.URL.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // ResolvedURL
            // 
            this.ResolvedURL.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ResolvedURL.FillWeight = 45F;
            this.ResolvedURL.HeaderText = "Resolved URL";
            this.ResolvedURL.Name = "ResolvedURL";
            this.ResolvedURL.ReadOnly = true;
            this.ResolvedURL.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // StartOBS
            // 
            this.StartOBS.Location = new System.Drawing.Point(12, 343);
            this.StartOBS.Name = "StartOBS";
            this.StartOBS.Size = new System.Drawing.Size(770, 37);
            this.StartOBS.TabIndex = 1;
            this.StartOBS.Text = "Resolve Streams and Start OBS";
            this.StartOBS.UseVisualStyleBackColor = true;
            this.StartOBS.Click += new System.EventHandler(this.StartOBS_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(794, 391);
            this.Controls.Add(this.StartOBS);
            this.Controls.Add(this.Sources);
            this.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Stream Mosaic";
            ((System.ComponentModel.ISupportInitialize)(this.Sources)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView Sources;
        private System.Windows.Forms.Button StartOBS;
        private System.Windows.Forms.DataGridViewTextBoxColumn URL;
        private System.Windows.Forms.DataGridViewTextBoxColumn ResolvedURL;
    }
}

