using Microsoft.AspNet.SignalR.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EpbxManagerClient.Atendimento
{
    public partial class AtendimentoSignalrClient : IAtendimentoClient
    {
        private readonly Uri _signalrUri;
        private readonly string _oauthClientId;

        private string _idPaOrIpOrRamal;
        private TipoLogon _tipoLogon;

        private OAuthInfo _authInfo;
        private Timer _timerRefreshToken;

        private bool _stopCalled;
        private readonly SynchronizationContext _syncContext;
        private readonly WebApiClient _webApiClient;

        protected virtual HubConnection SignalrConnection { get; private set; }

        protected virtual IHubProxy AtendimentoHubProxy { get; private set; }

        protected virtual IHubProxy SupervisaoHubProxy { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceUrl"></param>
        public AtendimentoSignalrClient(string serviceUrl) : this(serviceUrl, Constantes.AuthorizationDefaultClientId, null) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceUrl"></param>
        public AtendimentoSignalrClient(string serviceUrl, string oauthClientId, WebApiClient webApiClient)
        {
            // quando a classe for criada a partir da Thread UI, o contexto de sync será preenchido corretamente
            _syncContext = SynchronizationContext.Current;

            var baseUri = new Uri(Helper.FormatUrlString(serviceUrl));
            _signalrUri = new Uri(baseUri, Constantes.UrlSignal);

            _oauthClientId = oauthClientId ?? Constantes.AuthorizationDefaultClientId;

            _webApiClient = webApiClient ?? new WebApiClient(serviceUrl, _oauthClientId);
        }

        public async Task Logar(string usuario, string senha, string idPaOrIpOrRamal, TipoLogon tipoLogon)
        {
            AssertNotEmpty(usuario, nameof(usuario));
            AssertNotEmpty(senha, nameof(senha));
            AssertNotEmpty(idPaOrIpOrRamal, nameof(idPaOrIpOrRamal));

            // autenticar e buscar access token
            await Autenticar(usuario, senha);

            // conectar no Signalr
            await ConectarSignalr();

            // logar o ramal (telefone deve estar conectado)
            await LogarRamal(idPaOrIpOrRamal, tipoLogon);
        }

        public virtual async Task RelogarRamal()
        {
            if(string.IsNullOrWhiteSpace(_idPaOrIpOrRamal))
            {
                throw new InvalidOperationException(Constantes.ErrorMsgUseLogon);
            }

            if (string.IsNullOrWhiteSpace(_authInfo.RefreshToken))
            {
                throw new InvalidOperationException(Constantes.ErrorMsgUsuarioExpirou);
            }

            _authInfo = await _webApiClient.AtualizarAccessToken(_authInfo);

            await ConectarSignalr();

            await LogarRamal(_idPaOrIpOrRamal, _tipoLogon);
        }

        protected virtual async Task Autenticar(string usuario, string senha)
        {
            _authInfo = await _webApiClient.Autenticar(usuario, senha);

            SetTimerAtualizarRefreshToken();
        }

        protected virtual Task ConectarSignalr()
        {
            SignalrConnection?.Dispose();

            _stopCalled = false;

            SignalrConnection = new HubConnection(_signalrUri.ToString());
            AtualizarHeaderAuthorization();

            SignalrConnection.Closed += Connection_Closed;
            SignalrConnection.Error += Connection_Error;
            SignalrConnection.ConnectionSlow += Connection_Slow;
            SignalrConnection.Reconnecting += Connection_Reconnecting;
            SignalrConnection.Reconnected += Connection_Reconnected;

            AtendimentoHubProxy = SignalrConnection.CreateHubProxy(Constantes.AtendimentoHubName);
            SupervisaoHubProxy = SignalrConnection.CreateHubProxy(Constantes.SupervisaoHubName);

            PrepareEventsHub();

            return SignalrConnection.Start();
        }

        protected virtual async Task LogarRamal(string idPaOrIpOrRamal, TipoLogon tipoLogon)
        {
            if(AtendimentoHubProxy == null)
            {
                await ConectarSignalr();
            }

            // salvar para relogar
            _tipoLogon = tipoLogon;
            _idPaOrIpOrRamal = idPaOrIpOrRamal;

            await AtendimentoHubProxy.Invoke(Constantes.SignalrMetodoIniciar, idPaOrIpOrRamal, tipoLogon);
            await SupervisaoHubProxy.Invoke(Constantes.SignalrMetodoIniciar);
        }

        public virtual async Task Deslogar()
        {
            _stopCalled = true;
            await AtendimentoHubProxy.Invoke(Constantes.SignalrMetodoTerminar);
            await SupervisaoHubProxy.Invoke(Constantes.SignalrMetodoParar);
            ClearConnection();
        }

        private void AtualizarHeaderAuthorization()
        {
            if(SignalrConnection != null)
            {
                SignalrConnection.Headers[Constantes.AuthorizationHeaderName] = $"{Constantes.AuthorizationHeaderType} {_authInfo.AccessToken}";
            }
        }

        /// <summary>
        /// Atualiza o Access Token a cada 5 minutos para não receber a mensagem de login expirado
        /// </summary>
        protected virtual void SetTimerAtualizarRefreshToken()
        {
            if (_timerRefreshToken == null)
            {
                _timerRefreshToken = new Timer(_ =>
                {
                    if (!string.IsNullOrWhiteSpace(_authInfo.RefreshToken))
                    {
                        _webApiClient.AtualizarAccessToken(_authInfo)
                            .ContinueWith(t => {
                                if (t.IsFaulted)
                                {
                                    // ignorar erros
                                }
                                else
                                {
                                    _authInfo = t.Result;
                                    AtualizarHeaderAuthorization();
                                }
                            });
                    }

                }, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
            }
        }

        private void Connection_Error(Exception ex)
        {
            ClearConnection();
            // aplicação deve reconectar em um momento apropriado
            if (!_stopCalled)
            {
                
            }
        }

        private void Connection_Closed()
        {
            ClearConnection();
        }

        private void Connection_Reconnected()
        {

        }

        private void Connection_Reconnecting()
        {

        }

        private void Connection_Slow()
        {
            
        }
    
        private void ClearConnection()
        {
            AtendimentoHubProxy = null;
            SupervisaoHubProxy = null;
            _idPaOrIpOrRamal = null;
            _authInfo = null;
            _timerRefreshToken?.Dispose();
            _timerRefreshToken = null;

            if (SignalrConnection != null)
            {
                SignalrConnection.Closed -= Connection_Closed;
                SignalrConnection.Error -= Connection_Error;
                SignalrConnection.ConnectionSlow -= Connection_Slow;
                SignalrConnection.Reconnecting -= Connection_Reconnecting;
                SignalrConnection.Reconnected -= Connection_Reconnected;

                try
                {
                    SignalrConnection.Stop();
                }
                catch
                {
                    // não nos importamos com erro de desconexões causadas ao parar a conexão
                }

                SignalrConnection?.Dispose();
                SignalrConnection = null;
            }
        }

        public void Dispose()
        {
            _stopCalled = true;
            _timerRefreshToken?.Dispose();
            ClearConnection();
        }

        private void AssertNotEmpty(string value)
        {
            AssertNotEmpty(value, "Argumento");
        }

        private void AssertNotNull(object value)
        {
            AssertNotNull(value, "Argumento");
        }

        private void AssertNotEmpty(string value, string paramName)
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(paramName, string.Format(Constantes.ErrorMsgArgumentoNull, paramName));
            }
        }

        private void AssertNotNull(object value, string paramName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName, string.Format(Constantes.ErrorMsgArgumentoNull, paramName));
            }
        }

        private Action<ArgType> CreateHandler<ArgType>(EventHandler<ArgType> anyEvent)
        {
            return arg => RaiseEvent(anyEvent, arg);
        }

        private Action<ArgType1, ArgType2> CreateHandler<ArgType1, ArgType2>(EventHandler<ArgType1, ArgType2> anyEvent)
        {
            return (arg1,arg2) => RaiseEvent(anyEvent, arg1, arg2);
        }

        private Action<ArgType1, ArgType2, ArgType3> CreateHandler<ArgType1, ArgType2, ArgType3>(EventHandler<ArgType1, ArgType2, ArgType3> anyEvent)
        {
            return (arg1, arg2, arg3) => RaiseEvent(anyEvent, arg1, arg2, arg3);
        }

        private Action CreateHandler(EventHandler anyEvent)
        {
            return () => RaiseEvent(anyEvent);
        }

        private void RaiseEvent(EventHandler anyEvent)
        {
            if (anyEvent != null)
            {
                if (_syncContext != null)
                {
                    _syncContext.Post(delegate
                    {
                        anyEvent(this, new EventArgs());
                    }, null);
                }
                else
                {
                    anyEvent(this, new EventArgs());
                }
            }
        }

        private void RaiseEvent<ArgType>(EventHandler<ArgType> anyEvent, ArgType eventArgument)
        {
            if (anyEvent != null)
            {
                // se temos o contexto, enviar mensagem para a Thread UI para executar o callback
                if (_syncContext != null)
                {
                    _syncContext.Post(delegate
                    {
                        anyEvent(this, eventArgument);
                    }, null);
                }
                else
                {
                    // se não temos thread UI, gerar evento imediatamente
                    anyEvent(this, eventArgument);
                }
            }
        }

        private void RaiseEvent<ArgType1, ArgType2>(EventHandler<ArgType1, ArgType2> anyEvent, ArgType1 eventArgument1, ArgType2 eventArgument2)
        {
            if (anyEvent != null)
            {
                // se temos o contexto, enviar mensagem para a Thread UI para executar o callback
                if (_syncContext != null)
                {
                    _syncContext.Post(delegate
                    {
                        anyEvent(this, eventArgument1, eventArgument2);
                    }, null);
                }
                else
                {
                    // se não temos thread UI, gerar evento imediatamente
                    anyEvent(this, eventArgument1, eventArgument2);
                }
            }
        }

        private void RaiseEvent<ArgType1, ArgType2, ArgType3>(EventHandler<ArgType1, ArgType2, ArgType3> anyEvent, ArgType1 eventArgument1, ArgType2 eventArgument2, ArgType3 eventArgument3)
        {
            if (anyEvent != null)
            {
                // se temos o contexto, enviar mensagem para a Thread UI para executar o callback
                if (_syncContext != null)
                {
                    _syncContext.Post(delegate
                    {
                        anyEvent(this, eventArgument1, eventArgument2, eventArgument3);
                    }, null);
                }
                else
                {
                    // se não temos thread UI, gerar evento imediatamente
                    anyEvent(this, eventArgument1, eventArgument2, eventArgument3);
                }
            }
        }
    }
}
