namespace Station.Application.Additions.Triggers;
public sealed class InitialTrigger : IEntrance
{
    readonly IMainProfile _mainProfile;
    readonly IMaintainTrigger _maintainTrigger;
    public InitialTrigger(IMainProfile mainProfile, IMaintainTrigger maintainTrigger)
    {
        _mainProfile = mainProfile;
        _maintainTrigger = maintainTrigger;
    }
    public async void Build()
    {
        try
        {
            await _mainProfile.RefreshAsync();
            await _maintainTrigger.CreateSwitchFileAsync();
            if (_mainProfile.Text is not null) await _mainProfile.OverwriteAsync(_mainProfile.Text);
        }
        catch (Exception e)
        {
            Log.Error(Menu.Title, nameof(InitialTrigger), new { e.Message });
        }
    }
}