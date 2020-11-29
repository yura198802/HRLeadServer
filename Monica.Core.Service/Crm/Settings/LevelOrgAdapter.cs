﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Monica.Core.Abstraction.Crm.Settings;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelCrm.Profile;
using Monica.Core.DbModel.ModelDto.LevelOrg;

namespace Monica.Core.Service.Crm.Settings
{
    /// <summary>
    /// Адаптер для levelorg
    /// </summary>
    public class LevelOrgAdapter : ILevelOrgAdapter
    {
        private ReportDbContext _crmDbContext;
        public LevelOrgAdapter(ReportDbContext crmDbContext)
        {
            _crmDbContext = crmDbContext;
        }
        public async Task<ResultCrmDb> AddAsync(LevelOrgAddArgs args)
        {
            var result = new ResultCrmDb();
            var toAdd = new t_levelorg();
            try
            {
                var org = await _crmDbContext.t_levelorg.FirstOrDefaultAsync(r => r.Inn == args.Inn & r.Kpp == args.Kpp & r.Oktmo == args.Oktmo);
                if (org != null)
                {
                    result.AddError("", "Организация с такими ИНН, КПП, ОКТМО уже существует!");
                    return result;
                }
                var l = _crmDbContext.t_levelorg.Count();
                toAdd = args;
                if (toAdd.Parent == null || toAdd.Parent == 0)
                    if (await _crmDbContext.t_levelorg.FirstOrDefaultAsync(f => f.Parent == null || f.Parent == 0) != null)
                        throw new Exception("Не возможно иметь 2 основных уровня levelorg");
                    else
                    {
                        if (_crmDbContext.t_levelorg.Count() != 0)
                            if (await _crmDbContext.t_levelorg.FirstOrDefaultAsync(f => toAdd.Parent.ToString() == f.Sysid.ToString()) == null)
                                throw new Exception("Не существует родительской записи для levelorg");
                    }
                await _crmDbContext.t_levelorg.AddAsync(toAdd);
                await _crmDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.AddError("", e.Message);
            }
            return result;
        }
        public async Task<ResultCrmDb> RemoveAsync(int idOrg)
        {
            var result = new ResultCrmDb();
            try
            {

                var removeable = await _crmDbContext.t_levelorg.FirstOrDefaultAsync(r => r.Sysid == idOrg);
                if (removeable != null)
                {
                    if (removeable.Parent == null || removeable.Parent == 0)
                    {
                        var child = await _crmDbContext.t_levelorg.FirstOrDefaultAsync(c => c.Parent == removeable.Sysid);
                        if (child != null)
                            throw new Exception($"Сначала удалите организации, которые ссылаются на {removeable.Caption}.");
                    }
                    var roles = await _crmDbContext.UserRole.Where(r => r.LevelOrgId == idOrg).ToListAsync();
                    roles.ForEach(r => _crmDbContext.UserLinkRole.RemoveRange(_crmDbContext.UserLinkRole.Where(l => l.UserRoleId == r.Id)));
                    _crmDbContext.UserRole.RemoveRange(roles);
                    _crmDbContext.t_levelorg.Remove(removeable);
                    await _crmDbContext.SaveChangesAsync();
                }
                else
                    throw new Exception($"{removeable.Caption} не найдена");
            }
            catch (Exception e)
            {
                result.AddError("", e.Message);
            }
            return result;
        }
        public async Task<IEnumerable<LevelOrgDto>> GetAll()
        {
            var result = new List<LevelOrgDto>();
            var levelorg = _crmDbContext.t_levelorg;
            foreach (var crm in levelorg)
            {
                result.Add(crm);
            }
            return result;//???
        }
        public async Task<ResultCrmDb> EditLevelOrgAsync(LevelOrgDto levelOrg)
        {
            var result = new ResultCrmDb();
            try
            {
                var org = await _crmDbContext.t_levelorg.FirstOrDefaultAsync(r => r.Inn == levelOrg.Inn & r.Kpp == levelOrg.Kpp & r.Oktmo == levelOrg.Oktmo);
                if (org != null)
                {
                    result.AddError("", "Организация с такими ИНН, КПП, ОКТМО уже существует!");
                    return result;
                }
                var levelorg = new t_levelorg();
                levelorg = levelOrg;
                _crmDbContext.t_levelorg.Update(levelorg);
                await _crmDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.AddError($"{e.Message}", "");
            }
            return result;
        }

        //public async Task<IEnumerable<UserDto>> GetUsersByUserRole(int sysIdRole)
        //{
        //    var result = new List<UserDto>();
        //    var link = _crmDbContext.userlinkrole.Where(us => us.UserRoleId == sysIdRole);
        //    foreach (var l in link)
        //    {
        //        result.Add(l.User?.Map(new UserDto()));
        //    }
        //    return result;
        //}
        //public async Task<IEnumerable<UserDto>> GetUsersByLevelOrgId(int sysIdLevelorg)
        //{
        //    var result = new List<UserDto>();
        //    var link = _crmDbContext.User.Where(us => us.LevelOrgId == sysIdLevelorg);
        //    foreach (var l in link)
        //    {
        //        result.Add(l?.Map(new UserDto()));
        //    }
        //    return result;
        //}
    }

}

