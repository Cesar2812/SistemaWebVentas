using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayerBusiness.Interface;
using LayerDataBase.Interface;
using LayerEntity;

namespace LayerBusiness.Implementation
{
    public class RoleService : IRoleService
    {
        private readonly IGenericRepository<Rol> _repository;

        public RoleService(IGenericRepository<Rol> repository)
        {
            _repository = repository;
        }
        public async Task<List<Rol>> List()
        {
            IQueryable<Rol> query = await _repository.Consult();
            return query.ToList();
        }
    }
}
