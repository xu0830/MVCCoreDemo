using AutoMapper;
using CJ.Entities;
using CJ.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Services.Roles
{
    public class RoleService : IRoleService
    {
        private IRepository<Role> _repository;

        private IMapper _mapper; 

        public RoleService(IRepository<Role> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public RoleDto GetRole(int id)
        {
            Role role = _repository.Get(id);

            RoleDto roleDto = _mapper.Map<RoleDto>(role);

            return roleDto;
        }
    }
}
