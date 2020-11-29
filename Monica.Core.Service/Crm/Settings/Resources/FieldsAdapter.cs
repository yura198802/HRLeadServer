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
    public class FieldsAdapter : IFieldsAdapter
    {
        private ReportDbContext _crmDbContext;
        public FieldsAdapter(ReportDbContext crmDbContext)
        {
            _crmDbContext = crmDbContext;
        }
        public async Task<IEnumerable<ItemAccess>> GetFieldsTreeAsync(int idRole)
        {
            List<ItemAccess> items = new List<ItemAccess>();
            List<int> itemsForm = new List<int>();
            List<int> itemsTypes = new List<int>();
            var count =_crmDbContext.AccessForm.Where(x => x.UserRoleId == idRole).Count();
            var list = await _crmDbContext.AccessForm.ToListAsync();
            foreach (var item in _crmDbContext.Field)
            {

                var fieldAccess = count > 0 ? list.Where(x => x.UserRoleId == idRole).FirstOrDefault(x => x.FieldId == item.Id) : null;
                //var fieldAccess = count > 0 ? await _crmDbContext.accessForm.Where(x => x.UserRoleId == idRole).FirstOrDefaultAsync(x => x.FieldId == item.Id) : null;
                itemsForm.Add(item.FormModelId);
                items.Add(new ItemAccess()
                {
                    Id = item.Id,
                    IsField = true,
                    FieldId = item.Id,
                    FormId = item.FormModelId,
                    ParentId = item.ParentId == null ? 0 : (int)item.ParentId,
                    typeAccess = fieldAccess == null ? TypeAccec.Full : fieldAccess.TypeAccec,
                    Text = string.IsNullOrWhiteSpace(item.DisplayName) ? item.Name : item.DisplayName
                });
            }
            var i = items.Max(x => x.Id);
            foreach (var f in _crmDbContext.FormModel)
            {
                if (itemsForm.Contains(f.Id))
                {
                    var item = (int)f.TypeFormId == 0 ? 0 : (int)f.TypeFormId;
                    itemsTypes.Add(item);
                }
            }
            foreach (var t in _crmDbContext.TypeForm)
            {
                if (!itemsTypes.Contains(t.Id))
                    continue;
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
                if (!itemsForm.Contains(f.Id))
                    continue;
                var parent = (int)f.TypeFormId == 0 ? 0 : (int)f.TypeFormId;
                var item = new ItemAccess() {
                    Id = ++i,
                    IsForm = true,
                    FormId = f.Id,
                    ParentId = parent == 0?0:items.FirstOrDefault(x=>x.TypeId==parent).Id ,
                    Text = f.Caption,
                    typeAccess = TypeAccec.Full
                };
                items.Add(item);
            }
            foreach (var item in items)
            {
                if (item.ParentId == 0 & item.IsField == true)
                {
                    var parent = items.FirstOrDefault(x => x.FormId == item.FormId & x.IsField == false).Id;
                    item.ParentId = parent;
                }
                 continue;
            }
            return items;
        }
        public async Task<ResultCrmDb> EditAccessAsync(int idRole, ItemAccess[] fieldsArgs)
        {
            var result = new ResultCrmDb();
            try
            {
                foreach(var field in fieldsArgs.Where(x=>x.IsField == true))
                {
                    var updated = await _crmDbContext.AccessForm.FirstOrDefaultAsync(x => x.UserRoleId == idRole & x.FormModelId == field.FormId & x.FieldId == field.FieldId);
                    if(updated != null)
                    {
                        updated.TypeAccec = field.typeAccess;
                        _crmDbContext.AccessForm.Update(updated);
                    }
                    else
                    {
                        var access = new AccessForm()
                        {
                            FieldId = field.FieldId,
                            UserRoleId = idRole,
                            FormModelId = field.FormId,
                            TypeAccec = field.typeAccess
                        };
                        await _crmDbContext.AccessForm.AddAsync(access);
                    }
                } 
                await _crmDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.AddError("", e.Message);
            }
            return result;
        }
    }
}
