@echo off
echo ========================================
echo CalibrationTuning - Simulation Mode
echo ========================================
echo.
echo Setting CALIBRATION_SIMULATE=true
set CALIBRATION_SIMULATE=true
echo.
echo Starting CalibrationTuning...
echo.
start "" "bin\Debug\CalibrationTuning.exe"
echo.
echo Application started in simulation mode.
echo Close this window when done.
pause
