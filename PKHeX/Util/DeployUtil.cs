using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PKHeX
{
    public partial class Util
    {
        public static bool IsClickonceDeployed
        {
            get
            {
#if CLICKONCE
                return System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed;
#else
                return false;
#endif
            }            
        }
    }
}
