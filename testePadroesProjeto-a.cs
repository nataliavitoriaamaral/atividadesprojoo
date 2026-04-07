public class Program
{
    public static void Main()
    {
        Console.WriteLine("teste singleton\n");
        TesteSingleton();
        Console.WriteLine("teste factory\n");
        TesteFactory();
    }

    public static void TesteSingleton()
    {
        ConfigGlobal config1 = ConfigGlobal.GetInstance();
        ConfigGlobal config2 = ConfigGlobal.GetInstance();

        config1.NomeAplicacao = "exercicio_pratico";

        if (Object.ReferenceEquals(config1, config2))
        {
            Console.WriteLine("config1 e config2 são a mesma instância na memória.");
        }
        else
        {
            Console.WriteLine("as instâncias são diferentes");
        }

        // Verificamos se a alteração em config1 apareceu na config2
        if (config2.NomeAplicacao == "exercicio_pratico")
        {
            Console.WriteLine("A alteração feita em config1 foi identificada em config2.");
        }
        else
        {
            Console.WriteLine("Os dados não estão sincronizados");
        }
    }

    public static void TesteFactory()
    {
        // Testa a criação correta dos objetos
        INotificacao email = ComunicacaoFactory.CriarComunicacao("EMAIL");
        INotificacao sms = ComunicacaoFactory.CriarComunicacao("SMS");
        INotificacao push = ComunicacaoFactory.CriarComunicacao("PUSH");

        if (email is EmailNotificacao) Console.WriteLine("Objeto Email criado");
        else Console.WriteLine("Erro ao criar Email.");

        if (sms is SmsNotificacao) Console.WriteLine("Objeto SMS criado");
        else Console.WriteLine("Erro ao criar SMS.");

        if (push is PushNotificacao) Console.WriteLine("Objeto Push criado");
        else Console.WriteLine("Erro ao criar Push.");

        // T5esta o envio das mensagens 
        Console.WriteLine("\n>teste de envio de mensagem");
        email.Enviar("1");
        sms.Enviar("2");
        push.Enviar("3");

        Console.WriteLine("\nteste que deve retornar inválido");
        try
        {
            ComunicacaoFactory.CriarComunicacao("Ab");
            Console.WriteLine("O código deveria ter bloqueado");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"A Factory bloqueou o erro corretamente");
        }
    }
}