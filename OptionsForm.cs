using System.Diagnostics.CodeAnalysis;

namespace WindowMinimizer
{
    /// <summary>
    /// A lightweight settings window to configure the keybind and startup behavior.
    /// </summary>
    public class OptionsForm : Form
    {
        private readonly ComboBox _keyComboBox;
        private readonly CheckBox _startupCheckBox;
        private readonly Button _saveButton;
        private readonly TrayApplicationContext _appContext;

        public OptionsForm(TrayApplicationContext appContext)
        {
            _appContext = appContext;

            // Form Configuration
            Text = "Window Minimizer Settings";
            Size = new Size(280, 180);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            // Trigger Key Label
            Controls.Add(new Label { Text = "Trigger Key:", Location = new Point(20, 20), AutoSize = true });

            // Trigger Key Dropdown (F13 - F24)
            _keyComboBox = new ComboBox { Location = new Point(100, 17), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            for (int i = 13; i <= 24; i++)
            {
                _keyComboBox.Items.Add((Keys)Enum.Parse(typeof(Keys), $"F{i}"));
            }
            _keyComboBox.SelectedItem = SettingsManager.GetTriggerKey();
            Controls.Add(_keyComboBox);

            // Startup Checkbox
            _startupCheckBox = new CheckBox { Text = "Start with Windows", Location = new Point(20, 60), AutoSize = true, Width = 200 };
            _startupCheckBox.Checked = SettingsManager.GetRunAtStartup();
            Controls.Add(_startupCheckBox);

            // Save Button
            _saveButton = new Button { Text = "Save && Close", Location = new Point(80, 100), Width = 100 };
            _saveButton.Click += SaveButton_Click;
            Controls.Add(_saveButton);
        }

        [AllowNull] public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            Keys selectedKey = (Keys)_keyComboBox.SelectedItem!;

            // Save to Registry
            SettingsManager.SetTriggerKey(selectedKey);
            SettingsManager.SetRunAtStartup(_startupCheckBox.Checked);

            // Update the running application instantly
            _appContext.ApplyNewKeybind(selectedKey);

            Close();
        }
    }
}
