using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Monica.Core.Abstraction.Crm.Settings;
using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelCrm.EngineReport;
using Monica.Core.DbModel.ModelCrm.Settings;

namespace Monica.Core.Service.Crm.Settings.Resources
{
    public class ModesAdapter : IModesAdapter
    {
        private ReportDbContext _crmDbContext;
        public ModesAdapter(ReportDbContext crmDbContext)
        {
            _crmDbContext = crmDbContext;
        }
        public async Task<IEnumerable<ItemAccess>> GetModesTreeAsync(int idRole)
        {
            List<ItemAccess> items = new List<ItemAccess>();
            var i = 0;
            var count = _crmDbContext.AccessForm.Where(x => x.UserRoleId == idRole).Count();
            var list = await _crmDbContext.AccessForm.ToListAsync();
            foreach (var t in _crmDbContext.TypeForm)
            {
                var item = new ItemAccess()
                {
                    Id = ++i,
                    IsType = true,
                    TypeId = t.Id,
                    ParentId = 0,
                    Text = t.DisplayName,
                    typeAccess = TypeAccec.Full
                };
                items.Add(item);
            }
            foreach (var f in _crmDbContext.FormModel)
            {
                var formAccess = count > 0 ? list.Where(x => x.UserRoleId == idRole).FirstOrDefault(x => x.FormModelId == f.Id & x.ButtonFormId == null & x.FieldId == null) : null;
                var parent = (int)f.TypeFormId == 0 ? 0 : (int)f.TypeFormId;
                var item = new ItemAccess()
                {
                    Id = ++i,
                    IsForm = true,
                    FormId = f.Id,
                    ParentId = parent == 0 ? 0 : items.FirstOrDefault(x => x.TypeId == parent).Id,
                    Text = f.Caption,
                    typeAccess = formAccess == null ? TypeAccec.Full : formAccess.TypeAccec,
                };
                items.Add(item);
            }
            return items;
        }
        public async Task<ResultCrmDb> EditAccessAsync(int idRole, ItemAccess[] items)
        {
            var result = new ResultCrmDb();
            try
            {
                var it = items.Where(x => x.IsForm == true);
                var ac = _crmDbContext.AccessForm.ToList();
                
                foreach (var forms in items.Where(x => x.IsForm == true))
                {

                    var updated = await _crmDbContext.AccessForm.Where(x => x.UserRoleId == idRole).Where(x=>x.ButtonFormId == null).Where(x=> x.FieldId == null).FirstOrDefaultAsync(x=>x.FormModelId == forms.FormId);
                    var updated1 = _crmDbContext.AccessForm.Where(x => x.UserRoleId == idRole & (x.FormModelId == forms.FormId) & (x.ButtonFormId == null) & (x.FieldId == null)).FirstOrDefault();
                    if (updated != null)
                    {
                        
                        updated.TypeAccec = forms.typeAccess;
                        _crmDbContext.AccessForm.Update(updated);
                        await _crmDbContext.SaveChangesAsync();
                    }
                    else
                    {
                        var access = new AccessForm()
                        {
                            UserRoleId = idRole,
                            FormModelId = forms.FormId,
                            TypeAccec = forms.typeAccess
                        };
                        await _crmDbContext.AccessForm.AddAsync(access);
                        await _crmDbContext.SaveChangesAsync();
                    }
                }
                
            }
            catch (Exception e)
            {
                result.AddError("", e.Message);
            }
            return result;
        }
    }
}
