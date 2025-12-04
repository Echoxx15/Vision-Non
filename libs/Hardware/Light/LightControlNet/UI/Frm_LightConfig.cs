using System.Windows.Forms;

namespace LightControlNet.UI;

/// <summary>
/// 光源配置窗体 - 包装 uLightConfig UserControl
/// </summary>
public class Frm_LightConfig : Form
{
    private uLightConfig _configControl;

    public Frm_LightConfig()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        _configControl = new uLightConfig();

        this.SuspendLayout();

        // 配置控件
        _configControl.Dock = DockStyle.Fill;

        // 窗体设置
        this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(900, 600);
        this.Controls.Add(_configControl);
        this.MinimumSize = new System.Drawing.Size(800, 500);
        this.Name = "Frm_LightConfigNew";
        this.StartPosition = FormStartPosition.CenterParent;
        this.Text = "光源配置";

        this.ResumeLayout(false);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _configControl?.Dispose();
        }
        base.Dispose(disposing);
    }
}
