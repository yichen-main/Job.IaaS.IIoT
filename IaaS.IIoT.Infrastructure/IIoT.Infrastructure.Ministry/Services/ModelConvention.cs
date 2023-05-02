namespace Infrastructure.Ministry.Services;
public sealed class ModelConvention : IControllerModelConvention
{
    public void Apply(ControllerModel model)
    {
        if (model.Selectors.Any(item => item.AttributeRouteModel is not null)) return;
        if (model.Selectors.Count is 0) model.Selectors.Add(new());
        for (int i = default; i < model.Selectors.Count; i++) model.Selectors[i].AttributeRouteModel = new()
        {
            Template = AttributeRouteModel.CombineTemplates("IIoT", model.ControllerName)
        };
        for (int i = default; i < model.Actions.Count; i++)
        {
            if (model.Actions[i].Selectors.Any(item => item.AttributeRouteModel is not null)) continue;
            if (model.Actions[i].Selectors.Count is 0) model.Actions[i].Selectors.Add(new SelectorModel());
            for (int item = default; item < model.Actions[i].Selectors.Count; item++) model.Actions[i].Selectors[item].AttributeRouteModel = new()
            {
                Template = model.Actions[i].ActionName
            };
        }
    }
}