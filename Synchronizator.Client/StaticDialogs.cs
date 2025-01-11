using System.Threading.Channels;

namespace Synchronizator.Client;

public static class StaticDialogs
{
    public static void PrintMainMenu()
    {
        Console.Clear();
        Console.WriteLine("\"LOCAL\" pro vypsání všech souborů v lokální synchronizační složce");
        Console.WriteLine("\"REMOTE\" pro vypsání souborů které jsou uloženy na serveru");
        Console.WriteLine("\"SYNC\" pro spuštění synchronizace souborů ve složce");
        Console.WriteLine("\"INFO\" pro vypsání popisu aplikace");
        Console.Write("Zadejte slovo:");
    }

    public static void PrintInfo()
    {
        Console.Clear();
        Console.WriteLine("Aplikace slouží jako pomocník při synchronizaci souborů mezi zařízeními. \n " +
                          "Jednoduše na jednom zařízení spustíte synchronizaci a soubory z disku se nahrají na server.\n" +
                          " Následně spustíte synchronizaci na druhém zařízení a aplikace stáhne ze \n " +
                          "serveru soubory které na disku chybí.");
        PrintContinue();
    }

    public static void PrintContinue()
    {
        Console.WriteLine();
        Console.WriteLine("Stiskněte klávesu ENTER pro pokračování...");
        Console.ReadLine();
        Console.Clear();
    }
}