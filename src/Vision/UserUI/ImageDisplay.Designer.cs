using System.Windows.Forms;
using Cognex.VisionPro;

namespace Vision.UserUI
{
    partial class ImageDisplay
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageDisplay));
            this.grb_ShowName = new System.Windows.Forms.GroupBox();
            this.cogRecordDisplay = new Cognex.VisionPro.CogRecordDisplay();
            this.grb_ShowName.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cogRecordDisplay)).BeginInit();
            this.SuspendLayout();
            // 
            // grb_ShowName
            // 
            this.grb_ShowName.BackColor = System.Drawing.Color.DarkSlateGray;
            this.grb_ShowName.Controls.Add(this.cogRecordDisplay);
            this.grb_ShowName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grb_ShowName.Location = new System.Drawing.Point(0, 0);
            this.grb_ShowName.Name = "grb_ShowName";
            this.grb_ShowName.Size = new System.Drawing.Size(535, 415);
            this.grb_ShowName.TabIndex = 2;
            this.grb_ShowName.TabStop = false;
            this.grb_ShowName.Text = "图_1";
            // 
            // cogRecordDisplay
            // 
            this.cogRecordDisplay.ColorMapLowerClipColor = System.Drawing.Color.Black;
            this.cogRecordDisplay.ColorMapLowerRoiLimit = 0D;
            this.cogRecordDisplay.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.None;
            this.cogRecordDisplay.ColorMapUpperClipColor = System.Drawing.Color.Black;
            this.cogRecordDisplay.ColorMapUpperRoiLimit = 1D;
            this.cogRecordDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cogRecordDisplay.DoubleTapZoomCycleLength = 2;
            this.cogRecordDisplay.DoubleTapZoomSensitivity = 2.5D;
            this.cogRecordDisplay.Location = new System.Drawing.Point(3, 24);
            this.cogRecordDisplay.MouseWheelMode = Cognex.VisionPro.Display.CogDisplayMouseWheelModeConstants.Zoom1;
            this.cogRecordDisplay.MouseWheelSensitivity = 1D;
            this.cogRecordDisplay.Name = "cogRecordDisplay";
            this.cogRecordDisplay.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("cogRecordDisplay.OcxState")));
            this.cogRecordDisplay.Size = new System.Drawing.Size(529, 388);
            this.cogRecordDisplay.TabIndex = 0;
            // 
            // ImageDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grb_ShowName);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ImageDisplay";
            this.Size = new System.Drawing.Size(535, 415);
            this.Load += new System.EventHandler(this.ImageDisplay_Load);
            this.grb_ShowName.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cogRecordDisplay)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox grb_ShowName;
        private CogRecordDisplay cogRecordDisplay;
    }
}
