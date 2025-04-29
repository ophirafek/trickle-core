using Microsoft.Extensions.Localization;
using System.Web.Mvc;

namespace Base
{
    public abstract class LocalizedControllerBase : ControllerBase
    {
        protected readonly IStringLocalizer<SharedResource> _localizer;

        public LocalizedControllerBase(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }

    }
}
