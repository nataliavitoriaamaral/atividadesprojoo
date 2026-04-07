using System;

public class ConfigGlobal
{
    private static ConfigGlobal _instance;

    // configurações globais
    public string NomeAplicacao { get; set; }
    public string ServidorEnvio { get; set; }
    public int Tentativas { get; set; }

    // Construtor privado para impedir o uso de 'new' fora da classe
    private ConfigGlobal()
    {
        // Valores iniciais
        NomeAplicacao = "sistema inicial";
        ServidorEnvio = "unifesp.com";
        Tentativas = 5;
    }

    // Método para obter a instância única
    public static ConfigGlobal GetInstance()
    {
        if (_instance == null)
        {
            _instance = new ConfigGlobal();
        }
        return _instance;
    }
}


// Interface comum para todas as notificações 
public interface INotificacao
{
    void Enviar(string mensagem);
}

// Implementações para e-mail, SMS e push notification
public class EmailNotificacao : INotificacao
{
    public void Enviar(string mensagem)
    {
        string servidor = ConfigGlobal.GetInstance().ServidorEnvio;
        Console.WriteLine($"e-mail de {servidor}: {mensagem}");
    }
}

public class SmsNotificacao : INotificacao
{
    public void Enviar(string mensagem)
    {
        Console.WriteLine($"SMS: {mensagem}");
    }
}

public class PushNotificacao : INotificacao
{
    public void Enviar(string mensagem)
    {
        string app = ConfigGlobal.GetInstance().NomeAplicacao;
        Console.WriteLine($"push para '{app}': {mensagem}");
    }
}

// Classe Factory
public class ComunicacaoFactory
{
    public static INotificacao CriarComunicacao(string tipo)
    {
        if (string.IsNullOrEmpty(tipo)) return null;

       // converte para maiusculas
        switch (tipo.ToUpper())
        {
            case "EMAIL":
                return new EmailNotificacao();
            case "SMS":
                return new SmsNotificacao();
            case "PUSH":
                return new PushNotificacao();
            default:
                throw new ArgumentException($"Tipo de notificação desconhecido.");
        }
    }
}