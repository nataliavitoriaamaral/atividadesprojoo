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

// API externa da qual não temos acesso
public class servicoSMSExterno
{
    public void dispararSMS(string numeroDestino, string texto)
    {
        Console.WriteLine($"Enviando para {numeroDestino} -> mensagem: {texto}");
    }
}

// O Adaptador faz a conexão entre a interface INotificacao e a API externa 
public class adaptadorSMS : INotificacao
{
    private servicoSMSExterno _servicoLegado;

    public adaptadorSMS()
    {
        _servicoLegado = new servicoSMSExterno();
    }

    public void Enviar(string mensagem)
    {
        // Adapta a chamada do sistema para o formato que a API externa exige
        string numeroFicticio = "(12) 98899-5634"; 
        _servicoLegado.dispararSMS(numeroFicticio, mensagem);
    }
}

// O Proxy controla o acesso ao objeto original adicionando logs e validações
public class ProxyNotificacaoSegura : INotificacao
{
    private INotificacao _notificacaoReal;
    private int _tentativasAtuais; 

    public ProxyNotificacaoSegura(INotificacao notificacaoReal)
    {
        _notificacaoReal = notificacaoReal;
        _tentativasAtuais = 0;
    }

    public void Enviar(string mensagem)
    {
        Console.WriteLine("\nRequisição de envio interceptada pelo Proxy para o usuário");

        if (string.IsNullOrWhiteSpace(mensagem))
        {
            Console.WriteLine("Validação falhou: A mensagem não pode ser vazia.");
            return;
        }

        int limiteGlobal = ConfigGlobal.GetInstance().Tentativas;
        if (_tentativasAtuais >= limiteGlobal)
        {
            Console.WriteLine($"O limite de {limiteGlobal} envios para esta instância foi alcançado.");
            return;
        }

        _tentativasAtuais++;
        Console.WriteLine($"A validação foi concluída. Tentativa {_tentativasAtuais}/{limiteGlobal}.");
        
        _notificacaoReal.Enviar(mensagem);
        
        Console.WriteLine("O envio foi finalizado e registrado");
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
            case "SMS_EXTERNO": 
                return new adaptadorSMS();
            default:
                throw new ArgumentException($"Tipo de notificação desconhecido.");
        }
    }
}