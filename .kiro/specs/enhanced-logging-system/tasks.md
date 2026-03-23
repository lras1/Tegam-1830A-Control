# Implementation Plan: Enhanced Logging System

## Overview

This implementation plan converts the Enhanced Logging System design into discrete coding tasks. The system extends the existing Tegam 1830A data logging to support unified CSV logging with Type column, settings tracking, automatic sampling, and on-screen display. Implementation follows a bottom-up approach: core data models → CSV handling → logging manager → controller integration → UI components.

## Tasks

- [x] 1. Implement core data models and enums
  - Create LogEntry abstract base class with Timestamp, Type property, ToCsvLine(), and ToDisplayString() methods
  - Create DataEntry class extending LogEntry with Frequency, Power, SensorId properties
  - Create SettingEntry class extending LogEntry with SettingName, SettingValue, Context properties and CSV escaping logic
  - Create LoggingState enum (NotStarted, Active, Stopped)
  - Create LoggingStateChangedEventArgs, ParseResult<T>, and AutoSamplingConfig classes
  - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.6, 3.2_

- [ ]* 1.1 Write property test for core data models
  - **Property 3: DataEntry Structure Completeness**
  - **Validates: Requirements 1.3, 3.2**

- [ ]* 1.2 Write property test for SettingEntry structure
  - **Property 4: SettingEntry Structure Completeness**
  - **Validates: Requirements 1.4**

- [ ]* 1.3 Write property test for timestamp format
  - **Property 5: Timestamp Format Preservation**
  - **Validates: Requirements 1.6**

- [x] 2. Implement CSV writer with RFC 4180 escaping
  - Create CsvWriter class implementing ICsvWriter interface
  - Implement WriteHeader() method to write CSV header row
  - Implement WriteEntry(LogEntry entry) method that calls entry.ToCsvLine()
  - Implement CSV escaping for commas, quotes, and newlines per RFC 4180
  - Implement Flush() and Close() methods for file management
  - Add thread-safe file access using lock object
  - _Requirements: 1.1, 1.5, 1.7, 7.3_

- [ ]* 2.1 Write property test for CSV round-trip preservation
  - **Property 1: CSV Round-Trip Preservation**
  - **Validates: Requirements 1.7, 8.7**

- [ ]* 2.2 Write property test for Type column validity
  - **Property 2: Type Column Validity**
  - **Validates: Requirements 1.1, 1.2**

- [ ]* 2.3 Write property test for power value precision
  - **Property 6: Power Value Precision**
  - **Validates: Requirements 3.5**

- [x] 3. Implement CSV parser with validation
  - Create CsvParser class implementing ICsvParser interface
  - Implement Parse(string filename) method to read and parse CSV files
  - Implement ParseLines(IEnumerable<string> lines) method for line-by-line parsing
  - Add validation for Type column values ("Data" or "Setting")
  - Add validation for Timestamp format (yyyy-MM-dd HH:mm:ss.fff)
  - Handle CSV escaping per RFC 4180 (quoted fields, escaped quotes)
  - Return ParseResult<List<LogEntry>> with success/failure and error details
  - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5, 8.8_

- [ ]* 3.1 Write property test for valid CSV parser output
  - **Property 16: Valid CSV Parser Output**
  - **Validates: Requirements 8.2**

- [ ]* 3.2 Write unit tests for CSV parser edge cases
  - Test empty files, single entry, malformed timestamps
  - Test invalid Type values, missing columns
  - _Requirements: 8.3_

- [x] 4. Checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.

- [x] 5. Implement LogManager with queue and flush strategy
  - Create LogManager class implementing ILogManager interface
  - Implement StartLogging(string filename) to initialize CSV writer and create/append to file
  - Implement StopLogging() to flush, close file, and update state
  - Implement LogEntry(LogEntry entry) to add entries to queue
  - Create thread-safe ConcurrentQueue<LogEntry> for pending entries
  - Implement background processing thread to dequeue and write entries
  - Implement dual-trigger flush strategy (10 entries or 5 seconds)
  - Implement write retry logic (3 attempts with 100ms delay)
  - Add CurrentState, TotalEntryCount, CurrentLogFile properties
  - Raise EntryLogged, StateChanged, and WriteError events
  - _Requirements: 7.1, 7.2, 7.3, 7.4, 7.5, 7.6, 7.7, 9.1_

- [ ]* 5.1 Write property test for settings queuing when inactive
  - **Property 8: Settings Queuing When Inactive**
  - **Validates: Requirements 2.7**

- [ ]* 5.2 Write property test for thread-safe concurrent writes
  - **Property 10: Thread-Safe Concurrent Writes**
  - **Validates: Requirements 7.3**

- [ ]* 5.3 Write property test for chronological ordering
  - **Property 7: Chronological Ordering**
  - **Validates: Requirements 3.7**

- [ ]* 5.4 Write property test for entry count accuracy
  - **Property 14: Entry Count Accuracy**
  - **Validates: Requirements 9.5**

- [ ]* 5.5 Write property test for logging state transitions
  - **Property 15: Logging State Transitions**
  - **Validates: Requirements 9.1**

- [ ]* 5.6 Write unit tests for LogManager error handling
  - Test write retry logic, file access failures
  - Test flush strategy timing
  - _Requirements: 7.4, 7.5, 7.6_

- [x] 6. Implement EnhancedLoggingController with event subscriptions
  - Create EnhancedLoggingController class extending DataLoggingController
  - Inject ILogManager, IPowerMeterService, and other controller dependencies via constructor
  - Subscribe to FrequencySet event from FrequencyConfigurationController
  - Subscribe to SensorSelected event from SensorManagementController
  - Subscribe to CalibrationStarted event from CalibrationController
  - Subscribe to ConnectionStateChanged event from IPowerMeterService
  - Implement event handlers to create SettingEntry objects and call LogManager.LogEntry()
  - Implement ManualSample() method to trigger single measurement and log DataEntry
  - Implement StartAutomaticSampling(int rateMs, int count) with System.Threading.Timer
  - Implement StopAutomaticSampling() to cancel timer and log completion
  - Implement AutoSamplingCallback to trigger measurements at configured rate
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 3.1, 3.3, 4.1, 5.1, 5.2, 5.3, 5.4, 5.8, 5.9_

- [ ]* 6.1 Write property test for data entry type consistency
  - **Property 9: Data Entry Type Consistency**
  - **Validates: Requirements 3.1**

- [ ]* 6.2 Write unit tests for EnhancedLoggingController
  - Test event subscription and handler invocation
  - Test manual sampling trigger and timing
  - Test automatic sampling start, stop, and completion
  - _Requirements: 4.1, 4.3, 5.3, 5.5, 5.6, 5.7_

- [x] 7. Checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.

- [x] 8. Implement LogDisplayControl UI component
  - Create LogDisplayControl as UserControl with ListView in Details mode
  - Add three columns: Type, Timestamp, Details
  - Implement AddEntry(LogEntry entry) method with Control.Invoke for thread safety
  - Implement Clear() method to remove all entries
  - Implement SetMaxEntries(int maxEntries) to configure circular buffer size (default 100)
  - Apply color coding: Data entries in blue, Setting entries in green
  - Implement auto-scroll to bottom on new entry
  - Format timestamps as "HH:mm:ss.fff" for display
  - Format DataEntry details as "Power: {value} dBm @ {frequency} Hz"
  - Format SettingEntry details as "{SettingName}: {SettingValue}"
  - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5, 6.6, 6.7, 6.8_

- [ ]* 8.1 Write property test for display format DataEntry
  - **Property 11: Display Format for DataEntry**
  - **Validates: Requirements 6.3**

- [ ]* 8.2 Write property test for display format SettingEntry
  - **Property 12: Display Format for SettingEntry**
  - **Validates: Requirements 6.4**

- [ ]* 8.3 Write property test for display timestamp format
  - **Property 13: Display Timestamp Format**
  - **Validates: Requirements 6.8**

- [ ]* 8.4 Write unit tests for LogDisplayControl
  - Test circular buffer behavior (max 100 entries)
  - Test thread-safe updates via Invoke
  - Test color coding and auto-scroll
  - _Requirements: 6.1, 6.5, 6.6, 6.7_

- [x] 9. Implement EnhancedLoggingPanel UI component
  - Create EnhancedLoggingPanel as UserControl
  - Add file path TextBox and Browse button
  - Add status indicator (green for Active, gray for Inactive)
  - Add Start Logging and Stop Logging buttons
  - Add Manual Sampling section with Measure Now button
  - Add Automatic Sampling section with Sample Rate and Count inputs
  - Add Start Auto and Stop Auto buttons with progress display
  - Embed LogDisplayControl for recent entries display
  - Wire button clicks to EnhancedLoggingController methods
  - Subscribe to LogManager events (EntryLogged, StateChanged, WriteError)
  - Update UI state based on logging state (enable/disable buttons)
  - Display current log file path and total entry count
  - _Requirements: 4.1, 4.4, 5.7, 9.2, 9.3, 9.4, 9.5, 9.6, 9.7_

- [ ]* 9.1 Write unit tests for EnhancedLoggingPanel
  - Test button state management based on logging state
  - Test UI updates on state change events
  - Test error message display on WriteError event
  - _Requirements: 5.7, 9.2, 9.3_

- [x] 10. Integrate EnhancedLoggingController with existing controllers
  - Modify application startup to instantiate EnhancedLoggingController
  - Pass FrequencyConfigurationController, SensorManagementController, CalibrationController references to constructor
  - Replace DataLoggingController usage with EnhancedLoggingController in main form
  - Verify backward compatibility: existing event subscriptions still work
  - Add EnhancedLoggingPanel to main form UI
  - Test integration: trigger frequency changes, sensor selections, calibration starts
  - Verify Setting entries are logged for all configuration changes
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 10.1, 10.2, 10.6_

- [ ]* 10.1 Write integration tests for controller event flow
  - Test frequency change → SettingEntry logged
  - Test sensor selection → SettingEntry logged
  - Test calibration start → SettingEntry logged
  - Test connection state change → SettingEntry logged
  - _Requirements: 2.1, 2.2, 2.3, 2.4_

- [ ] 11. Implement backward compatibility mode
  - Add EnableEnhancedLogging configuration property to EnhancedLoggingController
  - When EnableEnhancedLogging is false, use legacy CSV format (no Type column)
  - Ensure MeasurementLogged events are raised for backward compatibility
  - Add configuration UI toggle for enhanced logging mode
  - Test that disabling enhanced mode produces legacy-compatible CSV files
  - _Requirements: 10.3, 10.4, 10.5_

- [ ]* 11.1 Write unit tests for backward compatibility
  - Test legacy CSV format output when enhanced mode disabled
  - Test MeasurementLogged event raising
  - _Requirements: 10.2, 10.3_

- [x] 12. Implement floating log viewer window
  - Create LogViewerForm as a modeless Form
  - Add DataGridView with columns: Type, Timestamp, Details
  - Implement real-time updates from LogManager.EntryLogged event
  - Add column sorting by clicking headers
  - Add filter ComboBox for Type (All, Data, Setting)
  - Apply color coding: Data rows in blue, Setting rows in green
  - Add Export button to save filtered data to CSV
  - Add Clear Display button to clear grid
  - Set form properties: TopMost=true, ShowInTaskbar=false
  - Add "View Log Window" button to EnhancedLoggingPanel
  - Wire button click to show/create LogViewerForm instance
  - _Requirements: 12.1, 12.2, 12.3, 12.4, 12.5, 12.6, 12.7, 12.8, 12.9, 12.10, 12.11, 12.12_

- [ ]* 12.1 Write unit tests for LogViewerForm
  - Test filtering by Type
  - Test sorting by columns
  - Test export functionality
  - Test real-time updates
  - _Requirements: 12.5, 12.6, 12.8_

- [x] 13. Enable logging before device connection
  - Modify EnhancedLoggingController to not require device connection for StartLogging
  - Update UI to enable Start Logging button regardless of connection state
  - Ensure connection/disconnection events are logged as Setting entries
  - Test logging workflow: Start logging → Connect → Disconnect → Stop logging
  - Verify Setting entries are queued when logging inactive and written when started
  - _Requirements: 11.1, 11.2, 11.3, 11.4, 11.5, 11.6, 11.7_

- [ ]* 13.1 Write integration tests for connection-independent logging
  - Test start logging before connection
  - Test connection event logging
  - Test disconnection event logging
  - Test setting queue behavior
  - _Requirements: 11.1, 11.3, 11.4, 11.5_

- [x] 14. Final checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.

## Notes

- Tasks marked with `*` are optional and can be skipped for faster MVP
- Each task references specific requirements for traceability
- Property tests validate universal correctness properties using FsCheck
- Unit tests validate specific examples, edge cases, and integration scenarios
- The design uses C# as the implementation language
- FsCheck is already available in the project (packages/FsCheck.2.16.5)
- All property tests should run minimum 100 iterations
- Thread safety is critical: use locks for file access, ConcurrentQueue for entries, Control.Invoke for UI updates
