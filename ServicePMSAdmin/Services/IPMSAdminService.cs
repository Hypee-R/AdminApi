using CorePMSAdmin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServicePMSAdmin.Services
{
    public interface IPMSAdminService
    {

        #region Auth
        Task<object> auth(Usuarios usuario);

        Task logOff();
        #endregion

    }
}
