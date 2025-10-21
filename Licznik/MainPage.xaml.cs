using System.Text.Json;
namespace Licznik
{
    public partial class MainPage : ContentPage
    {
        private const string SaveData = "CountersData";
        public MainPage()
        {
            InitializeComponent();
            LoadCounters();
        }

        private void OnAddCounterClicked(object? sender, EventArgs e)
        {
            var name = string.IsNullOrWhiteSpace(CounterNameEntry.Text) ? "Unnamed" : CounterNameEntry.Text;
            int startValue = 0;
            int.TryParse(StartValueEntry.Text, out startValue);

            var r = (float)RedSlider.Value / 255f;
            var g = (float)GreenSlider.Value / 255f;
            var b = (float)BlueSlider.Value / 255f;
            var color = Color.FromRgb(r, g, b);

            var counter = new CounterView(name, startValue, SaveCounters, color);

            CounterContainer.Children.Add(counter);

            CounterNameEntry.Text = string.Empty;
            StartValueEntry.Text = string.Empty;

            RedSlider.Value = 0;
            GreenSlider.Value = 0;
            BlueSlider.Value = 0;

            SaveCounters();
        }

        private void CounterPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SaveCounters();
        }

        private void SaveCounters()
        {
            var data = CounterContainer.Children
            .OfType<CounterView>()
            .Select(c => new CounterData
            {
                Name = c.CounterName,
                Value = c.GetValue(),
                ColorHex = c.BackgroundColor.ToHex()
            })
            .ToList();

            var json = JsonSerializer.Serialize(data);
            Preferences.Set(SaveData, json);
        }

        private void LoadCounters()
        {
            if (!Preferences.ContainsKey(SaveData)) return;

            var json = Preferences.Get(SaveData, string.Empty);
            var data = JsonSerializer.Deserialize<List<CounterData>>(json);

            foreach (var c in data)
            {
                Color color = Colors.DarkGray;
                if (!string.IsNullOrEmpty(c.ColorHex))
                {
                    try
                    {
                        color = Color.FromArgb(c.ColorHex);
                    }
                    catch
                    {
                        color = Colors.DarkGray;
                    }
                }
                var counter = new CounterView(c.Name, c.Value, SaveCounters, color);
                CounterContainer.Children.Add(counter);
            }
        }

        public class CounterData
        {
            public string Name { get; set; }
            public int Value { get; set; }
            public string ColorHex { get; set; } = "A9A9A9";
        }
    }
}