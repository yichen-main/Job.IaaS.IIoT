namespace Station.Domain.Functions.Pools;
internal sealed class FoundationPool : IFoundationPool
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