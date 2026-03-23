namespace Tegam._1830A.DeviceLibrary.Models
{
    /// <summary>
    /// Represents the identity information of the Tegam 1830A device.
    /// </summary>
    public class DeviceIdentity
    {
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string FirmwareVersion { get; set; }

        public DeviceIdentity()
        {
        }

        public DeviceIdentity(string manufacturer, string model, string serialNumber, string firmwareVersion)
        {
            Manufacturer = manufacturer;
            Model = model;
            SerialNumber = serialNumber;
            FirmwareVersion = firmwareVersion;
        }
    }
}
