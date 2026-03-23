using NUnit.Framework;
using Siglent.SDG6052X.DeviceLibrary.Commands;
using Siglent.SDG6052X.DeviceLibrary.Models;
using System;

namespace Siglent.SDG6052X.Tests.Unit
{
    [TestFixture]
    public class ScpiCommandBuilderTests
    {
        private ScpiCommandBuilder _builder;

        [SetUp]
        public void SetUp()
        {
            _builder = new ScpiCommandBuilder();
        }

        #region BuildBasicWaveCommand Tests

        [Test]
        public void BuildBasicWaveCommand_SineWave_BuildsCorrectCommand()
        {
            // Arrange
            var parameters = new WaveformParameters
            {
                Frequency = 1000,
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };

            // Act
            var command = _builder.BuildBasicWaveCommand(1, WaveformType.Sine, parameters);

            // Assert
            Assert.That(command, Does.StartWith("C1:BSWV "));
            Assert.That(command, Does.Contain("WVTP,SINE"));
            Assert.That(command, Does.Contain("FRQ,1"));
            Assert.That(command, Does.Contain("AMP,5VPP"));
            Assert.That(command, Does.Contain("OFST,0V"));
            Assert.That(command, Does.Contain("PHSE,0"));
        }

        [Test]
        public void BuildBasicWaveCommand_SquareWave_IncludesDutyCycle()
        {
            // Arrange
            var parameters = new WaveformParameters
            {
                Frequency = 1000,
                Amplitude = 3.3,
                Offset = 0.0,
                Phase = 0.0,
                DutyCycle = 50.0,
                Unit = AmplitudeUnit.Vpp
            };

            // Act
            var command = _builder.BuildBasicWaveCommand(1, WaveformType.Square, parameters);

            // Assert
            Assert.That(command, Does.Contain("WVTP,SQUARE"));
            Assert.That(command, Does.Contain("DUTY,50"));
        }

        [Test]
        public void BuildBasicWaveCommand_PulseWave_IncludesAllPulseParameters()
        {
            // Arrange
            var parameters = new WaveformParameters
            {
                Frequency = 1000,
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                DutyCycle = 25.0,
                Width = 0.00025,
                Rise = 0.00001,
                Fall = 0.00001,
                Unit = AmplitudeUnit.Vpp
            };

            // Act
            var command = _builder.BuildBasicWaveCommand(1, WaveformType.Pulse, parameters);

            // Assert
            Assert.That(command, Does.Contain("WVTP,PULSE"));
            Assert.That(command, Does.Contain("DUTY,25"));
            Assert.That(command, Does.Contain("WIDTH,"));
            Assert.That(command, Does.Contain("RISE,"));
            Assert.That(command, Does.Contain("FALL,"));
        }

        [Test]
        public void BuildBasicWaveCommand_FrequencyInKHz_UsesKHZUnit()
        {
            // Arrange
            var parameters = new WaveformParameters
            {
                Frequency = 10000, // 10 kHz
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };

            // Act
            var command = _builder.BuildBasicWaveCommand(1, WaveformType.Sine, parameters);

            // Assert
            Assert.That(command, Does.Contain("FRQ,10KHZ"));
        }

        [Test]
        public void BuildBasicWaveCommand_FrequencyInMHz_UsesMHZUnit()
        {
            // Arrange
            var parameters = new WaveformParameters
            {
                Frequency = 1000000, // 1 MHz
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };

            // Act
            var command = _builder.BuildBasicWaveCommand(1, WaveformType.Sine, parameters);

            // Assert
            Assert.That(command, Does.Contain("FRQ,1MHZ"));
        }

        [Test]
        public void BuildBasicWaveCommand_VrmsUnit_UsesVRMS()
        {
            // Arrange
            var parameters = new WaveformParameters
            {
                Frequency = 1000,
                Amplitude = 3.5,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vrms
            };

            // Act
            var command = _builder.BuildBasicWaveCommand(1, WaveformType.Sine, parameters);

            // Assert
            Assert.That(command, Does.Contain("AMP,3.5VRMS"));
        }

        [Test]
        public void BuildBasicWaveCommand_dBmUnit_UsesDBM()
        {
            // Arrange
            var parameters = new WaveformParameters
            {
                Frequency = 1000,
                Amplitude = 10.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.dBm
            };

            // Act
            var command = _builder.BuildBasicWaveCommand(1, WaveformType.Sine, parameters);

            // Assert
            Assert.That(command, Does.Contain("AMP,10DBM"));
        }

        [Test]
        public void BuildBasicWaveCommand_Channel2_UsesC2Prefix()
        {
            // Arrange
            var parameters = new WaveformParameters
            {
                Frequency = 1000,
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };

            // Act
            var command = _builder.BuildBasicWaveCommand(2, WaveformType.Sine, parameters);

            // Assert
            Assert.That(command, Does.StartWith("C2:BSWV "));
        }

        [Test]
        public void BuildBasicWaveCommand_NullParameters_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _builder.BuildBasicWaveCommand(1, WaveformType.Sine, null));
        }

        #endregion

        #region BuildOutputStateCommand Tests

        [Test]
        public void BuildOutputStateCommand_EnableOutput_ReturnsONCommand()
        {
            // Act
            var command = _builder.BuildOutputStateCommand(1, true);

            // Assert
            Assert.That(command, Is.EqualTo("C1:OUTP ON"));
        }

        [Test]
        public void BuildOutputStateCommand_DisableOutput_ReturnsOFFCommand()
        {
            // Act
            var command = _builder.BuildOutputStateCommand(1, false);

            // Assert
            Assert.That(command, Is.EqualTo("C1:OUTP OFF"));
        }

        [Test]
        public void BuildOutputStateCommand_Channel2_UsesC2Prefix()
        {
            // Act
            var command = _builder.BuildOutputStateCommand(2, true);

            // Assert
            Assert.That(command, Is.EqualTo("C2:OUTP ON"));
        }

        #endregion

        #region BuildLoadCommand Tests

        [Test]
        public void BuildLoadCommand_HighZ_ReturnsHZValue()
        {
            // Arrange
            var load = LoadImpedance.HighZ;

            // Act
            var command = _builder.BuildLoadCommand(1, load);

            // Assert
            Assert.That(command, Is.EqualTo("C1:OUTP LOAD,HZ"));
        }

        [Test]
        public void BuildLoadCommand_FiftyOhm_Returns50Value()
        {
            // Arrange
            var load = LoadImpedance.FiftyOhm;

            // Act
            var command = _builder.BuildLoadCommand(1, load);

            // Assert
            Assert.That(command, Is.EqualTo("C1:OUTP LOAD,50"));
        }

        [Test]
        public void BuildLoadCommand_Custom75Ohm_Returns75Value()
        {
            // Arrange
            var load = LoadImpedance.Custom(75);

            // Act
            var command = _builder.BuildLoadCommand(1, load);

            // Assert
            Assert.That(command, Is.EqualTo("C1:OUTP LOAD,75"));
        }

        [Test]
        public void BuildLoadCommand_NullLoad_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                _builder.BuildLoadCommand(1, null));
        }

        #endregion

        #region BuildModulationCommand Tests

        [Test]
        public void BuildModulationCommand_AM_BuildsCorrectCommand()
        {
            // Arrange
            var parameters = new ModulationParameters
            {
                Source = ModulationSource.Internal,
                Depth = 50.0,
                Rate = 100.0
            };

            // Act
            var command = _builder.BuildModulationCommand(1, ModulationType.AM, parameters);

            // Assert
            Assert.That(command, Does.StartWith("C1:MDWV AM"));
            Assert.That(command, Does.Contain("SRC,INT"));
            Assert.That(command, Does.Contain("DEPTH,50"));
            Assert.That(command, Does.Contain("FRQ,100HZ"));
        }

        [Test]
        public void BuildModulationCommand_FM_BuildsCorrectCommand()
        {
            // Arrange
            var parameters = new ModulationParameters
            {
                Source = ModulationSource.External,
                Deviation = 1000.0,
                Rate = 100.0
            };

            // Act
            var command = _builder.BuildModulationCommand(1, ModulationType.FM, parameters);

            // Assert
            Assert.That(command, Does.StartWith("C1:MDWV FM"));
            Assert.That(command, Does.Contain("SRC,EXT"));
            Assert.That(command, Does.Contain("DEVI,1KHZ"));
            Assert.That(command, Does.Contain("FRQ,100HZ"));
        }

        [Test]
        public void BuildModulationCommand_FSK_IncludesHopFrequency()
        {
            // Arrange
            var parameters = new ModulationParameters
            {
                Source = ModulationSource.Internal,
                HopFrequency = 2000.0,
                Rate = 100.0
            };

            // Act
            var command = _builder.BuildModulationCommand(1, ModulationType.FSK, parameters);

            // Assert
            Assert.That(command, Does.Contain("HOP_FRQ,2KHZ"));
        }

        #endregion

        #region BuildModulationStateCommand Tests

        [Test]
        public void BuildModulationStateCommand_Enable_ReturnsONCommand()
        {
            // Act
            var command = _builder.BuildModulationStateCommand(1, true);

            // Assert
            Assert.That(command, Is.EqualTo("C1:MDWV STATE,ON"));
        }

        [Test]
        public void BuildModulationStateCommand_Disable_ReturnsOFFCommand()
        {
            // Act
            var command = _builder.BuildModulationStateCommand(1, false);

            // Assert
            Assert.That(command, Is.EqualTo("C1:MDWV STATE,OFF"));
        }

        #endregion

        #region BuildSweepCommand Tests

        [Test]
        public void BuildSweepCommand_LinearSweep_BuildsCorrectCommand()
        {
            // Arrange
            var parameters = new SweepParameters
            {
                Type = SweepType.Linear,
                StartFrequency = 100.0,
                StopFrequency = 10000.0,
                Time = 1.0,
                Direction = SweepDirection.Up,
                TriggerSource = TriggerSource.Internal,
                ReturnTime = 0.1,
                HoldTime = 0.0
            };

            // Act
            var command = _builder.BuildSweepCommand(1, parameters);

            // Assert
            Assert.That(command, Does.StartWith("C1:SWV "));
            Assert.That(command, Does.Contain("TYPE,LINE"));
            Assert.That(command, Does.Contain("START,100HZ"));
            Assert.That(command, Does.Contain("STOP,10KHZ"));
            Assert.That(command, Does.Contain("TIME,1"));
            Assert.That(command, Does.Contain("DIR,UP"));
            Assert.That(command, Does.Contain("TRSR,INT"));
        }

        [Test]
        public void BuildSweepCommand_LogarithmicSweep_UsesLOGType()
        {
            // Arrange
            var parameters = new SweepParameters
            {
                Type = SweepType.Logarithmic,
                StartFrequency = 100.0,
                StopFrequency = 10000.0,
                Time = 1.0,
                Direction = SweepDirection.Up,
                TriggerSource = TriggerSource.Internal,
                ReturnTime = 0.1,
                HoldTime = 0.0
            };

            // Act
            var command = _builder.BuildSweepCommand(1, parameters);

            // Assert
            Assert.That(command, Does.Contain("TYPE,LOG"));
        }

        #endregion

        #region BuildSweepStateCommand Tests

        [Test]
        public void BuildSweepStateCommand_Enable_ReturnsONCommand()
        {
            // Act
            var command = _builder.BuildSweepStateCommand(1, true);

            // Assert
            Assert.That(command, Is.EqualTo("C1:SWV STATE,ON"));
        }

        [Test]
        public void BuildSweepStateCommand_Disable_ReturnsOFFCommand()
        {
            // Act
            var command = _builder.BuildSweepStateCommand(1, false);

            // Assert
            Assert.That(command, Is.EqualTo("C1:SWV STATE,OFF"));
        }

        #endregion

        #region BuildBurstCommand Tests

        [Test]
        public void BuildBurstCommand_NCycleBurst_BuildsCorrectCommand()
        {
            // Arrange
            var parameters = new BurstParameters
            {
                Mode = BurstMode.NCycle,
                Cycles = 10,
                Period = 0.001,
                TriggerSource = TriggerSource.Internal,
                TriggerEdge = TriggerEdge.Rising,
                StartPhase = 0.0
            };

            // Act
            var command = _builder.BuildBurstCommand(1, parameters);

            // Assert
            Assert.That(command, Does.StartWith("C1:BTWV "));
            Assert.That(command, Does.Contain("STATE,NCYC"));
            Assert.That(command, Does.Contain("TRSR,INT"));
            Assert.That(command, Does.Contain("TIME,10"));
            Assert.That(command, Does.Contain("PRD,0.001"));
            Assert.That(command, Does.Contain("EDGE,RISE"));
        }

        [Test]
        public void BuildBurstCommand_GatedBurst_BuildsCorrectCommand()
        {
            // Arrange
            var parameters = new BurstParameters
            {
                Mode = BurstMode.Gated,
                Cycles = 5,
                TriggerSource = TriggerSource.External,
                GatePolarity = GatePolarity.Positive,
                StartPhase = 0.0
            };

            // Act
            var command = _builder.BuildBurstCommand(1, parameters);

            // Assert
            Assert.That(command, Does.Contain("STATE,GATE"));
            Assert.That(command, Does.Contain("GATE_NCYC,5"));
            Assert.That(command, Does.Contain("PLRT,POS"));
        }

        #endregion

        #region BuildBurstStateCommand Tests

        [Test]
        public void BuildBurstStateCommand_Enable_ReturnsONCommand()
        {
            // Act
            var command = _builder.BuildBurstStateCommand(1, true);

            // Assert
            Assert.That(command, Is.EqualTo("C1:BTWV STATE,ON"));
        }

        [Test]
        public void BuildBurstStateCommand_Disable_ReturnsOFFCommand()
        {
            // Act
            var command = _builder.BuildBurstStateCommand(1, false);

            // Assert
            Assert.That(command, Is.EqualTo("C1:BTWV STATE,OFF"));
        }

        #endregion

        #region BuildArbitraryWaveCommand Tests

        [Test]
        public void BuildArbitraryWaveCommand_ValidName_BuildsCorrectCommand()
        {
            // Act
            var command = _builder.BuildArbitraryWaveCommand(1, "MyWaveform");

            // Assert
            Assert.That(command, Is.EqualTo("C1:ARWV NAME,MyWaveform"));
        }

        [Test]
        public void BuildArbitraryWaveCommand_NullName_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _builder.BuildArbitraryWaveCommand(1, null));
        }

        [Test]
        public void BuildArbitraryWaveCommand_EmptyName_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _builder.BuildArbitraryWaveCommand(1, ""));
        }

        #endregion

        #region BuildStoreArbitraryWaveCommand Tests

        [Test]
        public void BuildStoreArbitraryWaveCommand_ValidData_BuildsCorrectCommand()
        {
            // Arrange
            var points = new double[] { 0.0, 0.5, 1.0, 0.5, 0.0 };

            // Act
            var command = _builder.BuildStoreArbitraryWaveCommand("TestWave", points);

            // Assert
            Assert.That(command, Does.StartWith("STL TestWave,"));
            Assert.That(command, Does.Contain("5,"));
            Assert.That(command, Does.Contain("0"));
            Assert.That(command, Does.Contain("0.5"));
            Assert.That(command, Does.Contain("1"));
        }

        [Test]
        public void BuildStoreArbitraryWaveCommand_NullPoints_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _builder.BuildStoreArbitraryWaveCommand("TestWave", null));
        }

        [Test]
        public void BuildStoreArbitraryWaveCommand_EmptyPoints_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _builder.BuildStoreArbitraryWaveCommand("TestWave", new double[0]));
        }

        #endregion

        #region BuildQueryCommand Tests

        [Test]
        public void BuildQueryCommand_BasicWaveform_ReturnsCorrectQuery()
        {
            // Act
            var command = _builder.BuildQueryCommand(1, QueryType.BasicWaveform);

            // Assert
            Assert.That(command, Is.EqualTo("C1:BSWV?"));
        }

        [Test]
        public void BuildQueryCommand_OutputState_ReturnsCorrectQuery()
        {
            // Act
            var command = _builder.BuildQueryCommand(1, QueryType.OutputState);

            // Assert
            Assert.That(command, Is.EqualTo("C1:OUTP?"));
        }

        [Test]
        public void BuildQueryCommand_Load_ReturnsCorrectQuery()
        {
            // Act
            var command = _builder.BuildQueryCommand(1, QueryType.Load);

            // Assert
            Assert.That(command, Is.EqualTo("C1:OUTP? LOAD"));
        }

        #endregion

        #region BuildSystemCommand Tests

        [Test]
        public void BuildSystemCommand_Identity_ReturnsIDNQuery()
        {
            // Act
            var command = _builder.BuildSystemCommand(SystemCommandType.Identity);

            // Assert
            Assert.That(command, Is.EqualTo("*IDN?"));
        }

        [Test]
        public void BuildSystemCommand_Reset_ReturnsRSTCommand()
        {
            // Act
            var command = _builder.BuildSystemCommand(SystemCommandType.Reset);

            // Assert
            Assert.That(command, Is.EqualTo("*RST"));
        }

        [Test]
        public void BuildSystemCommand_Error_ReturnsErrorQuery()
        {
            // Act
            var command = _builder.BuildSystemCommand(SystemCommandType.Error);

            // Assert
            Assert.That(command, Is.EqualTo("SYST:ERR?"));
        }

        [Test]
        public void BuildSystemCommand_RecallSetup_ReturnsRCLCommand()
        {
            // Act
            var command = _builder.BuildSystemCommand(SystemCommandType.RecallSetup, 1);

            // Assert
            Assert.That(command, Is.EqualTo("*RCL 1"));
        }

        [Test]
        public void BuildSystemCommand_SaveSetup_ReturnsSAVCommand()
        {
            // Act
            var command = _builder.BuildSystemCommand(SystemCommandType.SaveSetup, 2);

            // Assert
            Assert.That(command, Is.EqualTo("*SAV 2"));
        }

        #endregion
    }
}
