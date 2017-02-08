using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HueLib2
{
    public class CommandResult
    {
        private bool _success;
        private object _resultobject;
        private Exception _ex;
        private int _errorcode;

        public CommandResult()
        {
            Success = false;
            Errorcode = -1;
        }

        public bool Success
        {
            get { return _success; }
            set { _success = value;}
        }

        public object Resultobject
        {
            get { return _resultobject; }
            set { _resultobject = value;}
        }

        public Exception Ex
        {
            get { return _ex; }
            set { _ex = value;}
        }

        public int Errorcode
        {
            get { return _errorcode; }
            set { _errorcode = value;}
        }
    }
}
