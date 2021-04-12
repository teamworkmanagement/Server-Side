using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamApp.Application.DTOs.Tag;
using TeamApp.Application.Interfaces.Repositories;
using TeamApp.Infrastructure.Persistence.Entities;
using TeamApp.Application.Utils;

namespace TeamApp.Infrastructure.Persistence.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly TeamAppContext _dbContext;

        public TagRepository(TeamAppContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<string> AddTag(TagObject tagObj)
        {
            var entity = new Tag
            {
                TagId = Guid.NewGuid().ToString(),
                TagContent = tagObj.TagContent,
                TagLink = tagObj.TagLink,
            };

            await _dbContext.Tag.AddAsync(entity);

            return entity.TagId;

        }

        public async Task<TagObject> GetById(string tagId)
        {
            var entity = await _dbContext.Tag.FindAsync(tagId);
            if (entity == null)
                return null;

            var outPut = new TagObject
            {
                TagId = entity.TagId,
                TagContent = entity.TagContent,
                TagLink = entity.TagLink,
            };

            return outPut;
        }
    }
}
