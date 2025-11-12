using Mökinvaraus.Data;
using Mökinvaraus.Models;
using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Mökinvaraus.Pages
{
    public partial class MokkiPage : ContentPage
    {
        private readonly Tietokantapalvelu _tietokanta;
        private Mokki? valittuMokki;

        public MokkiPage()
        {
            InitializeComponent();

            string dbPolku = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mokit.db");
            _tietokanta = new Tietokantapalvelu(dbPolku);

            _ = LataaMokitAsync();
        }

        private async Task LataaMokitAsync()
        {
            mokkiLista.ItemsSource = await _tietokanta.HaeMokitAsync();
        }

        private async void Lisaa_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nimiEntry.Text) || string.IsNullOrWhiteSpace(hintaEntry.Text))
            {
                await DisplayAlert("Virhe", "Anna vähintään nimi ja hinta.", "OK");
                return;
            }

            var mokki = new Mokki
            {
                NIMI = nimiEntry.Text,
                SIJAINTI = sijaintiEntry.Text,
                ASUKASMAARA = int.TryParse(asukasEntry.Text, out int asukas) ? asukas : 0,
                KUVAUS = kuvausEntry.Text,
                YOHINTA = double.TryParse(hintaEntry.Text, out double hinta) ? hinta : 0
            };

            await _tietokanta.LisaaMokkiAsync(mokki);
            await LataaMokitAsync();
            TyhjennaKentat();
        }

        private async void Paivita_Clicked(object sender, EventArgs e)
        {
            if (valittuMokki == null)
            {
                await DisplayAlert("Huomio", "Valitse mökki listasta.", "OK");
                return;
            }

            valittuMokki.NIMI = nimiEntry.Text;
            valittuMokki.SIJAINTI = sijaintiEntry.Text;
            valittuMokki.ASUKASMAARA = int.TryParse(asukasEntry.Text, out int asukas) ? asukas : 0;
            valittuMokki.KUVAUS = kuvausEntry.Text;
            valittuMokki.YOHINTA = double.TryParse(hintaEntry.Text, out double hinta) ? hinta : 0;

            await _tietokanta.PaivitaMokkiAsync(valittuMokki);
            await LataaMokitAsync();
            TyhjennaKentat();
        }

        private async void Poista_Clicked(object sender, EventArgs e)
        {
            if (valittuMokki == null)
            {
                await DisplayAlert("Huomio", "Valitse poistettava mökki listasta.", "OK");
                return;
            }

            bool vahvista = await DisplayAlert("Vahvista", $"Haluatko varmasti poistaa mökin '{valittuMokki.NIMI}'?", "Kyllä", "Peruuta");
            if (vahvista)
            {
                await _tietokanta.PoistaMokkiAsync(valittuMokki.MOKKI_ID);
                await LataaMokitAsync();
                TyhjennaKentat();
            }
        }

        private async void Lataa_Clicked(object sender, EventArgs e)
        {
            await LataaMokitAsync();
        }

        private void mokkiLista_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            valittuMokki = e.CurrentSelection.FirstOrDefault() as Mokki;
            if (valittuMokki != null)
            {
                nimiEntry.Text = valittuMokki.NIMI;
                sijaintiEntry.Text = valittuMokki.SIJAINTI;
                asukasEntry.Text = valittuMokki.ASUKASMAARA.ToString();
                kuvausEntry.Text = valittuMokki.KUVAUS;
                hintaEntry.Text = valittuMokki.YOHINTA.ToString();
            }
        }


        private void TyhjennaKentat()
        {
            nimiEntry.Text = "";
            sijaintiEntry.Text = "";
            asukasEntry.Text = "";
            kuvausEntry.Text = "";
            hintaEntry.Text = "";
            mokkiLista.SelectedItem = null;
            valittuMokki = null;
        }
    }
}
