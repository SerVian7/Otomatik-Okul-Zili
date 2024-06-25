using AxWMPLib;
using MaterialSkin.Controls;
using Newtonsoft.Json;

namespace AutoBell
{
    public partial class Form1 : MaterialForm
    {
        #region Definitions

        private enum State
        {
            Initializing = 0,
            OgrenciZili = 4,
            OgretmenZili = 5,
            CikisZili = 6
        }

        private const string JsonFilePath = "BellSchedule.json";
        private const string SoundsPath = "Sounds";
        private const string LogsPath = "Logs";

        private State currentState;
        private MaterialButton playingButton = null;
        private TimeScheduler _timeScheduler;
        private ArduinoConnector _arduinoConnector;

        public List<MaskedTextBox> Sabahci_MT_List { get; set; } = new List<MaskedTextBox>();
        public List<MaskedTextBox> Oglenci_MT_List { get; set; } = new List<MaskedTextBox>();
        public List<MaskedTextBox> All_MT_List { get; set; } = new List<MaskedTextBox>();
        public string MT_InitialValue { get; set; }

        #endregion

        #region Main

        public Form1()
        {
            InitializeComponent();
            SwitchState(State.Initializing);
            Hide();
            TimeScheduler_OnStateChanged(_timeScheduler, _timeScheduler.GetLastExecutionPair().Key);
            if(currentState == State.Initializing) currentState = State.OgrenciZili;
        }

        private void SwitchState(State targetState, string period = "", string time = "")
        {
            switch (targetState)
            {
                case State.Initializing:
                    ProgramStatus_Label.Text = "Program açılıyor...";

                    Sabahci_MT_List.AddRange(new MaskedTextBox[]
                    {
                        S_1_1_MT, S_1_2_MT, S_1_3_MT,
                        S_2_1_MT, S_2_2_MT, S_2_3_MT,
                        S_3_1_MT, S_3_2_MT, S_3_3_MT,
                        S_4_1_MT, S_4_2_MT, S_4_3_MT,
                        S_5_1_MT, S_5_2_MT, S_5_3_MT,
                        S_6_1_MT, S_6_2_MT, S_6_3_MT,
                        S_7_1_MT, S_7_2_MT, S_7_3_MT,
                        S_8_1_MT, S_8_2_MT, S_8_3_MT
                    });

                    Oglenci_MT_List.AddRange(new MaskedTextBox[]
                    {
                        O_1_1_MT, O_1_2_MT, O_1_3_MT,
                        O_2_1_MT, O_2_2_MT, O_2_3_MT,
                        O_3_1_MT, O_3_2_MT, O_3_3_MT,
                        O_4_1_MT, O_4_2_MT, O_4_3_MT,
                        O_5_1_MT, O_5_2_MT, O_5_3_MT,
                        O_6_1_MT, O_6_2_MT, O_6_3_MT,
                        O_7_1_MT, O_7_2_MT, O_7_3_MT,
                        O_8_1_MT, O_8_2_MT, O_8_3_MT
                    });

                    All_MT_List.AddRange(Sabahci_MT_List);
                    All_MT_List.AddRange(Oglenci_MT_List);

                    if (Directory.Exists(SoundsPath))
                    {
                        var soundFiles = Directory.GetFiles(SoundsPath).Select(Path.GetFileName).ToArray();

                        Sounds_ListBox.Items.AddRange(soundFiles);
                        Ogretmen_Zili_CB.Items.AddRange(soundFiles);
                        Ogrenci_Zili_CB.Items.AddRange(soundFiles);
                        Cikis_Zili_CB.Items.AddRange(soundFiles);
                    }

                    if (File.Exists(JsonFilePath))
                    {
                        var bellSchedule = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(JsonFilePath));
                        foreach (var mt in All_MT_List)
                        {
                            if (bellSchedule.TryGetValue(mt.Name, out var value))
                            {
                                mt.Text = value.ToString();
                            }
                        }

                        SetControlValueFromJson(bellSchedule, "SabahciSwitch", value => Sabahci_Switch.Checked = Convert.ToBoolean(value));
                        SetControlValueFromJson(bellSchedule, "OglenciSwitch", value => Oglenci_Switch.Checked = Convert.ToBoolean(value));
                        SetComboBoxAndSliderFromJson(bellSchedule, "OgretmenZili", Ogretmen_Zili_CB, Ogretmen_Volume_Slider);
                        SetComboBoxAndSliderFromJson(bellSchedule, "OgrenciZili", Ogrenci_Zili_CB, Ogrenci_Volume_Slider);
                        SetComboBoxAndSliderFromJson(bellSchedule, "TeneffusZili", Cikis_Zili_CB, Cikis_Volume_Slider);

                        _timeScheduler = new TimeScheduler(bellSchedule);
                        _timeScheduler.OnStateChanged += TimeScheduler_OnStateChanged;
                    }
                    else
                    {
                        UpdateJson(reset: true);
                    }
                    _timeScheduler.Start();

                    ProgramStatus_Label.Text = "Arduino aranıyor...";
                    _arduinoConnector = new ArduinoConnector();
                    _arduinoConnector.ConnectionStatusChanged += ArduinoConnector_ConnectionStatusChanged;
                    _arduinoConnector.ErrorOccurred += ArduinoConnector_ErrorOccurred;
                    _arduinoConnector.StartConnection();
                    break;

                case State.OgrenciZili:
                    BellStatus_Label.Text = $"[{period}] {time} - Öğrenci zili çaldı.";

                    if (currentState != State.Initializing) PlaySound(Ogrenci_Zili_CB.SelectedItem.ToString(), Ogrenci_Volume_Slider.Value);
                    break;

                case State.OgretmenZili:
                    BellStatus_Label.Text = $"[{period}] {time} - Öğretmen zili çaldı.";

                    if (currentState != State.Initializing) PlaySound(Ogretmen_Zili_CB.SelectedItem.ToString(), Ogretmen_Volume_Slider.Value);
                    break;

                case State.CikisZili:
                    BellStatus_Label.Text = $"[{period}] {time} - Ders sonu zili çaldı.";

                    if (currentState != State.Initializing) PlaySound(Cikis_Zili_CB.SelectedItem.ToString(), Cikis_Volume_Slider.Value);
                    break;
            }

            currentState = targetState;
            _arduinoConnector.SendState((int)currentState);
            SaveLog(BellStatus_Label.Text);
            SaveLog(ProgramStatus_Label.Text);
        }

        private void UpdateJson(bool reset = false)
        {
            if (currentState == State.Initializing && !reset) return;

            var bellSchedule = new Dictionary<string, object>();

            foreach (var mt in All_MT_List)
                bellSchedule[mt.Name] = mt.Text;

            bellSchedule["SabahciSwitch"] = Sabahci_Switch.Checked;
            bellSchedule["OglenciSwitch"] = Oglenci_Switch.Checked;
            bellSchedule["OgretmenZili"] = new { Selection = Ogretmen_Zili_CB.Text, Volume = Ogretmen_Volume_Slider.Value };
            bellSchedule["OgrenciZili"] = new { Selection = Ogrenci_Zili_CB.Text, Volume = Ogrenci_Volume_Slider.Value };
            bellSchedule["TeneffusZili"] = new { Selection = Cikis_Zili_CB.Text, Volume = Cikis_Volume_Slider.Value };

            if (reset)
            {
                _timeScheduler = new TimeScheduler(bellSchedule);
                _timeScheduler.OnStateChanged += TimeScheduler_OnStateChanged;
            }
            else _timeScheduler._schedule = bellSchedule;

            File.WriteAllText(JsonFilePath, JsonConvert.SerializeObject(bellSchedule, Formatting.Indented));
        }

        private void SaveLog(string logMessage)
        {
            if (!Directory.Exists(LogsPath)) Directory.CreateDirectory(LogsPath);

            string fileName = $"{LogsPath}/day_{DateTime.Now.ToString("yyyy-MM-dd")}.log";
            using (StreamWriter w = File.AppendText(fileName))
            {
                w.WriteLine($"[{DateTime.Now.ToString("HH:MM:ss")}]: {logMessage}");
            }
        }

        #endregion

        #region Event Handlers

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
            notifyIcon1.ShowBalloonTip(1000, "Otomatik Okul Zili", "Çalışmaya devam ediyor.", ToolTipIcon.Info);
        }

        private void All_MT_Enter(object sender, EventArgs e)
        {
            if (sender is MaskedTextBox mt)
            {
                MT_InitialValue = mt.Text;
            }
        }

        private void All_MT_TextChanged(object sender, EventArgs e)
        {
            if (sender is MaskedTextBox mt)
            {
                mt.ForeColor = ValidateTimeInput(mt) ? Color.Black : Color.Maroon;
            }
        }

        private void All_MT_Leave(object sender, EventArgs e)
        {
            if (sender is MaskedTextBox mt)
            {
                if (ValidateTimeInput(mt))
                {
                    mt.ForeColor = Color.Black;
                    UpdateJson();
                }
                else
                {
                    mt.Text = MT_InitialValue;
                    mt.ForeColor = Color.Black;
                }
            }
        }

        private void TimeScheduler_OnStateChanged(object? sender, string key)
        {
            var period = "";

            if (key.StartsWith("S_"))
            {
                if (!Sabahci_Switch.Checked) return;
                else period = "Sabahçı";
            }
            else if (key.StartsWith("O_"))
            {
                if (!Oglenci_Switch.Checked) return;
                else period = "Öğlenci";
            }

            string time = _timeScheduler.GetLastExecutionPair().Value.ToString();

            if (key.EndsWith("1_MT")) SwitchState(State.OgrenciZili, period, time);
            else if (key.EndsWith("2_MT")) SwitchState(State.OgretmenZili, period, time);
            else if (key.EndsWith("3_MT")) SwitchState(State.CikisZili, period, time);
        }

        private void ArduinoConnector_ConnectionStatusChanged(object sender, bool isConnected)
        {
            if (isConnected)
            {
                _arduinoConnector.SendState((int)currentState);
                ProgramStatus_Label.Text = "Arduino (" + _arduinoConnector._currentPort + ") bağlı.";
            }
            else
            {
                ProgramStatus_Label.Text = "Arduino bağlantısı kesildi.";
            }

            SaveLog(ProgramStatus_Label.Text);
        }

        private void ArduinoConnector_ErrorOccurred(object sender, string errorMessage)
        {
            SaveLog(errorMessage);
        }

        private bool ValidateTimeInput(MaskedTextBox mt)
            => mt.Text == "__:__" || (mt.Text.Length == 5 && DateTime.TryParse(mt.Text, out _));

        private void Ogretmen_Listen_Btn_Click(object sender = null, EventArgs e = null)
        {
            if (Ogretmen_Zili_CB.SelectedItem != null)
            {
                if (Ogretmen_Listen_Btn.Text == "Dene") SwitchState(State.OgretmenZili, "Manuel", DateTime.Now.ToString("HH:mm:ss"));
                HandleButton(Ogretmen_Listen_Btn);
            }
        }

        private void Ogrenci_Listen_Btn_Click(object sender = null, EventArgs e = null)
        {
            if (Ogrenci_Zili_CB.SelectedItem != null)
            {
                if (Ogrenci_Listen_Btn.Text == "Dene") SwitchState(State.OgrenciZili, "Manuel", DateTime.Now.ToString("HH:mm:ss"));
                HandleButton(Ogrenci_Listen_Btn);
            }
        }

        private void Cikis_Listen_Btn_Click(object sender = null, EventArgs e = null)
        {
            if (Cikis_Zili_CB.SelectedItem != null)
            {
                if (Cikis_Listen_Btn.Text == "Dene") SwitchState(State.CikisZili, "Manuel", DateTime.Now.ToString("HH:mm:ss"));
                HandleButton(Cikis_Listen_Btn);
            }
        }

        private void Secim_Listen_Btn_Click(object sender = null, EventArgs e = null)
        {
            if (Sounds_ListBox.SelectedItem != null)
            {
                if (Secim_Listen_Btn.Text == "Dene") PlaySound(Sounds_ListBox.SelectedItem.ToString(), Secim_Volume_Slider.Value);
                HandleButton(Secim_Listen_Btn);
            }
        }

        private void Secim_Volume_Slider_onValueChanged(object sender, int newValue)
        {
            if (playingButton == Secim_Listen_Btn)
                axWindowsMediaPlayer1.settings.volume = Secim_Volume_Slider.Value;
        }

        private void Teneffus_Volume_Slider_onValueChanged(object sender, int newValue)
        {
            if (playingButton == Cikis_Listen_Btn)
                axWindowsMediaPlayer1.settings.volume = Cikis_Volume_Slider.Value;
        }

        private void Ogrenci_Volume_Slider_onValueChanged(object sender, int newValue)
        {
            if (playingButton == Ogrenci_Listen_Btn)
                axWindowsMediaPlayer1.settings.volume = Ogrenci_Volume_Slider.Value;
        }

        private void Ogretmen_Volume_Slider_onValueChanged(object sender, int newValue)
        {
            if (playingButton == Ogretmen_Listen_Btn)
                axWindowsMediaPlayer1.settings.volume = Ogretmen_Volume_Slider.Value;
        }

        private void Switch_CheckedChanged(object sender, EventArgs e)
        {
            UpdateJson();
        }

        private void ComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdateJson();
        }

        private void axWindowsMediaPlayer1_PlayStateChange(object sender, _WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (e.newState == 1 || e.newState == 2 || e.newState == 8)
            {
                if (playingButton != null)
                {
                    SwitchButtonState(playingButton, "Dene");
                    playingButton = null;
                }
            }
        }

        private void Exit_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _timeScheduler.Stop();
            notifyIcon1.Visible = false;
            Application.Exit();
            Environment.Exit(-1);
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        #endregion

        #region Methods

        private void SetControlValueFromJson(Dictionary<string, object> jsonDict, string key, Action<object> setValueAction)
        {
            if (jsonDict.TryGetValue(key, out var value)) setValueAction(value);
        }

        private void SetComboBoxAndSliderFromJson(Dictionary<string, object> jsonDict, string key, MaterialComboBox comboBox, MaterialSlider slider)
        {
            if (jsonDict.TryGetValue(key, out var value))
            {
                var obj = JsonConvert.DeserializeObject<dynamic>(value.ToString());
                comboBox.Text = obj.Selection.ToString();
                slider.Value = (int)obj.Volume;
            }
        }

        public void PlaySound(string soundFile, int volume)
        {
            axWindowsMediaPlayer1.URL = Path.Combine(SoundsPath, soundFile);
            axWindowsMediaPlayer1.settings.volume = volume;
            axWindowsMediaPlayer1.Ctlcontrols.play();
        }

        private void StopSound()
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
        }

        private void HandleButton(MaterialButton button)
        {

            if (playingButton != null && playingButton != button) SwitchButtonState(playingButton, "Dene");

            if (button.Text == "Dene")
            {
                SwitchButtonState(button, "Durdur");
                playingButton = button;
            }
            else
            {
                StopSound();
                SwitchButtonState(button, "Dene");
                playingButton = null;
            }
        }

        private void SwitchButtonState(MaterialButton button, string text)
        {
            switch (text)
            {
                case "Dene":
                    button.Text = "Dene";
                    button.FlatStyle = FlatStyle.Standard;
                    break;

                case "Durdur":
                    button.FlatStyle = FlatStyle.System;
                    button.Text = "Durdur";
                    break;
            }
        }

        #endregion
    }
}