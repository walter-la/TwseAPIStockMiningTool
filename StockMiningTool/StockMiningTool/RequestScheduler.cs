using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Threading.Tasks
{
    public class RequestScheduler
    {
        private bool _isBusy = false;

        private IRequestObject _requestObject = null;

        private CancellationTokenSource _cancellationTokenSource = null;

        public DateTime NextRequestTme { get; private set; }

        public int IntervalSeconds { get; set; }

        public bool IsBusy
        {
            get
            {
                return _isBusy || (_requestObject != null &&  _requestObject.IsBusy); 
            }
        }

        public RequestScheduler()
        {
            IntervalSeconds = 2;
        }

        public void Start(IRequestObject requestObject)
        {
            if (requestObject == null)
                return;

            if (IsBusy)
                return;

            _cancellationTokenSource = new CancellationTokenSource();
            _isBusy = true;

            this._requestObject = requestObject;

            var token = _cancellationTokenSource.Token;
            Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    var now = DateTime.Now;
                    if (this._requestObject.CanCreateRequst && now >= NextRequestTme)
                    {
                        NextRequestTme = now.AddSeconds(IntervalSeconds);
                        HandleRequestAsync();
                    }
                    System.Threading.Thread.Sleep(500);
                }

                _isBusy = false;
            }, _cancellationTokenSource.Token);

        }

        async void HandleRequestAsync()
        {
            await Task.Run(() =>
            {
                _requestObject.Handle();
            });
        }

        public void Stop()
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }
        }
    }
}
