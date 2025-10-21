namespace Licznik;

using Microsoft.Maui.Graphics;

public partial class CounterView : ContentView
{
	private int _value;
    private int _initialValue;
    public string CounterName { get; set; }
    private readonly Action _onValueChanged;

	public CounterView(string name, int startValue, Action onValueChange,Color color)
	{
        InitializeComponent();
		CounterName = name;
        _value = startValue; // z tym
        _initialValue = startValue; // sprawdz czy to siê nie gryzie
        _onValueChanged = onValueChange;

        NameLabel.Text = name;
		UpdateValue();

        this.BackgroundColor = color;
    }

    private void OnPlusClicked(object? sender, EventArgs e) // PLUS
    {
        _value++;
        UpdateValue();
        _onValueChanged?.Invoke();
    }

    private void OnMinusClicked(object? sender, EventArgs e) // MINUS
    {
        _value--;
        UpdateValue();
        _onValueChanged?.Invoke();
    }

    private void OnDeleteClicked(object? sender, EventArgs e) // USUÑ
    {
        if(Parent is Layout parentLayout)
        {
            parentLayout.Children.Remove(this);
            _onValueChanged?.Invoke();
            // Przyciski to dzieci :)
        }
    }

    private void OnResetClicked(object? sender, EventArgs e) // RESET DO PODSTAWOWEJ WARTOŒCI
    {
        if (Parent is Layout parentLayout)
        {
            _value = _initialValue;
            UpdateValue();
            _onValueChanged?.Invoke();
        }
    }

    private void UpdateValue() // Tutaj jest auto-zapis po naciœniêciu guzika
    {
        ValueLabel.Text = _value.ToString();
    }

    public int GetValue() => _value;
    public void SetValue(int value)
    {
        _value = value;
        UpdateValue();
    }

}