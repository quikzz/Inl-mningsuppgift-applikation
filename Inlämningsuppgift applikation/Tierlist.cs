using ConsoleTables;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlämningsuppgift_applikation
{
    public class Tierlist
    {
        //Attribut
        public string Class;
        public string Specialization;
        public string Tier;

        // Skapande av ny lista som får namnet Tiers.
        public static List<Tierlist> Tiers = new List<Tierlist>();
        
        //Konstruktor
        public Tierlist(string Class, string Specialization, string Tier)
        {
            //Objekt
            this.Class = Class;
            this.Specialization = Specialization;
            this.Tier = Tier;

            //Lägger till det specifika objektet som skapats i listan Tiers.
            Tiers.Add(this);
            
        }
        
        public static void FetchRoles(MySqlConnection conn, string role, string category)
        {

            // [ METOD FÖR ATT HÄMTA ROLL]


            // Tömmer listan Tiers för att endast visa en lista vid återupprepade anrop.
            Tiers.Clear();
            
            // SQL-Query som anropar tierlist_{vald lista} denna stored procedure hämtar en lista beroende på användarens val.
            string sqlQuerry = $"CALL tierlist_{role}();";

            // Öppnar connection till databas.
            conn.Open();

            // Skapar nytt objekt.
            MySqlCommand cmd = new MySqlCommand(sqlQuerry, conn);

            // Hämtar data från databasen och sparar i "reader".
            MySqlDataReader reader = cmd.ExecuteReader();

            // While-loop som körs så länge det finns data att tillgå.
            while (reader.Read())

            {
                
                // Spara data till lista som ska visas.
                new Tierlist(reader["Class"].ToString(), reader["Specialization"].ToString(), reader["Tier"].ToString());
                Console.Clear();
                
            }

            // När det inte finns mer data att hämta stängs connection till databasen.
            conn.Close();

            // Skriver ut navigator för att användaren ska veta vart den befinner sig.
            Console.WriteLine($"\n[ Main menu ] > [ Tier lists ] > [ {category} ]\n");

            // Skriver ut förklaring till de olika rankerna.
            Console.WriteLine("  S = Exceptional");
            Console.WriteLine("  A = Great");
            Console.WriteLine("  B = Good");
            Console.WriteLine("  C = Mediocre\n");

            // Skapar en ny konsoltabell med rubrikerna "Class", "Specialization och "Tier".
            var table = new ConsoleTable("Class", "Specialization", "Tier");

            // Skriver ut värdena som hämtats från databasen till respektive kolumner så länge det finns värden att skriva ut.
            foreach (Tierlist items in Tiers)
            {
                //Lägger till en ny rad för varje gång loopen körs. Dikterar även på vilken plats värdena ska skrivas ut.
                table.AddRow(items.Class, items.Specialization, items.Tier);
                

            }

            // Skriver ut tabellen
            table.Write(Format.Minimal);
            
            // Skriver ut ett meddelande till användaren att hämtningen av datan lyckats samt instruktion om hur användaren återvänder till menyn.
            Console.WriteLine("Data Fetched successfully! Press any key to go back");
            Console.ReadKey();
        }
        public static void TierlistMenu(MySqlConnection conn, string role, string category)
        {
            // [ METOD FÖR MENY TIERLISTS ]

            ConsoleKeyInfo input;
            do
            {
                // Rensar konsolen för ett renare gränssnitt.
                Console.Clear();

                // Skriver ut navigator för att användaren ska veta vart den befinner sig.
                Console.WriteLine($"\n[ Main menu ] > [ Tier lists ]");

                //Skriver ut en meny över existerande roller till användaren.
                Console.WriteLine("\nChoose to view which races belonging to each faction ");
                Console.WriteLine("by pressing the following numbers.");
                Console.WriteLine("------------------------------------------------------\n");
                Console.WriteLine("1. Melee DPS");
                Console.WriteLine("2. Ranged DPS");
                Console.WriteLine("3. Tanks");
                Console.WriteLine("4. Healers");
                Console.WriteLine("5. Go back");

                //Låter användaren välja ett med hjälp av knapptryck och värdet i variabeln input.
                input = Console.ReadKey();

                //SwitchCase funktion som först rensar skärmen och sedan anropar rätt metod beroende på användarens val.
                switch (input.KeyChar.ToString())
                {

                    // Hämtar data från databasen där call-anropet ändras beroende på vilket knappval (interpolation)

                    // "CALL tierlist_meleedps();"
                    case "1":
                        Console.Clear();
                        FetchRoles(conn,"meleedps","Melee dps classes");
                        break;

                    // "CALL tierlist_rangedps();"
                    case "2":
                        Console.Clear();
                        FetchRoles(conn,"rangeddps", "Ranged dps classes");
                        break;

                    // "CALL tierlist_tanks();"
                    case "3":
                        Console.Clear();
                        FetchRoles(conn,"tanks", "Tank classes");
                        break;

                    // "CALL tierlist_healers();"
                    case "4":
                        Console.Clear();
                        FetchRoles(conn,"healers", "Healer classes");
                        break;

                    // Vid val "5" återvänder användaren till huvudmenyn.
                    case "5":
                        break;
                    default:
                        Console.WriteLine("Incorrect input value (Press any key to continue...)");
                        Console.ReadKey();
                        break;
                }
                // While-loop: Så länge knappvalet inte är "5" så ska loopen fortsätta köras.
            } while (input.KeyChar.ToString() != "5");

        }

    }
}
