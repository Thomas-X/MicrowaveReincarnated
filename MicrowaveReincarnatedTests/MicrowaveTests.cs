using AutoFixture.Xunit2;
using System;
using System.Collections.Generic;
using Fare;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Microwave;
using Xunit;
using Microwave.Models;
using Moq;

namespace MicrowaveReincarnatedTests
{
    /// <summary>
    /// Holds all tests for the StateEngine of the Microwave.
    /// </summary>
    public class MicrowaveTests
    {
        private enum EventMessages
        {
            Should_Handle_Door_Open,
            Should_Not_Start_When_Door_Is_Open
        }

        /// <summary>
        /// Data structure for cleanly checking messages returned from the StateEngine.
        /// </summary>
        private Dictionary<EventMessages, string> _messages = new Dictionary<EventMessages, string>()
        {
            {EventMessages.Should_Handle_Door_Open, "Door is open"},
            {EventMessages.Should_Not_Start_When_Door_Is_Open, "Can not start when the door is open"}
        };

        private Mock<IStateEngineWrapper> _stateEngineWrapperMock;

        public MicrowaveTests()
        {
            _stateEngineWrapperMock = new Mock<IStateEngineWrapper>();
        }

        /// <summary>
        /// Checks if the message and right events are triggered 
        /// </summary>
        [Fact]
        public void Should_Handle_IsOpen_Correctly()
        {
            // Arrange
            var timesCalled = 0;

            _stateEngineWrapperMock.Setup(m => m.ShowMessage(It.IsAny<object>(), It.IsAny<string>()))
                .Callback<object, string>((sender, args) =>
                {
                    if (args == _messages[EventMessages.Should_Handle_Door_Open]) timesCalled++;
                });
            _stateEngineWrapperMock.Setup(m => m.OpenDoor(It.IsAny<object>(), It.IsAny<object>()))
                .Callback<object, object>((sender, args) => { timesCalled++; });

            var microwaveData = new MicrowaveData
            {
                IsOpen = true
            };

            StateEngine.OpenDoor += _stateEngineWrapperMock.Object.OpenDoor;
            StateEngine.ShowMessage += _stateEngineWrapperMock.Object.ShowMessage;

            // Act
            StateEngine.MicrowaveEngine(microwaveData);

            // Assert
            Assert.Equal(2, timesCalled);
        }

        /// <summary>
        /// Checks if the message and right events are triggered, but also checks if the current state of the state engine is correct.
        /// </summary>
        [Fact]
        public void Should_Handle_Closed_Door_Higher_Time_Than_0()
        {
            // Arrange
            var microwaveData = new MicrowaveData
            {
                IsOpen = false,
                Time = 1337
            };
            var timesCalled = 0;

            _stateEngineWrapperMock.Setup(m => m.SetReady(It.IsAny<object>(), It.IsAny<object>()))
                .Callback<object, object>((sender, args) => { timesCalled++; });
            _stateEngineWrapperMock.Setup(m => m.SetLightReady(It.IsAny<object>(), It.IsAny<object>()))
                .Callback<object, object>((sender, args) => { timesCalled++; });

            StateEngine.SetReady += _stateEngineWrapperMock.Object.SetReady;
            StateEngine.SetLightReady += _stateEngineWrapperMock.Object.SetLightReady;
            
            // Act
            StateEngine.MicrowaveEngine(microwaveData);

            // Assert
            Assert.Equal(2, timesCalled);
            Assert.Equal(StateEngine.State.Ready, StateEngine.CurrentState);
        }

        /// <summary>
        /// Checks if the message and right events are triggered 
        /// </summary>
        [Fact]
        public void Should_Not_Start_When_Door_Is_Open()
        {
            // Arrange
            var microwaveData = new MicrowaveData
            {
                IsOpen = false,
                Time = 0,
                IsStartClicked = true
            };
            var timesCalled = 0;

            _stateEngineWrapperMock.Setup(m => m.ShowMessage(It.IsAny<object>(), It.IsAny<string>()))
                .Callback<object, string>((sender, args) =>
                {
                    if (args == _messages[EventMessages.Should_Not_Start_When_Door_Is_Open]) timesCalled++;
                });

            StateEngine.ShowMessage += _stateEngineWrapperMock.Object.ShowMessage;
            
            // Act
            StateEngine.MicrowaveEngine(microwaveData);

            // Assert
            Assert.Equal(1, timesCalled);
        }
    }
}