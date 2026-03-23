namespace Tegam._1830A.DeviceLibrary.Models
{
    /// <summary>
    /// Represents a frequency response from the Tegam 1830A device.
    /// </summary>
    public class FrequencyResponse
    {
        public double Frequency { get; set; }
        public FrequencyUnit Unit { get; set; }

        public FrequencyResponse()
        {
        }

        public FrequencyResponse(double frequency, FrequencyUnit unit)
        {
            Frequency = frequency;
            Unit = unit;
        }
    }
}
