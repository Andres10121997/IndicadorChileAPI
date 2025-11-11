using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    public class BaseController : ControllerBase
    {
        #region Readonly
        private readonly string url;
        #endregion

        #region Constant
        protected const string ContentType = "application/json";
        #endregion

        #region Interfaces
        private readonly ILogger<BaseController> Logger;
        #endregion



        #region Constructor Method
        public BaseController(ILogger<BaseController> Logger,
                              string url)
            : base()
        {
            this.url = url;
            this.Logger = Logger;
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