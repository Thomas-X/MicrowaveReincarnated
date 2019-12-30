// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateEngine.cs" company="EPDME">
//   Copyright pending
// </copyright>
// <summary>
//   The state engine.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microwave
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Models;

    /// <summary>
    ///     The state engine.
    /// </summary>
    public static class StateEngine
    {
        #region Properties

        /// <summary>
        /// Gets or sets the current state.
        /// Private modifier changed to public for testing purposes.
        /// </summary>
        public static Enum CurrentState { get; private set; } = State.Closed;

        #endregion

        /// <summary>
        ///     The microwave engine.
        /// </summary>
        /// <param name="currInput">
        ///     The current state of the microwave input.
        /// </param>
        public static void MicrowaveEngine(MicrowaveData currInput)
        {
            switch (CurrentState)
            {
                case State.Closed:
                    if (currInput.IsOpen)
                    {
                        // If the door is open
                        CurrentState = State.Open;

                        RaiseOpenDoor();
                        RaiseShowMessage($"Door is open");
                    }
                    
                    else if (currInput.Time > 0)
                    {
                        CurrentState = State.Ready;
                        RaiseSetReady();
                        RaiseSetLightReady();
                    }
                    else if (currInput.IsStartClicked)
                    {
                        // If start is clicked and door is closed
                        RaiseShowMessage($"Can not start when the door is open");
                        currInput.IsStartClicked = false;
                    }

                    break;
                case State.Open:
                    if (currInput.IsOpen == false && currInput.Time > 0)
                    {
                        // If the door is closed and the timer has been set
                        CurrentState = State.Ready;

                        RaiseCloseDoor();
                        RaiseSetReady();
                        RaiseSetLightReady();
                    }
                    else if (!currInput.IsOpen)
                    {
                        // If door is closed
                        CurrentState = State.Closed;
                        RaiseCloseDoor();
                    }
                    else if (currInput.IsStartClicked)
                    {
                        // If start is pressed
                        RaiseShowMessage("I can't do that like this senpai!");
                        currInput.IsStartClicked = false;
                    }

                    break;
                case State.Cooking:
                    // Door closed and timer 0
                    if (currInput.Time <= 0)
                    {
                        CurrentState = State.Closed;

                        currInput.IsStartClicked = false;
                        RaiseSetLightInactive();
                        RaiseShowMessage("Your meal is ready");
                    }
                    else if (currInput.IsOpen && currInput.Time > 0)
                    {
                        // If the door is open and timer not 0
                        CurrentState = State.Open;

                        RaiseOpenDoor();
                        currInput.IsStartClicked = false;
                        RaiseSetLightInactive();
                        RaisePauseTimer();
                        RaiseShowMessage($"Close the door en press start to resume cooking");
                    }
                    else if (!currInput.IsStartClicked)
                    {
                        // If the start is clicked off
                        currInput.IsStartClicked = true;
                        RaiseShowMessage("Can not start without a time set");
                    }

                    break;
                case State.Ready:
                    if (currInput.IsOpen)
                    {
                        // If the door is open
                        RaiseShowMessage("The door is forcefully closed");
                        currInput.IsOpen = false;
                    }
                    else if (currInput.IsStartClicked)
                    {
                        // If start is pressed
                        CurrentState = State.Cooking;

                        RaiseStartTimer();
                        RaiseSetLightActive();
                        RaiseShowMessage($"The cooking started");
                    }
                    else if (currInput.Time <= 0)
                    {
                        CurrentState = State.Closed;

                        RaiseShowMessage($"The microwave stopped");
                        RaiseSetLightInactive();
                    }

                    break;
                default:
                    throw new Exception("ERROR: Not existing state");
            }
        }

        /// <summary>
        ///     Raise the open door event.
        /// </summary>
        private static void RaiseOpenDoor()
        {
            OpenDoor?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        ///     Raise the closed door event.
        /// </summary>
        private static void RaiseCloseDoor()
        {
            CloseDoor?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        ///     Raise the set ready event.
        /// </summary>
        private static void RaiseSetReady()
        {
            SetReady?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        ///     Raise the show message event.
        /// </summary>
        /// <param name="message">
        ///     The message to display.
        /// </param>
        private static void RaiseShowMessage(string message)
        {
            ShowMessage?.Invoke(null, message);
        }

        /// <summary>
        ///     Raise the start timer event.
        /// </summary>
        private static void RaiseStartTimer()
        {
            StartTimer?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        ///     Raise the pause timer event.
        /// </summary>
        private static void RaisePauseTimer()
        {
            PauseTimer?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        ///     Raise the set light inactive event.
        /// </summary>
        private static void RaiseSetLightInactive()
        {
            SetLightInactive?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        ///     Raise the set light ready event.
        /// </summary>
        private static void RaiseSetLightReady()
        {
            SetLightReady?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        ///     Raise the set light active event.
        /// </summary>
        private static void RaiseSetLightActive()
        {
            SetLightActive?.Invoke(null, EventArgs.Empty);
        }

        #region Attributes

        /// <summary>
        /// The state.
        /// Changed to public for testing purposes.
        /// </summary>
        public enum State
        {
            /// <summary>
            ///     The closed.
            /// </summary>
            Closed,

            /// <summary>
            ///     The open.
            /// </summary>
            Open,

            /// <summary>
            ///     The cooking.
            /// </summary>
            Cooking,

            /// <summary>
            ///     The ready.
            /// </summary>
            Ready
        }

        #endregion

        #region Events

        /// <summary>
        ///     The open the door event.
        ///     Open the door of the microwave.
        /// </summary>
        public static event EventHandler OpenDoor;

        /// <summary>
        ///     The close the door event.
        ///     Close the door of the microwave.
        /// </summary>
        public static event EventHandler CloseDoor;

        /// <summary>
        ///     The set light inactive event.
        ///     Set the light to the inactive color.
        /// </summary>
        public static event EventHandler SetLightInactive;

        /// <summary>
        ///     The set light ready event.
        ///     Set the light to the ready color.
        /// </summary>
        public static event EventHandler SetLightReady;

        /// <summary>
        ///     The set light active event.
        ///     Set the light to the active color.
        /// </summary>
        public static event EventHandler SetLightActive;

        /// <summary>
        ///     The set ready event.
        ///     Show that the microwave is ready.
        /// </summary>
        public static event EventHandler SetReady;

        /// <summary>
        ///     The start timer event.
        ///     Start the timer on the microwave.
        /// </summary>
        public static event EventHandler StartTimer;

        /// <summary>
        ///     The pause timer event.
        ///     Start the timer on the microwave.
        /// </summary>
        public static event EventHandler PauseTimer;

        /// <summary>
        ///     The show message event.
        ///     Show a message in the message box.
        /// </summary>
        public static event EventHandler<string> ShowMessage;

        #endregion
    }

    namespace Models
    {
        /// <inheritdoc />
        /// <summary>
        ///     The model for all the current input of the microwave.
        /// </summary>
        public class MicrowaveData : INotifyPropertyChanged
        {
            #region Attributes
            
            /// <summary>
            ///     The time.
            /// </summary>
            private int _time;
            
            /// <summary>
            ///     The is open clicked.
            /// </summary>
            private bool _isOpen;

            /// <summary>
            ///     The is start clicked.
            /// </summary>
            private bool _isStartClicked;

            #endregion

            #region Properties

            /// <summary>
            ///     Gets or sets the time.
            /// </summary>
            public int Time
            {
                get => _time;

                set
                {
                    if (_time == value) return;

                    _time = value;

                    // Send the event for this property
                    RaisePropertyChanged("Time");

                    // Send events for properties that depend on this property
                    RaisePropertyChanged("TimeWithMinutes");
                }
            }

            /// <summary>
            ///     Gets or sets a value indicating whether the door is opened.
            /// </summary>
            public bool IsOpen
            {
                get => _isOpen;
                set
                {
                    if (_isOpen == value) return;

                    _isOpen = value;

                    // Send the event for this property
                    RaisePropertyChanged("IsOpenClicked");

                    // Send events for properties that depend on this property
                    RaisePropertyChanged("OpenButtonText");
                }
            }

            /// <summary>
            ///     Gets or sets a value indicating whether the start is clicked.
            /// </summary>
            public bool IsStartClicked
            {
                get => _isStartClicked;
                set
                {
                    if (_isStartClicked == value) return;

                    _isStartClicked = value;

                    // Send the event for this property
                    RaisePropertyChanged("IsStartClicked");

                    // Send events for properties that depend on this property
                }
            }

            #endregion

            #region Events

            /// <summary>
            ///     the event that is send when a property is changed.
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            #endregion

            #region EventSenders

            /// <summary>
            ///     send the <see cref="PropertyChanged" /> event.
            /// </summary>
            /// <param name="propertyName">The property name.</param>
            protected virtual void RaisePropertyChanged(string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            #endregion
        }
    }
}