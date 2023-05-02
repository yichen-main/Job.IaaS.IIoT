namespace Station.Domain.Shared.Functions.Pools;
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