namespace Station.Application.Additions.Triggers;
public sealed class QueueTrigger : IEntrance
{
    readonly IQueueWrapper _queueWrapper;
    readonly IStructuralEngine _structuralEngine;
    public QueueTrigger(IQueueWrapper queueWrapper, IStructuralEngine structuralEngine)
    {
        _queueWrapper = queueWrapper;
        _structuralEngine = structuralEngine;
    }
    public void Build() => _structuralEngine.Transport.InterceptingPublishAsync += @event => Task.Run(() =>
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
                            _queueWrapper.Interior.PushSpindleThermalCompensation(text);
                            break;

                        case var item when item.Equals("VUX400_TC", StringComparison.OrdinalIgnoreCase):
                            _queueWrapper.Interior.PushSpindleThermalCompensation(text);
                            break;

                        case var item when item.Equals("UCT600_TC", StringComparison.OrdinalIgnoreCase):
                            _queueWrapper.Interior.PushSpindleThermalCompensation(text);
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
                                                                                _queueWrapper.Icpdas.PushWaterPumpMotorAverageVoltage(text.ToObject<IIcpdasQueue.Meta>());
                                                                                break;

                                                                            case "4408":
                                                                                _queueWrapper.Icpdas.PushWaterPumpMotorAverageCurrent(text.ToObject<IIcpdasQueue.Meta>());
                                                                                break;

                                                                            case "4410":
                                                                                _queueWrapper.Icpdas.PushWaterPumpMotorApparentPower(text.ToObject<IIcpdasQueue.Meta>());
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
                                                                                _queueWrapper.Icpdas.PushElectricalBoxAverageVoltage(text.ToObject<IIcpdasQueue.Meta>());
                                                                                break;

                                                                            case "4408":
                                                                                _queueWrapper.Icpdas.PushElectricalBoxAverageCurrent(text.ToObject<IIcpdasQueue.Meta>());
                                                                                break;

                                                                            case "4410":
                                                                                _queueWrapper.Icpdas.PushElectricalBoxApparentPower(text.ToObject<IIcpdasQueue.Meta>());
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
                                                                                _queueWrapper.Icpdas.PushElectricalBoxHumidity(text.ToObject<IIcpdasQueue.Meta>());
                                                                                break;

                                                                            case "1":
                                                                                _queueWrapper.Icpdas.PushElectricalBoxTemperature(text.ToObject<IIcpdasQueue.Meta>());
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
                                                                                _queueWrapper.Icpdas.PushCuttingFluidTemperature(text.ToObject<IIcpdasQueue.Meta>());
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
                                                                                _queueWrapper.Icpdas.PushCuttingFluidPotentialOfHydrogen(text.ToObject<IIcpdasQueue.Meta>());
                                                                                break;

                                                                            case "10":
                                                                                _queueWrapper.Icpdas.PushWaterTankTemperature(text.ToObject<IIcpdasQueue.Meta>());
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
            Log.Error(Menu.Title, nameof(_structuralEngine.Transport.InterceptingPublishAsync), new { e.Message });
        }
    });
}