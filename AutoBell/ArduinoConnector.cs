using System;
using System.IO.Ports;
using System.Management;

namespace AutoBell
{
    public class ArduinoConnector
    {
        private SerialPort _serialPort;
        public string _currentPort;
        public int _currentState;

        public bool IsConnected => _serialPort != null && _serialPort.IsOpen;

        public event EventHandler<bool> ConnectionStatusChanged;
        public event EventHandler<string> ErrorOccurred;

        public ArduinoConnector()
        {
            StartConnection();
        }

        private async void StartConnection()
        {
            while (!IsConnected)
            {
                ScanAndConnect();
                await Task.Delay(5000); // 5 saniyede bir dene
            }
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
                            var description = device["Description"].ToString();
                            if (description.Contains("Arduino"))
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
                _serialPort = new SerialPort(portName, 9600);
                _serialPort.Open();
                _currentPort = portName;
                ConnectionStatusChanged?.Invoke(this, true);
            }
            catch (Exception ex)
            {
                HandleError($"Failed to connect to Arduino on {portName}: {ex.Message}");
                _serialPort = null;
                ConnectionStatusChanged?.Invoke(this, false);
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