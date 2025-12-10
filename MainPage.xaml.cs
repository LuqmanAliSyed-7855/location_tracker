using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;

namespace LocationTracker;

public partial class MainPage : ContentPage
{
    readonly LocationService locationService;
    readonly DatabaseService db;

    public MainPage()
    {
        InitializeComponent();
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "locations.db3");
        db = new DatabaseService(dbPath);
        locationService = new LocationService();
    }

    async void OnTrackClicked(object sender, EventArgs e)
    {
        var loc = await locationService.GetCurrentAsync();
        if (loc == null) return;

        await db.SaveAsync(new UserLocation
        {
            Latitude = loc.Latitude,
            Longitude = loc.Longitude,
            Timestamp = DateTime.Now
        });

        var positions = await db.GetAllAsync();
        map.Pins.Clear();
        foreach (var p in positions)
        {
            map.Pins.Add(new Pin
            {
                Label = p.Timestamp.ToShortTimeString(),
                Location = new Location(p.Latitude, p.Longitude)
            });
        }
    }
}
