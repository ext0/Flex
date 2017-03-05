using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Execution.Data.States
{
    public class ASynchronousCancellationToken
    {
        private bool _cancel;

        private Object _lock;

        private bool Canceled
        {
            get
            {
                return _cancel;
            }
            set
            {
                lock (_lock)
                {
                    _cancel = value;
                }
            }
        }

        public ASynchronousCancellationToken()
        {
            Canceled = false;
        }

        public ASynchronousCancellationToken(bool canceled)
        {
            Canceled = canceled;
        }

        public void Cancel()
        {
            if (!Canceled)
            {
                throw new InvalidOperationException("Cannot cancel already cancelled token!");
            }
            Canceled = true;
        }

        public bool IsCancelled()
        {
            return Canceled;
        }
    }
}
