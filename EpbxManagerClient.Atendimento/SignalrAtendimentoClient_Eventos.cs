using System;
using Microsoft.AspNet.SignalR.Client;

namespace EpbxManagerClient.Atendimento
{
    // contem todos os eventos de telefonia (chamada, conferencia, intervalos, etc)


    public partial class AtendimentoSignalrClient
    {
        public event EventHandler OnSignalrConectado;
        public event EventHandler OnSignalrDesconectado;

        public event EventHandler<RamalInfo> OnDeslogado;
        public event EventHandler<RamalInfo, IntervaloInfo> OnInfoIntervaloRamal;
        public event EventHandler<RamalInfo, IntervaloInfo> OnSetIntervaloRamal;
        public event EventHandler<RamalInfo, ChamadaInfo> OnDisca;
        public event EventHandler<RamalInfo, ChamadaInfo> OnChamada;
        public event EventHandler<RamalInfo, ChamadaInfo> OnChamadaPerdida;
        public event EventHandler<RamalInfo, ChamadaInfo> OnAtendido;
        public event EventHandler<RamalInfo, ChamadaInfo> OnDesliga;
        public event EventHandler<RamalInfo, ChamadaInfo> OnChamadaTransferida;
        public event EventHandler<RamalInfo, ChamadaInfo> OnChamadaEntrouNaFila;
        public event EventHandler<RamalInfo, ChamadaInfo> OnChamadaSaiuDaFila;
        public event EventHandler<RamalInfo, ChamadaInfo, string> OnChamadaGlobalId;
        public event EventHandler<RamalInfo, ChamadaInfo, ResultadoDiscagem> OnDiscaStatus;
        public event EventHandler<RamalInfo> OnNumerosSigaMeMultiplo;
        public event EventHandler<RamalInfo> OnInicioIntervalo;
        public event EventHandler<RamalInfo> OnTerminoIntervalo;
        public event EventHandler<RamalInfo> OnInicioNaoDisponivel;
        public event EventHandler<RamalInfo> OnTerminoNaoDisponivel;
        public event EventHandler<RamalInfo> OnInicioEspera;
        public event EventHandler<RamalInfo> OnTerminoEspera;
        public event EventHandler<RamalInfo> OnEntrouEmConferencia;
        public event EventHandler<RamalInfo> OnConferenciaInicio;
        public event EventHandler<RamalInfo> OnConferenciaTermino;
        public event EventHandler<RamalInfo, ChamadaInfo> OnConferenciaDisca;
        public event EventHandler<RamalInfo, ChamadaInfo> OnConferenciaAtendido;
        public event EventHandler<RamalInfo, ChamadaInfo> OnConferenciaChamadaEncerrada;
        public event EventHandler<RamalInfo, ChamadaInfo> OnConsultaChamada;
        public event EventHandler<RamalInfo, ChamadaInfo> OnConsultaAtendido;
        public event EventHandler<RamalInfo, Exception> OnConexaoErro;
        public event EventHandler<RamalInfo, Exception> OnConferenciaErro;
        public event EventHandler<RamalInfo, Exception> OnConferenciaDiscaErro;
        public event EventHandler<RamalInfo, ChamadaInfo, Exception> OnDiscaErro;
        public event EventHandler<RamalInfo, Exception> OnAlterarIntervaloTipoErro;

        public event EventHandler<RamalStatusInfo> OnRamalStatusInfo;

        /// <summary>
        /// Inscrever-se no Hub signalr para receber os eventos da Telefonia
        /// </summary>
        private void PrepareEventsHub()
        {
            AtendimentoHubProxy.On(nameof(OnSignalrConectado), CreateHandler(OnSignalrConectado));
            AtendimentoHubProxy.On(nameof(OnSignalrDesconectado), CreateHandler(OnSignalrDesconectado));
            AtendimentoHubProxy.On(nameof(OnDeslogado), CreateHandler(OnDeslogado));
            AtendimentoHubProxy.On(nameof(OnInfoIntervaloRamal), CreateHandler(OnInfoIntervaloRamal));
            AtendimentoHubProxy.On(nameof(OnSetIntervaloRamal), CreateHandler(OnSetIntervaloRamal));
            AtendimentoHubProxy.On(nameof(OnDisca), CreateHandler(OnDisca));
            AtendimentoHubProxy.On(nameof(OnChamada), CreateHandler(OnChamada));
            AtendimentoHubProxy.On(nameof(OnChamadaPerdida), CreateHandler(OnChamadaPerdida));
            AtendimentoHubProxy.On(nameof(OnAtendido), CreateHandler(OnAtendido));
            AtendimentoHubProxy.On(nameof(OnDesliga), CreateHandler(OnDesliga));
            AtendimentoHubProxy.On(nameof(OnChamadaTransferida), CreateHandler(OnChamadaTransferida));
            AtendimentoHubProxy.On(nameof(OnChamadaEntrouNaFila), CreateHandler(OnChamadaEntrouNaFila));
            AtendimentoHubProxy.On(nameof(OnChamadaSaiuDaFila), CreateHandler(OnChamadaSaiuDaFila));
            AtendimentoHubProxy.On(nameof(OnChamadaGlobalId), CreateHandler(OnChamadaGlobalId));
            AtendimentoHubProxy.On(nameof(OnDiscaStatus), CreateHandler(OnDiscaStatus));
            AtendimentoHubProxy.On(nameof(OnNumerosSigaMeMultiplo), CreateHandler(OnNumerosSigaMeMultiplo));
            AtendimentoHubProxy.On(nameof(OnInicioIntervalo), CreateHandler(OnInicioIntervalo));
            AtendimentoHubProxy.On(nameof(OnTerminoIntervalo), CreateHandler(OnTerminoIntervalo));
            AtendimentoHubProxy.On(nameof(OnInicioNaoDisponivel), CreateHandler(OnInicioNaoDisponivel));
            AtendimentoHubProxy.On(nameof(OnTerminoNaoDisponivel), CreateHandler(OnTerminoNaoDisponivel));
            AtendimentoHubProxy.On(nameof(OnInicioEspera), CreateHandler(OnInicioEspera));
            AtendimentoHubProxy.On(nameof(OnTerminoEspera), CreateHandler(OnTerminoEspera));
            AtendimentoHubProxy.On(nameof(OnEntrouEmConferencia), CreateHandler(OnEntrouEmConferencia));
            AtendimentoHubProxy.On(nameof(OnConferenciaInicio), CreateHandler(OnConferenciaInicio));
            AtendimentoHubProxy.On(nameof(OnConferenciaTermino), CreateHandler(OnConferenciaTermino));
            AtendimentoHubProxy.On(nameof(OnConferenciaDisca), CreateHandler(OnConferenciaDisca));
            AtendimentoHubProxy.On(nameof(OnConferenciaAtendido), CreateHandler(OnConferenciaAtendido));
            AtendimentoHubProxy.On(nameof(OnConferenciaChamadaEncerrada), CreateHandler(OnConferenciaChamadaEncerrada));
            AtendimentoHubProxy.On(nameof(OnConsultaChamada), CreateHandler(OnConsultaChamada));
            AtendimentoHubProxy.On(nameof(OnConsultaAtendido), CreateHandler(OnConsultaAtendido));
            AtendimentoHubProxy.On(nameof(OnConexaoErro), CreateHandler(OnConexaoErro));
            AtendimentoHubProxy.On(nameof(OnConferenciaErro), CreateHandler(OnConferenciaErro));
            AtendimentoHubProxy.On(nameof(OnConferenciaDiscaErro), CreateHandler(OnConferenciaDiscaErro));
            AtendimentoHubProxy.On(nameof(OnDiscaErro), CreateHandler(OnDiscaErro));
            AtendimentoHubProxy.On(nameof(OnAlterarIntervaloTipoErro), CreateHandler(OnAlterarIntervaloTipoErro));

            SupervisaoHubProxy.On<string, RamalStatusInfo>(Constantes.SupervisaoModelEvento, (modelName, info) =>
            {
                if (Constantes.SupervisaoRamalEvento.Equals(modelName, StringComparison.InvariantCultureIgnoreCase))
                {
                    RaiseEvent(OnRamalStatusInfo, info);
                }
            });
        }
    }
}
