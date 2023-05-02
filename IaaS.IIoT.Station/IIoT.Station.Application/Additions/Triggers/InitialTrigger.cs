namespace Station.Application.Additions.Triggers;
public sealed class InitialTrigger : IEntranceTrigger
{
    public async void Build()
    {
        try
        {
            await MainProfile.RefreshAsync();
            await MaintainTrigger.CreateSwitchFileAsync();
            if (MainProfile.Text is not null) await MainProfile.OverwriteAsync(MainProfile.Text);
        }
        catch (Exception e)
        {
            Log.Error(Menu.Title, nameof(InitialTrigger), new { e.Message });
        }
    }
    public required IMainProfile MainProfile { get; init; }
    public required IMaintainTrigger MaintainTrigger { get; init; }
}