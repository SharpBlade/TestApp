using System.Windows;

namespace SharpBlade.TestApp
{
    using System;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Windows.Interop;
    using System.Windows.Media.Imaging;

    using SharpBlade.Events;
    using SharpBlade.Razer;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Random Random = new Random();

        private readonly ObservableCollection<string> _dkLog;

        private readonly ObservableCollection<string> _gestureLog;

        private readonly ISwitchblade _sb;

        private int _count;

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

        private IDynamicKey GetDk(DynamicKeyType type = DynamicKeyType.DK1)
        {
            return _sb.DynamicKeys[type]
                   ?? _sb.DynamicKeys.Enable(type, "dk_image.bmp", "dk_image_down.bmp", true);
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

        private void DkBitmapClick(object sender, RoutedEventArgs e)
        {
            var dk = GetDk(DynamicKeyType.DK1 + _count);
            _count++;
            if (_count > 9)
                _count = 0;
            var bmp = new Bitmap(115, 115);
            for (var x = 0; x < bmp.Width; x++)
                for (var y = 0; y < bmp.Height; y++)
                    bmp.SetPixel(x, y, Color.FromArgb(255 * x / bmp.Width, 255 * y / bmp.Height, 0));
            dk.Draw(bmp);
            DkBitmapPreview.Source = Imaging.CreateBitmapSourceFromHBitmap(
                bmp.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
