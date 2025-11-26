using System;
using System.Drawing;
using System.Windows.Forms;

namespace LightControlNet.UI
{
    public class Frm_LightTester : Form
    {
        private ComboBox _cmbControllers;
        private Panel _panelHost;
        private ILightController _current;
        private Control _currentForm;

        public Frm_LightTester()
        {
            Text = "光源测试";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(800, 600);

            _cmbControllers = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList, Height = 28 };
            _panelHost = new Panel { Dock = DockStyle.Fill };
            Controls.Add(_panelHost);
            Controls.Add(_cmbControllers);

            _cmbControllers.SelectedIndexChanged += CmbControllers_SelectedIndexChanged;
            Load += Frm_LightTester_Load;
        }

        private void Frm_LightTester_Load(object sender, EventArgs e)
        {
            _cmbControllers.Items.Clear();
            foreach (var c in LightFactory.Instance.Configs.Configs)
            {
                _cmbControllers.Items.Add(c.Name);
            }
            if (_cmbControllers.Items.Count > 0) _cmbControllers.SelectedIndex = 0;
        }

        private void CmbControllers_SelectedIndexChanged(object sender, EventArgs e)
        {
            var name = _cmbControllers.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(name)) return;

            _current = LightFactory.Instance.GetController(name);
            ShowTestForm(_current?.TestForm);
        }

        private void ShowTestForm(Form form)
        {
            if (_currentForm != null)
            {
                _panelHost.Controls.Remove(_currentForm);
                _currentForm.Dispose();
                _currentForm = null;
            }

            if (form == null) return;
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            _panelHost.Controls.Add(form);
            _currentForm = form;
            form.Show();
        }
    }
}
