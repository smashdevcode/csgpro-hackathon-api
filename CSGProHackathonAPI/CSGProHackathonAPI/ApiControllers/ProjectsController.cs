﻿using CSGProHackathonAPI.Infrastructure;
using CSGProHackathonAPI.Shared.Data;
using CSGProHackathonAPI.Shared.Infrastructure;
using CSGProHackathonAPI.Shared.Models;
using CSGProHackathonAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CSGProHackathonAPI.ApiControllers
{
    [BasicAuthorize]
    public class ProjectsController : BaseApiController<Project>
    {
        private Repository _repository;

        public ProjectsController()
        {
            _repository = new Repository();
        }

        // GET api/projects
        public IEnumerable<Project> Get()
        {
            var user = GetCurrentUser();

            return _repository.GetProjects(user.UserId);
        }

        // GET api/projects/5
        public IHttpActionResult Get(int id)
        {
            try
            {
                var project = _repository.GetProject(id);
                if (project == null)
                {
                    return NotFound();
                }

                var user = GetCurrentUser();
                if (project.UserId != user.UserId)
                {
                    return Forbidden("The current user does not have access to the requested resource.");
                }

                return Ok(project);
            }
            catch (Exception exc)
            {
                return InternalServerError(exc);
            }
        }

        // POST api/projects
        public IHttpActionResult Post([FromBody]ProjectViewModel viewModel)
        {
            try
            {
                var user = GetCurrentUser();

                ValidateViewModel(viewModel, user);

                if (ModelState.IsValid)
                {
                    var project = viewModel.GetModel(user);

                    _repository.SaveProject(project);

                    var uriString = Url.Link("DefaultApi", new { controller = "Projects", id = project.ProjectId });

                    return Created(uriString, project);
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception exc)
            {
                return InternalServerError(exc);
            }
        }

        // PUT api/projects/5
        public IHttpActionResult Put(int id, [FromBody]ProjectViewModel viewModel)
        {
            try
            {
                var project = _repository.GetProject(id);

                var user = GetCurrentUser();
                if (project.UserId != user.UserId)
                {
                    return Forbidden("You can only update projects for the current user.");
                }

                ValidateViewModel(viewModel, user);

                if (ModelState.IsValid)
                {
                    viewModel.UpdateModel(project);

                    _repository.SaveProject(project);

                    return NoContent();
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception exc)
            {
                return InternalServerError(exc);
            }
        }

        // DELETE api/projects/5
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var project = _repository.GetProject(id);

                var user = GetCurrentUser();
                if (project.UserId != user.UserId)
                {
                    return Forbidden("You can only delete projects for the current user.");
                }

                _repository.DeleteProject(project);

                return NoContent();
            }
            catch (Exception exc)
            {
                return InternalServerError(exc);
            }
        }
    }
}