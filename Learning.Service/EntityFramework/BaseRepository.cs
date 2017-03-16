using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voxteneo.Core.Domains.Contracts;

namespace Learning.Service.EntityFramework
{
    public class BaseRepository
    {
        protected IUnitOfWork UnitOfWork { get; set; }
        public BaseRepository(IUnitOfWork unitOfWork) 
        {
            UnitOfWork = unitOfWork;
            Context = unitOfWork.GetContext() as LearningContext;
        }
        protected LearningContext Context { get; set; }
    }
}
