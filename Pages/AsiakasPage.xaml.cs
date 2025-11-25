using Mökinvaraus.Data;
using Mökinvaraus.Models;
using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Mökinvaraus.Pages;

public partial class AsiakasPage : ContentPage
{
    private readonly Tietokantapalvelu _db;
    private Asiakas? valittuAsiakas;

    public AsiakasPage()
    {
        InitializeComponent();
        _db = new Tietokantapalvelu(Path.Combine(FileSystem.AppDataDirectory, "mokit.db"));
        _ = LataaAsiakkaat();
    }

    private async Task LataaAsiakkaat()
    {
        var lista = await _db.HaeAsiakasAsync();
      
    }

    private void Asiakas_Selected(object sender, SelectionChangedEventArgs e)
    {
        var valinta = e.CurrentSelection.FirstOrDefault();
        if (valinta == null) return;

        valittuAsiakas = ((dynamic)valinta).Asiakas;

        EtunimiEntry.Text = valittuAsiakas.ETUNIMI;
        SukunimiEntry.Text = valittuAsiakas.SUKUNIMI;
        PuhelinEntry.Text = valittuAsiakas.PUHELIN;
        SahkopostiEntry.Text = valittuAsiakas.SAHKOPOSTI;
    }

    private async void LisaaAsiakas_Clicked(object sender, EventArgs e)
    {
        var uusi = new Asiakas
        {
            ETUNIMI = EtunimiEntry.Text,
            SUKUNIMI = SukunimiEntry.Text,
            PUHELIN = PuhelinEntry.Text,
            SAHKOPOSTI = SahkopostiEntry.Text
        };

        await _db.LisaaAsiakasAsync(uusi);
        await LataaAsiakkaat();
    }

    private async void PaivitaAsiakas_Clicked(object sender, EventArgs e)
    {
        if (valittuAsiakas == null) return;

        valittuAsiakas.ETUNIMI = EtunimiEntry.Text;
        valittuAsiakas.SUKUNIMI = SukunimiEntry.Text;
        valittuAsiakas.PUHELIN = PuhelinEntry.Text;
        valittuAsiakas.SAHKOPOSTI = SahkopostiEntry.Text;

        await _db.PaivitaAsiakasAsync(valittuAsiakas);
        await LataaAsiakkaat();
    }

    private async void PoistaAsiakas_Clicked(object sender, EventArgs e)
    {
        if (valittuAsiakas == null) return;

        await _db.PoistaAsiakasAsync(valittuAsiakas.ASIAKAS_ID);
        await LataaAsiakkaat();
    }
}
