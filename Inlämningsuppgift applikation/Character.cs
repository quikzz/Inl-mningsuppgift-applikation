using ConsoleTables;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Inlämningsuppgift_applikation
{
    internal class CharacterList
    {
        //Attribut

        public string name;
        public string klass;
        public string spec;
        public string race;
        public string faction;

        //Statisk lista
        public static List<CharacterList> Character = new List<CharacterList>();

        //Konstruktor
        public CharacterList(string name, string klass, string spec, string race, string faction)
        {
            //Skapade objekt
            this.name = name;
            this.klass = klass;
            this.spec = spec;
            this.race = race;
            this.faction = faction;

            //Add THIS objekt to list
            Character.Add(this);
        }

        public static string FetchClassname(MySqlConnection conn, int KlassId)
        {
           /* SQL querry för att hämta klassnamnet beroende på det val användaren gör. Denna metod översätter endast användarens numeriska val 
              till ett strängvärde.*/ 
            string sqlQuerry = $"CALL Retrieve_classname('{KlassId}');";

            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sqlQuerry, conn);


            MySqlDataReader reader = cmd.ExecuteReader();

            reader.Read();

            string Classname = reader.GetString("classes_names");

            conn.Close();

            //Detta ska vara klassnamnet.
            return Classname;
        }
        public static void CharacterMenu(MySqlConnection conn)
        {

            // [ MENY FÖR CHARRACTERS ]

            int input;
            do
            {

                // Rensar applikationen för ett renare användargränssnitt, skriver sedan ut en navigator samt vilka möjliga val användaren kan göra.
                Console.Clear();

                Console.WriteLine($"\n[ Main menu ] > [ Characters ]");

                Console.WriteLine("\n- Choose what you want to do by typing a number and ENTER");
                Console.WriteLine("\n--------------------------");
                Console.WriteLine("| 1. Create character       |");
                Console.WriteLine("| 2. View characters        |");
                Console.WriteLine("| 3. Delete character       |");
                Console.WriteLine("| 4. Go back                |");
                Console.WriteLine(" ---------------------------\n");

                // Ger instruktion att användaren måste skriva in vilket val den behöver göra med hjälp av numeriska värden.
                Console.Write("Choose one of the options by pressing a number and ENTER: ");
                input = Convert.ToInt32(Console.ReadLine());

                // Statement som avgör vilken metod som ska anropas beroende på användarens inmatning.
                switch (input.ToString())
                {
                    case "1":
                        Console.Clear();
                        CreateCharacter(conn);
                        break;
                    case "2":
                        Console.Clear();
                        ViewCharacters(conn,"","","","","");
                        break;
                    case "3":
                        Console.Clear();
                        DeleteCharacter(conn, "", "", "", "", "");
                        break;
                    case "4":
                        break;

                    default:
                        Console.WriteLine("\nIncorrect input value (Press any key to go back...)");
                        Console.ReadKey();
                        break;

                }
                // Så länge användarens input inte är "3" ska loopen köras.
            } while (input.ToString() != "4");
        }
        public static void CreateCharacter(MySqlConnection conn)
        {
            
 
            // [ METOD FÖR Create Character ]
            
            ConsoleKeyInfo input;

            // Variablerna nedan är tomma by default men ändras beroende på vilket val användaren gör, detta sparas sedan i SQLQuery:s som skickas till databasen.
            string faction = ChooseFaction();
            string race = "";
            int klassid = 0;
            string klassnamn = "";
            string spec = "";
            string name = "";

            /* Användaren får välja faction. Metod Choose{faction}Race anropas vilket ger användaren menyval över tillgängliga raser
                  beroende på vilken faction som valts */
            switch (faction)
            {
                case "Horde":
                    Console.Clear();
                    race = ChooseHordeRace();
                    break;

                case "Alliance":
                    Console.Clear();
                    race = ChooseAllianceRace();
                    break;
                
            }
            
            // Användaren får välja vilken klass utifrån ett urval som är beroende på vilken ras som valts.
            klassid = ChooseClass(conn,race);

            // FetchClassName översätter användarens val till ett strängvärde som sedan skickas med i Call-anropet till databasen.
            klassnamn = FetchClassname(conn, klassid);

            // Användaren får välja vilken specialization utifrån ett urval som är beroende på vilken klass som valts.
            spec = ChooseSpec(conn,klassid);

            // Användaren får välja ett namn till sin karaktär.
            name = ChooseName(conn, name);

            // Rensar applikationen för ett renare gränssnitt för användaren.
            Console.Clear();

            // Skriver ut bekräftelse till användaren att karaktären skapats.
            Console.WriteLine("Character creation successful!\n");
            Console.WriteLine("Following character has been created.\n");

            // Skapar en tabell med information om karaktären som skapats.
            var table = new ConsoleTable($"Name", "Class", "Specialization", "Race", "Faction");

            table.AddRow(name, klassnamn, spec, race, faction);

            table.Write();

            Console.WriteLine("\n< Press ENTER to go back >");
            Console.ReadKey();


            // Skapar en SQL-Query som anropar stored procedure "create_new_character" för att fylla kolumnerna i chars-tabellen med information.
            conn.Open();
            string SQLQuery = $"CALL create_new_character('{name}','{klassnamn}','{spec}','{race}','{faction}')"; 

            MySqlCommand cmd = new MySqlCommand(SQLQuery,conn);

            cmd.ExecuteReader();

 
            conn.Close();

        }
        public static string ChooseFaction()
        {
            // [ MENY FÖR VAL AV FACTION ]
            // Sparar användarens val i faction som sedan returneras till CreateCharacter-metoden.

            string faction = "";

            ConsoleKeyInfo input;
            
                Console.Clear();

                //Skriva ut en meny för användaren
                Console.WriteLine("\n- Choose a faction to view the races belonging to each faction");
                Console.WriteLine("by typing a number and ENTER -\n");
                Console.WriteLine(" ---------------------------");
                Console.WriteLine("| 1. Horde                  |");
                Console.WriteLine("| 2. Alliance               |");
                Console.WriteLine(" ---------------------------\n");

                //Låta användaren välja ett alternativ
                input = Console.ReadKey();

                //Ta värdet till en SwitchCase
                switch (input.KeyChar.ToString())
                {
                    case "1":
                        Console.Clear();
                        faction = "Horde";
                        return faction;
                        break;
                    case "2":
                        Console.Clear();
                        faction = "Alliance";
                        return faction;
                        break;
                    default:
                        Console.WriteLine("Du har matat in ett felaktigt värde. (Press any key to continue...)");
                        Console.ReadKey();
                        break;

                }return faction;


        }
        public static string ChooseAllianceRace()
        {
            /* [ METOD SOM LÅTER ANVÄNDAREN VÄLJA ALLIANCE RACE ]
             
             Värdet sparas sedan i variabeln race som i sin tur används i call-anropet till databasen.
            */
            string Race = "";

            ConsoleKeyInfo input;

            Console.Clear();

            //Skriva ut en meny för användaren
            Console.WriteLine("\n- Choose a alliance race ");
            Console.WriteLine("by typing a number and ENTER -\n");
            Console.WriteLine(" ------------------------");
            Console.WriteLine("| 1. Human               |");
            Console.WriteLine("| 2. Dwarf               |");
            Console.WriteLine("| 3. Gnome               |");
            Console.WriteLine("| 4. Night elf           |");
            Console.WriteLine("| 5. Draenei             |");
            Console.WriteLine(" ------------------------");

            //Låta användaren välja ett alternativ
            input = Console.ReadKey();

            //Ta värdet till en SwitchCase
            switch (input.KeyChar.ToString())
            {
                case "1":
                    Console.Clear();
                    Race = "Human";
                    break;
                case "2":
                    Console.Clear();
                    Race = "Dwarf";
                    break;
                case "3":
                    Race = "Gnome";
                    Console.Clear();
                    break;
                case "4":
                    Race = "Night elf";
                    Console.Clear();
                    break;
                case "5":
                    Race = "Draenei";
                    Console.Clear();
                    break;

                default:
                    Console.WriteLine("Du har matat in ett felaktigt värde. (Press any key to continue...)");
                    Console.ReadKey();
                    break;

            }return Race;

            /*string sqlQuerry = $"CALL insert_race('{Race}');";

            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sqlQuerry, conn);

            //Exekvera MySQLCommand
            cmd.ExecuteReader();

            //Stänga DB koppling
            conn.Close();*/
        }
        public static string ChooseHordeRace()
        {
            /* [ METOD SOM LÅTER ANVÄNDAREN VÄLJA HORDE RACE ]

           Värdet sparas sedan i variabeln race som i sin tur används i call-anropet till databasen.
          */

            string Race = "";

            ConsoleKeyInfo input;

            Console.Clear();

            //Skriva ut en meny för användaren
            Console.WriteLine("\n- Choose a horde race");
            Console.WriteLine("by typing a number and ENTER -\n");
            Console.WriteLine(" ------------------------");
            Console.WriteLine("| 1. Orc                 |");
            Console.WriteLine("| 2. Troll               |");
            Console.WriteLine("| 3. Undead              |");
            Console.WriteLine("| 4. Tauren              |");
            Console.WriteLine("| 5. Blood elf           |");
            Console.WriteLine(" ------------------------");

            //Låta användaren välja ett alternativ
            input = Console.ReadKey();

            //Ta värdet till en SwitchCase
            switch (input.KeyChar.ToString())
            {
                case "1":
                    Console.Clear();
                    Race = "Orc";
                    break;
                case "2":
                    Console.Clear();
                    Race = "Troll";
                    break;
                case "3":
                    Race = "Undead";
                    Console.Clear();
                    break;
                case "4":
                    Race = "Tauren";
                    Console.Clear();
                    break;
                case "5":
                    Race = "Blood elf";
                    Console.Clear();
                    break;



                default:
                    Console.WriteLine("Du har matat in ett felaktigt värde. (Press any key to continue...)");
                    Console.ReadKey();
                    break;

            }return Race;

      
        }
        public static int ChooseClass(MySqlConnection conn, string race)
        {
            // Hämta klasser som kan vara raser med call-anrop Race_can_be_class. 
            // Skapa meny utifrån Race_can_be_class.
            // Välj ett värde + returnera värde ex. "Mage"

            string sqlQuerry = $"CALL Race_can_be_class('{race}');";

            string klass = "";

            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sqlQuerry, conn);


            MySqlDataReader reader = cmd.ExecuteReader();

            List<int> IDlist = new List<int>();
            List<string> Namelist = new List<string>();

            while (reader.Read())

            {

                IDlist.Add(Convert.ToInt32(reader["classes_id"]));
                Namelist.Add(reader["classes_names"].ToString());

                
                
                //Spara data till Lista
                
                Console.Clear();

            }
            conn.Close();

            Console.WriteLine();
            ConsoleKeyInfo input;

            Console.Clear();

            var index = 1;
            
            //Skriver ut en meny för användaren som ökar sålänge det finns data att hämta i Namelist.
            Console.WriteLine("\n- Choose a class by pressing a numerical key -");
            Console.WriteLine("\n-----------------------");
            foreach(string Class in Namelist)
            {
              
                Console.WriteLine($"|{index}. {Class,-19}|");
                index++;
            }
            

            Console.WriteLine("------------------------\n");

            //Låta användaren välja ett alternativ och returnera valet.
            input = Console.ReadKey();
            return IDlist[int.Parse(input.KeyChar.ToString())-1];

        }
        public static string ChooseSpec(MySqlConnection conn, int klass)
        {
            // Hämta specs som kan vara klasser med call-anrop Class_can_be_specs. 
            // Skapa meny utifrån stored procedure Race_can_be_class.
            // Välj ett värde + returnera värde ex. "Mage" i Call-anropet "sqlQuerry".

            string sqlQuerry = $"CALL Class_can_be_specs('{klass}');";

            string spec = "";

            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sqlQuerry, conn);


            MySqlDataReader reader = cmd.ExecuteReader();

            // Skapar en ny lista med namn Speclist.

            List<string> Speclist = new List<string>();

            while (reader.Read())

            {

                Speclist.Add(reader["specs_name"].ToString());

                //Sparar data till Lista.

                Console.Clear();

            }
            conn.Close();

            Console.WriteLine();
            ConsoleKeyInfo input;

            Console.Clear();

            var index = 1;

            //Skriver ut en meny över tillgängliga val för användaren. Listan hämtar Specialization och även index med hjälp av foreach + index increment.
            Console.WriteLine("\n- Choose a specialization for your class by pressing a numerical key --");;
            Console.WriteLine("\n------------------------------------------");
            foreach (string Spec in Speclist)
            {

                Console.WriteLine($"|{index}. {Spec,-19}|");
                index++;
            }


            Console.WriteLine("------------------------------------------\n");

            //Låta användaren välja ett alternativ
            input = Console.ReadKey();

            // Returnerar valet användaren gjort.
            return Speclist[int.Parse(input.KeyChar.ToString()) - 1];

        }
        public static string ChooseName(MySqlConnection conn, string name)
     {
            // [ METOD SOM LÅTER ANVÄNDAREN SKAPA ETT NAMN OCH SPARAR DENNA I VARIABEL name]
            Console.Clear();
            Console.WriteLine("Enter the name for your character\n");
            name = Console.ReadLine();
            return name;
     }
        public static void ViewCharacters(MySqlConnection conn, string Name, string Class, string Specialization, string Race, string faction)
      {
            // Rensar listan Characters för att undvika återupprepning av data om metoden skulle anropas igen.
            Character.Clear();

            // SQL-Querry som anropar stored procedure characters.
            string sqlQuerry = $"CALL characters()";

            // Öppnar connection till databasen, hämtar in information och skriver sedan ut till användaren i tabell-format över de karaktärer som skapats.

            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sqlQuerry, conn);


            MySqlDataReader reader = cmd.ExecuteReader();

           

            while (reader.Read())

            {

                //Spara data till Lista
                    new CharacterList(
                        reader[$"Name"].ToString(),
                        reader[$"Class"].ToString(),
                        reader[$"Specialization"].ToString(),
                        reader[$"Race"].ToString(),
                        reader[$"Faction"].ToString());

                Console.Clear();

            }
            conn.Close();

            Console.WriteLine("\n[ Mainmenu ] > [ Character ] > [ Characters ]\n");
            Console.WriteLine("This list contains all created characters.");
            var table = new ConsoleTable($"Name", "Class", "Specialization", "Race", "Faction");

            // Skriver ut samtliga items i listan Tiers
            foreach (CharacterList items in Character)
            {

                table.AddRow(items.name, items.klass, items.spec, items.race, items.faction);


            }

            table.Write();
            //Stänga DB koppling

            Console.WriteLine("\nData Fetched successfully!");
            Console.WriteLine("< Press any key to go back >");
            Console.ReadKey();

        }
        public static void DeleteCharacter(MySqlConnection conn, string Name, string Class, string Specialization, string Race, string faction)
         {

            // Rensar listan Characters för att undvika återupprepning av data om metoden skulle anropas igen.
            Character.Clear();

            // SQL-Querry som anropar stored procedure characters.
            string sqlQuerry = $"CALL characters()";

            // Öppnar connection till databasen, hämtar in information och skriver sedan ut till användaren i tabell-format över de karaktärer som skapats.
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sqlQuerry, conn);

            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())

            {

                //Spara data till Lista
                new CharacterList(
                    reader[$"Name"].ToString(),
                    reader[$"Class"].ToString(),
                    reader[$"Specialization"].ToString(),
                    reader[$"Race"].ToString(),
                    reader[$"Faction"].ToString());

                Console.Clear();

            }
            conn.Close();

            Console.WriteLine("\n[ Mainmenu ] > [ Character ] > [ Characters ]\n");
            Console.WriteLine("This list contains all created characters.");
            var table = new ConsoleTable($"Name", "Class", "Specialization", "Race", "Faction");

            // Skriver ut samtliga items i listan Character
            foreach (CharacterList items in Character)
            {

                table.AddRow(items.name, items.klass, items.spec, items.race, items.faction);

            }

            table.Write();


            conn.Open();

            var name = "";
            Console.WriteLine("\nData Fetched successfully!");

            // [ DELETE CHARACTER ANROPET ]

           // Låter användaren skriva in namnet på den character som ska tas bort från databasen och sparar detta i variabeln "name".
            Console.WriteLine("\nType the character name of the character you want to delete\n");
            Console.Write("Character name: ");
            name = Console.ReadLine();

            // Anropar stored procedure delete_character och skickar med användarens inmatning som sedan exekveras.
            string sqlQuerry1 = $"CALL delete_character('{name}')";

            MySqlCommand cmd1 = new MySqlCommand(sqlQuerry1, conn);


            cmd1.ExecuteReader();

            Console.Clear();

            // Kvitto till användaren på att karaktären + namn tagits bort från databasen.
            Console.WriteLine($"\n Character < {name} > has been deleted from database ");
            Console.WriteLine("\n< Press any key to go back >");

            Console.ReadKey();
            
            conn.Close();
        }
    }
}
