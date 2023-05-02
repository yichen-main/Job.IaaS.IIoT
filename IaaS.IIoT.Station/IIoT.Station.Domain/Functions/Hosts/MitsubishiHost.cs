using static Station.Domain.Shared.Functions.Hosts.IMitsubishiHost;

namespace Station.Domain.Functions.Hosts;
internal sealed class MitsubishiHost : AttachDevelop, IMitsubishiHost
{
    public async ValueTask CreateAsync(string ip, int port)
    {
        try
        {
            var token = new CancellationTokenSource(TimeSpan.FromSeconds(15)).Token;
            Warship = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true,
                SendTimeout = 15
            };
            await Warship.ConnectAsync(ip, port, token);
            await PartGateAsync(Action(20300, 1, DeviceCode.M), token);
            await FixtureGateAsync(Action(20316, 1, DeviceCode.M), token);
            await AlarmGateAsync(Action(20332, 1, DeviceCode.M), token);
            await MetaParameterAsync(Action(3000, 1, DeviceCode.D), token);
            await MaintanenceItemAsync(Action(9571, 29, DeviceCode.R), token);
            await SpindleMileageAsync(Action(910, 15, DeviceCode.D), Action(9200, 30, DeviceCode.R), token);
            if (Histories.Any()) Histories.Clear();
        }
        catch (Exception e)
        {
            if (Array.IndexOf(new[]
            {
                "No route to host",
                "Connection refused",
                "The operation was canceled."
            }, e.Message) is Timeout.Infinite)
            {
                if (!Histories.Contains(e.Message))
                {
                    Histories.Add(e.Message);
                    HistoryEngine.Record(new IHistoryEngine.SenderPayload
                    {
                        Name = nameof(MainText.TextController.TextType.Mitsubishi),
                        Message = e.Message
                    });
                }
            }
        }
        finally
        {
            if (Warship is not null)
            {
                if (Warship.Connected)
                {
                    Warship.Shutdown(SocketShutdown.Both);
                    Warship.Close();
                }
                Warship.Dispose();
            }
        }
    }
    async ValueTask PartGateAsync(byte[] values, CancellationToken token)
    {
        await Warship!.SendAsync(values, token);
        var buffers = StructuralEngine.BytePool.Rent(16);
        var status = ConvertBinary(BitConverter.ToString(buffers, default, await Warship.ReceiveAsync(buffers, token)));
        MitsubishiPool.Push(new IMitsubishiPool.PartGate
        {
            Ecomode = status[0],
            CuttingFluidMotor = status[1],
            ChassisCleanerMotor = status[2],
            ChipRemovalMotor = status[3],
            ChipRemovalBackwashMotor = status[4],
            CoolantThroughSpindleMotor = status[5],
            PumpMotor = status[6]
        });
        StructuralEngine.BytePool.Return(buffers);
    }
    async ValueTask FixtureGateAsync(byte[] values, CancellationToken token)
    {
        await Warship!.SendAsync(values, token);
        var buffers = StructuralEngine.BytePool.Rent(16);
        var status = ConvertBinary(BitConverter.ToString(buffers, default, await Warship.ReceiveAsync(buffers, token)));
        MitsubishiPool.Push(new IMitsubishiPool.FixtureGate
        {
            SpindleClamp = status[0],
            MachineClamp = status[1],
            MachineUnclamp = status[2],
            ToolUnclamp = status[3],
            ToolClamp = status[4],
            ToolManualRelaxation = status[5],
            ToolSetUpper = status[6],
            ToolSetLower = status[7],
            ToolMagazineCounter = status[8],
            ArmZeroPoint = status[9],
            ArmStopPoint = status[10],
            ArmPoint60Degrees = status[11],
            ArmPoint180Degrees = status[12]
        });
        StructuralEngine.BytePool.Return(buffers);
    }
    async ValueTask AlarmGateAsync(byte[] values, CancellationToken token)
    {
        await Warship!.SendAsync(values, token);
        var buffers = StructuralEngine.BytePool.Rent(16);
        var status = ConvertBinary(BitConverter.ToString(buffers, default, await Warship.ReceiveAsync(buffers, token)));
        MitsubishiPool.Push(new IMitsubishiPool.AlarmGate
        {
            DoorInterlock = status[0],
            SpindleJudgmentNoTool = status[1],
            RotaryTableOverheat = status[2],
            MotorOverload = status[3],
            AirPressureAlarm = status[4],
            SpindleCoolerAlarm = status[5],
            LubeAlarm = status[6],
            LubePressureAlarm = status[7],
            CoolantTankHigh = status[8],
            CoolantTankLow = status[9]
        });
        StructuralEngine.BytePool.Return(buffers);
    }
    async ValueTask MetaParameterAsync(byte[] values, CancellationToken token)
    {
        await Warship!.SendAsync(values, token);
        var buffers = StructuralEngine.BytePool.Rent(16);
        var receives = ConvertDecimal(BitConverter.ToString(buffers, default, await Warship.ReceiveAsync(buffers, token))).ToArray();
        StructuralEngine.BytePool.Return(buffers);
        MitsubishiPool.Push(receives[0] switch
        {
            1 => IRootInformation.Data.MachineStatusType.Run,
            2 => IRootInformation.Data.MachineStatusType.Error,
            3 => IRootInformation.Data.MachineStatusType.Repair,
            _ => IRootInformation.Data.MachineStatusType.Idle
        });
    }
    async ValueTask MaintanenceItemAsync(byte[] values, CancellationToken token)
    {
        await Warship!.SendAsync(values, token);
        var buffers = StructuralEngine.BytePool.Rent(128);
        var receives = ConvertDecimal(BitConverter.ToString(buffers, default, await Warship.ReceiveAsync(buffers, token))).ToArray();
        StructuralEngine.BytePool.Return(buffers);
        MitsubishiPool.Push(new IMitsubishiPool.MaintenanceCycleData
        {
            Weeklies = new[]
            {
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 1,
                    CumulativeDay = receives[0]
                },
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 2,
                    CumulativeDay = receives[1]
                },
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 3,
                    CumulativeDay = receives[2]
                },
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 4,
                    CumulativeDay = receives[3]
                },
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 5,
                    CumulativeDay = receives[4]
                },
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 6,
                    CumulativeDay = receives[5]
                },
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 7,
                    CumulativeDay = receives[6]
                },
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 8,
                    CumulativeDay = receives[7]
                },
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 9,
                    CumulativeDay = receives[8]
                }
            },
            Monthlies = new[]
            {
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 1,
                    CumulativeDay = receives[9]
                },
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 2,
                    CumulativeDay = receives[10]
                },
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 3,
                    CumulativeDay = receives[11]
                },
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 4,
                    CumulativeDay = receives[12]
                },
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 5,
                    CumulativeDay = receives[13]
                },
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 6,
                    CumulativeDay = receives[14]
                },
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 7,
                    CumulativeDay = receives[15]
                },
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 8,
                    CumulativeDay = receives[16]
                },
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 9,
                    CumulativeDay = receives[17]
                },
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 10,
                    CumulativeDay = receives[18]
                },
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 11,
                    CumulativeDay = receives[19]
                }
            },
            Years = new[]
            {
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 1,
                    CumulativeDay = receives[20]
                },
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 2,
                    CumulativeDay = receives[21]
                },
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 3,
                    CumulativeDay = receives[22]
                },
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 4,
                    CumulativeDay = receives[23]
                },
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 5,
                    CumulativeDay = receives[24]
                },
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 6,
                    CumulativeDay = receives[25]
                },
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 7,
                    CumulativeDay = receives[26]
                },
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 8,
                    CumulativeDay = receives[27]
                },
                new IMitsubishiPool.MaintenanceCycleData.Meta
                {
                    SerialNo = 9,
                    CumulativeDay = receives[28]
                }
            }
        });
    }
    async ValueTask SpindleMileageAsync(byte[] flashes, byte[] keeps, CancellationToken token)
    {
        await Warship!.SendAsync(flashes, token);
        var flasheBuffers = StructuralEngine.BytePool.Rent(64);
        var flasheReceives = ConvertDecimal(BitConverter.ToString(flasheBuffers, default, await Warship.ReceiveAsync(flasheBuffers, token))).ToArray();
        StructuralEngine.BytePool.Return(flasheBuffers);
        await Warship.SendAsync(keeps, token);
        var keepBuffers = StructuralEngine.BytePool.Rent(128);
        var keepReceives = ConvertDecimal(BitConverter.ToString(keepBuffers, default, await Warship.ReceiveAsync(keepBuffers, token))).ToArray();
        StructuralEngine.BytePool.Return(keepBuffers);
        MitsubishiPool.Push(new[]
        {
            new IMitsubishiPool.SpindleSpeedOdometer
            {
                SerialNo = 1,
                Hour = keepReceives[1],
                Minute = keepReceives[0],
                Second = flasheReceives[0],
                Description = "1~2000RPM"
            },
            new IMitsubishiPool.SpindleSpeedOdometer
            {
                SerialNo = 2,
                Hour = keepReceives[3],
                Minute = keepReceives[2],
                Second = flasheReceives[1],
                Description = "2001~4000RPM"
            },
            new IMitsubishiPool.SpindleSpeedOdometer
            {
                SerialNo = 3,
                Hour = keepReceives[5],
                Minute = keepReceives[4],
                Second = flasheReceives[2],
                Description = "4001~6000RPM"
            },
            new IMitsubishiPool.SpindleSpeedOdometer
            {
                SerialNo = 4,
                Hour = keepReceives[7],
                Minute = keepReceives[6],
                Second = flasheReceives[3],
                Description = "6001~8000RPM"
            },
            new IMitsubishiPool.SpindleSpeedOdometer
            {
                SerialNo = 5,
                Hour = keepReceives[9],
                Minute = keepReceives[8],
                Second = flasheReceives[4],
                Description = "8001~10000RPM"
            },
            new IMitsubishiPool.SpindleSpeedOdometer
            {
                SerialNo = 6,
                Hour = keepReceives[11],
                Minute = keepReceives[10],
                Second = flasheReceives[5],
                Description = "10001~12000RPM"
            },
            new IMitsubishiPool.SpindleSpeedOdometer
            {
                SerialNo = 7,
                Hour = keepReceives[13],
                Minute = keepReceives[12],
                Second = flasheReceives[6],
                Description = "12001~14000RPM"
            },
            new IMitsubishiPool.SpindleSpeedOdometer
            {
                SerialNo = 8,
                Hour = keepReceives[15],
                Minute = keepReceives[14],
                Second = flasheReceives[7],
                Description = "14001~16000RPM"
            },
            new IMitsubishiPool.SpindleSpeedOdometer
            {
                SerialNo = 9,
                Hour = keepReceives[17],
                Minute = keepReceives[16],
                Second = flasheReceives[8],
                Description = "16001~18000RPM"
            },
            new IMitsubishiPool.SpindleSpeedOdometer
            {
                SerialNo = 10,
                Hour = keepReceives[19],
                Minute = keepReceives[18],
                Second = flasheReceives[9],
                Description = "18001~20000RPM"
            },
            new IMitsubishiPool.SpindleSpeedOdometer
            {
                SerialNo = 11,
                Hour = keepReceives[21],
                Minute = keepReceives[20],
                Second = flasheReceives[10],
                Description = "20001~22000RPM"
            },
            new IMitsubishiPool.SpindleSpeedOdometer
            {
                SerialNo = 12,
                Hour = keepReceives[23],
                Minute = keepReceives[22],
                Second = flasheReceives[11],
                Description = "22001~24000RPM"
            },
            new IMitsubishiPool.SpindleSpeedOdometer
            {
                SerialNo = 13,
                Hour = keepReceives[25],
                Minute = keepReceives[24],
                Second = flasheReceives[12],
                Description = "24001~26000RPM"
            },
            new IMitsubishiPool.SpindleSpeedOdometer
            {
                SerialNo = 14,
                Hour = keepReceives[27],
                Minute = keepReceives[26],
                Second = flasheReceives[13],
                Description = "26001~28000RPM"
            },
            new IMitsubishiPool.SpindleSpeedOdometer
            {
                SerialNo = 15,
                Hour = keepReceives[29],
                Minute = keepReceives[28],
                Second = flasheReceives[14],
                Description = "28001~30000RPM"
            }
        });
    }
    static byte[] Action(int serialNo, int length, DeviceCode code) => HexBytes(new ReadText
    {
        Fixed = FixedPart.FixedHead,
        Timer = FixedPart.DataTimer,
        Length = FixedPart.DataLength,
        Command = FixedPart.ReadCommand,
        SubCommand = FixedPart.MultiPoint,
        DeviceCode = code.GetDescription(),
        StartPoint = ConvertThreeWordHEX(serialNo),
        Quantity = ConvertTwoWordHEX(length)
    });
    static byte[] HexBytes(ReadText text)
    {
        var tag = $"{text.Fixed}{text.Length}{text.Timer}{text.Command}{text.SubCommand}{text.StartPoint}{text.DeviceCode}{text.Quantity}";
        var results = new byte[tag.Length / 2];
        {
            for (int item = default; item < results.Length; item++) results[item] = Convert.ToByte(tag.Substring(item * 2, 2).Trim(), 16);
            return results;
        }
    }
    static IEnumerable<int> ConvertDecimal(string receive)
    {
        var lower = string.Empty;
        var bytes = PullBody(receive);
        for (int item = default; item < bytes.Length; item++)
        {
            if (item % 2 is 0) lower = bytes[item];
            else yield return Convert.ToInt32($"{bytes[item]}{lower}", 16);
        }
    }
    static byte[] ConvertBinary(string receive)
    {
        var bytes = PullBody(receive);
        return Convert.ToString(int.Parse($"{bytes[1]}{bytes[0]}"), 2).PadRight(16, '0').ToCharArray().Select(item =>
        byte.Parse(item.ToString())).ToArray();
    }
    static string[] PullBody(string receive) => receive.Split('-').Skip(11).ToArray();
    Socket? Warship { get; set; }
    List<string> Histories { get; init; } = new();
    public required IHistoryEngine HistoryEngine { get; init; }
    public required IStructuralEngine StructuralEngine { get; init; }
    public required IMitsubishiPool MitsubishiPool { get; init; }
}