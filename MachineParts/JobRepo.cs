using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineParts
{
    public class JobRepo
    {
        private readonly ApplicationDbContext context_;

        public JobRepo(ApplicationDbContext context_)
        {
            this.context_ = context_;
        }

        public IEnumerable<DBJob> GetAll()
        {
            return context_.Jobs.ToList();
        }

        public DBJob? GetById(int id)
        {
            return context_.Jobs?.Find(id);
        }

        public void Add(DBJob DBJob)
        {
            context_.Jobs.Add(DBJob);
            context_.SaveChanges();
        }

        public void Update(DBJob DBJob)
        {
            context_.Jobs.Update(DBJob);
            context_.SaveChanges();
        }

        public void Delete(int id)
        {
            var DBJob = context_.Jobs.Find(id);
            if (DBJob != null)
            {
                context_.Jobs.Remove(DBJob);
                context_.SaveChanges();
            }
        }
    }
}
