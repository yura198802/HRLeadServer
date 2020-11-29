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
    public class BtnsAdapter : IBtnsAdapter
    {
        private ReportDbContext _crmDbContext;
        public BtnsAdapter(ReportDbContext crmDbContext)
        {
            _crmDbContext = crmDbContext;
        }
        public async Task<IEnumerable<ItemAccess>> GetButtonsTreeAsync(int idRole)
        {
            List<ItemAccess> items = new List<ItemAccess>();
            List<int> itemsForm = new List<int>();
            List<int> itemsTypes = new List<int>();
            var count = _crmDbContext.AccessForm.Where(x => x.UserRoleId == idRole).Count();
            var list = await _crmDbContext.AccessForm.ToListAsync();
            foreach (var item in _crmDbContext.ButtonForm)
            {
                var buttonAccess = count > 0 ? list.Where(x => x.UserRoleId == idRole).FirstOrDefault(x => x.ButtonFormId == item.Id) : null;
                itemsForm.Add((int)item.FormId);
                items.Add(new ItemAccess()
                {
                    Id = item.Id,
                    IsBtn = true,
                    BtnId = item.Id,
                    FormId = (int)item.FormId,
                    ParentId = 0,
                    typeAccess = buttonAccess == null ? TypeAccec.Full : buttonAccess.TypeAccec,
                    Text = string.IsNullOrWhiteSpace(item.DisplayName) ? item.ToolTip : item.DisplayName
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
                var item = new ItemAccess()
                {
                    Id = ++i,
                    IsForm = true,
                    FormId = f.Id,
                    ParentId = parent == 0 ? 0 : items.FirstOrDefault(x => x.TypeId == parent).Id,
                    Text = f.Caption,
                    typeAccess = TypeAccec.Full
                };
                items.Add(item);
            }
            foreach (var item in items)
            {
                if (item.ParentId == 0 & item.IsBtn == true)
                {
                    var parent = items.FirstOrDefault(x => x.FormId == item.FormId & x.IsBtn == false).Id;
                    item.ParentId = parent;
                }
                continue;
            }
            return items;
        }
        public async Task<ResultCrmDb> EditAccessAsync(int idRole, ItemAccess[] items)
        {
            var result = new ResultCrmDb();
            try
            {
                foreach (var btn in items.Where(x => x.IsBtn == true))
                {
                    var updated = await _crmDbContext.AccessForm.FirstOrDefaultAsync(x => x.UserRoleId == idRole & x.FormModelId == btn.FormId & x.ButtonFormId == btn.BtnId);
                    if (updated != null)
                    {
                        updated.TypeAccec = btn.typeAccess;
                        _crmDbContext.AccessForm.Update(updated);
                    }
                    else
                    {
                        var access = new AccessForm()
                        {
                            ButtonFormId = btn.BtnId,
                            UserRoleId = idRole,
                            FormModelId = btn.FormId,
                            TypeAccec = btn.typeAccess
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
