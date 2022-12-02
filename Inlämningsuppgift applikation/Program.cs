
using System.Linq.Expressions;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities;
using System.Xml.Linq;
using System;
using Inlämningsuppgift_applikation;

/* READ ME:

Majoriteten av databasen är enligt 3NF förutom tabellen 'chars' som jag aktivt valt att stå ensam. 

Detta därför att man endast ska kunna skriva till och ta bort från tabellen. Denna ska inte ha någon koppling till övriga tabeller mer än stored procedures.

 */

namespace MySQLDBConnectionExample
{
    internal class Program
    {
        static void Main(string[] args)
        {


            // Skapar variabler som i sin tur ska skickas med i anslutningssträngen till databasen.
            string server = "LOCALHOST";
            string database = "worldofwarcraft";
            string username = "root";
            string pass = "hy67ujHY&/UJ"; //Ange lösenord

            string strConn = $"SERVER={server};DATABASE={database};UID={username};PASSWORD={pass};";

            //Establera koppling till Databas
            MySqlConnection conn = new MySqlConnection(strConn);

            ConsoleKeyInfo input;

            //Meny
            do
            {
                Console.Clear();

                //Skriva ut en meny för användaren
                Console.WriteLine("\n< WORLD OF WARCRAFT {Wrath of the Lich King} DATABASE > \n");
                Console.WriteLine("Choose from the menu by pressing the following numbers");
                Console.WriteLine("------------------------------------------------------");
                Console.WriteLine("1. Factions");
                Console.WriteLine("2. Classes");
                Console.WriteLine("3. Tier lists");
                Console.WriteLine("4. Characters");
                Console.WriteLine("5. Quit");

                //Låta användaren välja ett alternativ
                input = Console.ReadKey();

                // Switch case som utifrån användarens val kallar på vald klass.metod.
                switch (input.KeyChar.ToString())
                {
                    case "1":
                        Console.Clear();
                        Factions.FactionsMenu(conn,"","");
                        break;
                    case "2":
                        Console.Clear();
                        Classes.ClassesMenu(conn,"","");
                        break;
                    case "3":
                        Console.Clear();
                        Tierlist.TierlistMenu(conn,"","");
                        break;
                    case "4":
                        Console.Clear();
                        CharacterList.CharacterMenu(conn);
                        break;
                    case "5":
                        break;
                    default:
                        Console.WriteLine("\nDu har matat in ett felaktigt värde. (Press any key to continue...)");
                        Console.ReadKey();
                        break;

                }
            } while (input.KeyChar.ToString() != "5");
        }
    } 
}

    