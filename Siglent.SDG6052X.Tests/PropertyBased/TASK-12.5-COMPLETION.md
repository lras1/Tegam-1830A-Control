# Task 12.5 Completion: Amplitude-Offset Constraint Property Test

## Overview

This document describes the implementation of Task 12.5: Write property test for amplitude-offset constraint. This property-based test verifies that the InputValidator correctly enforces the critical safety constraint that |offset| + (amplitude/2) must not exceed the maximum voltage for the load impedance.

## Implementation Details

### Test Location

**File**: `Siglent.SDG6052X.Tests/PropertyBased/PropertyBasedTests.cs`

**Test Methods**:
1. `Property_AmplitudeOffsetConstraint_EnforcedForAllLoads()` - Comprehensive constraint verification
2. `Property_AmplitudeOffsetConstraint_BoundaryConditions()` - Focused boundary condition testing

### Property 1: Comprehensive Constraint Enforcement

**Test Method**: `Property_AmplitudeOffsetConstraint_EnforcedForAllLoads()`

**Purpose**: Verifies that the amplitude-offset constraint is correctly enforced across all possible amplitude and offset combinations for both 50Ω and High-Z loads.

**Test Strategy**:
- Generates random amplitude and offset values across wide ranges
- Tests both 50Ω and High-Z load impedances
- Calculates peak voltage: |offset| + (amplitude/2)
- Verifies validation passes if and only if:
  - Amplitude is positive and within range
  - Offset is within range
  - Peak voltage ≤ maximum voltage for the load

**Key Validations**:
1. **No False Negatives**: Valid configurations must pass validation
2. **No False Positives**: Invalid configurations must fail validation (critical for safety!)
3. **Error Messages**: Constraint violations should produce descriptive error messages
4. **Load-Specific Limits**: 
   - 50Ω: max voltage = 10V
   - High-Z: max voltage = 20V

**Generator Design**:
```csharp
var parametersGen = from amp in Arb.Default.NormalFloat().Generator
                   from offset in Arb.Default.NormalFloat().Generator
                   from loadType in Gen.Elements(LoadType.FiftyOhm, LoadType.HighZ)
                   where !double.IsNaN(amp.Get) && !double.IsInfinity(amp.Get) &&
                         !double.IsNaN(offset.Get) && !double.IsInfinity(offset.Get)
                   select new { Amplitude, Offset, LoadType };
```

### Property 2: Boundary Condition Testing

**Test Method**: `Property_AmplitudeOffsetConstraint_BoundaryConditions()`

**Purpose**: Specifically targets the boundary conditions of the amplitude-offset constraint to ensure correct behavior at the limits.

**Test Strategy**:
- Generates amplitudes within valid range
- For each amplitude, calculates the maximum allowed offset: maxVoltage - (amplitude/2)
- Generates offsets both within and beyond this limit
- Verifies validation correctly accepts/rejects based on constraint

**Boundary Testing Approach**:
- 50% of generated offsets are valid (within constraint)
- 50% of generated offsets are invalid (violate constraint)
- Tests both positive and negative offsets
- Focuses on values near the constraint boundary

**Key Insight**: This test is particularly effective at finding edge cases where the constraint might be incorrectly implemented (e.g., off-by-one errors, incorrect inequality operators).

## Requirements Validated

This property test validates the following requirements from the design document:

**Requirement 3.3**: Amplitude validation for different load impedances
- 50Ω load: 1 mVpp ≤ amplitude ≤ 20 Vpp
- High-Z load: 2 mVpp ≤ amplitude ≤ 40 Vpp

**Requirement 3.4**: Offset validation for different load impedances
- 50Ω load: -10V ≤ offset ≤ +10V
- High-Z load: -20V ≤ offset ≤ +20V

**Requirement 3.5**: Amplitude-offset constraint enforcement
- The constraint |offset| + (amplitude/2) ≤ maxVoltage must be enforced
- This prevents configurations that would exceed device voltage limits

## Correctness Property

This test validates **Property 9** from the design document:

```
∀ params ∈ WaveformParameters, load ∈ LoadImpedance:
  |params.Offset| + (params.Amplitude / 2) ≤ MaxVoltage(load)
```

**Interpretation**: For all waveform parameters and load impedances, the sum of the absolute offset and half the amplitude must not exceed the maximum voltage for that load.

**Safety Significance**: This constraint is critical for device safety. Violating it could:
- Damage the device output stage
- Damage connected equipment
- Produce incorrect signals
- Void device warranty

## Test Execution

### Running the Tests

```bash
# Run all property-based tests
dotnet test Siglent.SDG6052X.Tests/Siglent.SDG6052X.Tests.csproj --filter "FullyQualifiedName~PropertyBasedTests"

# Run only amplitude-offset constraint tests
dotnet test Siglent.SDG6052X.Tests/Siglent.SDG6052X.Tests.csproj --filter "FullyQualifiedName~Property_AmplitudeOffsetConstraint"
```

### Expected Behavior

**When tests pass**:
- FsCheck generates hundreds of random test cases
- All test cases verify the constraint is correctly enforced
- No counterexamples are found
- Tests complete successfully

**If tests fail**:
- FsCheck will report a counterexample showing the failing case
- The counterexample will include:
  - Amplitude value
  - Offset value
  - Load type
  - Expected validation result
  - Actual validation result
- This indicates a bug in the InputValidator implementation

## Example Test Cases

### Valid Configurations (Should Pass Validation)

**50Ω Load**:
- Amplitude: 10 Vpp, Offset: 0V → Peak: 5V ≤ 10V ✓
- Amplitude: 8 Vpp, Offset: 5V → Peak: 9V ≤ 10V ✓
- Amplitude: 20 Vpp, Offset: 0V → Peak: 10V ≤ 10V ✓

**High-Z Load**:
- Amplitude: 20 Vpp, Offset: 0V → Peak: 10V ≤ 20V ✓
- Amplitude: 30 Vpp, Offset: 5V → Peak: 20V ≤ 20V ✓
- Amplitude: 40 Vpp, Offset: 0V → Peak: 20V ≤ 20V ✓

### Invalid Configurations (Should Fail Validation)

**50Ω Load**:
- Amplitude: 10 Vpp, Offset: 6V → Peak: 11V > 10V ✗
- Amplitude: 20 Vpp, Offset: 5V → Peak: 15V > 10V ✗
- Amplitude: 15 Vpp, Offset: -8V → Peak: 15.5V > 10V ✗

**High-Z Load**:
- Amplitude: 30 Vpp, Offset: 10V → Peak: 25V > 20V ✗
- Amplitude: 40 Vpp, Offset: 5V → Peak: 25V > 20V ✗
- Amplitude: 35 Vpp, Offset: -8V → Peak: 25.5V > 20V ✗

## Integration with Existing Tests

This test completes the property-based testing suite for Phase 12:

- **Task 12.1**: Property-based test infrastructure setup ✓
- **Task 12.2**: Command-parse roundtrip property test ✓
- **Task 12.3**: Validation consistency property test ✓
- **Task 12.4**: Frequency unit conversion property test ✓
- **Task 12.5**: Amplitude-offset constraint property test ✓ (THIS TASK)

All property-based tests work together to provide comprehensive verification of the device library's correctness properties.

## Code Quality

### Test Characteristics

- **Comprehensive**: Tests all combinations of amplitude, offset, and load types
- **Focused**: Includes dedicated boundary condition testing
- **Clear**: Well-documented with inline comments explaining the logic
- **Maintainable**: Uses helper calculations that mirror the validator's logic
- **Robust**: Handles edge cases like negative offsets and extreme values

### Documentation

Each test method includes:
- XML documentation comments
- Requirements validation annotations
- Property description
- Implementation strategy explanation

## Conclusion

Task 12.5 is now complete. The amplitude-offset constraint property tests provide strong verification that the InputValidator correctly enforces this critical safety constraint for both 50Ω and High-Z loads. The tests use FsCheck to generate hundreds of random test cases, ensuring the constraint is enforced across the entire input space.

The implementation includes two complementary tests:
1. Comprehensive constraint enforcement across all values
2. Focused boundary condition testing

Together, these tests provide high confidence that the amplitude-offset constraint is correctly implemented and will prevent unsafe device configurations.

## Next Steps

With all Phase 12 property-based tests complete, the next phase (Phase 13) involves creating the WinForms UI project for user interaction with the device library.
