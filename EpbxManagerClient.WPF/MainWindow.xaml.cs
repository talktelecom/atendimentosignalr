using EpbxManagerClient.Atendimento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Configuration;
using System.Net;

namespace EpbxManagerClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private readonly IAtendimentoClient Atendimento;
        private readonly AtendimentoSignalrClient Atendimento;
        private readonly WindowState State;

        public MainWindow()
        {
            InitializeComponent();

            //URL de conexão com o Signalr
            Atendimento = new AtendimentoSignalrClient(ConfigurationManager.AppSettings["EpbxManager.ServiceUrl"] ?? "http://integracao.epbx.com.br/Service/");

            InscreverEventos();

            DataContext = State = new WindowState();

            Height = LogarGroupBox.Height + LogarGroupBox.Margin.Bottom + LogarGroupBox.Margin.Top + 20;

            if(optCtiIp.IsChecked == true)
            {
                //Pegar o IP da maquina aonde esta o SoftPhone
                string nome = Dns.GetHostName();

                IPAddress[] ip = Dns.GetHostAddresses(nome);
                
                txtIpOrRamal.Text = ip[1].ToString();
            }
            btnRetirarEspera.IsEnabled = false;
        }

        private async void btnLogar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // o metodo abaixo encapsula 3 ações:
                // 1º: logar na WebApi com as credenciais do usuário usando o padrão OAuth e receber um Access Token
                // 2º: usa o Access Token para abrir uma conexão permanente com o servidor, usando o SignalR.
                // 3º: envia o comando de logar o ramal através do Signalr
                await Atendimento.Logar(txtUsuario.Text, txtSenha.Password, txtIpOrRamal.Text, GetTipoLogonSelecionado());

                State.IsLogado = true;
            }
            catch(Exception ex)
            {
                ShowError(ex);
            }
        }

        private TipoLogon GetTipoLogonSelecionado()
        {
            if (optCtiHd.IsChecked == true) return TipoLogon.IdPa;

            // ramais possuem menos de 5 digitos. 
            if ((txtIpOrRamal.Text?.Length ?? 0) < 5) return TipoLogon.RamalVirtual;

            // default: logar com o ip
            return TipoLogon.Ip;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Atendimento.Dispose();

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            Atendimento.Dispose();

            base.OnClosed(e);
        }

        private async void btnDeslogar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Evento de deslogar ramal
                await Atendimento.Deslogar();

                cboIntervalo.Items.Clear();

                State.IsLogado = false;

            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void btnDiscar_Click(object sender, RoutedEventArgs e)
        {
            //Escolher o tipo de discagem 
            var tipoDiscagem = chkDiscarExterno.IsChecked == true ? TipoDiscagem.LigacaoExterna : TipoDiscagem.LigacaoRamal;

            try
            {
                //Discagem
                await Atendimento.Discar(txtDiscarNumero.Text, tipoDiscagem);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void ShowError(Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowMessage(string message, string titulo = "Info")
        {
            MessageBox.Show(message, titulo, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void btnDesligar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Evento para desligar uma chamada
                await Atendimento.Desligar();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }
        
        private async void btnIntervalo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Evento para entrar em um status de intervalo
                await Atendimento.AlterarIntervaloTipo(State.IntervaloInfoAtivo.RamalStatusDetalheId);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void btnEspera_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Evento para entrar em um status de intervalo
                await Atendimento.IniciarEspera();
                btnRetirarEspera.IsEnabled = true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void btnRetirarEspera_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Evento para entrar em um status de intervalo
                await Atendimento.TerminarEspera();
                btnRetirarEspera.IsEnabled = false;
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }


        //Retornos do Atendimento
        private void InscreverEventos()
        {
            Atendimento.OnSignalrDesconectado += Atendimento_OnSignalrDesconectado;
            Atendimento.OnConexaoErro += Atendimento_OnConexaoErro;
            Atendimento.OnDeslogado += Atendimento_OnDeslogado;
            Atendimento.OnDesliga += Atendimento_OnDesliga;
            Atendimento.OnDisca += Atendimento_OnDisca;
            Atendimento.OnChamada += Atendimento_OnChamada;
            Atendimento.OnAtendido += Atendimento_OnAtendido;
            Atendimento.OnInfoIntervaloRamal += Atendimento_OnInfoIntervaloRamal;
            Atendimento.OnSetIntervaloRamal += Atendimento_OnSetIntervaloRamal;
            Atendimento.OnDiscaStatus += Atendimento_OnDiscaStatus;
        }

        //Status da Discagem de Oculpado, Não Atende, Serviço
        private void Atendimento_OnDiscaStatus(object sender, RamalInfo ramal, ChamadaInfo chamada, ResultadoDiscagem status)
        {
            lblStatus.Content = status.StatusDiscagem;
        }

        //Retorno do Intervalo Selecionado
        private void Atendimento_OnSetIntervaloRamal(object sender, RamalInfo ramal, IntervaloInfo intervalo)
        {
            if (intervalo.RamalStatusDestinoId == 0) // Destino se entrou em intervalo == 0 se 1 intervalo pendente
            {
                if(State.IntervaloInfoAtivo !=null)
                    lblStatus.Content = State.IntervaloInfoAtivo.Descricao;
            }
                
        }
        
        //Retorno de todos os Status de Intervalos cadastrados
        private void Atendimento_OnInfoIntervaloRamal(object sender, RamalInfo ramal, IntervaloInfo intervalo)
        {
            /*
             * Retornos
             * ramal.Numero = ramal Logado
             * intervalo.Descricao = Descrição do intervalo
             * intervalo.RamalStatusDetalheId = Id do Intervalo
             * intervalo.Produtivo = Se o Intervalo é Produtivo = true ou Improdutivo = false
             */
            if (!State.IntervaloInfoList.Any(i => i.RamalStatusDetalheId == intervalo.RamalStatusDetalheId))
            {
                State.IntervaloInfoList.Add(intervalo);
            }
            // cboIntervalo.Items.Add(intervalo);
            //cboIntervalo.Items.Add(new ComboBoxItem { Content = intervalo.Descricao, Tag = intervalo.RamalStatusDetalheId.ToString() });
        }

        //Desconexão do Signalr
        private void Atendimento_OnSignalrDesconectado(object sender, EventArgs e)
        {
            ShowMessage("Conexão do signalr indisponivel");
            cboIntervalo.Items.Clear();
            State.IsLogado = false;
        }

        //Deslogando o Ramal
        private void Atendimento_OnDeslogado(object sender, RamalInfo e)
        {
            ShowMessage("Ramal deslogado");
            cboIntervalo.Items.Clear();
            State.IsLogado = false;
        }

        //Retorno do discagem
        private void Atendimento_OnDisca(object sender, RamalInfo ramal, ChamadaInfo chamada)
        {
            // Realizando uma chamada ( discando )
            ShowMessage("Ramal Discando");
            State.IsEmChamada = true;
            State.ChamadaAtiva = chamada;
        }
        
        //Retorno de chamada Atendida
        private void Atendimento_OnAtendido(object sender, RamalInfo ramal, ChamadaInfo chamada)
        {
            // Chamada conectada
            /*
             * chamada.CanalId = Canal da chamada no servidor de telefonia
             * chamada.CodigoCampanha = Codigo da Campanha no Discador (para ligações ativas)
             * chamada.CodigoCliente = Codigo de CLiente para Discador (para ligações ativas)
             * chamada.DDRLocal = Numero DialOut numero a binar no Cliente 
             * chamada.InfoAdicional1 = Informações Adicionais no Discador (para ligações ativas)
             * chamada.InfoAdicional2 = Informações Adicionais no Discador (para ligações ativas)
             * chamada.InfoAdicional3 = Informações Adicionais no Discador (para ligações ativas)
             * chamada.InfoAdicional4 = Informações Adicionais no Discador (para ligações ativas)
             * chamada.InfoAdicional5 = Informações Adicionais no Discador (para ligações ativas)
             * chamada.Nome Cliente = Nome do Cliente para discador (para ligações ativas)
             * chamada.Telefone = Numero Discado 
             * chamada.TelefoneCliente = Numero do telefone Cliente para Discador (para ligações ativas)
             * chamada.LigacaoExterna = Direção da Ligação (LigacaoExterna ou LigacaoRamal)
            */

            ShowMessage("Ligação Atendido");
            State.IsEmChamada = true;
            State.ChamadaAtiva = chamada;
        }

        //Retorno de Recebendo Chamada
        private void Atendimento_OnChamada(object sender, RamalInfo ramal, ChamadaInfo chamada)
        {
            // Recebendo chamada
            ShowMessage("Ramal Chamada");
            State.IsEmChamada = true;
            State.ChamadaAtiva = chamada;
        }

        //Retorno do Desligou chamada
        private void Atendimento_OnDesliga(object sender, RamalInfo ramal, ChamadaInfo chamada)
        {
            // chamada em curso foi desligada
            State.IsEmChamada = false;
            State.ChamadaAtiva = null;
        }

        //Retorno de erro de conexão
        private void Atendimento_OnConexaoErro(object sender, RamalInfo ramal, Exception ex)
        {
            ShowError(ex);

            State.IsLogado = false;

        }

        private void optCtiIp_Click(object sender, RoutedEventArgs e)
        {
            string nome = Dns.GetHostName();
            IPAddress[] ip = Dns.GetHostAddresses(nome);
            txtIpOrRamal.Text = ip[1].ToString();
        }

        private void optCtiHd_Click(object sender, RoutedEventArgs e)
        {
            txtIpOrRamal.Text = "";
        }

        private void cboIntervalo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
