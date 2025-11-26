using System;
using System.Windows.Forms;
using LightControlNet;

namespace Fgen.LightPlugin.UI
{
    public partial class Frm_FgenTest : Form
    {
        private readonly ILightController _controller;

        public Frm_FgenTest(ILightController controller)
        {
            _controller = controller;
            InitializeComponent();
            Load += Frm_FgenTest_Load;
            rbOn.CheckedChanged += (_, __) => { if (rbOn.Checked) for (int ch = 1; ch <= _controller.ChannelCount; ch++) _controller.TurnOn(ch); };
            rbOff.CheckedChanged += (_, __) => { if (rbOff.Checked) for (int ch = 1; ch <= _controller.ChannelCount; ch++) _controller.TurnOff(ch); };
        }

        private void Frm_FgenTest_Load(object sender, EventArgs e)
        {
            tlpBrightness.RowCount = _controller.ChannelCount;
            tlpBrightness.Controls.Clear();
            for (int i = 0; i < _controller.ChannelCount; i++)
            {
                var lbl = new Label { Text = "CH" + (i + 1) + ":", AutoSize = true, Anchor = AnchorStyles.Left };
                var track = new TrackBar { Minimum = 0, Maximum = 255, TickFrequency = 5, Dock = DockStyle.Fill };
                var num = new NumericUpDown { Minimum = 0, Maximum = 255, Width = 60, Anchor = AnchorStyles.Right };
                int ch = i + 1;
                track.Scroll += (_, __) => { num.Value = track.Value; _controller.SetBrightness(ch, track.Value); };
                num.ValueChanged += (_, __) => { track.Value = (int)num.Value; _controller.SetBrightness(ch, (int)num.Value); };
                try { var cur = _controller.GetBrightness(ch); if (cur >= 0) { track.Value = cur; num.Value = cur; } } catch { }
                tlpBrightness.Controls.Add(lbl, 0, i);
                tlpBrightness.Controls.Add(track, 1, i);
                tlpBrightness.Controls.Add(num, 2, i);
            }
        }
    }
}
