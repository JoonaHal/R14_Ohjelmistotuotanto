using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mökinvaraus.Models;

namespace Mökinvaraus.Data
{
    public class Tietokantapalvelu
    {
        private readonly string _dbPolku;
        // Konstruktorissa luodaan tietokanta ja tarvittavat taulut, jos niitä ei vielä ole
        public Tietokantapalvelu(string dbPolku)
        {
            _dbPolku = dbPolku;

            using var yhteys = new SqliteConnection($"Data Source={_dbPolku}");
            yhteys.Open();

            var komento = yhteys.CreateCommand();
            komento.CommandText = @"
                CREATE TABLE IF NOT EXISTS MOKIT (
                    MOKKI_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    NIMI TEXT NOT NULL,
                    SIJAINTI TEXT,
                    ASUKASMAARA INTEGER,
                    KUVAUS TEXT,
                    YOHINTA REAL NOT NULL
                );

                CREATE TABLE IF NOT EXISTS ASIAKAS (
                    ASIAKAS_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    ETUNIMI TEXT NOT NULL,
                    SUKUNIMI TEXT NOT NULL,
                    PUHELIN TEXT,
                    SAHKOPOSTI TEXT
                );

                CREATE TABLE IF NOT EXISTS VARAUS (
                    VARAUS_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    MOKKI_ID INTEGER NOT NULL,
                    ASIAKAS_ID INTEGER NOT NULL,
                    ALKUPVM DATE NOT NULL,
                    LOPPUPVM DATE NOT NULL,
                    FOREIGN KEY (MOKKI_ID) REFERENCES MOKIT(MOKKI_ID),
                    FOREIGN KEY (ASIAKAS_ID) REFERENCES ASIAKAS(ASIAKAS_ID)
                );
            ";
            komento.ExecuteNonQuery();
        }
        // Tämä hakee kaikki mökit tietokannasta
        public async Task<List<Mokki>> HaeMokitAsync()
        {
            var lista = new List<Mokki>();

            using var yhteys = new SqliteConnection($"Data Source={_dbPolku}");
            await yhteys.OpenAsync();

            var komento = yhteys.CreateCommand();
            komento.CommandText = "SELECT MOKKI_ID, NIMI, SIJAINTI, ASUKASMAARA, KUVAUS, YOHINTA FROM MOKIT;";

            using var lukija = await komento.ExecuteReaderAsync();
            while (await lukija.ReadAsync())
            {
                lista.Add(new Mokki
                {
                    MOKKI_ID = lukija.GetInt32(0),
                    NIMI = lukija.GetString(1),
                    SIJAINTI = lukija.IsDBNull(2) ? "" : lukija.GetString(2),
                    ASUKASMAARA = lukija.GetInt32(3),
                    KUVAUS = lukija.IsDBNull(4) ? "" : lukija.GetString(4),
                    YOHINTA = lukija.GetDouble(5)
                });
            }

            return lista;
        }
        // Tämä toiminnnallisuus lisää uuden mökin tietokantaan
        public async Task LisaaMokkiAsync(Mokki mokki)
        {
            using var yhteys = new SqliteConnection($"Data Source={_dbPolku}");
            await yhteys.OpenAsync();

            var komento = yhteys.CreateCommand();
            komento.CommandText = @"
                INSERT INTO MOKIT (NIMI, SIJAINTI, ASUKASMAARA, KUVAUS, YOHINTA)
                VALUES ($nimi, $sijainti, $asukasmaara, $kuvaus, $yohinta);
            ";

            komento.Parameters.AddWithValue("$nimi", mokki.NIMI);
            komento.Parameters.AddWithValue("$sijainti", mokki.SIJAINTI);
            komento.Parameters.AddWithValue("$asukasmaara", mokki.ASUKASMAARA);
            komento.Parameters.AddWithValue("$kuvaus", mokki.KUVAUS);
            komento.Parameters.AddWithValue("$yohinta", mokki.YOHINTA);

            await komento.ExecuteNonQueryAsync();
        }

        //Tällä päivitetään olemassa olevan mökin tietoja
        public async Task PaivitaMokkiAsync(Mokki mokki)
        {
            using var yhteys = new SqliteConnection($"Data Source={_dbPolku}");
            await yhteys.OpenAsync();

            var komento = yhteys.CreateCommand();
            komento.CommandText = @"
        UPDATE MOKIT 
        SET NIMI = $nimi, 
            SIJAINTI = $sijainti, 
            ASUKASMAARA = $asukasmaara, 
            KUVAUS = $kuvaus, 
            YOHINTA = $yohinta
        WHERE MOKKI_ID = $id;
    ";

            komento.Parameters.AddWithValue("$id", mokki.MOKKI_ID);
            komento.Parameters.AddWithValue("$nimi", mokki.NIMI);
            komento.Parameters.AddWithValue("$sijainti", mokki.SIJAINTI);
            komento.Parameters.AddWithValue("$asukasmaara", mokki.ASUKASMAARA);
            komento.Parameters.AddWithValue("$kuvaus", mokki.KUVAUS);
            komento.Parameters.AddWithValue("$yohinta", mokki.YOHINTA);

            await komento.ExecuteNonQueryAsync();
        }
        // Tämä poistaa mökin tietokannasta
        public async Task PoistaMokkiAsync(int mokkiId)
        {
            using var yhteys = new SqliteConnection($"Data Source={_dbPolku}");
            await yhteys.OpenAsync();

            var komento = yhteys.CreateCommand();
            komento.CommandText = "DELETE FROM MOKIT WHERE MOKKI_ID = $id;";
            komento.Parameters.AddWithValue("$id", mokkiId);

            await komento.ExecuteNonQueryAsync();
        }
        // Tämä hakee yksittäisen mökin ID:n perusteella
        public async Task<Mokki?> HaeMokkiAsync(int id)
        {
            using var yhteys = new SqliteConnection($"Data Source={_dbPolku}");
            await yhteys.OpenAsync();

            var komento = yhteys.CreateCommand();
            komento.CommandText = "SELECT * FROM MOKIT WHERE MOKKI_ID = $id;";
            komento.Parameters.AddWithValue("$id", id);

            using var lukija = await komento.ExecuteReaderAsync();
            if (await lukija.ReadAsync())
            {
                return new Mokki
                {
                    MOKKI_ID = lukija.GetInt32(0),
                    NIMI = lukija.GetString(1),
                    SIJAINTI = lukija.IsDBNull(2) ? "" : lukija.GetString(2),
                    ASUKASMAARA = lukija.GetInt32(3),
                    KUVAUS = lukija.IsDBNull(4) ? "" : lukija.GetString(4),
                    YOHINTA = lukija.GetDouble(5)
                };
            }
            return null;
        }

        // tämä hakee kaikki asiakkaat tietokannasta
        public async Task<List<Asiakas>> HaeAsiakkaatAsync()
        {
            var lista = new List<Asiakas>();
            using var yhteys = new SqliteConnection($"Data Source={_dbPolku}");
            await yhteys.OpenAsync();

            var komento = yhteys.CreateCommand();
            komento.CommandText = "SELECT ASIAKAS_ID, ETUNIMI, SUKUNIMI, PUHELIN, SAHKOPOSTI FROM ASIAKAS;";

            using var lukija = await komento.ExecuteReaderAsync();
            while (await lukija.ReadAsync())
            {
                lista.Add(new Asiakas
                {
                    ASIAKAS_ID = lukija.GetInt32(0),
                    ETUNIMI = lukija.GetString(1),
                    SUKUNIMI = lukija.GetString(2),
                    PUHELIN = lukija.IsDBNull(3) ? "" : lukija.GetString(3),
                    SAHKOPOSTI = lukija.IsDBNull(4) ? "" : lukija.GetString(4)
                });
            }

            return lista;
        }

        // Tämä toiminnnallisuus lisää uuden asiakkaan tietokantaan
        public async Task LisaaAsiakasAsync(Asiakas asiakas)
        {
            using var yhteys = new SqliteConnection($"Data Source={_dbPolku}");
            await yhteys.OpenAsync();

            var komento = yhteys.CreateCommand();
            komento.CommandText = @"
        INSERT INTO ASIAKAS (ETUNIMI, SUKUNIMI, PUHELIN, SAHKOPOSTI)
        VALUES ($etunimi, $sukunimi, $puhelin, $sahkoposti);
    ";

            komento.Parameters.AddWithValue("$etunimi", asiakas.ETUNIMI);
            komento.Parameters.AddWithValue("$sukunimi", asiakas.SUKUNIMI);
            komento.Parameters.AddWithValue("$puhelin", asiakas.PUHELIN);
            komento.Parameters.AddWithValue("$sahkoposti", asiakas.SAHKOPOSTI);

            await komento.ExecuteNonQueryAsync();
        }

        //tämä päivittää olemassa olevan asiakkaan tietoja
        public async Task PaivitaAsiakasAsync(Asiakas asiakas)
        {
            using var yhteys = new SqliteConnection($"Data Source={_dbPolku}");
            await yhteys.OpenAsync();

            var komento = yhteys.CreateCommand();
            komento.CommandText = @"
        UPDATE ASIAKAS
        SET ETUNIMI = $etunimi,
            SUKUNIMI = $sukunimi,
            PUHELIN = $puhelin,
            SAHKOPOSTI = $sahkoposti
        WHERE ASIAKAS_ID = $id;
    ";

            komento.Parameters.AddWithValue("$id", asiakas.ASIAKAS_ID);
            komento.Parameters.AddWithValue("$etunimi", asiakas.ETUNIMI);
            komento.Parameters.AddWithValue("$sukunimi", asiakas.SUKUNIMI);
            komento.Parameters.AddWithValue("$puhelin", asiakas.PUHELIN);
            komento.Parameters.AddWithValue("$sahkoposti", asiakas.SAHKOPOSTI);

            await komento.ExecuteNonQueryAsync();
        }

        // tämä poistaa asiakkaan tietokannasta
        public async Task PoistaAsiakasAsync(int asiakasId)
        {
            using var yhteys = new SqliteConnection($"Data Source={_dbPolku}");
            await yhteys.OpenAsync();

            var komento = yhteys.CreateCommand();
            komento.CommandText = "DELETE FROM ASIAKAS WHERE ASIAKAS_ID = $id;";
            komento.Parameters.AddWithValue("$id", asiakasId);

            await komento.ExecuteNonQueryAsync();
        }
        // tämä hakee kaikki varaukset tietokannasta
        public async Task<List<Varaus>> HaeVarauksetAsync()
        {
            var lista = new List<Varaus>();
            using var yhteys = new SqliteConnection($"Data Source={_dbPolku}");
            await yhteys.OpenAsync();

            var komento = yhteys.CreateCommand();
            komento.CommandText = "SELECT VARAUS_ID, MOKKI_ID, ASIAKAS_ID, ALKUPVM, LOPPUPVM FROM VARAUS;";

            using var lukija = await komento.ExecuteReaderAsync();
            while (await lukija.ReadAsync())
            {
                lista.Add(new Varaus
                {
                    VARAUS_ID = lukija.GetInt32(0),
                    MOKKI_ID = lukija.GetInt32(1),
                    ASIAKAS_ID = lukija.GetInt32(2),
                    ALKUPVM = DateTime.Parse(lukija.GetString(3)),
                    LOPPUPVM = DateTime.Parse(lukija.GetString(4))
                });
            }

            return lista;
        }
        // tämä tarkistaa onko mökille jo päällekkäinen varaus annetulle ajanjaksolle
        public async Task<bool> OnkoPaallekkainenVarausAsync(int mokkiId, DateTime alku, DateTime loppu)
        {
            using var yhteys = new SqliteConnection($"Data Source={_dbPolku}");
            await yhteys.OpenAsync();

            var komento = yhteys.CreateCommand();
            komento.CommandText = @"
        SELECT COUNT(*) FROM VARAUS
        WHERE MOKKI_ID = $mokkiId
        AND NOT (LOPPUPVM <= $alku OR ALKUPVM >= $loppu);
    ";
            komento.Parameters.AddWithValue("$mokkiId", mokkiId);
            komento.Parameters.AddWithValue("$alku", alku);
            komento.Parameters.AddWithValue("$loppu", loppu);

            var tulos = (long)await komento.ExecuteScalarAsync();
            return tulos > 0;
        }
        // tämä lisää varauksen tietokantaan
        public async Task LisaaVarausAsync(Varaus varaus)
        {
            if (await OnkoPaallekkainenVarausAsync(varaus.MOKKI_ID, varaus.ALKUPVM, varaus.LOPPUPVM))
                throw new Exception("Päällekkäinen varaus tälle mökille!");

            using var yhteys = new SqliteConnection($"Data Source={_dbPolku}");
            await yhteys.OpenAsync();

            var komento = yhteys.CreateCommand();
            komento.CommandText = @"
        INSERT INTO VARAUS (MOKKI_ID, ASIAKAS_ID, ALKUPVM, LOPPUPVM)
        VALUES ($mokki, $asiakas, $alku, $loppu);
    ";

            komento.Parameters.AddWithValue("$mokki", varaus.MOKKI_ID);
            komento.Parameters.AddWithValue("$asiakas", varaus.ASIAKAS_ID);
            komento.Parameters.AddWithValue("$alku", varaus.ALKUPVM);
            komento.Parameters.AddWithValue("$loppu", varaus.LOPPUPVM);

            await komento.ExecuteNonQueryAsync();
        }
        // tämä poistaa varauksen tietokannasta
        public async Task PoistaVarausAsync(int varausId)
        {
            using var yhteys = new SqliteConnection($"Data Source={_dbPolku}");
            await yhteys.OpenAsync();

            var komento = yhteys.CreateCommand();
            komento.CommandText = "DELETE FROM VARAUS WHERE VARAUS_ID = $id;";
            komento.Parameters.AddWithValue("$id", varausId);

            await komento.ExecuteNonQueryAsync();
        }

        // tämä päivittää olemassa olevan varauksen tietoja
        public async Task PaivitaVarausAsync(Varaus varaus)
        {
            if (await OnkoPaallekkainenVarausAsync(varaus.MOKKI_ID, varaus.ALKUPVM, varaus.LOPPUPVM))
                throw new Exception("Päällekkäinen varaus tälle mökille!");

            using var yhteys = new SqliteConnection($"Data Source={_dbPolku}");
            await yhteys.OpenAsync();

            var komento = yhteys.CreateCommand();
            komento.CommandText = @"
        UPDATE VARAUS
        SET MOKKI_ID = $mokki,
            ASIAKAS_ID = $asiakas,
            ALKUPVM = $alku,
            LOPPUPVM = $loppu
        WHERE VARAUS_ID = $id;
    ";

            komento.Parameters.AddWithValue("$id", varaus.VARAUS_ID);
            komento.Parameters.AddWithValue("$mokki", varaus.MOKKI_ID);
            komento.Parameters.AddWithValue("$asiakas", varaus.ASIAKAS_ID);
            komento.Parameters.AddWithValue("$alku", varaus.ALKUPVM);
            komento.Parameters.AddWithValue("$loppu", varaus.LOPPUPVM);

            await komento.ExecuteNonQueryAsync();
        }


    }
}
