﻿using CSGProHackathonAPI.Infrastructure;
using CSGProHackathonAPI.Shared.Data;
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
    public class UsersController : BaseApiController<User>
    {
        private Repository _repository;

        public UsersController()
        {
            _repository = new Repository();
        }

        // GET api/users
        [BasicAuthorize]
        public IHttpActionResult Get()
        {
            try
            {
                var currentUser = GetCurrentUser();

                return Ok(currentUser);
            }
            catch (Exception exc)
            {
                return InternalServerError(exc);
            }
        }

        // POST api/users
        public IHttpActionResult Post([FromBody]UserViewModel viewModel)
        {
            try
            {
                ValidateViewModel(viewModel, null);

                if (ModelState.IsValid)
                {
                    var user = viewModel.GetModel(null);

                    _repository.SaveUser(user);

                    var uriString = Url.Link("DefaultApi", new { controller = "Users", id = user.UserId });

                    return Created(uriString, user);
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

        // PUT api/users/5
        [BasicAuthorize]
        public IHttpActionResult Put(int id, [FromBody]UserViewModel viewModel)
        {
            try
            {
                var user = _repository.GetUser(id);

                var currentUser = GetCurrentUser();
                if (user.UserId != currentUser.UserId)
                {
                    return Forbidden("You can only update your own user record.");
                }

                ValidateViewModel(viewModel, currentUser);

                if (ModelState.IsValid)
                {
                    viewModel.UpdateModel(user);

                    _repository.SaveUser(user);

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

        // DELETE api/users/5
        [BasicAuthorize]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var user = _repository.GetUser(id);

                var currentUser = GetCurrentUser();
                if (user.UserId != currentUser.UserId)
                {
                    return Forbidden("You can only delete your own user record.");
                }

                _repository.DeleteUser(user);

                return NoContent();
            }
            catch (Exception exc)
            {
                return InternalServerError(exc);
            }
        }
    }
}