namespace Station.Application.Additions.Triggers;
public sealed class QueueTrigger : IEntranceTrigger
{
    public void Build() => StructuralEngine.Transport.InterceptingPublishAsync += @event => Task.Run(() =>
    {
        try
        {
            var paths = @event.ApplicationMessage.Topic.Split('/');
            var text = Encoding.UTF8.GetString(@event.ApplicationMessage.PayloadSegment);
            switch (paths.Length)
            {
                case 1:
                    switch (paths[0])
                    {
                        case var item when item.Equals("VTM415_TC", StringComparison.OrdinalIgnoreCase):
                            QueueWrapper.Interior.PushSpindleThermalCompensation(text);
                            break;

                        case var item when item.Equals("VUX400_TC", StringComparison.OrdinalIgnoreCase):
                            QueueWrapper.Interior.PushSpindleThermalCompensation(text);
                            break;

                        case var item when item.Equals("UCT600_TC", StringComparison.OrdinalIgnoreCase):
                            QueueWrapper.Interior.PushSpindleThermalCompensation(text);
                            break;
                    }
                    break;

                case 7:
                    {
                        switch (paths[0])
                        {
                            case "":
                                switch (paths[1])
                                {
                                    #region ICP DAS
                                    case "vmx1020":
                                        switch (paths[2])
                                        {
                                            case "wise5231":
                                                switch (paths[3])
                                                {
                                                    case "com3":
                                                        switch (paths[4])
                                                        {
                                                            case "no1":
                                                                switch (paths[5])
                                                                {
                                                                    case "input_register":
                                                                        switch (paths[6])
                                                                        {
                                                                            case "4406":
                                                                                QueueWrapper.Icpdas.PushWaterPumpMotorAverageVoltage(text.ToObject<IIcpdasQueue.Meta>());
                                                                                break;

                                                                            case "4408":
                                                                                QueueWrapper.Icpdas.PushWaterPumpMotorAverageCurrent(text.ToObject<IIcpdasQueue.Meta>());
                                                                                break;

                                                                            case "4410":
                                                                                QueueWrapper.Icpdas.PushWaterPumpMotorApparentPower(text.ToObject<IIcpdasQueue.Meta>());
                                                                                break;
                                                                        }
                                                                        break;
                                                                }
                                                                break;

                                                            case "no2":
                                                                switch (paths[5])
                                                                {
                                                                    case "input_register":
                                                                        switch (paths[6])
                                                                        {
                                                                            case "4406":
                                                                                QueueWrapper.Icpdas.PushElectricalBoxAverageVoltage(text.ToObject<IIcpdasQueue.Meta>());
                                                                                break;

                                                                            case "4408":
                                                                                QueueWrapper.Icpdas.PushElectricalBoxAverageCurrent(text.ToObject<IIcpdasQueue.Meta>());
                                                                                break;

                                                                            case "4410":
                                                                                QueueWrapper.Icpdas.PushElectricalBoxApparentPower(text.ToObject<IIcpdasQueue.Meta>());
                                                                                break;
                                                                        }
                                                                        break;
                                                                }
                                                                break;

                                                            case "no3":
                                                                switch (paths[5])
                                                                {
                                                                    case "ai":
                                                                        switch (paths[6])
                                                                        {
                                                                            case "0":
                                                                                QueueWrapper.Icpdas.PushElectricalBoxHumidity(text.ToObject<IIcpdasQueue.Meta>());
                                                                                break;

                                                                            case "1":
                                                                                QueueWrapper.Icpdas.PushElectricalBoxTemperature(text.ToObject<IIcpdasQueue.Meta>());
                                                                                break;
                                                                        }
                                                                        break;
                                                                }
                                                                break;

                                                            case "no5":
                                                                switch (paths[5])
                                                                {
                                                                    case "ai":
                                                                        switch (paths[6])
                                                                        {
                                                                            case "0":
                                                                                QueueWrapper.Icpdas.PushCuttingFluidTemperature(text.ToObject<IIcpdasQueue.Meta>());
                                                                                break;
                                                                        }
                                                                        break;
                                                                }
                                                                break;
                                                        }
                                                        break;

                                                    case "com4":
                                                        switch (paths[4])
                                                        {
                                                            case "no1":
                                                                switch (paths[5])
                                                                {
                                                                    case "holding_register":
                                                                        switch (paths[6])
                                                                        {
                                                                            case "0":
                                                                                QueueWrapper.Icpdas.PushCuttingFluidPotentialOfHydrogen(text.ToObject<IIcpdasQueue.Meta>());
                                                                                break;

                                                                            case "10":
                                                                                QueueWrapper.Icpdas.PushWaterTankTemperature(text.ToObject<IIcpdasQueue.Meta>());
                                                                                break;
                                                                        }
                                                                        break;
                                                                }
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                        }
                                        break;
                                        #endregion
                                }
                                break;
                        }
                    }
                    break;
            }
        }
        catch (Exception e)
        {
            Log.Error(Menu.Title, nameof(StructuralEngine.Transport.InterceptingPublishAsync), new { e.Message });
        }
    });
    public required IStructuralEngine StructuralEngine { get; init; }
    public required IQueueWrapper QueueWrapper { get; init; }
}