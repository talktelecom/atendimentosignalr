using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace EpbxManagerClient.Atendimento
{
    /// <summary>
    /// Classe que auxilia nas requisições a WebApi
    /// </summary>
    public class WebApiClient
    {
        private readonly Uri _serviceUri;
        private readonly HttpMessageHandler _handler;
        private readonly string _oauthClientId;

        public WebApiClient(string serviceUrl) : this(serviceUrl, Constantes.AuthorizationDefaultClientId, null) { }

        public WebApiClient(string serviceUrl, string oauthClientId) : this(serviceUrl, oauthClientId, null) { }

        public WebApiClient(string serviceUrl, HttpMessageHandler handler) : this(serviceUrl, Constantes.AuthorizationDefaultClientId, handler) { }

        public WebApiClient(string serviceUrl, string oauthClientId, HttpMessageHandler handler)
        {
            _serviceUri = new Uri(serviceUrl);
            _handler = handler;
            _oauthClientId = oauthClientId;
        }

        protected virtual HttpClient CreateHttpClient()
        {
            var httpClient = _handler == null ? new HttpClient() : new HttpClient(_handler);

            httpClient.BaseAddress = _serviceUri;

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constantes.AcceptJsonHeaderValue));

            return httpClient;
        }

        /// <summary>
        /// Cria o HttpClient, injeta as credenciais no Header e executa a ação do parametro httpClientAction.
        /// O retorno é convertido do JSON no objeto esperado
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpClientAction"></param>
        /// <param name="authInfo"></param>
        /// <returns></returns>
        public virtual async Task<T> Request<T>(Func<HttpClient, Task<HttpResponseMessage>> httpClientAction, OAuthInfo authInfo)
        {
            using (var http = CreateHttpClient())
            {
                if(!string.IsNullOrWhiteSpace(authInfo?.AccessToken))
                {
                    http.DefaultRequestHeaders.Add(Constantes.AuthorizationHeaderName, $"{Constantes.AuthorizationHeaderType} {authInfo.AccessToken}");
                }

                var response = await httpClientAction(http);
                response.EnsureSuccessStatusCode();

                return JToken.Parse(await response.Content.ReadAsStringAsync()).Value<T>();
            }
        }

        public virtual Task<OAuthInfo> Autenticar(string usuario, string senha)
        {
            var body = new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", usuario),
                    new KeyValuePair<string, string>("password", senha),
                    new KeyValuePair<string, string>("client_id", _oauthClientId)
                };

            return SendOAuthTokenRequest(body);
        }

        /// <summary>
        /// Usa o Refresh Token para buscar um novo Access Token
        /// </summary>
        /// <param name="authInfo"></param>
        /// <returns></returns>
        public virtual Task<OAuthInfo> AtualizarAccessToken(OAuthInfo authInfo)
        {
            if (string.IsNullOrWhiteSpace(authInfo?.RefreshToken))
            {
                throw new InvalidOperationException(Constantes.ErrorMsgUsuarioExpirou);
            }

            var body = new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("refresh_token", authInfo.RefreshToken),
                    new KeyValuePair<string, string>("client_id", _oauthClientId)
            };

            return SendOAuthTokenRequest(body);
        }

        protected virtual async Task<OAuthInfo> SendOAuthTokenRequest(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
        {
            using (var http = CreateHttpClient())
            {
                var response = await http.PostAsync(Constantes.UrlOAuth, new FormUrlEncodedContent(nameValueCollection));

                // erro 401 indica preenchimento incorreto do usuario, senha ou clientId
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new Exception(Constantes.ErrorMsgUsuarioSenhaInvalido);
                }

                // outros códigos de erros indicam problema no servidor ou na rede
                response.EnsureSuccessStatusCode();

                return await ResponseToOAuthInfo(response);
            }
        }

        private async Task<OAuthInfo> ResponseToOAuthInfo(HttpResponseMessage response)
        {
            var json = JObject.Parse(await response.Content.ReadAsStringAsync());

            return new OAuthInfo
            {
                AccessToken = json.Value<string>("access_token"),
                RefreshToken = json.Value<string>("refresh_token"),
                TokenExpiration = json.Value<long>("expires_in")
            };
        }
    }
}
