using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpbxManagerClient.Atendimento
{
    // contem todas as ações de telefonia (discar, desligar, colocar em espera, etc)

    public partial class AtendimentoSignalrClient
    {
        public Task AlterarIntervaloTipo(int tipoIntervalo)
        {
            return AtendimentoHubProxy.Invoke(nameof(AlterarIntervaloTipo), tipoIntervalo);
        }

        public Task AtendeChamadaNaFila(string canalId)
        {
            AssertNotEmpty(canalId, nameof(canalId));

            return AtendimentoHubProxy.Invoke(nameof(AtendeChamadaNaFila), canalId);
        }

        public Task CancelaSigaMe()
        {
            return AtendimentoHubProxy.Invoke(nameof(CancelaSigaMe));
        }

        public Task CapturaDirigida()
        {
            return AtendimentoHubProxy.Invoke(nameof(CapturaDirigida));
        }

        public Task ConferenciaAdicionar(string numero, TipoDiscagem tipoDiscagem)
        {
            AssertNotEmpty(numero, nameof(numero));

            return AtendimentoHubProxy.Invoke(nameof(ConferenciaAdicionar), numero, tipoDiscagem.GetHashCode());
        }

        public Task ConferenciaCancelar()
        {
            return AtendimentoHubProxy.Invoke(nameof(ConferenciaCancelar));
        }

        public Task ConferenciaIniciar()
        {
            return AtendimentoHubProxy.Invoke(nameof(ConferenciaIniciar));
        }

        public Task ConferenciaRemover(string numero)
        {
            return AtendimentoHubProxy.Invoke(nameof(ConferenciaRemover), numero);
        }

        public Task ConferenciaSair()
        {
            return AtendimentoHubProxy.Invoke(nameof(ConferenciaSair));
        }

        public Task ConferenciaTerminar()
        {
            return AtendimentoHubProxy.Invoke(nameof(ConferenciaTerminar));
        }

        public Task Consultar(string numero, TipoDiscagem tipoDiscagem)
        {
            AssertNotEmpty(numero, nameof(numero));

            return AtendimentoHubProxy.Invoke(nameof(Consultar), numero, tipoDiscagem.GetHashCode());
        }

        public Task Discar(string numero, TipoDiscagem tipoDiscagem)
        {
            AssertNotEmpty(numero, nameof(numero));

            return AtendimentoHubProxy.Invoke(nameof(Discar), numero, tipoDiscagem.GetHashCode());
        }

        public Task Desligar()
        {
            return AtendimentoHubProxy.Invoke(nameof(Desligar));
        }

        public Task DesligarChamada(string canalId)
        {
            return AtendimentoHubProxy.Invoke(nameof(DesligarChamada), canalId);
        }

        public Task IniciarEspera()
        {
            return AtendimentoHubProxy.Invoke(nameof(IniciarEspera));
        }

        public Task LiberarConsulta()
        {
            return AtendimentoHubProxy.Invoke(nameof(LiberarConsulta));
        }

        public Task SigaMe(string numero, TipoDiscagem tipoDiscagem)
        {
            AssertNotEmpty(numero, nameof(numero));

            return AtendimentoHubProxy.Invoke(nameof(SigaMe), numero, tipoDiscagem.GetHashCode());
        }

        public Task SigaMeMultiplo(IEnumerable<string> numeros)
        {
            AssertNotNull(numeros, nameof(numeros));

            return AtendimentoHubProxy.Invoke(nameof(SigaMeMultiplo), numeros);
        }

        public Task TerminarEspera()
        {
            return AtendimentoHubProxy.Invoke(nameof(TerminarEspera));
        }

        public Task TransfereVoiceMail()
        {
            return AtendimentoHubProxy.Invoke(nameof(TerminarEspera));
        }

        public Task Transferir(string numero, TipoDiscagem tipoDiscagem)
        {
            AssertNotEmpty(numero, nameof(numero));

            return AtendimentoHubProxy.Invoke(nameof(Transferir), numero, tipoDiscagem.GetHashCode());
        }

        public Task<IEnumerable<RamalStatusInfo>> ListarRamalStatus()
        {
            return SupervisaoHubProxy.Invoke<IEnumerable<RamalStatusInfo>>(nameof(ListarRamalStatus));
        }
    }
}
