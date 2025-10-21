using System.Text.Json;
using Microsoft.Maui.Graphics; //inaczej nie zrobie tych kolorów
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
            var name = string.IsNullOrWhiteSpace(CounterNameEntry.Text) ? "Unnamed" : CounterNameEntry.Text; // Dodawanie nazwy licznika, lub barku jeżeli nie została wpisana

            int startValue = 0;
            int.TryParse(StartValueEntry.Text, out startValue);

            //kontynuacja nowoczesnego wybierania kolorów
            var r = (float)RedSlider.Value / 255f;
            var g = (float)GreenSlider.Value / 255f;
            var b = (float)BlueSlider.Value / 255f;
            var color = Color.FromRgb(r, g, b); // FUNKCJA FROM RGB RATUJE ŻYCIE nie działa

            var counter = new CounterView(name,startValue, SaveCounters, color);//nowy licznik

            CounterContainer.Children.Add(counter);

            CounterNameEntry.Text = string.Empty;
            StartValueEntry.Text = string.Empty;

            // reset suwaków
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
            .OfType<CounterView>() // to filtruje children żeby nie wyświetlała niepotrzebnych rzeczy
            .Select(c => new CounterData { 
                Name = c.CounterName, 
                Value = c.GetValue(), 
                ColorHex = c.BackgroundColor.ToHex()}) // To tworzy nowe liczniki i przypisuje im wartości z counterview NIE DZIALA BEZ HEX
            .ToList(); // wykonuje to co wcześniej napisałam i nie trzeba się męczyć bo samo wykonuje liste :)

            var json = JsonSerializer.Serialize(data); // Przemienia dane w stringa żeby można go było zapisać w .jsonie!
            Preferences.Set(SaveData, json);
        }

        private void LoadCounters()
        {
            if (!Preferences.ContainsKey(SaveData)) return;

            var json = Preferences.Get(SaveData, string.Empty);
            var data = JsonSerializer.Deserialize<List<CounterData>>(json); // Tutaj zamienia ze stringa w normalne wartości

            foreach (var c in data) // c = pojedyńczy obiekt w data
                                        //c = CounterData { Name = "Pomidor", Value = 3 } taki przykład
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
                    var counter = new CounterView(c.Name, c.Value, SaveCounters, color); // i tutaj pętla nam ładnie robi obiekty
                    CounterContainer.Children.Add(counter); // i je dodaje
            }
        }
    }

    public class CounterData
    {
        public string Name { get; set; } //= string.Empty; // Tutaj jest jakiś problem z name
        public int Value { get; set; }
        public string ColorHex { get; set; } = "A9A9A9"; //niech będzie biały normalnie
    }
}

// SERIALIZACJA nie sterylizacja