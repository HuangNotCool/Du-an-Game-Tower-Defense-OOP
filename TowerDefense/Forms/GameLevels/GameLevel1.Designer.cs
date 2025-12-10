namespace TowerDefense.Forms.GameLevels
{
    partial class GameLevel1
    {
        private System.ComponentModel.IContainer components = null;

        // --- KHAI BÁO CONTROL ---
        private System.Windows.Forms.Button _btnStartWave;
        private System.Windows.Forms.Button _btnSelectArcher;
        private System.Windows.Forms.Button _btnSelectCannon;
        private System.Windows.Forms.Button _btnSkillMeteor;
        private System.Windows.Forms.Button _btnSkillFreeze;
        private System.Windows.Forms.Button _btnSave;
        private System.Windows.Forms.Button _btnLoad;
        private System.Windows.Forms.Button _btnPause; // Nút Tạm dừng mới

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this._btnStartWave = new System.Windows.Forms.Button();
            this._btnSelectArcher = new System.Windows.Forms.Button();
            this._btnSelectCannon = new System.Windows.Forms.Button();
            this._btnSkillMeteor = new System.Windows.Forms.Button();
            this._btnSkillFreeze = new System.Windows.Forms.Button();
            this._btnSave = new System.Windows.Forms.Button();
            this._btnLoad = new System.Windows.Forms.Button();
            this._btnPause = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // 
            // _btnStartWave (Góc dưới phải)
            // 
            this._btnStartWave.BackColor = System.Drawing.Color.Orange;
            this._btnStartWave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnStartWave.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this._btnStartWave.Location = new System.Drawing.Point(650, 500);
            this._btnStartWave.Name = "_btnStartWave";
            this._btnStartWave.Size = new System.Drawing.Size(120, 50);
            this._btnStartWave.TabIndex = 0;
            this._btnStartWave.Text = "START WAVE";
            this._btnStartWave.UseVisualStyleBackColor = false;
            this._btnStartWave.Click += new System.EventHandler(this.BtnStartWave_Click);

            // 
            // _btnSelectArcher (Chọn tháp 1)
            // 
            this._btnSelectArcher.BackColor = System.Drawing.Color.Yellow;
            this._btnSelectArcher.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnSelectArcher.Location = new System.Drawing.Point(200, 500);
            this._btnSelectArcher.Name = "_btnSelectArcher";
            this._btnSelectArcher.Size = new System.Drawing.Size(80, 60);
            this._btnSelectArcher.TabIndex = 1;
            this._btnSelectArcher.Text = "Archer\n(100 G)";
            this._btnSelectArcher.UseVisualStyleBackColor = false;
            this._btnSelectArcher.Click += (s, e) => this.SelectTower(1);

            // 
            // _btnSelectCannon (Chọn tháp 2)
            // 
            this._btnSelectCannon.BackColor = System.Drawing.Color.Gray;
            this._btnSelectCannon.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnSelectCannon.Location = new System.Drawing.Point(290, 500);
            this._btnSelectCannon.Name = "_btnSelectCannon";
            this._btnSelectCannon.Size = new System.Drawing.Size(80, 60);
            this._btnSelectCannon.TabIndex = 2;
            this._btnSelectCannon.Text = "Cannon\n(200 G)";
            this._btnSelectCannon.UseVisualStyleBackColor = false;
            this._btnSelectCannon.Click += (s, e) => this.SelectTower(2);

            // 
            // _btnSkillMeteor
            // 
            this._btnSkillMeteor.BackColor = System.Drawing.Color.OrangeRed;
            this._btnSkillMeteor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnSkillMeteor.ForeColor = System.Drawing.Color.White;
            this._btnSkillMeteor.Location = new System.Drawing.Point(400, 500);
            this._btnSkillMeteor.Name = "_btnSkillMeteor";
            this._btnSkillMeteor.Size = new System.Drawing.Size(80, 60);
            this._btnSkillMeteor.TabIndex = 3;
            this._btnSkillMeteor.Text = "METEOR";
            this._btnSkillMeteor.UseVisualStyleBackColor = false;
            this._btnSkillMeteor.Click += new System.EventHandler(this.BtnMeteor_Click);

            // 
            // _btnSkillFreeze
            // 
            this._btnSkillFreeze.BackColor = System.Drawing.Color.Cyan;
            this._btnSkillFreeze.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnSkillFreeze.Location = new System.Drawing.Point(490, 500);
            this._btnSkillFreeze.Name = "_btnSkillFreeze";
            this._btnSkillFreeze.Size = new System.Drawing.Size(80, 60);
            this._btnSkillFreeze.TabIndex = 4;
            this._btnSkillFreeze.Text = "FREEZE";
            this._btnSkillFreeze.UseVisualStyleBackColor = false;
            this._btnSkillFreeze.Click += new System.EventHandler(this.BtnFreeze_Click);

            // 
            // _btnSave
            // 
            this._btnSave.BackColor = System.Drawing.Color.LightGray;
            this._btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnSave.Location = new System.Drawing.Point(680, 10);
            this._btnSave.Name = "_btnSave";
            this._btnSave.Size = new System.Drawing.Size(100, 30);
            this._btnSave.TabIndex = 5;
            this._btnSave.Text = "SAVE";
            this._btnSave.UseVisualStyleBackColor = false;
            this._btnSave.Click += new System.EventHandler(this.BtnSave_Click);

            // 
            // _btnLoad
            // 
            this._btnLoad.BackColor = System.Drawing.Color.LightGray;
            this._btnLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnLoad.Location = new System.Drawing.Point(680, 50);
            this._btnLoad.Name = "_btnLoad";
            this._btnLoad.Size = new System.Drawing.Size(100, 30);
            this._btnLoad.TabIndex = 6;
            this._btnLoad.Text = "LOAD";
            this._btnLoad.UseVisualStyleBackColor = false;
            this._btnLoad.Click += new System.EventHandler(this.BtnLoad_Click);

            // 
            // _btnPause
            // 
            this._btnPause.BackColor = System.Drawing.Color.Yellow;
            this._btnPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnPause.Location = new System.Drawing.Point(630, 10);
            this._btnPause.Name = "_btnPause";
            this._btnPause.Size = new System.Drawing.Size(40, 30);
            this._btnPause.TabIndex = 7;
            this._btnPause.Text = "II";
            this._btnPause.UseVisualStyleBackColor = false;
            this._btnPause.Click += new System.EventHandler(this.BtnPause_Click);

            // 
            // GameLevel1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this._btnPause);
            this.Controls.Add(this._btnLoad);
            this.Controls.Add(this._btnSave);
            this.Controls.Add(this._btnSkillFreeze);
            this.Controls.Add(this._btnSkillMeteor);
            this.Controls.Add(this._btnSelectCannon);
            this.Controls.Add(this._btnSelectArcher);
            this.Controls.Add(this._btnStartWave);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "GameLevel1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tower Defense";
            this.ResumeLayout(false);
        }
        #endregion
    }
}