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
        Task<object> getUser();
        Task logOff();
        #endregion

        #region Users
        Task<object> getUsers();

        Task<object> postUser(Usuarios usuario);

        Task<object> putUser(Usuarios usuario);

        Task<bool> deleteUser(int idUsuario);
        #endregion

    }
}
