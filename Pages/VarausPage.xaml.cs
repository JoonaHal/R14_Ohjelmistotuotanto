using Mökinvaraus.Data;
using Mökinvaraus.Models;

namespace Mökinvaraus.Pages;

public partial class VarausPage : ContentPage
{
    private readonly Tietokantapalvelu _tietokanta;
    private Varaus? _valittu;

    public VarausPage()
    {
        InitializeComponent();
        _tietokanta = new Tietokantapalvelu(Path.Combine(FileSystem.AppDataDirectory, "mokit.db3"));
        Lataa();
    }

    


    private async void Lataa()
    {
        AsiakasPicker.ItemsSource = await _tietokanta.HaeAsiakasAsync();
        MokkiPicker.ItemsSource = await _tietokanta.HaeMokitAsync();

        var varaukset = await _tietokanta.HaeVarauksetAsync();

        // Lisää näkyvyyttä varten nimet
        foreach (var v in varaukset)
        {
            var mokki = await _tietokanta.HaeMokkiAsync(v.MOKKI_ID);
            var asiakas = await _tietokanta.HaeAsiakasAsync(v.ASIAKAS_ID);

            
        }

        VarausLista.ItemsSource = varaukset;
    }

    private void Lataa_Clicked(object sender, EventArgs e) => Lataa();

    private async void Lisaa_Clicked(object sender, EventArgs e)
    {
        if (MokkiPicker.SelectedItem is not Mokki mokki ||
            AsiakasPicker.SelectedItem is not Asiakas asiakas)
        {
            await DisplayAlert("Virhe", "Valitse mökki ja asiakas.", "OK");
            return;
        }

        var varaus = new Varaus
        {
            MOKKI_ID = mokki.MOKKI_ID,
            ASIAKAS_ID = asiakas.ASIAKAS_ID,
            ALKUPVM = AlkuPvmPicker.Date,
            LOPPUPVM = LoppuPvmPicker.Date
        };

        try
        {
            await _tietokanta.LisaaVarausAsync(varaus);
            Lataa();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", ex.Message, "OK");
        }
    }

    private async void Paivita_Clicked(object sender, EventArgs e)
    {
        if (_valittu == null)
        {
            await DisplayAlert("Virhe", "Valitse varaus ensin.", "OK");
            return;
        }

        _valittu.MOKKI_ID = ((Mokki)MokkiPicker.SelectedItem).MOKKI_ID;
        _valittu.ASIAKAS_ID = ((Asiakas)AsiakasPicker.SelectedItem).ASIAKAS_ID;
        _valittu.ALKUPVM = AlkuPvmPicker.Date;
        _valittu.LOPPUPVM = LoppuPvmPicker.Date;

        try
        {
            await _tietokanta.PaivitaVarausAsync(_valittu);
            Lataa();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", ex.Message, "OK");
        }
    }

    private async void Poista_Clicked(object sender, EventArgs e)
    {
        if (_valittu == null)
        {
            await DisplayAlert("Virhe", "Valitse varaus ensin.", "OK");
            return;
        }

        await _tietokanta.PoistaVarausAsync(_valittu.VARAUS_ID);

        Lataa();
    }

    private void VarausLista_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _valittu = e.CurrentSelection.FirstOrDefault() as Varaus;
        if (_valittu == null) return;

        // Asetetaan tiedot UI:hin
        MokkiPicker.SelectedItem =
            ((List<Mokki>)MokkiPicker.ItemsSource).First(x => x.MOKKI_ID == _valittu.MOKKI_ID);

        AsiakasPicker.SelectedItem =
            ((List<Asiakas>)AsiakasPicker.ItemsSource).First(x => x.ASIAKAS_ID == _valittu.ASIAKAS_ID);

        AlkuPvmPicker.Date = _valittu.ALKUPVM;
        LoppuPvmPicker.Date = _valittu.LOPPUPVM;
    }
}
