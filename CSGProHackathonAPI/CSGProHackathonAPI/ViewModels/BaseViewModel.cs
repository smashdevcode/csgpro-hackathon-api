﻿using CSGProHackathonAPI.Shared.Infrastructure;
using CSGProHackathonAPI.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSGProHackathonAPI.ViewModels
{
    public abstract class BaseViewModel<TModelType> where TModelType : BaseModel
    {
        /// <summary>
        /// Method that is called by the base controller class to validate the view model.
        /// </summary>
        /// <param name="currentUser">The current user.</param>
        /// <returns>A collection of ValidationMessage objects.</returns>
        public List<ValidationMessage> GetValidationMessages(User currentUser)
        {
            // NOTE The Validate() method returns an IEnumerable collection
            // so we need to make sure that the collection is not null and then
            // call ToList() on it to enumerate it in order to prevent enumerating
            // it twice by calling Count() and then ToList() if the count is greater than 0.
            var validationMessagesList = new List<ValidationMessage>();
            var validationMessages = Validate(currentUser);
            if (validationMessages != null)
            {
                validationMessagesList = validationMessages.ToList();
            }
            return validationMessagesList;
        }

        /// <summary>
        /// Method that is called by the base editable view model class to validate the view model.
        /// </summary>
        /// <param name="currentUser">The current user.</param>
        protected IEnumerable<ValidationMessage> Validate(User currentUser)
        {
            return null;
        }

        /// <summary>
        /// Gets an instance of the model from this view model.
        /// </summary>
        /// <returns>A model object.</returns>
        public abstract TModelType GetModel(User currentUser);

        /// <summary>
        /// Updates a model from this view model.
        /// </summary>
        /// <param name="model">The model object to update.</param>
        public abstract void UpdateModel(TModelType model);
    }
}