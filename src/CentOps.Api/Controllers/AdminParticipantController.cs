﻿using AutoMapper;
using CentOps.Api.Configuration;
using CentOps.Api.Models;
using CentOps.Api.Services.ModelStore.Exceptions;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentOps.Api.Controllers
{
    [Route("admin/participants")]
    [ApiController]
    [Authorize(Policy = AuthConfig.AdminPolicy)]
    public class AdminParticipantController : ControllerBase
    {
        private readonly IParticipantStore _store;
        private readonly IMapper _mapper;

        public AdminParticipantController(IParticipantStore store, IMapper mapper)
        {
            _store = store;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AdminParticipantResponseModel>>> Get()
        {
            var participantTypeFilter = QueryStringUtils.GetParticipantTypeFilter(Request.Query);
            var participants = await _store.GetAll(participantTypeFilter, true).ConfigureAwait(false);
            return Ok(_mapper.Map<IEnumerable<AdminParticipantResponseModel>>(participants));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdminParticipantResponseModel>> Get(string id)
        {
            var participant = await _store.GetById(id).ConfigureAwait(false);

            return participant != null
                ? Ok(_mapper.Map<AdminParticipantResponseModel>(participant))
                : NotFound(id);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AdminParticipantResponseModel>> Post(CreateUpdateParticipantModel participant)
        {
            try
            {
                ParticipantDto participantDTO = _mapper.Map<ParticipantDto>(participant);

                var createdParticipant = await _store.Create(participantDTO).ConfigureAwait(false);
                return Created(
                    new Uri($"/admin/participants/{createdParticipant.Id}", UriKind.Relative),
                    _mapper.Map<AdminParticipantResponseModel>(createdParticipant));
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(argEx.Message);
            }
            catch (ModelNotFoundException<InstitutionDto> institutionEx)
            {
                return BadRequest(institutionEx.Message);
            }
            catch (ModelExistsException<ParticipantDto>)
            {
                return Conflict();
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdminParticipantResponseModel>> Put(string id, CreateUpdateParticipantModel participant)
        {
            try
            {
                ParticipantDto participantDTO = _mapper.Map<ParticipantDto>(participant);
                participantDTO.Id = id;
                var updatedParticipant = await _store.Update(participantDTO).ConfigureAwait(false);
                return Ok(_mapper.Map<AdminParticipantResponseModel>(updatedParticipant));
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(argEx);
            }
            catch (ModelNotFoundException<InstitutionDto> institutionEx)
            {
                return BadRequest(institutionEx.Message);
            }
            catch (ModelNotFoundException<ParticipantDto>)
            {
                return NotFound(id);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var deleted = await _store.DeleteById(id).ConfigureAwait(false);
                return deleted
                    ? NoContent()
                    : NotFound(id);
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(argEx.Message);
            }
        }
    }
}
