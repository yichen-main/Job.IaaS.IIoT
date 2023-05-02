namespace Infrastructure.Core.Boundaries;
public interface IMainProfile
{
    ValueTask RefreshAsync();
    ValueTask OverwriteAsync(MainText text);
    MainText? Text { get; }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class MainProfile : IMainProfile
{
    public async ValueTask RefreshAsync()
    {
        MainText entity = new();
        await entity.CreateProfileAaync();
        Text = MainDilation.ReadFile(ref entity, Menu.ProfilePath);
    }
    public async ValueTask OverwriteAsync(MainText text) => await text.CreateProfileAaync(cover: true);
    public MainText? Text { get; private set; }
}