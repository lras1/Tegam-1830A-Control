# Requirements Document

## Introduction

The Calibration-Tuning Application is a C# .NET Framework 4.8 WinForms GUI application that integrates the Tegam 1830A RF Power Meter and the Siglent SDG6052X Signal Generator to perform automated power calibration and tuning. The application adjusts the signal generator's output voltage to achieve a target power measurement from the power meter, while logging all measurement data throughout the tuning process.

## Glossary

- **Calibration_Tuning_Application**: The WinForms GUI application that coordinates the signal generator and power meter
- **Signal_Generator**: The Siglent SDG6052X device that produces RF signals
- **Power_Meter**: The Tegam 1830A device that measures RF power
- **Tuning_Process**: The iterative adjustment of signal voltage to reach a target power setpoint
- **Setpoint**: The target power value that the tuning process attempts to achieve
- **Voltage_Step**: The increment by which the signal voltage is adjusted during tuning
- **Tolerance**: The acceptable deviation from the setpoint to consider tuning complete
- **Tuning_Session**: A complete tuning operation from start to completion or abort
- **Data_Logger**: The component that records signal parameters and power measurements to a file

## Requirements

### Requirement 1: Device Connection Management

**User Story:** As a calibration engineer, I want to connect to both the signal generator and power meter, so that I can control both devices from a single application.

#### Acceptance Criteria

1. THE Calibration_Tuning_Application SHALL provide connection controls for the Signal_Generator IP address
2. THE Calibration_Tuning_Application SHALL provide connection controls for the Power_Meter IP address
3. WHEN the user initiates connection, THE Calibration_Tuning_Application SHALL attempt to connect to both devices
4. WHEN both devices connect successfully, THE Calibration_Tuning_Application SHALL display connection status as "Connected" for each device
5. IF either device fails to connect, THEN THE Calibration_Tuning_Application SHALL display an error message indicating which device failed
6. THE Calibration_Tuning_Application SHALL provide a disconnect control to terminate connections to both devices
7. WHILE either device is disconnected, THE Calibration_Tuning_Application SHALL disable tuning controls

### Requirement 2: Signal Generator Configuration

**User Story:** As a calibration engineer, I want to configure the signal generator's frequency and initial voltage, so that I can set up the starting conditions for tuning.

#### Acceptance Criteria

1. THE Calibration_Tuning_Application SHALL provide an input control for signal frequency in Hz
2. THE Calibration_Tuning_Application SHALL validate that frequency is within the range 1 Hz to 500 MHz
3. THE Calibration_Tuning_Application SHALL provide an input control for initial signal voltage
4. THE Calibration_Tuning_Application SHALL validate that voltage is within the Signal_Generator specifications
5. WHEN the user sets frequency, THE Calibration_Tuning_Application SHALL configure the Signal_Generator to output at that frequency
6. WHEN the user sets initial voltage, THE Calibration_Tuning_Application SHALL configure the Signal_Generator amplitude to that voltage
7. THE Calibration_Tuning_Application SHALL display the current frequency and voltage settings from the Signal_Generator

### Requirement 3: Power Meter Configuration

**User Story:** As a calibration engineer, I want to configure the power meter's measurement frequency and sensor, so that measurements are accurate for my signal.

#### Acceptance Criteria

1. THE Calibration_Tuning_Application SHALL provide an input control for measurement frequency matching the signal frequency
2. WHEN signal frequency is set, THE Calibration_Tuning_Application SHALL automatically configure the Power_Meter to measure at that frequency
3. THE Calibration_Tuning_Application SHALL provide a sensor selection control for the Power_Meter
4. WHEN the user selects a sensor, THE Calibration_Tuning_Application SHALL configure the Power_Meter to use that sensor
5. THE Calibration_Tuning_Application SHALL display the currently selected sensor information

### Requirement 4: Tuning Parameter Configuration

**User Story:** As a calibration engineer, I want to configure tuning parameters including setpoint, voltage step size, and tolerance, so that I can control how the tuning process behaves.

#### Acceptance Criteria

1. THE Calibration_Tuning_Application SHALL provide an input control for power setpoint in dBm
2. THE Calibration_Tuning_Application SHALL validate that setpoint is within the Power_Meter measurement range
3. THE Calibration_Tuning_Application SHALL provide an input control for voltage step size
4. THE Calibration_Tuning_Application SHALL validate that voltage step is greater than 0 and less than maximum voltage
5. THE Calibration_Tuning_Application SHALL provide an input control for tolerance in dB
6. THE Calibration_Tuning_Application SHALL validate that tolerance is greater than 0
7. THE Calibration_Tuning_Application SHALL provide an input control for maximum tuning iterations
8. THE Calibration_Tuning_Application SHALL validate that maximum iterations is between 1 and 10000

### Requirement 5: Automated Tuning Process

**User Story:** As a calibration engineer, I want to start an automated tuning process that adjusts voltage to reach the power setpoint, so that I can achieve precise power calibration without manual intervention.

#### Acceptance Criteria

1. WHEN the user starts tuning, THE Calibration_Tuning_Application SHALL enable the Signal_Generator output
2. WHILE tuning is active, THE Calibration_Tuning_Application SHALL measure power from the Power_Meter
3. WHILE tuning is active, THE Calibration_Tuning_Application SHALL compare measured power to the setpoint
4. WHEN measured power is below setpoint minus tolerance, THE Calibration_Tuning_Application SHALL increase Signal_Generator voltage by the voltage step
5. WHEN measured power is above setpoint plus tolerance, THE Calibration_Tuning_Application SHALL decrease Signal_Generator voltage by the voltage step
6. WHEN measured power is within tolerance of setpoint, THE Calibration_Tuning_Application SHALL complete the Tuning_Session successfully
7. WHEN maximum iterations is reached, THE Calibration_Tuning_Application SHALL terminate the Tuning_Session with a timeout status
8. WHILE tuning is active, THE Calibration_Tuning_Application SHALL display current measured power and current voltage
9. THE Calibration_Tuning_Application SHALL provide a stop control to abort the Tuning_Session

### Requirement 6: Tuning Safety Limits

**User Story:** As a calibration engineer, I want the tuning process to respect voltage and power limits, so that I do not damage equipment.

#### Acceptance Criteria

1. THE Calibration_Tuning_Application SHALL provide an input control for maximum voltage limit
2. WHEN voltage would exceed maximum voltage limit, THE Calibration_Tuning_Application SHALL terminate the Tuning_Session with an error status
3. THE Calibration_Tuning_Application SHALL provide an input control for minimum voltage limit
4. WHEN voltage would fall below minimum voltage limit, THE Calibration_Tuning_Application SHALL terminate the Tuning_Session with an error status
5. IF the Power_Meter reports an overload condition, THEN THE Calibration_Tuning_Application SHALL immediately disable Signal_Generator output and terminate the Tuning_Session

### Requirement 7: Data Logging

**User Story:** As a calibration engineer, I want all tuning measurements logged to a file, so that I can analyze the tuning process and maintain calibration records.

#### Acceptance Criteria

1. THE Calibration_Tuning_Application SHALL provide a file path selection control for the log file
2. WHEN a Tuning_Session starts, THE Calibration_Tuning_Application SHALL create or append to the log file
3. WHILE tuning is active, THE Calibration_Tuning_Application SHALL log each measurement with timestamp, frequency, voltage, and measured power
4. WHEN a Tuning_Session completes, THE Calibration_Tuning_Application SHALL log the final status and parameters
5. THE Data_Logger SHALL format log entries as CSV with columns: Timestamp, Frequency_Hz, Voltage, Power_dBm, Status
6. IF the log file cannot be written, THEN THE Calibration_Tuning_Application SHALL display an error but continue tuning

### Requirement 8: Real-Time Status Display

**User Story:** As a calibration engineer, I want to see real-time status of the tuning process, so that I can monitor progress and identify issues.

#### Acceptance Criteria

1. WHILE tuning is active, THE Calibration_Tuning_Application SHALL display current iteration number
2. WHILE tuning is active, THE Calibration_Tuning_Application SHALL display current measured power
3. WHILE tuning is active, THE Calibration_Tuning_Application SHALL display current signal voltage
4. WHILE tuning is active, THE Calibration_Tuning_Application SHALL display the difference between measured power and setpoint
5. WHILE tuning is active, THE Calibration_Tuning_Application SHALL update the display after each measurement
6. THE Calibration_Tuning_Application SHALL display tuning status as "Idle", "Tuning", "Converged", "Timeout", "Error", or "Aborted"

### Requirement 9: Tuning History Visualization

**User Story:** As a calibration engineer, I want to see a graph of power versus iteration during tuning, so that I can visualize convergence behavior.

#### Acceptance Criteria

1. THE Calibration_Tuning_Application SHALL display a chart showing measured power over iteration number
2. WHILE tuning is active, THE Calibration_Tuning_Application SHALL update the chart with each new measurement
3. THE Calibration_Tuning_Application SHALL display the setpoint as a horizontal reference line on the chart
4. THE Calibration_Tuning_Application SHALL display tolerance bounds as horizontal reference lines on the chart
5. WHEN a Tuning_Session completes, THE Calibration_Tuning_Application SHALL retain the chart data for review

### Requirement 10: Error Handling and Recovery

**User Story:** As a calibration engineer, I want clear error messages and recovery options when problems occur, so that I can troubleshoot and continue working.

#### Acceptance Criteria

1. IF the Signal_Generator reports an error, THEN THE Calibration_Tuning_Application SHALL display the error message and pause tuning
2. IF the Power_Meter reports an error, THEN THE Calibration_Tuning_Application SHALL display the error message and pause tuning
3. IF communication with either device fails, THEN THE Calibration_Tuning_Application SHALL terminate the Tuning_Session and display a connection error
4. WHEN an error occurs, THE Calibration_Tuning_Application SHALL provide options to retry or abort
5. THE Calibration_Tuning_Application SHALL log all errors to the data log file

### Requirement 11: Configuration Persistence

**User Story:** As a calibration engineer, I want my tuning parameters and device settings saved between sessions, so that I do not have to re-enter them each time.

#### Acceptance Criteria

1. WHEN the application closes, THE Calibration_Tuning_Application SHALL save device IP addresses to configuration storage
2. WHEN the application closes, THE Calibration_Tuning_Application SHALL save tuning parameters to configuration storage
3. WHEN the application closes, THE Calibration_Tuning_Application SHALL save log file path to configuration storage
4. WHEN the application starts, THE Calibration_Tuning_Application SHALL load saved configuration values
5. THE Calibration_Tuning_Application SHALL store configuration in the user's AppData directory

### Requirement 12: Manual Power Measurement

**User Story:** As a calibration engineer, I want to take manual power measurements without starting a tuning session, so that I can verify readings before tuning.

#### Acceptance Criteria

1. THE Calibration_Tuning_Application SHALL provide a manual measure control
2. WHEN the user triggers manual measurement, THE Calibration_Tuning_Application SHALL read power from the Power_Meter
3. WHEN manual measurement completes, THE Calibration_Tuning_Application SHALL display the measured power value
4. THE Calibration_Tuning_Application SHALL log manual measurements to the data log file with a "Manual" status indicator
