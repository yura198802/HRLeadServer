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
    public class BtnsAdapter : IBtnsAdapter
    {
        private SettingsDbContext _crmDbContext;
        public BtnsAdapter(SettingsDbContext crmDbContext)
        {
            _crmDbContext = crmDbContext;
        }
        public async Task<IEnumerable<ItemAccess>> GetButtonsTreeAsync(int idRole)
        {
            List<ItemAccess> items = new List<ItemAccess>();
            List<int> itemsForm = new List<int>();
            List<int> itemsTypes = new List<int>();
            var count = _crmDbContext.accessForm.Where(x => x.UserRoleId == idRole).Count();
            var list = await _crmDbContext.accessForm.ToListAsync();
            foreach (var item in _crmDbContext.buttonForm)
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
            foreach (var f in _crmDbContext.formModel)
            {
                if (itemsForm.Contains(f.Id))
                {
                    var item = (int)f.TypeFormId == 0 ? 0 : (int)f.TypeFormId;
                    itemsTypes.Add(item);
                }
            }
            foreach (var t in _crmDbContext.typeForm)
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
            foreach (var f in _crmDbContext.formModel)
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
                    var updated = await _crmDbContext.accessForm.FirstOrDefaultAsync(x => x.UserRoleId == idRole & x.FormModelId == btn.FormId & x.ButtonFormId == btn.BtnId);
                    if (updated != null)
                    {
                        updated.TypeAccec = btn.typeAccess;
                        _crmDbContext.accessForm.Update(updated);
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
                        await _crmDbContext.accessForm.AddAsync(access);
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
