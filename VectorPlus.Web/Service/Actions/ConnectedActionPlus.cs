using System;
using System.Threading.Tasks;
using VectorPlus.Lib;
using VectorPlus.Lib.Actions;

namespace VectorPlus.Web.Service.Actions
{
    public class ConnectedActionPlus : SimpleSpeechAction
    {
        public ConnectedActionPlus() : base(null, "Vector Plus is connected.")
        {
        }
    }
}
