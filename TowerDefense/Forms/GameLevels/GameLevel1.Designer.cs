namespace TowerDefense.Forms.GameLevels
{
    partial class GameLevel1
    {
        private System.ComponentModel.IContainer components = null;

        // --- UI CONTROLS ---
        private TowerDefense.Utils.GameButton _btnStartWave;
        private TowerDefense.Utils.GameButton _btnPause;
        private TowerDefense.Utils.GameButton _btnSave;
        private TowerDefense.Utils.GameButton _btnLoad;
        private TowerDefense.Utils.GameButton _btnSpeed;
        private TowerDefense.Utils.GameButton _btnAutoWave;

        // --- NEW: CONTAINER FOR TOWER LIST ---
        private System.Windows.Forms.FlowLayoutPanel _flowTowerPanel;

        // Skill Buttons
        private TowerDefense.Utils.GameButton _btnSkillMeteor;
        private TowerDefense.Utils.GameButton _btnSkillFreeze;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this._btnStartWave = new TowerDefense.Utils.GameButton();
            this._btnPause = new TowerDefense.Utils.GameButton();
            this._btnSave = new TowerDefense.Utils.GameButton();
            this._btnLoad = new TowerDefense.Utils.GameButton();
            this._btnSpeed = new TowerDefense.Utils.GameButton();
            this._btnAutoWave = new TowerDefense.Utils.GameButton();
            this._flowTowerPanel = new System.Windows.Forms.FlowLayoutPanel(); // Initialize Panel
            this._btnSkillMeteor = new TowerDefense.Utils.GameButton();
            this._btnSkillFreeze = new TowerDefense.Utils.GameButton();
            this.SuspendLayout();

            // 
            // _btnStartWave
            // 
            this._btnStartWave.BackColor = System.Drawing.Color.Orange;
            this._btnStartWave.Color1 = System.Drawing.Color.Orange;
            this._btnStartWave.Color2 = System.Drawing.Color.DarkOrange;
            this._btnStartWave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnStartWave.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this._btnStartWave.Location = new System.Drawing.Point(650, 500);
            this._btnStartWave.Name = "_btnStartWave";
            this._btnStartWave.Size = new System.Drawing.Size(120, 50);
            this._btnStartWave.TabIndex = 0;
            this._btnStartWave.Text = "START WAVE";
            this._btnStartWave.UseVisualStyleBackColor = false;
            this._btnStartWave.Click += new System.EventHandler(this.BtnStartWave_Click);

            // _btnPause
            this._btnPause.Text = "II"; this._btnPause.Location = new System.Drawing.Point(620, 10); this._btnPause.Size = new System.Drawing.Size(40, 30); this._btnPause.Color1 = System.Drawing.Color.Gold; this._btnPause.Color2 = System.Drawing.Color.Goldenrod; this._btnPause.Click += new System.EventHandler(this.BtnPause_Click);

            // _btnSave
            this._btnSave.Text = "SAVE"; this._btnSave.Location = new System.Drawing.Point(670, 10); this._btnSave.Size = new System.Drawing.Size(80, 30); this._btnSave.Color1 = System.Drawing.Color.Gray; this._btnSave.Color2 = System.Drawing.Color.DimGray; this._btnSave.Click += new System.EventHandler(this.BtnSave_Click);

            // _btnLoad
            this._btnLoad.Text = "LOAD"; this._btnLoad.Location = new System.Drawing.Point(670, 45); this._btnLoad.Size = new System.Drawing.Size(80, 30); this._btnLoad.Color1 = System.Drawing.Color.Gray; this._btnLoad.Color2 = System.Drawing.Color.DimGray; this._btnLoad.Click += new System.EventHandler(this.BtnLoad_Click);

            // _btnSpeed
            this._btnSpeed.Text = "x1"; this._btnSpeed.Location = new System.Drawing.Point(570, 10); this._btnSpeed.Size = new System.Drawing.Size(40, 30); this._btnSpeed.Color1 = System.Drawing.Color.Khaki; this._btnSpeed.Color2 = System.Drawing.Color.DarkKhaki; this._btnSpeed.Click += new System.EventHandler(this.BtnSpeed_Click);

            // _btnAutoWave
            this._btnAutoWave.Text = "AUTO: OFF"; this._btnAutoWave.Location = new System.Drawing.Point(650, 460); this._btnAutoWave.Size = new System.Drawing.Size(120, 30); this._btnAutoWave.Color1 = System.Drawing.Color.Gray; this._btnAutoWave.Color2 = System.Drawing.Color.Black; this._btnAutoWave.Click += new System.EventHandler(this.BtnAutoWave_Click);

            // 
            // _flowTowerPanel (SCROLLABLE LIST)
            // 
            this._flowTowerPanel.Location = new System.Drawing.Point(10, 490);
            this._flowTowerPanel.Name = "_flowTowerPanel";
            this._flowTowerPanel.Size = new System.Drawing.Size(360, 100);
            this._flowTowerPanel.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this._flowTowerPanel.AutoScroll = true;
            this._flowTowerPanel.WrapContents = false;

            // _btnSkillMeteor
            this._btnSkillMeteor.Text = "METEOR"; this._btnSkillMeteor.Location = new System.Drawing.Point(380, 500); this._btnSkillMeteor.Size = new System.Drawing.Size(80, 60); this._btnSkillMeteor.Color1 = System.Drawing.Color.OrangeRed; this._btnSkillMeteor.Color2 = System.Drawing.Color.Maroon; this._btnSkillMeteor.Click += new System.EventHandler(this.BtnMeteor_Click);

            // _btnSkillFreeze
            this._btnSkillFreeze.Text = "FREEZE"; this._btnSkillFreeze.Location = new System.Drawing.Point(470, 500); this._btnSkillFreeze.Size = new System.Drawing.Size(80, 60); this._btnSkillFreeze.Color1 = System.Drawing.Color.LightCyan; this._btnSkillFreeze.Color2 = System.Drawing.Color.DeepSkyBlue; this._btnSkillFreeze.Click += new System.EventHandler(this.BtnFreeze_Click);

            // Form Setup
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);

            this.Controls.Add(this._flowTowerPanel); // Add Panel
            this.Controls.Add(this._btnAutoWave);
            this.Controls.Add(this._btnSpeed);
            this.Controls.Add(this._btnLoad);
            this.Controls.Add(this._btnSave);
            this.Controls.Add(this._btnPause);
            this.Controls.Add(this._btnSkillFreeze);
            this.Controls.Add(this._btnSkillMeteor);
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