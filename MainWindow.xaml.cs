using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Forms = System.Windows.Forms;


namespace NDI_Telestrator
{
    public partial class MainWindow : MetroWindow
    {

        private void requestNDI(object caller, object args)
        {
            ndi.requestFrameUpdate();
        }
        public MainWindow()
        {
            InitializeComponent();
            InkControls.whiteboard = theWhiteboard;
            optionsDialogue.whiteboard = theWhiteboard;
            optionsDialogue.background = theBackground;




            // Send background updates every 250ms
            System.Windows.Threading.DispatcherTimer backgroundUpdateTimer = new System.Windows.Threading.DispatcherTimer();
            backgroundUpdateTimer.Interval = TimeSpan.FromMilliseconds(250);
            backgroundUpdateTimer.Tick += requestNDI;

            // Send canvas updates every 10ms
            System.Windows.Threading.DispatcherTimer canvasUpdateTimer = new System.Windows.Threading.DispatcherTimer();
            canvasUpdateTimer.Interval = TimeSpan.FromMilliseconds(10);
            canvasUpdateTimer.Tick += requestNDI;

            // Switch from the canvas update timer to the background update timer after 1 second
            System.Windows.Threading.DispatcherTimer backgroundUpdateTransitionTimer = new System.Windows.Threading.DispatcherTimer();
            backgroundUpdateTransitionTimer.Interval = TimeSpan.FromSeconds(1);
            backgroundUpdateTransitionTimer.Tick += (a, b) =>
            {
                backgroundUpdateTransitionTimer.Stop(); // Turn off after the tick
                backgroundUpdateTimer.Start();
                canvasUpdateTimer.Stop();
            };

            theWhiteboard.GotMouseCapture += (a, b) =>
            {
                backgroundUpdateTimer.Stop();
                canvasUpdateTimer.Start();
            };

            theWhiteboard.LostMouseCapture += (a, b) => backgroundUpdateTransitionTimer.Start();

            backgroundUpdateTimer.Start();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {

                case Key.Z:
                    if ((Forms.Control.ModifierKeys & Forms.Keys.Control) == Forms.Keys.Control)
                    {
                        if ((Forms.Control.ModifierKeys & Forms.Keys.Shift) == Forms.Keys.Shift)
                        {
                            // Ctrl + Shift + Z
                            theWhiteboard.Redo();
                        }
                        else
                        {
                            // Ctrl + Z
                            theWhiteboard.Undo();
                        }
                    }
                    break;

                // Ctrl + Y
                case Key.Y:
                    if ((Forms.Control.ModifierKeys & Forms.Keys.Control) == Forms.Keys.Control) theWhiteboard.Redo();
                    break;
            }
        }

        #region Toolbar Events
        private void Btn_Screenshot_Click(object sender, RoutedEventArgs e)
        {
            InkControls.Btn_Screenshot_Click(sender, e);
        }

        private void Btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            InkControls.clearWhiteboard();

        }

        private void Btn_Undo_Click(object sender, RoutedEventArgs e)
        {
            InkControls.undo();
        }

        private void Btn_Redo_Click(object sender, RoutedEventArgs e)
        {
            InkControls.redo();
        }
        private void Btn_Options_Click(object sender, RoutedEventArgs e)
        {
            optionsDialogue.IsOpen = !optionsDialogue.IsOpen;
        }

        private void onClrPickPen(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue != null) InkControls.setPenColour((Color)e.NewValue);
        }

        private void onClrPickBackground(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue != null) InkControls.setBackgroundColour(new SolidColorBrush((Color)e.NewValue));
        }

        public ICommand handleSelectThickness
        {
            get => new SimpleCommand(o => InkControls.setPenThickness(double.Parse((string)o, System.Globalization.CultureInfo.InvariantCulture)));
        }

        #endregion

    }
}
