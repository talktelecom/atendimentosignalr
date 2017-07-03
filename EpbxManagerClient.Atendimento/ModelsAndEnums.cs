using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EpbxManagerClient.Atendimento
{
    public enum TipoDiscagem
    {
        LigacaoExterna = 1,
        LigacaoRamal = 2,
    }

    public enum StatusDiscagemExterno
    {
        Desconhecido = -2,
        NaoAtende = -1,
        Ocupado = 0,
        Congestionamento = 1,
        Servico = 2,
        RamalDesligou = 3,
    }

    public enum TipoLogon
    {
        IdPa = 1,
        Ip = 2,
        RamalVirtual = 3,
    }

    internal static class Constantes
    {
        public const string AuthorizationHeaderName = "Authorization";
        public const string AuthorizationHeaderType = "Bearer";
        public const string AuthorizationDefaultClientId = "abc";
        public const string AcceptJsonHeaderValue = "application/json";

        public const string AtendimentoHubName = "AtendimentoHub";

        public const string PropriedadeNumeroRamal = "Ramal";

        public const string SignalrMetodoIniciar = "Iniciar";
        public const string SignalrMetodoTerminar = "Terminar";

        public const string UrlSignal = "signalr";
        public const string UrlWebApi = "api";
        public const string UrlOAuth = "oauth2/Token";

        public const string ErrorMsgUsuarioSenhaInvalido = "Nome do usuário ou senha inválidos";
        public const string ErrorMsgUseLogon = "Necessário usar o método logar antes";
        public const string ErrorMsgUsuarioExpirou = "A autenticação expirou. Use o método logar novamente";
        public const string ErrorMsgArgumentoNull = "O parametro {0} não pode ser nulo ou vazio";
    }

    public delegate void EventHandler<TEventArg1, TEventArg2>(object sender, TEventArg1 arg1, TEventArg2 arg2);
    public delegate void EventHandler<TEventArg1, TEventArg2, TEventArg3>(object sender, TEventArg1 arg1, TEventArg2 arg2, TEventArg3 arg3);

    public class OAuthInfo
    {
        public virtual string AccessToken { get; set; }

        public virtual string RefreshToken { get; set; }

        public virtual long TokenExpiration { get; set; }
    }

    public class RamalInfo
    {
        /// <summary>
        /// Número do Ramal
        /// </summary>
        /// <remarks>
        /// Renomeia a propriedade Ramal do json para Numero no csharp, para não confudir com o nome da classe
        /// </remarks>
        [JsonProperty(Constantes.PropriedadeNumeroRamal)]        
        public int Numero { get; set; }
    }

    public enum TipoIntervaloGeralDto
    {
        Livre = 0,
        IntervaloGeral = 1,
        IntervaloRetemLigacaoEmFila = 2
    }

    public class IntervaloInfo
    {
        public string Descricao { get; set; }
        public int RamalStatusDetalheId { get; set; }
        public int RamalStatusDestinoId { get; set; }
        public bool Produtivo { get; set; }
        
    }

    public class ResultadoDiscagem
    {
        /// <summary>
        /// Canal alocado para a discagem
        /// </summary>
        public string CanalSainte { get; set; }

        /// <summary>
        //     Todo verificar com o RH Lopes que tipo de informacao temos Todo neste pacote
        //     ou com o Claudio
        /// </summary>
        public string InfoAdicional { get; set; }

        /// <summary>
        //     Status da ligacao
        /// </summary>
        public StatusDiscagemExterno StatusDiscagem { get; }

    }

    public class ChamadaInfo
    {
        /// <summary>
        ///  Identifica o canal da chamada.
        /// </summary>
        public string CanalId { get; set; }

        /// <summary>
        /// Número do telefone ou ramal remoto
        /// </summary>
        public string Telefone { get; set; }

        /// <summary>
        /// Discagem Externa ou Interna
        /// </summary>
        public TipoDiscagem TipoDiscagem { get; set; }

        /// <summary>
        /// Representa o numero de telefone (DDR) do ramal em uso
        /// </summary>
        public string DDRLocal { get; set; }

        /// <summary>
        /// Intervalo Customizado
        /// </summary>
        public int RamalStatusDetalheId { get; set; }


        /// <summary>
        /// Codigo do cliente
        /// Atendimento ATIVO
        /// </summary>
        string CodigoCliente { get; set; }

        /// <summary>
        /// Nome do cliente
        /// Atendimento ATIVO
        /// </summary>
        string NomeCliente { get; set; }

        /// <summary>
        /// Informações adicionais customizadas
        /// no mailing, estas informações são
        /// fornecidas no momento de importar
        /// o mailing. Podemo ter até 5 info!
        /// </summary>
        string InfoAdicional1 { get; set; }
        string InfoAdicional2 { get; set; }
        string InfoAdicional3 { get; set; }
        string InfoAdicional4 { get; set; }
        string InfoAdicional5 { get; set; }

        /// <summary>
        /// Telefone do cliente
        /// Atendimento ATIVO
        /// </summary>
        string TelefoneCliente { get; set; }

        /// <summary>
        /// Codigo da campanha do discador
        /// Atendimento ATIVO
        /// </summary>
        string CodigoCampanha { get; set; }
    }
}