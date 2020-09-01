using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BIMonTime.Data.Entities;
using BIMonTime.Services.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BIMonTime.Web.Controllers
{
    [ApiController]
    public abstract class GenericController<TEntity, TModel, TRepository> : ControllerBase
        where TEntity : class, IEntity
        where TModel : class
        where TRepository : IRepository<TEntity>
    {
        private readonly TRepository repository;
        private readonly IMapper mapper;

        public GenericController(TRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TModel>>> Get()
        {
            try
            {
                var results = await repository.GetAll();

                TModel[] models = mapper.Map<TModel[]>(results);

                return models;
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<TModel>> Post(TEntity entity)
        {
            await repository.Add(entity);
            return CreatedAtAction("Get", new { id = entity.Id }, entity);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TModel>> Get(int id)
        {
            var entity = await repository.Get(id);
            if (entity == null) return NotFound();

            TModel model = mapper.Map<TModel>(entity);

            return model;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, TEntity model)
        {
            if (id != model.Id) return BadRequest();

            TEntity updatedEntity = await repository.Update(model);
            return Ok(updatedEntity);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<TEntity>> Delete(int id)
        {
            var entity = await repository.Delete(id);
            if (entity == null) return NotFound();

            return entity;
        }
    }
}
