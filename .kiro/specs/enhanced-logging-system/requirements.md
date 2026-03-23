# Requirements Document

## Introduction

This document specifies requirements for an enhanced logging system for the Tegam 1830A WinForms application. The system extends the existing data logging capabilities to capture both power measurement data and configuration settings changes in a unified CSV format. The enhanced system supports manual and automatic sampling modes, displays logged entries on-screen, and maintains a comprehensive audit trail of all system activities.

## Glossary

- **Enhanced_Logger**: The logging subsystem responsible for capturing both data measurements and settings changes
- **CSV_Writer**: The component that writes log entries to CSV files with proper formatting
- **Log_Entry**: A single record in the log file, either a Data type or Setting type
- **Data_Entry**: A log entry containing power measurement data (Type="Data")
- **Setting_Entry**: A log entry containing configuration change information (Type="Setting")
- **Log_Display**: The GUI component that shows recent log entries on screen
- **Manual_Sampling**: User-triggered single measurement capture
- **Automatic_Sampling**: System-triggered periodic measurement capture based on configured sample rate
- **Sample_Rate**: The time interval between automatic measurements in milliseconds
- **Setting_Name**: The identifier for a configuration parameter (e.g., "Frequency", "Sensor_ID", "Calibration_Mode")
- **Setting_Value**: The value assigned to a configuration parameter
- **Timestamp**: ISO 8601 formatted date-time string (yyyy-MM-dd HH:mm:ss.fff)

## Requirements

### Requirement 1: Enhanced CSV Format with Type Column

**User Story:** As a test engineer, I want log files to distinguish between data measurements and settings changes, so that I can analyze both measurement results and the configuration context in which they were captured.

#### Acceptance Criteria

1. THE CSV_Writer SHALL include "Type" as the first column in all CSV log files
2. THE CSV_Writer SHALL write "Setting" or "Data" as the only valid values for the Type column
3. WHEN writing a Data_Entry, THE CSV_Writer SHALL include columns: Type, Timestamp, Frequency (Hz), Power (dBm), Sensor ID
4. WHEN writing a Setting_Entry, THE CSV_Writer SHALL include columns: Type, Timestamp, Setting Name, Setting Value, Context
5. THE CSV_Writer SHALL write a header row as the first line of new CSV files
6. THE CSV_Writer SHALL format Timestamp values as "yyyy-MM-dd HH:mm:ss.fff"
7. THE CSV_Writer SHALL escape commas and quotes in Setting_Value and Context fields according to CSV RFC 4180 standard

### Requirement 2: Settings Change Logging

**User Story:** As a test engineer, I want all configuration changes to be logged automatically, so that I have a complete audit trail of system settings during test sessions.

#### Acceptance Criteria

1. WHEN the frequency is set via btnSetFrequency, THE Enhanced_Logger SHALL log a Setting_Entry with Setting_Name="Frequency" and Setting_Value containing the frequency value and unit
2. WHEN a sensor is selected or changed, THE Enhanced_Logger SHALL log a Setting_Entry with Setting_Name="Sensor_ID" and Setting_Value containing the sensor identifier
3. WHEN calibration is started via btnStartCalibration, THE Enhanced_Logger SHALL log a Setting_Entry with Setting_Name="Calibration" and Setting_Value="Started"
4. WHEN the device connection state changes, THE Enhanced_Logger SHALL log a Setting_Entry with Setting_Name="Connection" and Setting_Value="Connected" or "Disconnected"
5. WHEN any configuration button is clicked, THE Enhanced_Logger SHALL capture the Timestamp at the moment of the button click
6. THE Enhanced_Logger SHALL include relevant context information in the Context field (e.g., previous value, user action)
7. WHEN logging is not active, THE Enhanced_Logger SHALL queue Setting_Entry records for later writing when logging starts

### Requirement 3: Data Measurement Logging

**User Story:** As a test engineer, I want power measurements to be logged with proper type designation, so that they are clearly distinguished from settings in the unified log format.

#### Acceptance Criteria

1. WHEN a power measurement is received, THE Enhanced_Logger SHALL log a Data_Entry with Type="Data"
2. THE Enhanced_Logger SHALL include Timestamp, Frequency (Hz), Power (dBm), and Sensor ID in each Data_Entry
3. WHEN manual sampling is triggered via btnMeasure, THE Enhanced_Logger SHALL log exactly one Data_Entry
4. WHEN automatic sampling is active, THE Enhanced_Logger SHALL log one Data_Entry per measurement at the configured Sample_Rate
5. THE Enhanced_Logger SHALL preserve the existing measurement precision (2 decimal places for power values)
6. THE Enhanced_Logger SHALL write Data_Entry records to the same CSV file as Setting_Entry records
7. THE Enhanced_Logger SHALL maintain chronological order of all entries based on Timestamp

### Requirement 4: Manual Sampling Mode

**User Story:** As a test engineer, I want to trigger single measurements manually, so that I can capture data at specific moments during testing.

#### Acceptance Criteria

1. WHEN the user clicks the manual sample button, THE Enhanced_Logger SHALL trigger exactly one power measurement
2. THE Enhanced_Logger SHALL log the manual measurement as a Data_Entry with the current Timestamp
3. THE Enhanced_Logger SHALL complete the manual measurement within 500 milliseconds
4. WHEN logging is active, THE Enhanced_Logger SHALL write the manual measurement to the current log file
5. WHEN logging is not active, THE Enhanced_Logger SHALL display the measurement on screen without writing to file
6. THE Enhanced_Logger SHALL enable the manual sample button immediately after measurement completion
7. WHEN a manual sample is triggered during automatic sampling, THE Enhanced_Logger SHALL process both samples independently

### Requirement 5: Automatic Sampling Mode

**User Story:** As a test engineer, I want to configure automatic periodic sampling, so that I can collect time-series measurement data without manual intervention.

#### Acceptance Criteria

1. THE Enhanced_Logger SHALL accept a Sample_Rate configuration in milliseconds (range: 100 to 60000)
2. THE Enhanced_Logger SHALL accept a sample count configuration (range: 1 to 10000)
3. WHEN automatic sampling is started, THE Enhanced_Logger SHALL trigger measurements at intervals equal to Sample_Rate
4. THE Enhanced_Logger SHALL log each automatic measurement as a Data_Entry
5. WHEN the configured sample count is reached, THE Enhanced_Logger SHALL stop automatic sampling
6. THE Enhanced_Logger SHALL allow the user to stop automatic sampling before the count is reached
7. WHEN automatic sampling is active, THE Enhanced_Logger SHALL disable the start button and enable the stop button
8. THE Enhanced_Logger SHALL log a Setting_Entry when automatic sampling starts with Setting_Name="Auto_Sampling" and Setting_Value="Started: rate={Sample_Rate}ms, count={count}"
9. THE Enhanced_Logger SHALL log a Setting_Entry when automatic sampling stops with Setting_Name="Auto_Sampling" and Setting_Value="Stopped: {actual_count} samples collected"

### Requirement 6: On-Screen Log Display

**User Story:** As a test engineer, I want to see recent log entries displayed in the GUI, so that I can monitor system activity in real-time without opening the CSV file.

#### Acceptance Criteria

1. THE Log_Display SHALL show the most recent 100 log entries
2. THE Log_Display SHALL display columns: Type, Timestamp, and Details
3. WHEN a Data_Entry is logged, THE Log_Display SHALL show Type="Data" and Details containing "Power: {value} dBm @ {frequency} Hz"
4. WHEN a Setting_Entry is logged, THE Log_Display SHALL show Type="Setting" and Details containing "{Setting_Name}: {Setting_Value}"
5. THE Log_Display SHALL update within 100 milliseconds of a new log entry
6. THE Log_Display SHALL scroll automatically to show the newest entry
7. THE Log_Display SHALL use different text colors for Data entries (blue) and Setting entries (green)
8. THE Log_Display SHALL format Timestamp values as "HH:mm:ss.fff" for display (abbreviated format)

### Requirement 7: CSV File Management

**User Story:** As a test engineer, I want the logging system to manage CSV files properly, so that data is not lost and files are not corrupted.

#### Acceptance Criteria

1. WHEN logging is started, THE Enhanced_Logger SHALL create a new CSV file if it does not exist
2. WHEN logging is started with an existing file, THE Enhanced_Logger SHALL append to the existing file without overwriting
3. THE CSV_Writer SHALL use thread-safe file access to prevent corruption from concurrent writes
4. WHEN a write operation fails, THE Enhanced_Logger SHALL retry up to 3 times with 100ms delay between attempts
5. IF all write retries fail, THE Enhanced_Logger SHALL raise an error event and continue operation
6. THE Enhanced_Logger SHALL flush the file buffer after every 10 log entries or every 5 seconds, whichever comes first
7. WHEN logging is stopped, THE Enhanced_Logger SHALL flush and close the CSV file properly

### Requirement 8: CSV Parser and Pretty Printer

**User Story:** As a developer, I want to parse and validate CSV log files programmatically, so that I can verify data integrity and perform automated analysis.

#### Acceptance Criteria

1. THE CSV_Parser SHALL parse CSV log files conforming to the enhanced format
2. WHEN a valid CSV file is provided, THE CSV_Parser SHALL return a collection of Log_Entry objects
3. WHEN an invalid CSV file is provided, THE CSV_Parser SHALL return a descriptive error indicating the line number and issue
4. THE CSV_Parser SHALL validate that Type column contains only "Setting" or "Data" values
5. THE CSV_Parser SHALL validate that Timestamp values conform to "yyyy-MM-dd HH:mm:ss.fff" format
6. THE CSV_Pretty_Printer SHALL format Log_Entry collections back into valid CSV format
7. FOR ALL valid Log_Entry collections, parsing then printing then parsing SHALL produce equivalent Log_Entry objects (round-trip property)
8. THE CSV_Parser SHALL handle escaped commas and quotes according to CSV RFC 4180 standard

### Requirement 9: Logging State Management

**User Story:** As a test engineer, I want clear visual feedback about logging state, so that I know whether my actions are being recorded.

#### Acceptance Criteria

1. THE Enhanced_Logger SHALL maintain a logging state: "Not Started", "Active", or "Stopped"
2. WHEN logging state is "Active", THE Log_Display SHALL show a green indicator with text "Logging Active"
3. WHEN logging state is "Not Started" or "Stopped", THE Log_Display SHALL show a gray indicator with text "Logging Inactive"
4. THE Enhanced_Logger SHALL display the current log file path when logging state is "Active"
5. THE Enhanced_Logger SHALL display the total count of logged entries (Data + Setting) in real-time
6. WHEN logging is started, THE Enhanced_Logger SHALL log a Setting_Entry with Setting_Name="Logging" and Setting_Value="Started: {filename}"
7. WHEN logging is stopped, THE Enhanced_Logger SHALL log a Setting_Entry with Setting_Name="Logging" and Setting_Value="Stopped: {total_entries} entries"

### Requirement 10: Backward Compatibility

**User Story:** As a developer, I want the enhanced logging system to remain compatible with existing code, so that current functionality is not disrupted.

#### Acceptance Criteria

1. THE Enhanced_Logger SHALL maintain the existing DataLoggingController public API
2. THE Enhanced_Logger SHALL continue to raise MeasurementLogged events for backward compatibility
3. WHEN the enhanced format is disabled, THE CSV_Writer SHALL write files in the legacy format (without Type column)
4. THE Enhanced_Logger SHALL provide a configuration option to enable/disable enhanced logging features
5. WHERE enhanced logging is disabled, THE Enhanced_Logger SHALL behave identically to the legacy DataLoggingController
6. THE Enhanced_Logger SHALL not break existing event subscriptions or handlers
7. THE Enhanced_Logger SHALL maintain thread-safety guarantees of the legacy implementation



### Requirement 11: Logging Independent of Device Connection

**User Story:** As a test engineer, I want to start logging before connecting to the device, so that I can capture all connection events and settings from the beginning of my test session.

#### Acceptance Criteria

1. THE Enhanced_Logger SHALL allow logging to be started when the device is not connected
2. WHEN logging is started without device connection, THE Enhanced_Logger SHALL create the CSV file and write the header
3. WHEN the device connects after logging has started, THE Enhanced_Logger SHALL log a Setting_Entry with Setting_Name="Connection" and Setting_Value="Connected"
4. WHEN the device disconnects while logging is active, THE Enhanced_Logger SHALL log a Setting_Entry with Setting_Name="Connection" and Setting_Value="Disconnected"
5. THE Enhanced_Logger SHALL queue all Setting_Entry records when logging is inactive and write them when logging starts
6. THE Enhanced_Logger SHALL NOT require device connection to enable the Start Logging button
7. WHEN logging is active and device is not connected, THE Enhanced_Logger SHALL only log Setting entries (no Data entries until device connects)

### Requirement 12: Floating Log Viewer Window

**User Story:** As a test engineer, I want to view logged data in a separate floating window with a data grid, so that I can monitor logs while working with other parts of the application.

#### Acceptance Criteria

1. THE Enhanced_Logger SHALL provide a "View Log Window" button or menu item to open a floating window
2. THE Log_Viewer_Window SHALL display log entries in a DataGridView with columns: Type, Timestamp, and Details
3. THE Log_Viewer_Window SHALL be a modeless dialog that can remain open while using the main application
4. THE Log_Viewer_Window SHALL update in real-time as new log entries are added
5. THE Log_Viewer_Window SHALL support sorting by clicking column headers
6. THE Log_Viewer_Window SHALL support filtering by Type (Data, Setting, or All)
7. THE Log_Viewer_Window SHALL display all logged entries (not limited to 100 like the embedded display)
8. THE Log_Viewer_Window SHALL provide an Export button to save filtered/sorted data to a new CSV file
9. THE Log_Viewer_Window SHALL use color coding: Data entries in blue, Setting entries in green
10. THE Log_Viewer_Window SHALL remain on top of the main form but allow interaction with both windows
11. WHEN the Log_Viewer_Window is closed, THE Enhanced_Logger SHALL continue logging normally
12. THE Log_Viewer_Window SHALL provide a Clear Display button to clear the grid (without affecting the log file)
