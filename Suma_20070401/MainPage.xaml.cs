using Microsoft.Maui.Controls;

namespace Suma_20070401
{
    public partial class MainPage : ContentPage
    {
        private readonly LocalDbService _dbService;
        private int _editResultadoId;

        public MainPage(LocalDbService dbService)
        {
            InitializeComponent();
            _dbService = dbService;
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            listView.ItemsSource = await _dbService.GetResultado();
        }

        private async void sumarBtn_Clicked(object? sender, EventArgs e)
        {
            if (double.TryParse(Entryprimernumero.Text, out double num1) &&
                double.TryParse(Entrysegundonumero.Text, out double num2))
            {
                double resultadoSuma = num1 + num2;
                labelresultado.Text = resultadoSuma.ToString();
            }
            else
            {
                await DisplayAlert("Error", "Por favor ingresa números válidos", "OK");
                return;
            }

            if (_editResultadoId == 0)
            {
                await _dbService.Create(new Resultado
                {
                    Numero1 = Entryprimernumero.Text,
                    Numero2 = Entrysegundonumero.Text,
                    Suma = labelresultado.Text
                });
            }
            else
            {
                await _dbService.Update(new Resultado
                {
                    Id = _editResultadoId,
                    Numero1 = Entryprimernumero.Text,
                    Numero2 = Entrysegundonumero.Text,
                    Suma = labelresultado.Text
                });
                _editResultadoId = 0;
            }

            Entryprimernumero.Text = string.Empty;
            Entrysegundonumero.Text = string.Empty;
            labelresultado.Text = string.Empty;

            await LoadDataAsync();
        }

        private async void listView_ItemTapped(object? sender, ItemTappedEventArgs e)
        {
            if (e.Item is not Resultado resultado) return;

            var action = await DisplayActionSheet("Action", "Cancel", null, "Edit", "Delete");

            switch (action)
            {
                case "Edit":
                    _editResultadoId = resultado.Id;
                    Entryprimernumero.Text = resultado.Numero1;
                    Entrysegundonumero.Text = resultado.Numero2;
                    labelresultado.Text = resultado.Suma;
                    break;

                case "Delete":
                    await _dbService.Delete(resultado);
                    await LoadDataAsync();
                    break;
            }
        }
    }
}