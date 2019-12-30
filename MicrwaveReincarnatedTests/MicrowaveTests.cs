using AutoFixture.Xunit2;
using System;
using System.Collections.Generic;
using Fare;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Microwave;
using Xunit;
using Microwave.Models;

namespace MicrwaveReincarnatedTests
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
        
        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void Should_Handle_IsOpen_Correctly()
        {
            var microwaveData = new MicrowaveData
            {
                IsOpen = true
            };
            var timesCalled = 0;

            StateEngine.OpenDoor += (sender, args) =>
            {
                timesCalled++;
            };
            StateEngine.ShowMessage += (sender, args) =>
            {
                if (args == _messages[EventMessages.Should_Handle_Door_Open]) timesCalled++;
            };

            StateEngine.MicrowaveEngine(microwaveData);

            Assert.Equal(2, timesCalled);
        }
        
        [Fact]
        public void Should_Handle_Closed_Door_Higher_Time_Than_0()
        {
            var microwaveData = new MicrowaveData
            {
                IsOpen = false,
                Time = 1337
            };
            var timesCalled = 0;

            StateEngine.SetReady += (sender, args) =>
            {
                timesCalled++;
            };
            StateEngine.SetLightReady += (sender, args) =>
            {
                timesCalled++;
            };

            StateEngine.MicrowaveEngine(microwaveData);

            Assert.Equal(2, timesCalled);
            Assert.Equal(StateEngine.State.Ready, StateEngine.CurrentState);
        }
        
        [Fact]
        public void Should_Not_Start_When_Door_Is_Open()
        {
            var microwaveData = new MicrowaveData
            {
                IsOpen = false,
                Time = 0,
                IsStartClicked = true
            };
            var timesCalled = 0;

            StateEngine.ShowMessage += (sender, args) =>
            {
                if (args == _messages[EventMessages.Should_Not_Start_When_Door_Is_Open]) timesCalled++;
            };

            StateEngine.MicrowaveEngine(microwaveData);

            Assert.Equal(1, timesCalled);
        }
        
    }
}