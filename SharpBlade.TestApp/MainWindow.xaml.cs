using System.Windows;

namespace SharpBlade.TestApp
{
    using System;
    using System.Collections.ObjectModel;

    using SharpBlade.Events;
    using SharpBlade.Razer;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<string> _dkLog;

        private readonly ObservableCollection<string> _gestureLog;

        private ISwitchblade _sb;

        public MainWindow()
        {
            InitializeComponent();

            _sb = Switchblade.Instance;

            _dkLog = new ObservableCollection<string>();
            DkLogList.ItemsSource = _dkLog;
            LogDk("DK logger initialized!");

            _sb.DynamicKeys.DynamicKeyEvent += DynamicKeysOnDynamicKeyEvent;

            _gestureLog = new ObservableCollection<string>();
            GestureLogBox.ItemsSource = _gestureLog;
            LogGesture("Gesture logger initialized!");

            _sb.Touchpad.EnableGesture(GestureTypes.All);

            _sb.Touchpad.Gesture += TouchpadOnGesture;
        }

        private void DynamicKeysOnDynamicKeyEvent(object sender, DynamicKeyEventArgs e)
        {
            var msg = string.Format("{0}: {1}", e.KeyType, e.State);
            LogDk(msg);
        }

        private void TouchpadOnGesture(object sender, GestureEventArgs e)
        {
            var msg = string.Format("{0}: Param={1}, XYZ: ({2}, {3}, {4})", e.GestureTypes, e.Parameter, e.X, e.Y, e.Z);
            LogGesture(msg);
        }

        private IDynamicKey GetDk()
        {
            return _sb.DynamicKeys[DynamicKeyType.DK1]
                   ?? _sb.DynamicKeys.Enable(DynamicKeyType.DK1, "dk_image.bmp", "dk_image_down.bmp", true);
        }

        private void LogDk(string msg)
        {
            _dkLog.Insert(0, msg);
            if (_dkLog.Count > 100)
                _dkLog.RemoveAt(_dkLog.Count - 1);
        }

        private void LogGesture(string msg)
        {
            _gestureLog.Insert(0, msg);
            if (_gestureLog.Count > 100)
                _gestureLog.RemoveAt(_gestureLog.Count - 1);
        }

        private void WPFDrawClick(object sender, RoutedEventArgs e)
        {
            _sb.Touchpad.Draw(this);
        }

        private void TPDrawClick(object sender, RoutedEventArgs e)
        {
            _sb.Touchpad.Draw("tp_image.bmp");
        }

        private void DkDrawClick(object sender, RoutedEventArgs e)
        {
            GetDk();
        }

        private void WpfRenderChecked(object sender, RoutedEventArgs e)
        {
            _sb.Touchpad.Set(this);
        }

        private void WpfRenderUnchecked(object sender, RoutedEventArgs e)
        {
            _sb.Touchpad.Clear();
        }
    }
}
