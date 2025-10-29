using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IndicadorChileAPI.Controllers
{
    public class BaseController : ControllerBase
    {
        #region Readonly
        private readonly string url;
        #endregion

        #region Interfaces
        private readonly ILogger<BaseController> Logger;
        #endregion



        #region Constructor Method
        public BaseController(ILogger<BaseController> Logger, string url)
            : base()
        {
            this.Logger = Logger;
            this.url = url;
        }
        #endregion



        #region Property
        public string URL
        {
            get => url;
        }
        #endregion
    }
}