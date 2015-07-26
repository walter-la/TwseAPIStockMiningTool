using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Threading.Tasks
{
    public interface IRequestObject
    {
        bool IsBusy { get; }

        bool CanCreateRequst { get; }

        void Handle();
    }
}
