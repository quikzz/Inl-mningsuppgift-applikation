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
    internal class Classes
    {
        //Attribut
        public string Class;
        public string Specialization;
        public string Tier;

        // Skapande av ny lista som får namnet Classlist.
        public static List<Classes> Classlist = new List<Classes>();

        //Konstruktor
        public Classes (string Class)
        {
            this.Class = Class;

            //Lägger till det specifika objektet som skapats i listan Classlist.
            Classlist.Add(this);
        }

        public static void FetchClassRaces(MySqlConnection conn, string Class, string header)
        {

            // [ METOD FÖR ATT HÄMTA VILKA RASER EN KLASS KAN VARA ]

            //Rensar listan Classlist, detta för att förhinda upprepning om man kallar på samma stored procedure en gång till.
            Classlist.Clear();

            //Queryn som skickas för att hämta horde- och alliance_races.
            string sqlQuerry = $"CALL {Class}_races();";

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
                new Classes(reader[$"{Class}"].ToString());
                Console.Clear();
            }

            // Stänger connection till databas när all data är hämtad.
            conn.Close();

            // Skriver ut navigator för att användaren ska veta vart den befinner sig.
            Console.WriteLine($"\n[ Mainmenu ] > [ Classes ] > [ {header}s ] > [ Races ]\n");

            // Skapar en ny konsoltabell med rubriken "Available races for {användarens val}".
            var table = new ConsoleTable($"Available races for {Class}s");

            // Skriver ut samtliga items i listan Tiers
            foreach (Classes items in Classlist)
            {
                //Lägger till en ny rad för varje gång loopen körs. Dikterar även på vilken plats värdena ska skrivas ut.
                table.AddRow(items.Class);


            }

            // Skriver ut tabellen
            table.Write();

            // Skriver ut meddelande till användaren att inhämtning av data lyckats och instruktion för hur man återvänder till förgående meny.
            Console.WriteLine("\nData Fetched successfully!");
            Console.WriteLine("< Press any key to go back >");
            Console.ReadKey();



        }
        public static void FetchClassSpecs(MySqlConnection conn, string Class, string header)
        {
            // [ METOD FÖR ATT HÄMTA VILKA SPECS EN KLASS KAN VARA ]
            // Metoden fungerar på samma sätt som FetchClassRaces.

            Classlist.Clear();

            string sqlQuerry = $"CALL {Class}_specs();";

            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sqlQuerry, conn);


            MySqlDataReader reader = cmd.ExecuteReader();


            while (reader.Read())

            {

                //Spara data till Lista
                new Classes(reader[$"{Class}"].ToString());
                Console.Clear();

            }
            conn.Close();

            Console.WriteLine($"\n[ Mainmenu ] > [ Classes ] > [ {header}s ] > [ Specializations ]\n");
            var table = new ConsoleTable($"Available specializations for {Class}s");

            // Skriver ut samtliga items i listan Tiers
            foreach (Classes items in Classlist)
            {

                table.AddRow(items.Class);


            }

            table.Write();
            

            Console.WriteLine("\nData Fetched successfully!");
            Console.WriteLine("< Press any key to go back >");
            Console.ReadKey();



        }
        public static void ClassesMenu(MySqlConnection conn, string Class, string header)
        {

           // [ MENY FÖR CLASSES ]
            int input;
            do
            {   
                // Rensar applikationen för ett renare användargränssnitt, skriver sedan ut en navigator samt vilka möjliga val användaren kan göra.
                Console.Clear();

                Console.WriteLine($"\n[ Main menu ] > [ Classes ]");

                Console.WriteLine("\n- Choose a class to view available specializations and races");
                Console.WriteLine("by typing a number and ENTER -");
                Console.WriteLine(" \n--------------------------");
                Console.WriteLine("| 1. Warrior               |");
                Console.WriteLine("| 2. Rogue                 |");
                Console.WriteLine("| 3. Paladin               |");
                Console.WriteLine("| 4. Mage                  |");
                Console.WriteLine("| 5. Priest                |");
                Console.WriteLine("| 6. Warlock               |");
                Console.WriteLine("| 7. Shaman                |");
                Console.WriteLine("| 8. Death knight          |");
                Console.WriteLine("| 9. Hunter                |");
                Console.WriteLine("| 10. Druid                |");
                Console.WriteLine("| 11. Go back              |");
                Console.WriteLine(" --------------------------\n");

                // Ger instruktion att användaren måste skriva in vilket val den behöver göra med hjälp av numeriska värden.
                Console.Write("Choose one of the options by pressing a number and ENTER: ");

                input = Convert.ToInt32(Console.ReadLine());

               
                switch (input.ToString())
                {
                    case "1":
                        Console.Clear();
                        ChosenClassOptions(conn, "Warrior", "Warrior");
                        break;
                    case "2":
                        Console.Clear();
                        ChosenClassOptions(conn, "Rogue", "Rogue");
                        break;
                    case "3":
                        Console.Clear();
                        ChosenClassOptions(conn, "Paladin", "Paladin");
                        break;
                    case "4":
                        Console.Clear();
                        ChosenClassOptions(conn, "Mage", "Mage");
                        break;
                    case "5":
                        Console.Clear();
                        ChosenClassOptions(conn, "Priest", "Priest");
                        break;
                    case "6":
                        Console.Clear();
                        ChosenClassOptions(conn, "Warlock", "Warlock");
                        break;
                    case "7":
                        Console.Clear();
                        ChosenClassOptions(conn, "Shaman", "Shaman");
                        break;
                    case "8":
                        Console.Clear();
                        ChosenClassOptions(conn, "Deathknight", "Death Knight");
                        break;
                    case "9":
                        Console.Clear();
                        ChosenClassOptions(conn, "Hunter", "Hunter");
                        break;
                    case "10":
                        Console.Clear();
                        ChosenClassOptions(conn, "Druid", "Druid");
                        break;
                    case "11":
                        Console.WriteLine("Go back");
                        return; 

                    default:
                        Console.WriteLine("\nIncorrect input value (Press any key to go back...)");
                        Console.ReadKey();
                        break;

                };
            } while (input.ToString() != "11");   
        }
        public static void ChosenClassOptions(MySqlConnection conn, string Class, string header)
        {
            // [ MENY FÖR DEN CLASS ANVÄNDAREN VALT OCH SPECIFIKATION KRING KLASSEN ]
            // Denna metod har samma funktion som "ClassesMenu"-metoden
            int input;
            do
            {
                Console.Clear();

                Console.WriteLine($"\n[ Main menu ] > [ Classes ] > [ {header}s ]");

                Console.WriteLine("\n- Make a choice to view available races or specializations for a class");
                Console.WriteLine("by typing a number and ENTER -");
                Console.WriteLine("\n----------------------------");
                Console.WriteLine("| 1. Specializations         |");
                Console.WriteLine("| 2. Races                   |");
                Console.WriteLine("| 3. Go back                 |");
                Console.WriteLine(" ----------------------------\n");


                Console.Write("Choose one of the options by pressing a number and ENTER: ");
                input = Convert.ToInt32(Console.ReadLine());


                switch (input)
                {
                    case 1:
                        Console.Clear();
                        FetchClassSpecs(conn, Class, header);
                        break;
                    case 2:
                        Console.Clear();
                        FetchClassRaces(conn, Class, header);
                        break;

                    case 3:
                        Console.WriteLine("Go back");
                        return;


                    default:
                        Console.WriteLine("\nIncorrect input value (Press any key to go back...)");
                        Console.ReadKey();
                        break;

                };
            } while (input != 11);

        }
    }
}
