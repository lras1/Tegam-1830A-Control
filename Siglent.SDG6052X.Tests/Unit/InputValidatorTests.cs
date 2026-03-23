using NUnit.Framework;
using Siglent.SDG6052X.DeviceLibrary.Validation;
using Siglent.SDG6052X.DeviceLibrary.Models;
using System;

namespace Siglent.SDG6052X.Tests.Unit
{
    [TestFixture]
    public class InputValidatorTests
    {
        private InputValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new InputValidator();
        }

        #region ValidateFrequency Tests

        [Test]
        public void ValidateFrequency_SineWave_ValidFrequency_ReturnsValid()
        {
            // Act
            var result = _validator.ValidateFrequency(1000, WaveformType.Sine);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateFrequency_SineWave_MaxFrequency_ReturnsValid()
        {
            // Act
            var result = _validator.ValidateFrequency(500e6, WaveformType.Sine);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateFrequency_SineWave_ExceedsMax_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateFrequency(600e6, WaveformType.Sine);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("500 MHz"));
        }

        [Test]
        public void ValidateFrequency_SquareWave_ExceedsMax_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateFrequency(250e6, WaveformType.Square);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("200 MHz"));
        }

        [Test]
        public void ValidateFrequency_RampWave_ExceedsMax_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateFrequency(60e6, WaveformType.Ramp);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("50 MHz"));
        }

        [Test]
        public void ValidateFrequency_PulseWave_ExceedsMax_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateFrequency(150e6, WaveformType.Pulse);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("100 MHz"));
        }

        [Test]
        public void ValidateFrequency_BelowMinimum_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateFrequency(1e-7, WaveformType.Sine);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateFrequency_NaN_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateFrequency(double.NaN, WaveformType.Sine);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        #endregion

        #region ValidateAmplitude Tests

        [Test]
        public void ValidateAmplitude_50OhmLoad_ValidAmplitude_ReturnsValid()
        {
            // Arrange
            var load = LoadImpedance.FiftyOhm;

            // Act
            var result = _validator.ValidateAmplitude(5.0, load);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateAmplitude_50OhmLoad_MaxAmplitude_ReturnsValid()
        {
            // Arrange
            var load = LoadImpedance.FiftyOhm;

            // Act
            var result = _validator.ValidateAmplitude(20.0, load);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateAmplitude_50OhmLoad_ExceedsMax_ReturnsInvalid()
        {
            // Arrange
            var load = LoadImpedance.FiftyOhm;

            // Act
            var result = _validator.ValidateAmplitude(25.0, load);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("20"));
        }

        [Test]
        public void ValidateAmplitude_50OhmLoad_BelowMin_ReturnsInvalid()
        {
            // Arrange
            var load = LoadImpedance.FiftyOhm;

            // Act
            var result = _validator.ValidateAmplitude(0.0005, load);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateAmplitude_HighZLoad_ValidAmplitude_ReturnsValid()
        {
            // Arrange
            var load = LoadImpedance.HighZ;

            // Act
            var result = _validator.ValidateAmplitude(10.0, load);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateAmplitude_HighZLoad_MaxAmplitude_ReturnsValid()
        {
            // Arrange
            var load = LoadImpedance.HighZ;

            // Act
            var result = _validator.ValidateAmplitude(40.0, load);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateAmplitude_HighZLoad_ExceedsMax_ReturnsInvalid()
        {
            // Arrange
            var load = LoadImpedance.HighZ;

            // Act
            var result = _validator.ValidateAmplitude(45.0, load);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateAmplitude_NullLoad_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateAmplitude(5.0, null);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateAmplitude_NegativeAmplitude_ReturnsInvalid()
        {
            // Arrange
            var load = LoadImpedance.FiftyOhm;

            // Act
            var result = _validator.ValidateAmplitude(-5.0, load);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        #endregion

        #region ValidateOffset Tests

        [Test]
        public void ValidateOffset_50OhmLoad_ValidOffset_ReturnsValid()
        {
            // Arrange
            var load = LoadImpedance.FiftyOhm;

            // Act
            var result = _validator.ValidateOffset(2.0, 5.0, load);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateOffset_50OhmLoad_MaxOffset_ReturnsValid()
        {
            // Arrange
            var load = LoadImpedance.FiftyOhm;

            // Act
            var result = _validator.ValidateOffset(10.0, 0.0, load);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateOffset_50OhmLoad_ExceedsMax_ReturnsInvalid()
        {
            // Arrange
            var load = LoadImpedance.FiftyOhm;

            // Act
            var result = _validator.ValidateOffset(12.0, 0.0, load);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateOffset_50OhmLoad_BelowMin_ReturnsInvalid()
        {
            // Arrange
            var load = LoadImpedance.FiftyOhm;

            // Act
            var result = _validator.ValidateOffset(-12.0, 0.0, load);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateOffset_50OhmLoad_CombinedExceedsMax_ReturnsInvalid()
        {
            // Arrange
            var load = LoadImpedance.FiftyOhm;

            // Act - offset 8V + amplitude/2 (5V) = 13V > 10V max
            var result = _validator.ValidateOffset(8.0, 10.0, load);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Combined"));
        }

        [Test]
        public void ValidateOffset_HighZLoad_ValidOffset_ReturnsValid()
        {
            // Arrange
            var load = LoadImpedance.HighZ;

            // Act
            var result = _validator.ValidateOffset(5.0, 10.0, load);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateOffset_HighZLoad_MaxOffset_ReturnsValid()
        {
            // Arrange
            var load = LoadImpedance.HighZ;

            // Act
            var result = _validator.ValidateOffset(20.0, 0.0, load);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        #endregion

        #region ValidatePhase Tests

        [Test]
        public void ValidatePhase_ValidPhase_ReturnsValid()
        {
            // Act
            var result = _validator.ValidatePhase(180.0);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidatePhase_MinPhase_ReturnsValid()
        {
            // Act
            var result = _validator.ValidatePhase(0.0);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidatePhase_MaxPhase_ReturnsValid()
        {
            // Act
            var result = _validator.ValidatePhase(360.0);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidatePhase_BelowMin_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidatePhase(-10.0);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidatePhase_AboveMax_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidatePhase(370.0);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        #endregion

        #region ValidateDutyCycle Tests

        [Test]
        public void ValidateDutyCycle_ValidDutyCycle_ReturnsValid()
        {
            // Act
            var result = _validator.ValidateDutyCycle(50.0);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateDutyCycle_MinDutyCycle_ReturnsValid()
        {
            // Act
            var result = _validator.ValidateDutyCycle(0.01);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateDutyCycle_MaxDutyCycle_ReturnsValid()
        {
            // Act
            var result = _validator.ValidateDutyCycle(99.99);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateDutyCycle_BelowMin_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateDutyCycle(0.005);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateDutyCycle_AboveMax_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateDutyCycle(100.0);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        #endregion

        #region ValidateModulationDepth Tests

        [Test]
        public void ValidateModulationDepth_AM_ValidDepth_ReturnsValid()
        {
            // Act
            var result = _validator.ValidateModulationDepth(50.0, ModulationType.AM);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateModulationDepth_AM_MaxDepth_ReturnsValid()
        {
            // Act
            var result = _validator.ValidateModulationDepth(120.0, ModulationType.AM);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateModulationDepth_AM_ExceedsMax_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateModulationDepth(130.0, ModulationType.AM);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateModulationDepth_PWM_ValidDepth_ReturnsValid()
        {
            // Act
            var result = _validator.ValidateModulationDepth(50.0, ModulationType.PWM);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateModulationDepth_PWM_ExceedsMax_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateModulationDepth(100.0, ModulationType.PWM);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateModulationDepth_NegativeDepth_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateModulationDepth(-10.0, ModulationType.AM);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        #endregion

        #region ValidateModulationFrequency Tests

        [Test]
        public void ValidateModulationFrequency_ValidFrequency_ReturnsValid()
        {
            // Act
            var result = _validator.ValidateModulationFrequency(100.0, ModulationType.AM);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateModulationFrequency_MaxFrequency_ReturnsValid()
        {
            // Act
            var result = _validator.ValidateModulationFrequency(1e6, ModulationType.AM);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateModulationFrequency_ExceedsMax_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateModulationFrequency(2e6, ModulationType.AM);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateModulationFrequency_BelowMin_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateModulationFrequency(0.0001, ModulationType.AM);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        #endregion

        #region ValidateDeviation Tests

        [Test]
        public void ValidateDeviation_FM_ValidDeviation_ReturnsValid()
        {
            // Act
            var result = _validator.ValidateDeviation(1000.0, ModulationType.FM);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateDeviation_PM_ValidDeviation_ReturnsValid()
        {
            // Act
            var result = _validator.ValidateDeviation(90.0, ModulationType.PM);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateDeviation_PM_ExceedsMax_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateDeviation(400.0, ModulationType.PM);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateDeviation_NegativeDeviation_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateDeviation(-100.0, ModulationType.FM);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        #endregion

        #region ValidateSweepRange Tests

        [Test]
        public void ValidateSweepRange_ValidRange_ReturnsValid()
        {
            // Act
            var result = _validator.ValidateSweepRange(100.0, 10000.0);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateSweepRange_StartEqualsStop_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateSweepRange(1000.0, 1000.0);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateSweepRange_StartGreaterThanStop_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateSweepRange(10000.0, 1000.0);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateSweepRange_StopExceedsMax_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateSweepRange(100.0, 600e6);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        #endregion

        #region ValidateSweepTime Tests

        [Test]
        public void ValidateSweepTime_ValidTime_ReturnsValid()
        {
            // Act
            var result = _validator.ValidateSweepTime(1.0);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateSweepTime_MinTime_ReturnsValid()
        {
            // Act
            var result = _validator.ValidateSweepTime(0.001);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateSweepTime_MaxTime_ReturnsValid()
        {
            // Act
            var result = _validator.ValidateSweepTime(500.0);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateSweepTime_BelowMin_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateSweepTime(0.0005);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateSweepTime_AboveMax_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateSweepTime(600.0);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        #endregion

        #region ValidateBurstCycles Tests

        [Test]
        public void ValidateBurstCycles_ValidCycles_ReturnsValid()
        {
            // Act
            var result = _validator.ValidateBurstCycles(10);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateBurstCycles_MinCycles_ReturnsValid()
        {
            // Act
            var result = _validator.ValidateBurstCycles(1);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateBurstCycles_MaxCycles_ReturnsValid()
        {
            // Act
            var result = _validator.ValidateBurstCycles(1000000);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateBurstCycles_BelowMin_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateBurstCycles(0);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateBurstCycles_AboveMax_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateBurstCycles(2000000);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        #endregion

        #region ValidateBurstPeriod Tests

        [Test]
        public void ValidateBurstPeriod_ValidPeriod_ReturnsValid()
        {
            // Act
            var result = _validator.ValidateBurstPeriod(0.001);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateBurstPeriod_ZeroPeriod_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateBurstPeriod(0.0);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateBurstPeriod_NegativePeriod_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateBurstPeriod(-0.001);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        #endregion

        #region ValidateArbitraryWaveformPoints Tests

        [Test]
        public void ValidateArbitraryWaveformPoints_ValidPoints_ReturnsValid()
        {
            // Arrange
            var points = new double[] { 0.0, 0.5, 1.0, 0.5, 0.0 };

            // Act
            var result = _validator.ValidateArbitraryWaveformPoints(points);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateArbitraryWaveformPoints_MinPoints_ReturnsValid()
        {
            // Arrange
            var points = new double[] { 0.0, 1.0 };

            // Act
            var result = _validator.ValidateArbitraryWaveformPoints(points);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateArbitraryWaveformPoints_TooFewPoints_ReturnsInvalid()
        {
            // Arrange
            var points = new double[] { 0.0 };

            // Act
            var result = _validator.ValidateArbitraryWaveformPoints(points);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateArbitraryWaveformPoints_NullPoints_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateArbitraryWaveformPoints(null);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateArbitraryWaveformPoints_ContainsNaN_ReturnsInvalid()
        {
            // Arrange
            var points = new double[] { 0.0, double.NaN, 1.0 };

            // Act
            var result = _validator.ValidateArbitraryWaveformPoints(points);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        #endregion

        #region ValidateWaveformName Tests

        [Test]
        public void ValidateWaveformName_ValidName_ReturnsValid()
        {
            // Act
            var result = _validator.ValidateWaveformName("MyWaveform");

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateWaveformName_WithUnderscore_ReturnsValid()
        {
            // Act
            var result = _validator.ValidateWaveformName("My_Waveform_1");

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateWaveformName_EmptyName_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateWaveformName("");

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateWaveformName_NullName_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateWaveformName(null);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateWaveformName_TooLong_ReturnsInvalid()
        {
            // Arrange
            var name = "ThisNameIsWayTooLongForTheDevice";

            // Act
            var result = _validator.ValidateWaveformName(name);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateWaveformName_StartsWithDigit_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateWaveformName("1Waveform");

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateWaveformName_ContainsInvalidChars_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateWaveformName("My-Waveform");

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        #endregion
    }
}
