using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelCrm.EngineReport;
using Monica.Settings.DataAdapter.Interfaces;
using Monica.Settings.DataAdapter.Models;
using Monica.Settings.DataAdapter.Models.Crm;
using Monica.Settings.DataAdapter.Models.Crm.Core;

namespace Monica.Settings.DataAdapter.DataAdapter.Resources
{
    public class ModesAdapter : IModesAdapter
    {
        private SettingsDbContext _crmDbContext;
        public ModesAdapter(SettingsDbContext crmDbContext)
        {
            _crmDbContext = crmDbContext;
        }
        public async Task<IEnumerable<ItemAccess>> GetModesTreeAsync(int idRole)
        {
            List<ItemAccess> items = new List<ItemAccess>();
            var i = 0;
            var count = _crmDbContext.accessForm.Where(x => x.UserRoleId == idRole).Count();
            var list = await _crmDbContext.accessForm.ToListAsync();
            foreach (var t in _crmDbContext.typeForm)
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
            foreach (var f in _crmDbContext.formModel)
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
                var ac = _crmDbContext.accessForm.ToList();
                
                foreach (var forms in items.Where(x => x.IsForm == true))
                {

                    var updated = await _crmDbContext.accessForm.Where(x => x.UserRoleId == idRole).Where(x=>x.ButtonFormId == null).Where(x=> x.FieldId == null).FirstOrDefaultAsync(x=>x.FormModelId == forms.FormId);
                    var updated1 = _crmDbContext.accessForm.Where(x => x.UserRoleId == idRole & (x.FormModelId == forms.FormId) & (x.ButtonFormId == null) & (x.FieldId == null)).FirstOrDefault();
                    if (updated != null)
                    {
                        
                        updated.TypeAccec = forms.typeAccess;
                        _crmDbContext.accessForm.Update(updated);
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
                        await _crmDbContext.accessForm.AddAsync(access);
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
