@echo off
REM Run Calibration Tuning Application in Simulation Mode
REM This allows testing without physical hardware

echo.
echo ========================================
echo Calibration Tuning - SIMULATION MODE
echo ========================================
echo.
echo Starting application with mock devices...
echo.

REM Run the application with simulation flag
"%~dp0bin\Debug\CalibrationTuning.exe" --simulate

pause
