namespace IRCD
{
    partial class DebugForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DebugForm));
            this.timerCheckPort = new System.Windows.Forms.Timer(this.components);
            this.btnClear = new System.Windows.Forms.Button();
            this.chartFrames = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.framesTable = new IRCD.FramesTable();
            ((System.ComponentModel.ISupportInitialize)(this.chartFrames)).BeginInit();
            this.SuspendLayout();
            // 
            // timerCheckPort
            // 
            this.timerCheckPort.Enabled = true;
            this.timerCheckPort.Tick += new System.EventHandler(this.timerCheckPort_Tick);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClear.Location = new System.Drawing.Point(12, 705);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 1;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // chartFrames
            // 
            this.chartFrames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chartFrames.BorderlineColor = System.Drawing.Color.DarkRed;
            this.chartFrames.BorderlineWidth = 10;
            this.chartFrames.Location = new System.Drawing.Point(613, 12);
            this.chartFrames.Name = "chartFrames";
            this.chartFrames.Size = new System.Drawing.Size(616, 716);
            this.chartFrames.TabIndex = 2;
            this.chartFrames.Text = "chart1";
            // 
            // framesTable
            // 
            this.framesTable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.framesTable.Location = new System.Drawing.Point(12, 12);
            this.framesTable.Name = "framesTable";
            this.framesTable.Size = new System.Drawing.Size(595, 687);
            this.framesTable.TabIndex = 3;
            this.framesTable.SelectionChangedEvent += new IRCD.FramesTable.SelectionChangedHandler(this.framesTable_SelectionChangedEvent);
            // 
            // DebugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1241, 740);
            this.Controls.Add(this.framesTable);
            this.Controls.Add(this.chartFrames);
            this.Controls.Add(this.btnClear);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DebugForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Debug IR";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chartFrames)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timerCheckPort;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartFrames;
        private FramesTable framesTable;
    }
}

