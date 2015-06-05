using JCI.CAM.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace JCI.CAM.SiteProvisioningAppWeb
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LogHelper.LogInformation("Logging test!!", LogEventID.ServiceHelper);
        }
    }
}