using System;
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;
using Tegam._1830A.DeviceLibrary.Models;
using Tegam.WinFormsUI.Controls;

namespace Tegam._1830A.Tests.Unit
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class LogDisplayControlTests
    {
        private LogDisplayControl _control;

        [SetUp]
        public void SetUp()
        {
            _control = new LogDisplayControl();
        }

        [TearDown]
        public void TearDown()
        {
            _control?.Dispose();
        }

        [Test]
        public void AddEntry_WithDataEntry_AddsItemWithBlueColor()
        {
            // Arrange
            var dataEntry = new DataEntry
            {
                Timestamp = DateTime.Now,
                Frequency = 50000000,
                Power = 15.42,
                SensorId = 1
            };

            // Act
            _control.AddEntry(dataEntry);

            // Assert
            var listView = GetListView();
            Assert.AreEqual(1, listView.Items.Count);
            Assert.AreEqual("Data", listView.Items[0].Text);
            Assert.AreEqual(System.Drawing.Color.Blue, listView.Items[0].ForeColor);
        }

        [Test]
        public void AddEntry_WithSettingEntry_AddsItemWithGreenColor()
        {
            // Arrange
            var settingEntry = new SettingEntry
            {
                Timestamp = DateTime.Now,
                SettingName = "Frequency",
                SettingValue = "50 MHz",
                Context = "User set via UI"
            };

            // Act
            _control.AddEntry(settingEntry);

            // Assert
            var listView = GetListView();
            Assert.AreEqual(1, listView.Items.Count);
            Assert.AreEqual("Setting", listView.Items[0].Text);
            Assert.AreEqual(System.Drawing.Color.Green, listView.Items[0].ForeColor);
        }

        [Test]
        public void AddEntry_FormatsTimestampCorrectly()
        {
            // Arrange
            var timestamp = new DateTime(2024, 1, 15, 10, 30, 45, 123);
            var dataEntry = new DataEntry
            {
                Timestamp = timestamp,
                Frequency = 50000000,
                Power = 15.42,
                SensorId = 1
            };

            // Act
            _control.AddEntry(dataEntry);

            // Assert
            var listView = GetListView();
            Assert.AreEqual("10:30:45.123", listView.Items[0].SubItems[1].Text);
        }

        [Test]
        public void AddEntry_FormatsDataEntryDetailsCorrectly()
        {
            // Arrange
            var dataEntry = new DataEntry
            {
                Timestamp = DateTime.Now,
                Frequency = 50000000,
                Power = 15.42,
                SensorId = 1
            };

            // Act
            _control.AddEntry(dataEntry);

            // Assert
            var listView = GetListView();
            Assert.AreEqual("Power: 15.42 dBm @ 50000000 Hz", listView.Items[0].SubItems[2].Text);
        }

        [Test]
        public void AddEntry_FormatsSettingEntryDetailsCorrectly()
        {
            // Arrange
            var settingEntry = new SettingEntry
            {
                Timestamp = DateTime.Now,
                SettingName = "Frequency",
                SettingValue = "50 MHz"
            };

            // Act
            _control.AddEntry(settingEntry);

            // Assert
            var listView = GetListView();
            Assert.AreEqual("Frequency: 50 MHz", listView.Items[0].SubItems[2].Text);
        }

        [Test]
        public void Clear_RemovesAllEntries()
        {
            // Arrange
            _control.AddEntry(new DataEntry { Timestamp = DateTime.Now, Frequency = 50000000, Power = 15.42, SensorId = 1 });
            _control.AddEntry(new DataEntry { Timestamp = DateTime.Now, Frequency = 60000000, Power = 16.50, SensorId = 1 });

            // Act
            _control.Clear();

            // Assert
            var listView = GetListView();
            Assert.AreEqual(0, listView.Items.Count);
        }

        [Test]
        public void SetMaxEntries_MaintainsCircularBuffer()
        {
            // Arrange
            _control.SetMaxEntries(3);

            // Act - Add 5 entries
            for (int i = 0; i < 5; i++)
            {
                _control.AddEntry(new DataEntry
                {
                    Timestamp = DateTime.Now,
                    Frequency = 50000000 + i,
                    Power = 15.0 + i,
                    SensorId = 1
                });
            }

            // Assert - Should only have 3 entries (the last 3)
            var listView = GetListView();
            Assert.AreEqual(3, listView.Items.Count);
            Assert.AreEqual("Power: 17.00 dBm @ 50000002 Hz", listView.Items[0].SubItems[2].Text);
            Assert.AreEqual("Power: 18.00 dBm @ 50000003 Hz", listView.Items[1].SubItems[2].Text);
            Assert.AreEqual("Power: 19.00 dBm @ 50000004 Hz", listView.Items[2].SubItems[2].Text);
        }

        [Test]
        public void SetMaxEntries_WithInvalidValue_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => _control.SetMaxEntries(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => _control.SetMaxEntries(-1));
        }

        [Test]
        public void AddEntry_WithNullEntry_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _control.AddEntry(null));
            
            var listView = GetListView();
            Assert.AreEqual(0, listView.Items.Count);
        }

        [Test]
        public void AddEntry_ExceedsDefaultMaxEntries_MaintainsCircularBuffer()
        {
            // Act - Add 105 entries (default max is 100)
            for (int i = 0; i < 105; i++)
            {
                _control.AddEntry(new DataEntry
                {
                    Timestamp = DateTime.Now,
                    Frequency = 50000000 + i,
                    Power = 15.0 + i,
                    SensorId = 1
                });
            }

            // Assert - Should only have 100 entries
            var listView = GetListView();
            Assert.AreEqual(100, listView.Items.Count);
            
            // First entry should be the 6th one added (index 5)
            Assert.AreEqual("Power: 20.00 dBm @ 50000005 Hz", listView.Items[0].SubItems[2].Text);
        }

        private ListView GetListView()
        {
            // Access the private ListView field using reflection
            var field = typeof(LogDisplayControl).GetField("_listView",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (ListView)field.GetValue(_control);
        }
    }
}
