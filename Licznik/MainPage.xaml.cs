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

            var counter = new CounterView(name,startValue, SaveCounters);

            CounterContainer.Children.Add(counter);

            CounterNameEntry.Text = string.Empty;
            StartValueEntry.Text = string.Empty;

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
            .Select(c => new CounterData { 
                Name = c.CounterName, 
                Value = c.GetValue()})
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
                var counter = new CounterView(c.Name, c.Value, SaveCounters);
                CounterContainer.Children.Add(counter);
            } 
        }
    }

    public class CounterData
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}