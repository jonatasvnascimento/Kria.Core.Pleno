namespace Kria.Core.Pleno.Lib.Ultils;

public static class Terminal
{
    public static void Mensagem(string mensagem = "", string info = "Info: ", ConsoleColor color = ConsoleColor.DarkGreen)
    {
        Console.ForegroundColor = color;
        Console.Write(info);
        Console.ResetColor();
        Console.WriteLine(mensagem);
    }
}