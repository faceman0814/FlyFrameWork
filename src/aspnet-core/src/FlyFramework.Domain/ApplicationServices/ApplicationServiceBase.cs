﻿using AutoMapper;

using FlyFramework.Common.Attributes;
using FlyFramework.Domain.Localizations;
using FlyFramework.Repositories.Uow;

using System;
using System.Globalization;

namespace FlyFramework.Domain.ApplicationServices
{
    public abstract class ApplicationServiceBase
    {
        [IocSelect]
        public IServiceProvider ServiceProvider { get; set; } = default!;
        [IocSelect]
        public IMapper ObjectMapper { get; set; }

        private IUnitOfWorkManager _unitOfWorkManager;

        private ILocalizationSource _localizationSource;
        protected string LocalizationSourceName { get; set; }
        [IocSelect]
        public ILocalizationManager LocalizationManager { get; set; }
        protected ILocalizationSource LocalizationSource
        {
            get
            {
                if (LocalizationSourceName == null)
                {
                    throw new Exception("Must set LocalizationSourceName before, in order to get LocalizationSource");
                }

                if (_localizationSource == null || _localizationSource.Name != LocalizationSourceName)
                {
                    _localizationSource = LocalizationManager.GetSource(LocalizationSourceName);
                }

                return _localizationSource;
            }
        }
        [IocSelect]
        public IUnitOfWorkManager UnitOfWorkManager
        {
            get
            {
                if (_unitOfWorkManager == null)
                {
                    throw new Exception("Must set UnitOfWorkManager before use it.");
                }

                return _unitOfWorkManager;
            }
            set
            {
                _unitOfWorkManager = value;
            }
        }

        protected virtual string L(string name)
        {
            return LocalizationSource.GetString(name);
        }

        protected virtual string L(string name, params object[] args)
        {
            return LocalizationSource.GetString(name, args);
        }

        protected virtual string L(string name, CultureInfo culture)
        {
            return LocalizationSource.GetString(name, culture);
        }


        protected virtual string L(string name, CultureInfo culture, params object[] args)
        {
            return LocalizationSource.GetString(name, culture, args);
        }
    }
}
