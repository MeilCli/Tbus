namespace Tbus.App.NETStandard.Models
{
    interface IDayTableModel : IModel
    {
        string Id { get; }

        string Time { get; }

        string Destination { get; }

        string RemainingTime { get; }

        void StartCounter();

        void StopCounter();
    }
}
