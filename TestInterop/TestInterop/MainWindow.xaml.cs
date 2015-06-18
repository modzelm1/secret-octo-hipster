using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestInterop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// cool low level beep
        /// </summary>
        /// <param name="dwFreq"></param>
        /// <param name="dwDuration"></param>
        /// <returns></returns>
        [DllImport("Kernel32")]
        extern static bool Beep(short dwFreq, short dwDuration);

        /// <summary>
        /// play windows sounds
        /// </summary>
        /// <param name="uType"></param>
        /// <returns></returns>
        [DllImport("User32")]
        extern static bool MessageBeep(uint uType);

        [DllImport("User32")]
        extern static bool FlashWindow(IntPtr hWnd, bool bInvert);

        [DllImport("User32")]
        extern static bool FlashWindowEx(out FLASHWINFO pfwi);

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            var wih = new WindowInteropHelper(window);
            IntPtr hWnd = wih.Handle;

            Beep(750, 300);
            FlashWindow(hWnd, true);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            var wih = new WindowInteropHelper(window);
            IntPtr hWnd = wih.Handle;

            var s = System.Runtime.InteropServices.Marshal.SizeOf(typeof(FLASHWINFO));

            FLASHWINFO fi = new FLASHWINFO()
            {
                cbSize = (short)s,
                hwnd = hWnd,
                dwFlags = 0x00000003,
                uCount = 5,
                dwTimeout = 0
            };

            MessageBeep(0x00000040);
            FlashWindowEx(out fi);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FLASHWINFO
    {
        public short cbSize;
        public IntPtr hwnd;
        public uint dwFlags;
        public short uCount;
        public uint dwTimeout;
    }
}
