﻿namespace Station.Application.Operators.Attachs;
internal sealed class DigiwinAttach : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await new PeriodicTimer(Menu.RefreshTime).WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                if (MainProfile.Text is not null)
                {
                    if (MainProfile.Text.DigiwinEAI.Enable)
                    {

                    }
                }
                if (Histories.Any()) Histories.Clear();
                FoundationPool.PushDigiwinAttach(DateTime.UtcNow);
            }
            catch (Exception e)
            {
                if (!Histories.Contains(e.Message))
                {
                    Histories.Add(e.Message);
                    HistoryEngine.Record(new IHistoryEngine.FavorerPayload
                    {
                        Name = nameof(DigiwinAttach),
                        Message = e.Message,
                        Source = e.Source
                    });
                }
            }
        }
    }
    internal required List<string> Histories { get; init; } = new();
    IRootInformation.Data.MachineStatusType MachineStatus { get; set; }
    Dictionary<string, float> HistoricalDatas { get; init; } = new();
    public required IMainProfile MainProfile { get; init; }
    public required IHistoryEngine HistoryEngine { get; init; }
    public required IFoundationPool FoundationPool { get; init; }
    public required IStructuralEngine StructuralEngine { get; init; }
}