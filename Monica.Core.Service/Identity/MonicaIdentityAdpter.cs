using System;
using System.Collections.Generic;
using System.Text;
using Monica.Core.Abstraction.Identity;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelDto;
using Monica.Core.ModelParametrs.ModelsArgs;
using Monica.Core.Service.Identity.Models;
using Monica.Core.Service.WebAPI;

namespace Monica.Core.Service.Identity
{
    public class MonicaIdentityAdpter: IMonicaIdentityAdapter
    {
        public IEnumerable<UserDto> GetUsers()
        {
            try
            {
                var result = RestExtensions.SendPostAnonymous<IdentityUsersDto>("", "", null);
                return result.Users;
            }
            catch(Exception e)
            {
                return new List<UserDto>();
            }
        }
        public ResultCrmDb RemoveUser(string account)
        {
            var result = new ResultCrmDb();
            try
            {
                result = RestExtensions.SendPostAnonymous<ResultCrmDb>("", "", account);
            }
            catch (Exception e)
            {
                result.AddError("",e.Message);
            }
            return result;
        }
        //public ResultCrmDb ChangePassword(RegistrationUserArgs account)
        //{
        //    var result = new ResultCrmDb();
        //    try
        //    {
        //        result = RestExtensions.SendPostAnonymous<ResultCrmDb>("", "", account);
        //    }
        //    catch (Exception e)
        //    {
        //        result.AddError("", e.Message);
        //    }
        //    return result;
        //}
        public ResultCrmDb EditUser(RegistrationUserArgs args)
        {
            var result = new ResultCrmDb();
            try
            {
                result = RestExtensions.SendPostAnonymous<ResultCrmDb>("", "", args);
            }
            catch (Exception e)
            {
                result.AddError("", e.Message);
            }
            return result;
        }
    }
}
