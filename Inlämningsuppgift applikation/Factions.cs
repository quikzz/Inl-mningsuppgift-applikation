using ConsoleTables;
using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Inlämningsuppgift_applikation
{
    internal class Factions
    {
        //Attribut
        public string Faction;

        // Skapande av ny lista som får namnet Factionslist.
        public static List<Factions> Factionslist = new List<Factions>();

        //Konstruktor
        public Factions(string Faction)
        {
            this.Faction = Faction;

            //Lägger till det specifika objektet som skapats i listan Factionslist.
            Factionslist.Add(this);
        }

        public static void FetchFaction(MySqlConnection conn, string Faction, string header)
        {
            // [ METOD FÖR ATT HÄMTA FACTIONS OCH RASER ]


            //Rensar listan Factionslist, detta för att förhinda upprepning om man kallar på samma stored procedure en gång till.
            Factionslist.Clear();

            //Queryn som skickas för att hämta horde- och alliance_races.
            string sqlQuerry = $"CALL {Faction}_races();";

            // Öppnar connection till databas.
            conn.Open();

            // Skapar nytt objekt av MySqlCommand.
            MySqlCommand cmd = new MySqlCommand(sqlQuerry, conn);

            // Hämtar data från databasen och sparar i "reader".
            MySqlDataReader reader = cmd.ExecuteReader();

            // While-loop som körs så länge det finns data att tillgå.
            while (reader.Read())

            {

                //Spara data till Lista
                new Factions(reader[$"{Faction}"].ToString());
                Console.Clear();

            }
            // Stänger connection till databas när all data är hämtad.
            conn.Close();

            // Skriver ut navigator för att användaren ska veta vart den befinner sig.
            Console.WriteLine($"\n[ Mainmenu ] > [ Factions ] > [ {header} ]\n");

            // Skapar en ny konsoltabell med rubriken "Available races for {användarens val}".
            var table = new ConsoleTable($"Available races for {Faction}");

            // Skriver ut samtliga items i listan Factionslist
            foreach (Factions items in Factionslist)
            {
                //Lägger till en ny rad för varje gång loopen körs. Dikterar även på vilken plats värdena ska skrivas ut.
                table.AddRow(items.Faction);


            }

            // Skriver ut tabellen
            table.Write();
      
            // Skriver ut meddelande till användaren att inhämtning av data lyckats och instruktion för hur man återvänder till förgående meny.
            Console.WriteLine("\nData Fetched successfully!");
            Console.WriteLine("< Press any key to go back >");
            Console.ReadKey();



        }
        public static void FactionsMenu(MySqlConnection conn, string Faction, string header)
        {
            // [ MENY FÖR FACTIONS ]
            int input;
            do
            {
                // Rensar vyn i applikationen för ett renare gränssnitt
                Console.Clear();

                // Skriver ut en meny över existerande "Factions" och menyval att återvända till huvudmenyn
                Console.WriteLine($"\n[ Main menu ] > [ Factions ]");

                Console.WriteLine("\n- Choose a faction to view the races belonging to each faction");
                Console.WriteLine("by typing a number and ENTER -");
                Console.WriteLine("\n--------------------------");
                Console.WriteLine("| 1. Horde                  |");
                Console.WriteLine("| 2. Alliance               |");
                Console.WriteLine("| 3. Go back                |");
                Console.WriteLine(" ---------------------------\n");

                // Ger instruktion att användaren måste skriva in vilket val den behöver göra med hjälp av numeriska värden.
                Console.Write("Choose one of the options by pressing a number and ENTER: ");
                input = Convert.ToInt32(Console.ReadLine());

                // Statement som avgör vilken information som ska skickas in i sqlQuerry anropet under metoden Fetch Faction
                switch (input.ToString())
                {
                    case "1":
                        Console.Clear();
                        FetchFaction(conn, "Horde", "Horde");
                        break;
                    case "2":
                        Console.Clear();
                        FetchFaction(conn, "Alliance", "Alliance");
                        break;
                    case "3":
                        Console.WriteLine("Go back");
                        break;

                    default:
                        Console.WriteLine("\nIncorrect input value (Press any key to go back...)");
                        Console.ReadKey();
                        break;

                };
                // Så länge användarens input inte är "3" ska loopen köras.
            } while (input.ToString() != "3");
        }
       
    }
}
