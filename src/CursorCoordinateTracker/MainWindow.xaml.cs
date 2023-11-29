using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CursorCoordinateTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Imports the GetPhysicalCursorPos function from the user32.dll library.
        /// </summary>
        /// <param name="lpPoint">A pointer to a <see cref="TagPoint"/> structure that receives the screen coordinates of the cursor.</param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.
        /// To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>.
        /// </returns>
        [LibraryImport("user32.dll")]
        private static partial IntPtr GetPhysicalCursorPos(out TagPoint lpPoint);

        /// <summary>
        /// Imports the SetPhysicalCursorPos function from the user32.dll library.
        /// </summary>
        /// <param name="x">The new x-coordinate of the cursor, in screen coordinates.</param>
        /// <param name="y">The new y-coordinate of the cursor, in screen coordinates.</param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.
        /// To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>.
        /// </returns>
        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool SetPhysicalCursorPos(int x, int y);

        // Indicates whether the tracking is currently running.
        private bool _isRunning;
        private double _refreshSpeed = 1000;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the click event for the Start/Stop button.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void BtnStartStop_Click(object sender, RoutedEventArgs e)
        {
            // Toggle the running state
            _isRunning = !_isRunning;

            // Update the label on the button
            SetLabel((Button)sender);

            // Start a new task to continuously update the cursor position
            Task.Run(() =>
            {
                while (_isRunning)
                {
                    // Get the physical cursor position
                    GetPhysicalCursorPos(out TagPoint position);

                    // Update UI components using Dispatcher
                    Dispatcher.BeginInvoke(() =>
                    {
                        TxbAxisX.Text = position.X.ToString();
                        TxbAxisY.Text = position.Y.ToString();
                    });

                    // Pause for a short duration before the next update
                    Thread.Sleep(TimeSpan.FromMilliseconds(_refreshSpeed));
                }
            });
        }

        /// <summary>
        /// Handles the click event for the Set Position button.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void BtnSetPosition_Click(object sender, RoutedEventArgs e)
        {
            // Use Dispatcher to update UI components
            Dispatcher.BeginInvoke(() =>
            {
                // Parse values from text boxes
                _ = int.TryParse(TxbAxisX.Text, out int x);
                _ = int.TryParse(TxbAxisX.Text, out int y);

                // Set the physical cursor position
                SetPhysicalCursorPos(x, y);
            });
        }

        /// <summary>
        /// Handles the ValueChanged event of the SldRefreshSpeed slider, updating the refresh speed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        private void SldRefreshSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Update the refresh speed based on the slider value
            _refreshSpeed = ((Slider)sender).Value;
        }

        // Sets the label for a button based on the current state.
        private void SetLabel(Button button)
        {
            // If running, set the button label to indicate stopping
            if (_isRunning)
            {
                
                button.Content = "⬛ _Stop";
            }
            // If not running, set the button label to indicate starting
            else
            {
                button.Content = "▶ _Start";
            }
        }

        // Represents a point with integer X and Y coordinates.
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct TagPoint
        {
            /// <summary>
            /// Gets or sets the X coordinate of the point.
            /// </summary>
            public int X;

            /// <summary>
            /// Gets or sets the Y coordinate of the point.
            /// </summary>
            public int Y;
        }
    }
}