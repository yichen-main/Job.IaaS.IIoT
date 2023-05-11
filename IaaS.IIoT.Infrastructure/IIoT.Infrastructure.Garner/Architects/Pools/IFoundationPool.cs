namespace Infrastructure.Garner.Architects.Pools;
public interface IFoundationPool
{
    void PushDigiwinAttach(in DateTime value);
    void PushModbusAttach(in DateTime value);
    void PushCourierBottom(in DateTime value);
    void PushMonitorBottom(in DateTime value);
    void PushReaderBottom(in DateTime value);
    void PushShellerBottom(in DateTime value);
    void PushWriterBottom(in DateTime value);

    [StructLayout(LayoutKind.Auto)]
    readonly record struct HardwareDiagnosis
    {
        public double UsedMemory { get; init; }
        public double UsedCPUTime { get; init; }
        public int HardDriveFreeSpace { get; init; }
    }
    DateTime DigiwinAttach { get; }
    DateTime ModbusAttach { get; }
    DateTime CourierBottom { get; }
    DateTime MonitorBottom { get; }
    DateTime ReaderBottom { get; }
    DateTime ShellerBottom { get; }
    DateTime WriterBottom { get; }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class FoundationPool : IFoundationPool
{
    public void PushDigiwinAttach(in DateTime value) => DigiwinAttach = value;
    public void PushModbusAttach(in DateTime value) => ModbusAttach = value;
    public void PushCourierBottom(in DateTime value) => CourierBottom = value;
    public void PushMonitorBottom(in DateTime value) => MonitorBottom = value;
    public void PushReaderBottom(in DateTime value) => ReaderBottom = value;
    public void PushShellerBottom(in DateTime value) => ShellerBottom = value;
    public void PushWriterBottom(in DateTime value) => WriterBottom = value;
    public DateTime DigiwinAttach { get; private set; }
    public DateTime ModbusAttach { get; private set; }
    public DateTime CourierBottom { get; private set; }
    public DateTime MonitorBottom { get; private set; }
    public DateTime ReaderBottom { get; private set; }
    public DateTime ShellerBottom { get; private set; }
    public DateTime WriterBottom { get; private set; }
}