using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    public class BaseController : ControllerBase
    {
        #region Readonly
        private readonly string url;
        private readonly ILogger<BaseController> Logger;
        #endregion

        #region Constant
        protected const string ContentType = "application/json";
        #endregion



        #region Constructor Method
        public BaseController(string url,
                              ILogger<BaseController> Logger)
            : base()
        {
            #region Readonly
            this.url = url;
            this.Logger = Logger;
            #endregion
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