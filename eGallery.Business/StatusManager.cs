﻿using System;
using System.Collections.Generic;
using System.Text;
using eGallery.Contracts.Repositories;
using eGallery.Contracts.Services;
using eGallery.Contracts.Models;
using System.Threading.Tasks;

namespace eGallery.Business
{
    public class StatusManager: IStatusService
    {
        private readonly IStatusRepository _statusRepository;

        public StatusManager(IStatusRepository statusRepository)
        {
            _statusRepository = statusRepository;

        }

        public async Task<StatusModel[]> StatusList()
        {
            return await _statusRepository.StatusList();

        }
    }
}