using IndicadorChileAPI.Context;

namespace IndicadorChileAPI.Areas.SII.Context.ForeignExchange
{
    public class UFContext : ContextBase
    {
        #region ConstructorMethod
        public UFContext(ushort Year,
                         byte? Month)
            : base(Url: $"https://www.sii.cl/valores_y_fechas/uf/uf{Year}.htm",
                   Year: Year,
                   Month: Month)
        {
            
        }
        #endregion



        #region DeconstructorMethod
        ~UFContext()
        {

        }
        #endregion
    }
}