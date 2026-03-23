# Siglent.SDG6052X.Tests

Unit test project for the Siglent SDG6052X Device Library.

## Project Information

- **Target Framework**: .NET Framework 4.8
- **Project Type**: Class Library (Test Project)
- **Purpose**: Comprehensive testing of the Device Library components

## Test Structure

This project will contain three main categories of tests:

### Unit Tests
- Tests for individual components in isolation
- SCPI Command Builder tests
- SCPI Response Parser tests
- Input Validator tests
- Mock Communication Manager tests

### Integration Tests
- End-to-end tests using the mock communication manager
- Signal Generator Service integration tests
- Multi-layer interaction tests

### Property-Based Tests
- FsCheck-based property tests
- Command-parse roundtrip tests
- Validation consistency tests
- Unit conversion tests

## Dependencies

The following NuGet packages will be added in subsequent tasks:
- NUnit (test framework)
- NUnit3TestAdapter (test adapter for Visual Studio)
- Moq (mocking framework)
- FsCheck (property-based testing)
- Microsoft.NET.Test.Sdk (test SDK)

## Project References

- Siglent.SDG6052X.DeviceLibrary (the library being tested)

## Folder Structure

The test project will be organized as follows:
- `Unit/` - Unit tests for individual components
- `Integration/` - Integration tests
- `PropertyBased/` - Property-based tests using FsCheck
