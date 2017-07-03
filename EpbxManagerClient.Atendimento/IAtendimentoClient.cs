using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EpbxManagerClient.Atendimento
{
    public interface IAtendimentoClient
    {
        /// <summary>
        /// Evento que sinaliza que o Signalr foi conectado
        /// </summary>
        event EventHandler OnSignalrConectado;

        /// <summary>
        /// Evento que sinaliza que o Signalr foi desconectado
        /// </summary>
        event EventHandler OnSignalrDesconectado;

        event EventHandler<RamalInfo, Exception> OnAlterarIntervaloTipoErro;
        event EventHandler<RamalInfo, ChamadaInfo> OnAtendido;
        event EventHandler<RamalInfo, ChamadaInfo> OnChamada;
        event EventHandler<RamalInfo, ChamadaInfo> OnChamadaEntrouNaFila;
        event EventHandler<RamalInfo, ChamadaInfo, string> OnChamadaGlobalId;
        event EventHandler<RamalInfo, ChamadaInfo> OnChamadaPerdida;
        event EventHandler<RamalInfo, ChamadaInfo> OnChamadaSaiuDaFila;
        event EventHandler<RamalInfo, ChamadaInfo> OnChamadaTransferida;

        /// <summary>
        /// Indica um erro de conexão do ramal
        /// </summary>
        event EventHandler<RamalInfo, Exception> OnConexaoErro;

        event EventHandler<RamalInfo, ChamadaInfo> OnConferenciaAtendido;
        event EventHandler<RamalInfo, ChamadaInfo> OnConferenciaChamadaEncerrada;
        event EventHandler<RamalInfo, ChamadaInfo> OnConferenciaDisca;
        event EventHandler<RamalInfo, Exception> OnConferenciaDiscaErro;
        event EventHandler<RamalInfo, Exception> OnConferenciaErro;
        event EventHandler<RamalInfo> OnConferenciaInicio;
        event EventHandler<RamalInfo> OnConferenciaTermino;
        event EventHandler<RamalInfo, ChamadaInfo> OnConsultaAtendido;
        event EventHandler<RamalInfo, ChamadaInfo> OnConsultaChamada;
        event EventHandler<RamalInfo, ChamadaInfo> OnDesliga;

        /// <summary>
        /// Indica que o ramal foi deslogado remotamente
        /// </summary>
        event EventHandler<RamalInfo> OnDeslogado;

        event EventHandler<RamalInfo, ChamadaInfo> OnDisca;
        event EventHandler<RamalInfo, ChamadaInfo, Exception> OnDiscaErro;
        event EventHandler<RamalInfo, ChamadaInfo, ResultadoDiscagem> OnDiscaStatus;
        event EventHandler<RamalInfo> OnEntrouEmConferencia;
        event EventHandler<RamalInfo, IntervaloInfo> OnInfoIntervaloRamal;
        event EventHandler<RamalInfo> OnInicioEspera;
        event EventHandler<RamalInfo> OnInicioIntervalo;
        event EventHandler<RamalInfo> OnInicioNaoDisponivel;
        event EventHandler<RamalInfo> OnNumerosSigaMeMultiplo;
        event EventHandler<RamalInfo, IntervaloInfo> OnSetIntervaloRamal;
        event EventHandler<RamalInfo> OnTerminoEspera;
        event EventHandler<RamalInfo> OnTerminoIntervalo;
        event EventHandler<RamalInfo> OnTerminoNaoDisponivel;

        Task AlterarIntervaloTipo(int tipoIntervalo);

        Task AtendeChamadaNaFila(string canalId);

        Task CancelaSigaMe();

        Task CapturaDirigida();

        Task ConferenciaAdicionar(string numero, TipoDiscagem tipoDiscagem);

        Task ConferenciaCancelar();

        Task ConferenciaIniciar();

        Task ConferenciaRemover(string numero);

        Task ConferenciaSair();

        Task ConferenciaTerminar();

        Task Consultar(string numero, TipoDiscagem tipoDiscagem);

        Task Desligar();

        Task DesligarChamada(string canalId);

        Task Deslogar();

        Task Discar(string numero, TipoDiscagem tipoDiscagem);

        Task IniciarEspera();

        Task LiberarConsulta();

        Task Logar(string usuario, string senha, string idPaOrIpOrRamal, TipoLogon tipoLogon);

        Task SigaMe(string numero, TipoDiscagem tipoDiscagem);

        Task SigaMeMultiplo(IEnumerable<string> numeros);

        Task TerminarEspera();

        Task TransfereVoiceMail();

        Task Transferir(string numero, TipoDiscagem tipoDiscagem);
    }
}