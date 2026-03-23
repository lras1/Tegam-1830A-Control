# Implementation Tasks

## Phase 1: Device Library Project Setup

- [x] 1. Create Device Library Project
  - [x] 1.1 Create new Class Library project targeting .NET Framework 4.0
  - [x] 1.2 Name project "Siglent.SDG6052X.DeviceLibrary"
  - [x] 1.3 Add NuGet package: NationalInstruments.Visa (version 20.0.0 or compatible with .NET 4.0)
  - [x] 1.4 Create folder structure: Communication/, Commands/, Parsing/, Services/, Validation/, Models/, Simulation/

## Phase 2: Data Models and Enums

- [-] 2. Implement Core Data Models
  - [ ] 2.1 Create WaveformParameters class with all properties (Frequency, Amplitude, Offset, Phase, DutyCycle, Width, Rise, Fall, Delay, Unit)
  - [ ] 2.2 Create WaveformType enum (Sine, Square, Ramp, Pulse, Noise, Arbitrary, DC, PRBS, IQ)
  - [ ] 2.3 Create AmplitudeUnit enum (Vpp, Vrms, dBm)
  - [ ] 2.4 Create ModulationParameters class with all properties
  - [ ] 2.5 Create ModulationType enum (AM, FM, PM, PWM, FSK, ASK, PSK)
  - [ ] 2.6 Create ModulationSource enum (Internal, External, Channel1, Channel2)
  - [ ] 2.7 Create SweepParameters class with all properties
  - [ ] 2.8 Create SweepType enum (Linear, Logarithmic)
  - [ ] 2.9 Create SweepDirection enum (Up, Down, UpDown)
  - [ ] 2.10 Create TriggerSource enum (Internal, External, Manual)
  - [ ] 2.11 Create BurstParameters class with all properties
  - [ ] 2.12 Create BurstMode enum (NCycle, Gated)
  - [ ] 2.13 Create TriggerEdge enum (Rising, Falling)
  - [ ] 2.14 Create GatePolarity enum (Positive, Negative)
  - [ ] 2.15 Create DeviceIdentity class (Manufacturer, Model, SerialNumber, FirmwareVersion)
  - [ ] 2.16 Create OperationResult class with static factory methods
  - [ ] 2.17 Create CommandResult class
  - [ ] 2.18 Create LoadImpedance class with static factory methods
  - [ ] 2.19 Create LoadType enum (HighZ, FiftyOhm, Custom)
  - [ ] 2.20 Create ValidationResult class
  - [ ] 2.21 Create DeviceError class
  - [ ] 2.22 Create WaveformState, ModulationState, SweepState, BurstState classes

## Phase 3: SCPI Command Builder

- [x] 3. Implement SCPI Command Builder
  - [ ] 3.1 Create IScpiCommandBuilder interface
  - [ ] 3.2 Create ScpiCommandBuilder class implementing interface
  - [ ] 3.3 Implement BuildBasicWaveCommand() method
  - [ ] 3.4 Implement BuildOutputStateCommand() method
  - [ ] 3.5 Implement BuildLoadCommand() method
  - [ ] 3.6 Implement BuildModulationCommand() method
  - [ ] 3.7 Implement BuildModulationStateCommand() method
  - [ ] 3.8 Implement BuildSweepCommand() method
  - [ ] 3.9 Implement BuildSweepStateCommand() method
  - [ ] 3.10 Implement BuildBurstCommand() method
  - [ ] 3.11 Implement BuildBurstStateCommand() method
  - [ ] 3.12 Implement BuildArbitraryWaveCommand() method
  - [ ] 3.13 Implement BuildStoreArbitraryWaveCommand() method
  - [ ] 3.14 Implement BuildQueryCommand() method
  - [ ] 3.15 Implement BuildSystemCommand() method
  - [ ] 3.16 Implement helper methods for unit conversion (Hz, kHz, MHz, V, mV)
  - [ ] 3.17 Implement helper methods for SCPI enum mapping

## Phase 4: SCPI Response Parser

- [-] 4. Implement SCPI Response Parser
  - [ ] 4.1 Create IScpiResponseParser interface
  - [ ] 4.2 Create ScpiResponseParser class implementing interface
  - [ ] 4.3 Implement ParseBooleanResponse() method
  - [ ] 4.4 Implement ParseNumericResponse() method
  - [ ] 4.5 Implement ParseStringResponse() method
  - [ ] 4.6 Implement ParseWaveformState() method
  - [ ] 4.7 Implement ParseModulationState() method
  - [ ] 4.8 Implement ParseSweepState() method
  - [ ] 4.9 Implement ParseBurstState() method
  - [ ] 4.10 Implement ParseIdentityResponse() method
  - [ ] 4.11 Implement ParseSystemStatus() method
  - [ ] 4.12 Implement ParseArbitraryWaveformData() method
  - [ ] 4.13 Implement ParseErrorResponse() method
  - [ ] 4.14 Implement helper methods for unit parsing (Hz, kHz, MHz, V, mV, dBm)
  - [ ] 4.15 Implement helper methods for SCPI to enum mapping

## Phase 5: Input Validator

- [x] 5. Implement Input Validator
  - [ ] 5.1 Create IInputValidator interface
  - [ ] 5.2 Create InputValidator class implementing interface
  - [ ] 5.3 Implement ValidateFrequency() method with waveform-specific limits
  - [ ] 5.4 Implement ValidateAmplitude() method with load-specific limits
  - [ ] 5.5 Implement ValidateOffset() method with amplitude and load constraints
  - [ ] 5.6 Implement ValidatePhase() method (0-360 degrees)
  - [ ] 5.7 Implement ValidateDutyCycle() method (0.01-99.99%)
  - [ ] 5.8 Implement ValidateModulationDepth() method
  - [ ] 5.9 Implement ValidateModulationFrequency() method
  - [ ] 5.10 Implement ValidateDeviation() method
  - [ ] 5.11 Implement ValidateSweepRange() method
  - [ ] 5.12 Implement ValidateSweepTime() method
  - [ ] 5.13 Implement ValidateBurstCycles() method
  - [ ] 5.14 Implement ValidateBurstPeriod() method
  - [ ] 5.15 Implement ValidateArbitraryWaveformPoints() method
  - [ ] 5.16 Implement ValidateWaveformName() method

## Phase 6: VISA Communication Manager (Real)

- [-] 6. Implement Real VISA Communication Manager
  - [ ] 6.1 Create IVisaCommunicationManager interface
  - [ ] 6.2 Create VisaCommunicationManager class implementing interface
  - [ ] 6.3 Implement Connect() method with VISA session establishment
  - [ ] 6.4 Implement Disconnect() method with resource cleanup
  - [ ] 6.5 Implement IsConnected property
  - [ ] 6.6 Implement SendCommand() method
  - [ ] 6.7 Implement Query() method
  - [ ] 6.8 Implement QueryBinary() method
  - [ ] 6.9 Implement SendCommandAsync() method
  - [ ] 6.10 Implement QueryAsync() method
  - [ ] 6.11 Implement GetDeviceIdentity() method
  - [ ] 6.12 Implement CommunicationError event
  - [ ] 6.13 Implement IDisposable pattern for resource cleanup
  - [ ] 6.14 Implement timeout handling
  - [ ] 6.15 Implement error handling and exception wrapping

## Phase 7: Mock VISA Communication Manager (Simulation)

- [-] 7. Implement Mock VISA Communication Manager
  - [ ] 7.1 Create SimulatedDeviceState class
  - [ ] 7.2 Create SimulatedChannelState class
  - [ ] 7.3 Create MockVisaCommunicationManager class implementing IVisaCommunicationManager
  - [ ] 7.4 Implement Connect() method with simulated connection
  - [ ] 7.5 Implement Disconnect() method
  - [ ] 7.6 Implement IsConnected property
  - [ ] 7.7 Implement SendCommand() method with SCPI command parsing
  - [ ] 7.8 Implement Query() method with response generation
  - [ ] 7.9 Implement QueryBinary() method
  - [ ] 7.10 Implement SendCommandAsync() method
  - [ ] 7.11 Implement QueryAsync() method
  - [ ] 7.12 Implement GetDeviceIdentity() method returning simulated identity
  - [ ] 7.13 Implement ProcessCommand() method for state updates
  - [ ] 7.14 Implement ProcessQuery() method for response generation
  - [ ] 7.15 Implement GenerateWaveformQueryResponse() method
  - [ ] 7.16 Implement GenerateIdentityResponse() method
  - [ ] 7.17 Implement SimulateError() method for error injection
  - [ ] 7.18 Implement SimulateConnectionLoss() method
  - [ ] 7.19 Implement SimulateTimeout() method
  - [ ] 7.20 Implement parameter validation in mock
  - [ ] 7.21 Implement GetChannelState() method for test verification

## Phase 8: Signal Generator Service

- [x] 8. Implement Signal Generator Service
  - [ ] 8.1 Create ISignalGeneratorService interface
  - [ ] 8.2 Create SignalGeneratorService class implementing interface
  - [ ] 8.3 Implement constructor with dependency injection
  - [ ] 8.4 Implement ConnectAsync() method
  - [ ] 8.5 Implement DisconnectAsync() method
  - [ ] 8.6 Implement IsConnected property
  - [ ] 8.7 Implement DeviceInfo property
  - [ ] 8.8 Implement SetBasicWaveformAsync() method
  - [ ] 8.9 Implement GetWaveformStateAsync() method
  - [ ] 8.10 Implement SetOutputStateAsync() method
  - [ ] 8.11 Implement SetLoadImpedanceAsync() method
  - [ ] 8.12 Implement ConfigureModulationAsync() method
  - [ ] 8.13 Implement SetModulationStateAsync() method
  - [ ] 8.14 Implement GetModulationStateAsync() method
  - [ ] 8.15 Implement ConfigureSweepAsync() method
  - [ ] 8.16 Implement SetSweepStateAsync() method
  - [ ] 8.17 Implement GetSweepStateAsync() method
  - [ ] 8.18 Implement ConfigureBurstAsync() method
  - [ ] 8.19 Implement SetBurstStateAsync() method
  - [ ] 8.20 Implement GetBurstStateAsync() method
  - [ ] 8.21 Implement UploadArbitraryWaveformAsync() method
  - [ ] 8.22 Implement SelectArbitraryWaveformAsync() method
  - [ ] 8.23 Implement GetArbitraryWaveformListAsync() method
  - [ ] 8.24 Implement DeleteArbitraryWaveformAsync() method
  - [ ] 8.25 Implement RecallSetupAsync() method
  - [ ] 8.26 Implement SaveSetupAsync() method
  - [ ] 8.27 Implement GetSystemStatusAsync() method
  - [ ] 8.28 Implement ResetDeviceAsync() method
  - [ ] 8.29 Implement DeviceError event
  - [ ] 8.30 Implement ConnectionStateChanged event
  - [ ] 8.31 Implement state caching mechanism
  - [ ] 8.32 Implement GetLastDeviceErrorAsync() helper method

## Phase 9: Unit Test Project Setup

- [x] 9. Create Unit Test Project
  - [x] 9.1 Create new Unit Test project targeting .NET Framework 4.8
  - [x] 9.2 Name project "Siglent.SDG6052X.Tests"
  - [x] 9.3 Add project reference to Device Library
  - [x] 9.4 Add NuGet packages: NUnit, NUnit3TestAdapter, Moq, FsCheck, Microsoft.NET.Test.Sdk
  - [x] 9.5 Create folder structure: Unit/, Integration/, PropertyBased/

## Phase 10: Unit Tests for Device Library

- [-] 10. Implement Unit Tests
  - [ ] 10.1 Create ScpiCommandBuilderTests class
  - [ ] 10.2 Write tests for BuildBasicWaveCommand() with all waveform types
  - [ ] 10.3 Write tests for frequency unit conversion (Hz, kHz, MHz)
  - [ ] 10.4 Write tests for amplitude unit conversion (Vpp, Vrms, dBm)
  - [ ] 10.5 Write tests for all modulation command building
  - [ ] 10.6 Write tests for sweep command building
  - [ ] 10.7 Write tests for burst command building
  - [ ] 10.8 Create ScpiResponseParserTests class
  - [ ] 10.9 Write tests for ParseWaveformState() with valid responses
  - [ ] 10.10 Write tests for ParseWaveformState() with malformed responses
  - [ ] 10.11 Write tests for numeric parsing with various units
  - [ ] 10.12 Write tests for enum mapping (SCPI strings to C# enums)
  - [ ] 10.13 Write tests for ParseIdentityResponse()
  - [ ] 10.14 Write tests for ParseErrorResponse()
  - [ ] 10.15 Create InputValidatorTests class
  - [ ] 10.16 Write tests for ValidateFrequency() with all waveform types
  - [ ] 10.17 Write tests for ValidateAmplitude() with different loads
  - [ ] 10.18 Write tests for ValidateOffset() with amplitude constraints
  - [ ] 10.19 Write tests for all modulation parameter validation
  - [ ] 10.20 Write tests for sweep parameter validation
  - [ ] 10.21 Write tests for burst parameter validation
  - [ ] 10.22 Create MockCommunicationManagerTests class
  - [ ] 10.23 Write tests for simulated state management
  - [ ] 10.24 Write tests for SCPI response generation
  - [ ] 10.25 Write tests for error simulation
  - [ ] 10.26 Write tests for validation in mock

## Phase 11: Integration Tests

- [x] 11. Implement Integration Tests
  - [x] 11.1 Create SignalGeneratorServiceIntegrationTests class
  - [x] 11.2 Write test for end-to-end waveform configuration using mock
  - [x] 11.3 Write test for connection management
  - [x] 11.4 Write test for error propagation through layers
  - [x] 11.5 Write test for event raising (DeviceError, ConnectionStateChanged)
  - [x] 11.6 Write test for async operation coordination
  - [x] 11.7 Write test for state verification after commands
  - [x] 11.8 Write test for modulation configuration
  - [x] 11.9 Write test for sweep configuration
  - [x] 11.10 Write test for burst configuration

## Phase 12: Property-Based Tests

- [x] 12. Implement Property-Based Tests
  - [x] 12.1 Create PropertyBasedTests class
  - [x] 12.2 Write property test for command-parse roundtrip
  - [x] 12.3 Write property test for validation consistency
  - [x] 12.4 Write property test for frequency unit conversion
  - [x] 12.5 Write property test for amplitude-offset constraint

## Phase 13: WinForms UI Project Setup

- [x] 13. Create WinForms UI Project
  - [x] 13.1 Create new WinForms App project targeting .NET Framework 4.8
  - [x] 13.2 Name project "Siglent.SDG6052X.WinFormsUI"
  - [x] 13.3 Add project reference to Device Library DLL
  - [x] 13.4 Add optional NuGet packages: Serilog, Microsoft.Extensions.DependencyInjection
  - [x] 13.5 Create folder structure: Forms/, Controllers/

## Phase 14: Main Form and Connection UI

- [x] 14. Implement Main Form
  - [x] 14.1 Design MainForm with connection controls (IP address input, connect/disconnect buttons)
  - [x] 14.2 Add status label for connection state
  - [x] 14.3 Add tab control for different configuration sections
  - [x] 14.4 Implement btnConnect_Click event handler
  - [x] 14.5 Implement btnDisconnect_Click event handler
  - [x] 14.6 Subscribe to ConnectionStateChanged event
  - [x] 14.7 Subscribe to DeviceError event
  - [x] 14.8 Implement EnableControls() method to enable/disable UI based on connection
  - [x] 14.9 Display device identity information when connected
  - [x] 14.10 Implement dependency injection setup in Program.cs

## Phase 15: Waveform Configuration UI

- [x] 15. Implement Waveform Configuration Form/Panel
  - [x] 15.1 Design waveform configuration controls (channel selector, waveform type dropdown)
  - [x] 15.2 Add numeric inputs for frequency, amplitude, offset, phase
  - [x] 15.3 Add unit selector for amplitude (Vpp, Vrms, dBm)
  - [x] 15.4 Add duty cycle control for square/pulse waveforms
  - [x] 15.5 Add pulse-specific controls (width, rise, fall)
  - [x] 15.6 Add output enable checkbox
  - [x] 15.7 Add load impedance selector (50Ω, High-Z, Custom)
  - [x] 15.8 Implement btnSetWaveform_Click event handler
  - [x] 15.9 Implement input validation with visual feedback
  - [x] 15.10 Implement waveform type change handler to show/hide relevant controls
  - [x] 15.11 Add "Query Current State" button and handler

## Phase 16: Modulation Configuration UI

- [x] 16. Implement Modulation Configuration Form/Panel
  - [x] 16.1 Design modulation configuration controls (modulation type dropdown)
  - [x] 16.2 Add controls for modulation source (Internal, External, Channel1, Channel2)
  - [x] 16.3 Add numeric inputs for depth, deviation, rate
  - [x] 16.4 Add modulation waveform selector
  - [x] 16.5 Add FSK/ASK/PSK specific controls (hop frequency, hop amplitude, hop phase)
  - [x] 16.6 Add modulation enable checkbox
  - [x] 16.7 Implement btnConfigureModulation_Click event handler
  - [x] 16.8 Implement modulation type change handler to show/hide relevant controls
  - [x] 16.9 Implement input validation with visual feedback

## Phase 17: Sweep Configuration UI

- [x] 17. Implement Sweep Configuration Form/Panel
  - [x] 17.1 Design sweep configuration controls
  - [x] 17.2 Add numeric inputs for start frequency, stop frequency, time
  - [x] 17.3 Add sweep type selector (Linear, Logarithmic)
  - [x] 17.4 Add sweep direction selector (Up, Down, UpDown)
  - [x] 17.5 Add trigger source selector
  - [x] 17.6 Add numeric inputs for return time, hold time
  - [x] 17.7 Add sweep enable checkbox
  - [x] 17.8 Implement btnConfigureSweep_Click event handler
  - [x] 17.9 Implement input validation with visual feedback

## Phase 18: Burst Configuration UI

- [x] 18. Implement Burst Configuration Form/Panel
  - [x] 18.1 Design burst configuration controls
  - [x] 18.2 Add burst mode selector (N-Cycle, Gated)
  - [x] 18.3 Add numeric input for cycles
  - [x] 18.4 Add numeric input for period
  - [x] 18.5 Add trigger source selector
  - [x] 18.6 Add trigger edge selector (Rising, Falling)
  - [x] 18.7 Add numeric input for start phase
  - [x] 18.8 Add gate polarity selector (Positive, Negative)
  - [x] 18.9 Add burst enable checkbox
  - [x] 18.10 Implement btnConfigureBurst_Click event handler
  - [x] 18.11 Implement burst mode change handler to show/hide relevant controls
  - [x] 18.12 Implement input validation with visual feedback

## Phase 19: Arbitrary Waveform Management UI

- [x] 19. Implement Arbitrary Waveform Management Form/Panel
  - [x] 19.1 Design arbitrary waveform management controls
  - [x] 19.2 Add list box to display stored waveforms
  - [x] 19.3 Add "Refresh List" button and handler
  - [x] 19.4 Add "Upload Waveform" button with file dialog
  - [x] 19.5 Add "Delete Waveform" button and handler
  - [x] 19.6 Add "Select Waveform" button to assign to channel
  - [x] 19.7 Add waveform name input
  - [x] 19.8 Add progress bar for upload operations
  - [x] 19.9 Implement waveform file parsing (CSV or binary format)
  - [x] 19.10 Implement btnUploadWaveform_Click event handler with progress reporting
  - [x] 19.11 Implement btnDeleteWaveform_Click event handler with confirmation
  - [x] 19.12 Implement btnSelectWaveform_Click event handler

## Phase 20: Final Integration and Testing

- [ ] 20. Final Integration
  - [x] 20.1 Run all unit tests and verify 80%+ code coverage
  - [x] 20.2 Run all integration tests with mock communication
  - [x] 20.3 Run all property-based tests
  - [x] 20.4 Test UI application with mock communication manager
  - [x] 20.5 Test UI application with real SDG6052X device (if available)
  - [x] 20.6 Verify all SCPI commands generate correct syntax
  - [x] 20.7 Verify all SCPI responses are parsed correctly
  - [x] 20.8 Verify all validation rules are enforced
  - [x] 20.9 Verify error handling and event raising
  - [x] 20.10 Verify UI responsiveness with async operations
  - [x] 20.11 Test connection/disconnection scenarios
  - [x] 20.12 Test all waveform types and configurations
  - [x] 20.13 Test all modulation types
  - [x] 20.14 Test sweep and burst modes
  - [x] 20.15 Test arbitrary waveform upload and management
  - [x] 20.16 Build release version of Device Library DLL
  - [x] 20.17 Build release version of WinForms UI application
  - [x] 20.18 Create deployment package with dependencies
