using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CursorCoordinateTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Imports the GetPhysicalCursorPos function from the user32.dll library.
        // If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.
        // To get extended error information, call Marshal.GetLastWin32Error.
        [LibraryImport("user32.dll")]
        private static partial IntPtr GetPhysicalCursorPos(out TagPoint lpPoint);

        // Imports the SetPhysicalCursorPos function from the user32.dll library.
        // If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.
        // To get extended error information, call Marshal.GetLastWin32Error.
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

        #region *** Start/Stop   ***
        /// <summary>
        /// Handles the Click event for the Start/Stop button.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void BtnStartStop_Click(object sender, RoutedEventArgs e)
        {
            StartStop((Button)sender);
        }

        /// <summary>
        /// Handles the AccessKeyPressed event for the Start/Stop button.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void BtnStartStop_AccessKeyPressed(object sender, AccessKeyPressedEventArgs e)
        {
            StartStop((Button)sender);
        }

        // Handles the click event for the Start/Stop button.
        private void StartStop(Button startStopButton)
        {
            // Toggle the running state
            _isRunning = !_isRunning;

            // Update the label on the button
            SetLabel(startStopButton);

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
        #endregion

        #region *** Set Position ***
        /// <summary>
        /// Handles the Click event for the Set Position button.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void BtnSetPosition_Click(object sender, RoutedEventArgs e)
        {
            SetPosition();
        }

        /// <summary>
        /// Handles the AccessKeyPressed event for the Set Position button.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void BtnSetPosition_AccessKeyPressed(object sender, AccessKeyPressedEventArgs e)
        {
            SetPosition();
        }

        // Handles the AccessKeyPressed event for the Set Position button.
        private void SetPosition()
        {
            // Use Dispatcher to update UI components
            Dispatcher.BeginInvoke(() =>
            {
                // Parse values from text boxes
                _ = int.TryParse(TxbAxisX.Text, out int x);
                _ = int.TryParse(TxbAxisY.Text, out int y);

                // Set the physical cursor position
                SetPhysicalCursorPos(x, y);
            });
        }
        #endregion

        #region *** Set Speed    ***
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
        #endregion

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