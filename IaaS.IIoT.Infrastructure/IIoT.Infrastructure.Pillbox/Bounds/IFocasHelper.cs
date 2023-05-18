using static Infrastructure.Pillbox.Bounds.IFocasHelper;

namespace Infrastructure.Pillbox.Bounds;
public interface IFocasHelper
{
    void Close();
    void Open(string ip, ushort port);
    string GetProgramName();
    BaseInformation GetBaseInformation();
    JobInformation GetJobInformation();
    ProgramInformation GetProgramInformation();
    int GetSpindleSpeed();
    int GetCuttingSpeed();
    int GetFeedRate();
    short GetTurretCapacity();
    int GetOutputCounter();
    int GetCuttingTime();
    int GetRunTime();
    int GetBootTime();
    string AlarmMessage();
    IEnumerable<Coordinate> GetCoordinateAxes();
    void PushTemplate(TemplateEntity template);
    enum OperatingState
    {
        Idle = 0,
        Run = 1,
        Alarm = 2
    }
    readonly record struct BaseInformation
    {
        public required short MaxAxis { get; init; }
        public required string AxisQuantity { get; init; }
        public required string MtTypeName { get; init; }
        public required string Version { get; init; }
        public required string CNCSeries { get; init; }
        public required string CNCTypeName { get; init; }
    }
    readonly record struct JobInformation
    {
        public required OperatingState OperatingState { get; init; }
        public required string AutomaticMode { get; init; }
    }

    [StructLayout(LayoutKind.Auto)]
    readonly record struct ProgramInformation
    {
        public required short MainProgramNo { get; init; }
        public required short SubroutineNo { get; init; }
    }

    [StructLayout(LayoutKind.Auto)]
    public readonly record struct Coordinate
    {
        public required double AbsoluteAxis { get; init; }
        public required double RelativeAxis { get; init; }
    }
    readonly record struct TemplateEntity
    {
        public required string AlarmMessage { get; init; }
        public required string ProgramName { get; init; }
        public required int FeedRate { get; init; }
        public required int SpindleSpeed { get; init; }
        public required int CuttingSpeed { get; init; }
        public required int CuttingTime { get; init; }
        public required int BootTime { get; init; }
        public required int RunTime { get; init; }
        public required short TurretCapacity { get; init; }
        public required BaseInformation BaseInformation { get; init; }
        public required JobInformation JobInformation { get; init; }
        public required ProgramInformation ProgramInformation { get; init; }
        public required IEnumerable<Coordinate> Coordinates { get; init; }
    }
    bool Enabled { get; }
    TemplateEntity Template { get; }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class FocasHelper : FocasDevelop, IFocasHelper
{
    ushort _handle;
    public void Close()
    {
        Enabled = default;
        DisconnectCNC(_handle);
    }
    public void Open(string ip, ushort port)
    {
        if (WindowsPass)
        {
            Enabled = ConnectCNC(ip, port, 5, out _handle) == default;
        }
        else Enabled = default;
    }
    public string GetProgramName()
    {
        ODBEXEPRG info = new();
        var result = cnc_exeprgname(_handle, info);
        if (result is EW_OK)
        {
            var programName = new string(info.name);
            new string[] { "\0" }.ForEach(item =>
            {
                programName = programName.Replace(item, string.Empty);
            });
            return programName.Trim();
        }
        return string.Empty;
    }
    public BaseInformation GetBaseInformation()
    {
        SystemInfo info = new();
        var result = SystemInfoCNC(_handle, info);
        if (result is EW_OK) return new()
        {
            MaxAxis = info.max_axis,
            AxisQuantity = $"{info.axes[0]}{info.axes[1]}",
            MtTypeName = $"{info.mt_type[0]}{info.mt_type[1]}",
            Version = $"{info.version[0]}{info.version[1]}{info.version[2]}{info.version[3]}",
            CNCSeries = $"{info.series[0]}{info.series[1]}{info.series[2]}{info.series[3]}",
            CNCTypeName = $"{info.cnc_type[0]}{info.cnc_type[1]}" switch
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
            }
        };
        return default;
    }
    public JobInformation GetJobInformation()
    {
        ODBST info = new();
        var result = cnc_statinfo(_handle, info);
        if (result is EW_OK)
        {
            var status = OperatingState.Idle;
            if (info.run is 3) status = OperatingState.Run;
            if (info.alarm is not 0) status = OperatingState.Alarm;
            return new()
            {
                OperatingState = status,
                AutomaticMode = info.aut switch
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
                }
            };
        }
        return default;
    }
    public ProgramInformation GetProgramInformation()
    {
        ODBPRO program = new();
        var result = cnc_rdprgnum(_handle, program);
        if (result is EW_OK) return new()
        {
            MainProgramNo = program.mdata,
            SubroutineNo = program.data
        };
        return default;
    }
    public int GetSpindleSpeed()
    {
        ODBACT data = new();
        int spindleSpeed = default;
        var result = cnc_acts(_handle, data);
        if (result is EW_OK) spindleSpeed = data.data;
        return spindleSpeed;
    }
    public int GetCuttingSpeed()
    {
        ODBACT data = new();
        int cuttingSpeed = default;
        var result = cnc_actf(_handle, data);
        if (result is EW_OK) cuttingSpeed = data.data;
        return cuttingSpeed;
    }
    public int GetFeedRate()
    {
        IODBPMC0 pmc = new();
        int feedRate = default;
        var result = pmc_rdpmcrng(_handle, 0, 1, 12, 13, 8 + 1 * 2, pmc);
        if (result is EW_OK) feedRate = 255 - pmc.cdata[0];
        return feedRate;
    }
    public short GetTurretCapacity()
    {
        ODBLFNO info = new();
        short turretCapacity = default;
        var result = cnc_rdmaxgrp(_handle, info);
        if (result is EW_OK) return info.data;
        return turretCapacity;
    }
    public int GetOutputCounter()
    {
        ODBM info = new();
        int outputCounter = default;
        var result = cnc_rdmacro(_handle, 0xf3d, 0x0a, info);
        if (result is EW_OK) outputCounter = info.mcr_val / 100000;
        return outputCounter;
    }
    public int GetCuttingTime()
    {
        int cuttingTime = default;
        IODBPSD_1 parameter6753 = new();
        IODBPSD_1 parameter6754 = new();
        var result = cnc_rdparam(_handle, 6753, 0, 8 + 32, parameter6753);
        if (result is EW_OK)
        {
            var cuttingTimeSecond = parameter6753.ldata / 1000;
            result = cnc_rdparam(_handle, 6754, 0, 8 + 32, parameter6754);
            if (result is EW_OK) cuttingTime = parameter6754.ldata * 60 + cuttingTimeSecond;
        }
        return cuttingTime;
    }
    public int GetRunTime()
    {
        int runTime = default;
        IODBPSD_1 parameter6751 = new();
        IODBPSD_1 parameter6752 = new();
        var result = cnc_rdparam(_handle, 6751, 0, 8, parameter6751);
        if (result is EW_OK)
        {
            var workingTimeSecond = parameter6751.ldata / 1000;
            result = cnc_rdparam(_handle, 6752, 0, 8, parameter6752);
            if (result is EW_OK) runTime = parameter6752.ldata * 60 + workingTimeSecond;
        }
        return runTime;
    }
    public int GetBootTime()
    {
        int bootTime = default;
        IODBPSD_1 param6750 = new();
        var ret = cnc_rdparam(_handle, 6750, 0, 8 + 32, param6750);
        if (ret is EW_OK) bootTime = param6750.ldata * 60;
        return bootTime;
    }
    public IEnumerable<Coordinate> GetCoordinateAxes()
    {
        ODBPOS position = new();
        short axisQuantity = MAX_AXIS;
        List<Coordinate> axes = new();
        var result = cnc_rdposition(_handle, -1, ref axisQuantity, position);
        if (result is EW_OK)
        {
            for (int i = 0; i < axisQuantity; i++)
            {
                switch (i)
                {
                    case 0:
                        axes.Add(new()
                        {
                            AbsoluteAxis = To(position.p1.abs.data * Math.Pow(10, -position.p1.abs.dec)),
                            RelativeAxis = To(position.p1.rel.data * Math.Pow(10, -position.p1.rel.dec))
                        });
                        break;

                    case 1:
                        axes.Add(new()
                        {
                            AbsoluteAxis = To(position.p2.abs.data * Math.Pow(10, -position.p2.abs.dec)),
                            RelativeAxis = To(position.p2.rel.data * Math.Pow(10, -position.p2.rel.dec))
                        });
                        break;

                    case 2:
                        axes.Add(new()
                        {
                            AbsoluteAxis = To(position.p3.abs.data * Math.Pow(10, -position.p3.abs.dec)),
                            RelativeAxis = To(position.p3.rel.data * Math.Pow(10, -position.p3.rel.dec))
                        });
                        break;

                    case 3:
                        axes.Add(new()
                        {
                            AbsoluteAxis = To(position.p4.abs.data * Math.Pow(10, -position.p4.abs.dec)),
                            RelativeAxis = To(position.p4.rel.data * Math.Pow(10, -position.p4.rel.dec))
                        });
                        break;

                    case 4:
                        axes.Add(new()
                        {
                            AbsoluteAxis = To(position.p5.abs.data * Math.Pow(10, -position.p5.abs.dec)),
                            RelativeAxis = To(position.p5.rel.data * Math.Pow(10, -position.p5.rel.dec))
                        });
                        break;

                    case 5:
                        axes.Add(new()
                        {
                            AbsoluteAxis = To(position.p6.abs.data * Math.Pow(10, -position.p6.abs.dec)),
                            RelativeAxis = To(position.p6.rel.data * Math.Pow(10, -position.p6.rel.dec))
                        });
                        break;

                    case 6:
                        axes.Add(new()
                        {
                            AbsoluteAxis = To(position.p7.abs.data * Math.Pow(10, -position.p7.abs.dec)),
                            RelativeAxis = To(position.p7.rel.data * Math.Pow(10, -position.p7.rel.dec))
                        });
                        break;

                    case 7:
                        axes.Add(new()
                        {
                            AbsoluteAxis = To(position.p8.abs.data * Math.Pow(10, -position.p8.abs.dec)),
                            RelativeAxis = To(position.p8.rel.data * Math.Pow(10, -position.p8.rel.dec))
                        });
                        break;
                }

            }
        }
        static double To(double value) => Math.Round(value, 3, MidpointRounding.AwayFromZero);
        return axes;
    }
    public string AlarmMessage()
    {
        var alarmMessage = string.Empty;
        var result = cnc_alarm2(_handle, out int value);
        if (result is EW_OK) alarmMessage = value switch
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
        return alarmMessage;
    }
    public void PushTemplate(TemplateEntity template) => Template = template;
    public bool Enabled { get; private set; }
    public TemplateEntity Template { get; private set; }
}