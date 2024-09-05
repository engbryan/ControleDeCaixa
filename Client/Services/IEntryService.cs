using EntryControl.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntryControl.Services
{
    public interface IEntryService
    {
        event Action<Entry> OnEntryAdded;
        Task Add(string type, decimal ammount, string description);
        Task<List<Entry>> GetAll();
        Task<List<Entry>> GetAllSinchronized();
        Task<List<Entry>> GetAllDirty();
    }
}