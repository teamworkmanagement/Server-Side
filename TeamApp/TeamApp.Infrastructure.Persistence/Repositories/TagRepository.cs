using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Domain.Models.Tag;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class TagRepository : ITagRepository
    {
        public Task<string> AddTag(TagObject tagObj)
        {
            throw new NotImplementedException();
        }

        public Task<TagObject> GetById(string tagId)
        {
            throw new NotImplementedException();
        }
    }
}
