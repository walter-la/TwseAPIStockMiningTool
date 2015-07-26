using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net
{
    /// <summary>
    /// 包裝成以IP地址作為主機名稱的網址。
    /// </summary>
    public class UrlByIPWrapper
    {
        private IPAddress[] _ipAddresses = null;

        private Uri _ipUri = null;

        private Uri _uri = null;

        private bool _isParseSucceed = false;

        /// <summary>
        /// 若IP無法使用時，是否自動解析DNS來更新IP。
        /// </summary>
        public bool IsAutoUpdate { get; set; }

        /// <summary>
        /// 使用IP地址作為主機名稱的網址，當使用 Update 失敗時，會使用原始 Domain Name 作為網址。
        /// </summary>
        public Uri Url
        {
            get
            {
                if (_isParseSucceed)
                    return _ipUri;

                if (IsAutoUpdate)
                    Update();

                return _uri;
            }
        }

        public UrlByIPWrapper(string url, IPAddress ipAddress = null)
        {
            _uri = new Uri(url);

            if (ipAddress != null)
            {
                CreateUrl(ipAddress);
            }
            else
            {
                Update();
            }
        }

        /// <summary>
        /// 回報 Url 屬性無法使用，並嘗試更新是否有新的 IP地址。
        /// </summary>
        public async void Update()
        {
            _isParseSucceed = false;
            try
            {
                _ipAddresses = await Dns.GetHostAddressesAsync(_uri.Host);
                if (_ipAddresses != null && _ipAddresses.Any())
                {
                    CreateUrl(_ipAddresses[0]);
                }
            }
            catch (System.Net.Sockets.SocketException socketEx) // HostNotFound
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(socketEx.SocketErrorCode);
#endif
            }
        }

        private void CreateUrl(IPAddress ipAddress)
        {
            _ipUri = new UriBuilder(_uri.Scheme,
                                    ipAddress.ToString(),
                                    _uri.Port,
                                    _uri.LocalPath,
                                    _uri.Query
                                    ).Uri;
            _isParseSucceed = true;
        }
    }
}
