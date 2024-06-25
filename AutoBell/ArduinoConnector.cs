using System.IO.Ports;
using System.Management;
using Timer = System.Windows.Forms.Timer;

namespace AutoBell
{
    public class ArduinoConnector
    {
        private SerialPort _serialPort;
        public string _currentPort;
        public int _currentState;
        private Timer _pingTimer;

        public bool IsConnected => _serialPort != null && _serialPort.IsOpen;

        public event EventHandler<bool> ConnectionStatusChanged;
        public event EventHandler<string> ErrorOccurred;

        public ArduinoConnector()
        {
            // Initialize the Windows Forms Timer
            _pingTimer = new Timer();
            _pingTimer.Interval = 1000; // Ping interval in milliseconds
            _pingTimer.Tick += PingArduino;
        }

        public async void StartConnection()
        {
            while (true)
            {
                if (!IsConnected)
                {
                    ScanAndConnect();
                }
                await Task.Delay(5000);
            }
        }

        public void StopConnection()
        {
            Disconnect();
        }

        private void ScanAndConnect()
        {
            try
            {
                foreach (var port in SerialPort.GetPortNames())
                {
                    using (var searcher = new ManagementObjectSearcher($"SELECT * FROM Win32_PnPEntity WHERE Caption LIKE '%(COM{port.Remove(0, 3)})%'"))
                    {
                        foreach (var device in searcher.Get())
                        {
                            var description = device["Description"]?.ToString();
                            if (description != null && description.Contains("Arduino"))
                            {
                                Connect(port);
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError($"Error scanning and connecting to ports: {ex.Message}");
            }
        }

        private void Connect(string portName)
        {
            try
            {
                if (_serialPort != null)
                {
                    _serialPort.DataReceived -= SerialPort_DataReceived;
                    _serialPort.Close();
                }

                _serialPort = new SerialPort(portName, 9600);
                _serialPort.Open();
                _serialPort.DataReceived += SerialPort_DataReceived;
                _currentPort = portName;
                ConnectionStatusChanged?.Invoke(this, true);

                // Start the timer
                _pingTimer.Start();
            }
            catch (Exception ex)
            {
                HandleError($"Failed to connect to Arduino on {portName}: {ex.Message}");
                Disconnect();
            }
        }

        private void PingArduino(object sender, EventArgs e)
        {
            try
            {
                if (IsConnected)
                {
                    _serialPort.WriteLine("1");
                }
                else
                {
                    ConnectionStatusChanged?.Invoke(this, false);
                    _serialPort.DataReceived -= SerialPort_DataReceived;
                    _serialPort.Close();
                    _serialPort = null;
                    ConnectionStatusChanged?.Invoke(this, false);

                    // Stop the timer
                    _pingTimer.Stop();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Test");
                HandleError($"Failed to ping Arduino: {ex.Message}");
                Disconnect();
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string message = _serialPort.ReadLine().Trim();
                if (int.TryParse(message, out int state))
                {
                    _currentState = state;
                }
            }
            catch (Exception ex)
            {
                HandleError($"Error receiving data: {ex.Message}");
                Disconnect();
            }
        }

        private void Disconnect()
        {
            if (IsConnected)
            {
                _serialPort.DataReceived -= SerialPort_DataReceived;
                _serialPort.Close();
                _serialPort = null;
                ConnectionStatusChanged?.Invoke(this, false);

                // Stop the timer
                _pingTimer.Stop();
            }
        }

        public void SendState(int state)
        {
            try
            {
                _currentState = state;
                if (IsConnected)
                {
                    _serialPort.WriteLine(state.ToString());
                }
            }
            catch (Exception ex)
            {
                HandleError($"Failed to send state {state} to Arduino: {ex.Message}");
            }
        }

        private void HandleError(string errorMessage)
        {
            ErrorOccurred?.Invoke(this, errorMessage);
        }
    }
}