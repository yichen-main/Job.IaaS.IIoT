using static Infrastructure.Pillbox.Bounds.IFocasHelper;

namespace Infrastructure.Pillbox.Bounds;
public interface IFocasHelper
{
    void Open(string ip, ushort port);
    enum OperatingState
    {
        Idle = 0,
        Run = 1,
        Alarm = 2
    }
    record struct InformationEntity
    {
        public short MaxAxis { get; set; }
        public short ProgramNo { get; set; }
        public short SubroutineNo { get; set; }
        public string ProgramName { get; set; }
        public string AlarmMessage { get; set; }
        public string AxisQuantity { get; set; }
        public string MtTypeName { get; set; }
        public string Version { get; set; }
        public string CNCSeries { get; set; }
        public string CNCTypeName { get; set; }
        public OperatingState OperatingState { get; set; }
        public string AutomaticMode { get; set; }
        public int BootTime { get; set; }
        public int RunTime { get; set; }
        public int OutputCounter { get; set; }
        public short TurretCapacity { get; set; }
        public IEnumerable<Coordinate> Coordinates { get; set; }

        [StructLayout(LayoutKind.Auto)]
        public record struct Coordinate
        {
            public double AbsoluteAxis { get; set; }
            public double RelativeAxis { get; set; }
        }
    }

    [StructLayout(LayoutKind.Auto)]
    record struct PartSpindleEntity
    {
        public int FeedRate { get; set; }
        public int SpindleSpeed { get; set; }
        public int CuttingSpeed { get; set; }
        public int CuttingTime { get; set; }
    }
    InformationEntity Information { get; }
    PartSpindleEntity PartSpindle { get; }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class FocasHelper : FocasDevelop, IFocasHelper
{
    ushort _handle;
    public void Open(string ip, ushort port)
    {
        if (WindowsPass && cnc_allclibhndl3(ip, port, 5, out _handle) is EW_OK)
        {
            PushInformation();
            PushPartSpindle();
            cnc_freelibhndl(_handle);
        }
    }
    void PushInformation()
    {
        InformationEntity result = new();
        ODBPRO programInfo = new();
        var cncRdprgnum = cnc_rdprgnum(_handle, programInfo);
        if (cncRdprgnum is EW_OK)
        {
            result.ProgramNo = programInfo.mdata;
            result.SubroutineNo = programInfo.data;
        }
        ODBEXEPRG programEXEInfo = new();
        var cncExeprgname = cnc_exeprgname(_handle, programEXEInfo);
        if (cncExeprgname is EW_OK)
        {
            var name = new string(programEXEInfo.name);
            new string[] { "\0" }.ForEach(item => name = name.Replace(item, string.Empty));
            result.ProgramName = name.Trim();
        }
        var cncAlarm = cnc_alarm2(_handle, out int alarmMessage);
        if (cncAlarm is EW_OK) result.AlarmMessage = alarmMessage switch
        {
            0 => "Parameter switch on",
            1 => "Power off parameter set",
            2 => "I/O error",
            3 => "Foreground P/S",
            4 => "Overtravel,External data",
            5 => "Overheat alarm",
            6 => "Servo alarm",
            7 => "Data I/O error",
            8 => "Macro alarm",
            9 => "Spindle alarm",
            10 => "Other alarm(DS)",
            11 => "Alarm concerning Malfunction prevent functions",
            12 => "Background P/S",
            13 => "Synchronized error",
            14 => "(reserved)",
            15 => "External alarm message",
            16 => "(reserved)",
            17 => "(reserved)",
            18 => "(reserved)",
            19 => "PMC error",
            _ => "unknown mistake",
        };
        SystemInfo systemInfo = new();
        var cncSysinfo = cnc_sysinfo(_handle, systemInfo);
        if (cncSysinfo is EW_OK)
        {
            result.MaxAxis = systemInfo.max_axis;
            result.AxisQuantity = $"{systemInfo.axes[0]}{systemInfo.axes[1]}";
            result.MtTypeName = $"{systemInfo.mt_type[0]}{systemInfo.mt_type[1]}";
            result.Version = $"{systemInfo.version[0]}{systemInfo.version[1]}{systemInfo.version[2]}{systemInfo.version[3]}";
            result.CNCSeries = $"{systemInfo.series[0]}{systemInfo.series[1]}{systemInfo.series[2]}{systemInfo.series[3]}";
            result.CNCTypeName = $"{systemInfo.cnc_type[0]}{systemInfo.cnc_type[1]}" switch
            {
                "15" => "Series 15/15i",
                "16" => "Series 16/16i",
                "18" => "Series 18/18i",
                "21" => "Series 21/21i",
                "30" => "Series 30i",
                "31" => "Series 31i",
                "32" => "Series 32i",
                "35" => "Series 35i",
                " 0" => "Series 0i",
                "PD" => "Power Mate i-D",
                "PH" => "Power Mate i-H",
                "PM" => "Power Motion i",
                _ => "other",
            };
        }
        ODBST statusInfo = new();
        var cncStatinfo = cnc_statinfo(_handle, statusInfo);
        if (cncStatinfo is EW_OK)
        {
            var operatingState = OperatingState.Idle;
            if (statusInfo.run is 3) operatingState = OperatingState.Run;
            if (statusInfo.alarm is not 0) operatingState = OperatingState.Alarm;
            result.OperatingState = operatingState;
            result.AutomaticMode = statusInfo.aut switch
            {
                0 => "MDI",
                1 => "MEMory",
                2 => "Not Defined",
                3 => "EDIT",
                4 => "h",
                5 => "JOG",
                6 => "Teach in JOG",
                7 => "Teach in h",
                8 => "INC·feed",
                9 => "REFerence",
                10 => "ReMoTe",
                _ => "others mode",
            };
        }
        IODBPSD_1 parameter6751 = new();
        IODBPSD_1 parameter6752 = new();
        var cncRdparamRun = cnc_rdparam(_handle, 6751, 0, 8, parameter6751);
        if (cncRdparamRun is EW_OK)
        {
            var workingTimeSecond = parameter6751.ldata / 1000;
            cncRdparamRun = cnc_rdparam(_handle, 6752, 0, 8, parameter6752);
            if (cncRdparamRun is EW_OK) result.RunTime = parameter6752.ldata * 60 + workingTimeSecond;
        }
        IODBPSD_1 param6750 = new();
        var cncRdparamBoot = cnc_rdparam(_handle, 6750, 0, 8 + 32, param6750);
        if (cncRdparamBoot is EW_OK) result.BootTime = param6750.ldata * 60;
        ODBM productInfo = new();
        var cncRdmacro = cnc_rdmacro(_handle, 0xf3d, 0x0a, productInfo);
        if (cncRdmacro is EW_OK) result.OutputCounter = productInfo.mcr_val / 100000;
        ODBLFNO turretInfo = new();
        var cncRdmaxgrp = cnc_rdmaxgrp(_handle, turretInfo);
        if (cncRdmaxgrp is EW_OK) result.TurretCapacity = turretInfo.data;
        ODBPOS positionInfo = new();
        short axisQuantity = MAX_AXIS;
        List<InformationEntity.Coordinate> coordinates = new();
        var cncRdposition = cnc_rdposition(_handle, -1, ref axisQuantity, positionInfo);
        if (cncRdposition is EW_OK)
        {
            for (int i = 0; i < axisQuantity; i++)
            {
                switch (i)
                {
                    case 0:
                        coordinates.Add(new()
                        {
                            AbsoluteAxis = ToAxis(positionInfo.p1.abs.data * Math.Pow(10, -positionInfo.p1.abs.dec)),
                            RelativeAxis = ToAxis(positionInfo.p1.rel.data * Math.Pow(10, -positionInfo.p1.rel.dec))
                        });
                        break;

                    case 1:
                        coordinates.Add(new()
                        {
                            AbsoluteAxis = ToAxis(positionInfo.p2.abs.data * Math.Pow(10, -positionInfo.p2.abs.dec)),
                            RelativeAxis = ToAxis(positionInfo.p2.rel.data * Math.Pow(10, -positionInfo.p2.rel.dec))
                        });
                        break;

                    case 2:
                        coordinates.Add(new()
                        {
                            AbsoluteAxis = ToAxis(positionInfo.p3.abs.data * Math.Pow(10, -positionInfo.p3.abs.dec)),
                            RelativeAxis = ToAxis(positionInfo.p3.rel.data * Math.Pow(10, -positionInfo.p3.rel.dec))
                        });
                        break;

                    case 3:
                        coordinates.Add(new()
                        {
                            AbsoluteAxis = ToAxis(positionInfo.p4.abs.data * Math.Pow(10, -positionInfo.p4.abs.dec)),
                            RelativeAxis = ToAxis(positionInfo.p4.rel.data * Math.Pow(10, -positionInfo.p4.rel.dec))
                        });
                        break;

                    case 4:
                        coordinates.Add(new()
                        {
                            AbsoluteAxis = ToAxis(positionInfo.p5.abs.data * Math.Pow(10, -positionInfo.p5.abs.dec)),
                            RelativeAxis = ToAxis(positionInfo.p5.rel.data * Math.Pow(10, -positionInfo.p5.rel.dec))
                        });
                        break;

                    case 5:
                        coordinates.Add(new()
                        {
                            AbsoluteAxis = ToAxis(positionInfo.p6.abs.data * Math.Pow(10, -positionInfo.p6.abs.dec)),
                            RelativeAxis = ToAxis(positionInfo.p6.rel.data * Math.Pow(10, -positionInfo.p6.rel.dec))
                        });
                        break;

                    case 6:
                        coordinates.Add(new()
                        {
                            AbsoluteAxis = ToAxis(positionInfo.p7.abs.data * Math.Pow(10, -positionInfo.p7.abs.dec)),
                            RelativeAxis = ToAxis(positionInfo.p7.rel.data * Math.Pow(10, -positionInfo.p7.rel.dec))
                        });
                        break;

                    case 7:
                        coordinates.Add(new()
                        {
                            AbsoluteAxis = ToAxis(positionInfo.p8.abs.data * Math.Pow(10, -positionInfo.p8.abs.dec)),
                            RelativeAxis = ToAxis(positionInfo.p8.rel.data * Math.Pow(10, -positionInfo.p8.rel.dec))
                        });
                        break;
                }
            }
        }
        result.Coordinates = coordinates;
        Information = result;
        static double ToAxis(double value) => Math.Round(value, 3, MidpointRounding.AwayFromZero);
    }
    void PushPartSpindle()
    {
        PartSpindleEntity result = new();
        ODBACT spindleSpeedData = new();
        var cncActs = cnc_acts(_handle, spindleSpeedData);
        if (cncActs is EW_OK) result.SpindleSpeed = spindleSpeedData.data;
        IODBPMC0 pmcData = new();
        var pmcRdpmcrng = pmc_rdpmcrng(_handle, 0, 1, 12, 13, 8 + 1 * 2, pmcData);
        if (pmcRdpmcrng is EW_OK) result.FeedRate = 255 - pmcData.cdata[0];
        ODBACT cuttingData = new();
        var cncActf = cnc_actf(_handle, cuttingData);
        if (cncActf is EW_OK) result.CuttingSpeed = cuttingData.data;
        IODBPSD_1 parameter6753 = new();
        IODBPSD_1 parameter6754 = new();
        var cncRdparam = cnc_rdparam(_handle, 6753, 0, 8 + 32, parameter6753);
        if (cncRdparam is EW_OK)
        {
            var cuttingTimeSecond = parameter6753.ldata / 1000;
            cncRdparam = cnc_rdparam(_handle, 6754, 0, 8 + 32, parameter6754);
            if (cncRdparam is EW_OK) result.CuttingTime = parameter6754.ldata * 60 + cuttingTimeSecond;
        }
        PartSpindle = result;
    }
    internal void WritePMCByte(AddrType addr_type, short location, byte value)
    {
        IODBPMC0 result = new() { cdata = new byte[5] };
        result.cdata[0] = value;
        result.type_a = (short)addr_type;
        result.type_d = (short)LengthType.Byte;
        result.datano_s = location;
        result.datano_e = location;
        pmc_wrpmcrng(_handle, 9, result);
    }
    internal void WritePMCWord(AddrType addr_type, short location, short value)
    {
        IODBPMC1 result = new() { idata = new short[5] };
        result.idata[0] = value;
        result.type_a = (short)addr_type;
        result.type_d = (short)LengthType.Word;
        result.datano_s = location;
        result.datano_e = (short)(location + 1);
        pmc_wrpmcrng(_handle, 10, result);
    }
    internal void WritePMCDWord(AddrType addr_type, short location, int value)
    {
        IODBPMC2 result = new() { ldata = new int[5] };
        result.ldata[0] = value;
        result.type_a = (short)addr_type;
        result.type_d = (short)LengthType.DWord;
        result.datano_s = location;
        result.datano_e = (short)(location + 3);
        pmc_wrpmcrng(_handle, 12, result);
    }
    internal byte ReadPMCByte(AddrType addr_type, ushort location)
    {
        IODBPMC0 result = new();
        if (pmc_rdpmcrng(_handle, (short)addr_type, (short)LengthType.Byte, location, location, 9, result) is EW_OK) return result.cdata[0];
        return default;
    }
    internal short ReadPMCWord(AddrType addr_type, ushort location)
    {
        IODBPMC1 result = new();
        if (pmc_rdpmcrng(_handle, (short)addr_type, (short)LengthType.Word, location, (ushort)(location + 1), 10, result) is EW_OK) return result.idata[0];
        return default;
    }
    internal int ReadPMCDWord(AddrType addr_type, ushort location)
    {
        IODBPMC2 result = new();
        if (pmc_rdpmcrng(_handle, (short)addr_type, (short)LengthType.DWord, location, (ushort)(location + 3), 12, result) is EW_OK) return result.ldata[0];
        return default;
    }
    internal enum LengthType
    {
        Byte = 0,
        Word = 1,
        DWord = 2
    }
    internal enum AddrType
    {
        G = 0,
        F = 1,
        Y = 2,
        X = 3,
        A = 4,
        R = 5,
        T = 6,
        K = 7,
        C = 8,
        D = 9,
        M = 10,
        N = 11,
        E = 12,
        Z = 13
    }
    public InformationEntity Information { get; private set; }
    public PartSpindleEntity PartSpindle { get; private set; }
}