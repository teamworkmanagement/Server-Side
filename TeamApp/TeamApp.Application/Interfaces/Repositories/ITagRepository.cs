using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Domain.Models.Tag;

namespace TeamApp.Application.Interfaces.Repositories
{
    public interface ITagRepository
    {
        Task<TagObject> GetById(string tagId);
        Task<string> AddTag(TagObject tagObj);
    }
}
