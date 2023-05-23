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
        public OperatingState OperatingState { get; set; }
        public string AlarmMessage { get; set; }
        public string CNCSeries { get; set; }
        public string CNCTypeName { get; set; }
        public string Version { get; set; }
        public short MaxAxis { get; set; }
        public string AxisQuantity { get; set; }
        public short TurretCapacity { get; set; }
        public short ProgramNo { get; set; }
        public short SubroutineNo { get; set; }
        public string ProgramName { get; set; }
        public string MtTypeName { get; set; }
        public string AutomaticMode { get; set; }
        public int BootTime { get; set; }
        public int RunTime { get; set; }
        public int OutputCounter { get; set; }
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
        public int RotationalFrequency { get; set; }
        public int FeedRate { get; set; }
        public int CuttingSpeed { get; set; }
        public int CuttingTime { get; set; }
        public uint OpenCutterCount { get; set; }
        public IEnumerable<SpeedTimeMileage> SpeedTimeMileages { get; set; }
        public readonly record struct SpeedTimeMileage
        {
            public required int SerialNo { get; init; }
            public required double TimeConsuming { get; init; }
            public required string Description { get; init; }
        }
    }
    bool Connected { get; }
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
            Connected = true;
        }
        else Connected = default;
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
        if (cncActs is EW_OK) result.RotationalFrequency = spindleSpeedData.data;

        //G12 Feed rate
        var pmcRdpmcrng = ReadPMCWord(Register.G, 12);
        if (pmcRdpmcrng is EW_OK) result.FeedRate = 255 - pmcRdpmcrng;

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
        result.OpenCutterCount = 0;
        result.SpeedTimeMileages = new[]
        {
            new PartSpindleEntity.SpeedTimeMileage
            {
                SerialNo = 1,
                TimeConsuming = ReadPMCDWord(Register.D, 1004) +
                Math.Round(TimeSpan.FromSeconds(ReadPMCDWord(Register.D, 1000)).TotalHours, 2, MidpointRounding.AwayFromZero),
                Description = "30~2029RPM"
            },
            new PartSpindleEntity.SpeedTimeMileage
            {
                SerialNo = 2,
                TimeConsuming = ReadPMCDWord(Register.D, 1012) +
                Math.Round(TimeSpan.FromSeconds(ReadPMCDWord(Register.D, 10008)).TotalHours, 2, MidpointRounding.AwayFromZero),
                Description = "2030~4029RPM"
            },
            new PartSpindleEntity.SpeedTimeMileage
            {
                SerialNo = 3,
                TimeConsuming = ReadPMCDWord(Register.D, 1020) +
                Math.Round(TimeSpan.FromSeconds(ReadPMCDWord(Register.D, 1016)).TotalHours, 2, MidpointRounding.AwayFromZero),
                Description = "4030~6029RPM"
            },
            new PartSpindleEntity.SpeedTimeMileage
            {
                SerialNo = 4,
                TimeConsuming = ReadPMCDWord(Register.D, 1028) +
                Math.Round(TimeSpan.FromSeconds(ReadPMCDWord(Register.D, 1024)).TotalHours, 2, MidpointRounding.AwayFromZero),
                Description = "6030~8029RPM"
            },
            new PartSpindleEntity.SpeedTimeMileage
            {
                SerialNo = 5,
                TimeConsuming = ReadPMCDWord(Register.D, 1036) +
                Math.Round(TimeSpan.FromSeconds(ReadPMCDWord(Register.D, 1032)).TotalHours, 2, MidpointRounding.AwayFromZero),
                Description = "8030~10029RPM"
            },
            new PartSpindleEntity.SpeedTimeMileage
            {
                SerialNo = 6,
                TimeConsuming = ReadPMCDWord(Register.D, 1044) +
                Math.Round(TimeSpan.FromSeconds(ReadPMCDWord(Register.D, 1040)).TotalHours, 2, MidpointRounding.AwayFromZero),
                Description = "10030~12029RPM"
            },
            new PartSpindleEntity.SpeedTimeMileage
            {
                SerialNo = 7,
                TimeConsuming = ReadPMCDWord(Register.D, 1052) +
                Math.Round(TimeSpan.FromSeconds(ReadPMCDWord(Register.D, 1048)).TotalHours, 2, MidpointRounding.AwayFromZero),
                Description = "12030~14029RPM"
            },
            new PartSpindleEntity.SpeedTimeMileage
            {
                SerialNo = 8,
                TimeConsuming = ReadPMCDWord(Register.D, 1060) +
                Math.Round(TimeSpan.FromSeconds(ReadPMCDWord(Register.D, 1056)).TotalHours, 2, MidpointRounding.AwayFromZero),
                Description = "14030~16029RPM"
            },
            new PartSpindleEntity.SpeedTimeMileage
            {
                SerialNo = 9,
                TimeConsuming = ReadPMCDWord(Register.D, 1068) +
                Math.Round(TimeSpan.FromSeconds(ReadPMCDWord(Register.D, 1064)).TotalHours, 2, MidpointRounding.AwayFromZero),
                Description = "16030~18029RPM"
            },
            new PartSpindleEntity.SpeedTimeMileage
            {
                SerialNo = 10,
                TimeConsuming = ReadPMCDWord(Register.D, 1076) +
                Math.Round(TimeSpan.FromSeconds(ReadPMCDWord(Register.D, 1072)).TotalHours, 2, MidpointRounding.AwayFromZero),
                Description = "18030~20029RPM"
            },
            new PartSpindleEntity.SpeedTimeMileage
            {
                SerialNo = 11,
                TimeConsuming = ReadPMCDWord(Register.D, 1084) +
                Math.Round(TimeSpan.FromSeconds(ReadPMCDWord(Register.D, 1080)).TotalHours, 2, MidpointRounding.AwayFromZero),
                Description = "20030~22029RPM"
            },
            new PartSpindleEntity.SpeedTimeMileage
            {
                SerialNo = 12,
                TimeConsuming = ReadPMCDWord(Register.D, 1092) +
                Math.Round(TimeSpan.FromSeconds(ReadPMCDWord(Register.D, 1088)).TotalHours, 2, MidpointRounding.AwayFromZero),
                Description = "22030~24029RPM"
            },
            new PartSpindleEntity.SpeedTimeMileage
            {
                SerialNo = 13,
                TimeConsuming = ReadPMCDWord(Register.D, 1100) +
                Math.Round(TimeSpan.FromSeconds(ReadPMCDWord(Register.D, 1096)).TotalHours, 2, MidpointRounding.AwayFromZero),
                Description = "24030~26029RPM"
            },
            new PartSpindleEntity.SpeedTimeMileage
            {
                SerialNo = 14,
                TimeConsuming = ReadPMCDWord(Register.D, 1108) +
                Math.Round(TimeSpan.FromSeconds(ReadPMCDWord(Register.D, 1104)).TotalHours, 2, MidpointRounding.AwayFromZero),
                Description = "26030~28029RPM"
            },
            new PartSpindleEntity.SpeedTimeMileage
            {
                SerialNo = 15,
                TimeConsuming = ReadPMCDWord(Register.D, 1116) +
                Math.Round(TimeSpan.FromSeconds(ReadPMCDWord(Register.D, 1112)).TotalHours, 2, MidpointRounding.AwayFromZero),
                Description = "28030~30029RPM"
            },
            new PartSpindleEntity.SpeedTimeMileage
            {
                SerialNo = 16,
                TimeConsuming = ReadPMCDWord(Register.D, 1194) +
                Math.Round(TimeSpan.FromSeconds(ReadPMCDWord(Register.D, 1190)).TotalHours, 2, MidpointRounding.AwayFromZero),
                Description = "30~30029RPM"
            }
        };
        PartSpindle = result;
    }
    internal void WritePMCByte(Register register, short address, byte value)
    {
        IODBPMC0 result = new() { cdata = new byte[5] };
        result.cdata[default] = value;
        result.type_a = (short)register;
        result.type_d = (short)Length.Byte;
        result.datano_s = address;
        result.datano_e = address;
        pmc_wrpmcrng(_handle, 9, result);
    }
    internal void WritePMCWord(Register register, short address, short value)
    {
        IODBPMC1 result = new() { idata = new short[5] };
        result.idata[default] = value;
        result.type_a = (short)register;
        result.type_d = (short)Length.Word;
        result.datano_s = address;
        result.datano_e = (short)(address + 1);
        pmc_wrpmcrng(_handle, 10, result);
    }
    internal void WritePMCDWord(Register register, short address, int value)
    {
        IODBPMC2 result = new() { ldata = new int[5] };
        result.ldata[default] = value;
        result.type_a = (short)register;
        result.type_d = (short)Length.DWord;
        result.datano_s = address;
        result.datano_e = (short)(address + 3);
        pmc_wrpmcrng(_handle, 12, result);
    }
    internal byte ReadPMCByte(Register register, ushort address)
    {
        IODBPMC0 result = new();
        if (pmc_rdpmcrng(_handle, (short)register, (short)Length.Byte, address, address, 9, result) is EW_OK) return result.cdata[default];
        return default;
    }
    short ReadPMCWord(Register register, ushort address)
    {
        IODBPMC1 result = new();
        if (pmc_rdpmcrng(_handle, (short)register, (short)Length.Word, address, (ushort)(address + 1), 10, result) is EW_OK) return result.idata[default];
        return default;
    }
    int ReadPMCDWord(Register register, ushort address)
    {
        IODBPMC2 result = new();
        if (pmc_rdpmcrng(_handle, (short)register, (short)Length.DWord, address, (ushort)(address + 3), 12, result) is EW_OK) return result.ldata[default];
        return default;
    }
    internal enum Length
    {
        Byte = 0,
        Word = 1,
        DWord = 2
    }
    internal enum Register
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
    public bool Connected { get; private set; }
    public InformationEntity Information { get; private set; }
    public PartSpindleEntity PartSpindle { get; private set; }
}