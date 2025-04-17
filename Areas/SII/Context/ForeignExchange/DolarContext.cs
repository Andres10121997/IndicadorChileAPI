using IndicadorChileAPI.Context;

namespace IndicadorChileAPI.Areas.SII.Context.ForeignExchange
{
    public class DolarContext : ContextBase
    {
        #region ConstructorMethod
        public DolarContext(ushort Year,
                            byte? Month)
            : base(Url: $"https://www.sii.cl/valores_y_fechas/dolar/dolar{Year}.htm",
                   Year: Year,
                   Month: Month)
        {
            
        }
        #endregion



        #region DeconstructorMethod
        ~DolarContext()
        {

        }
        #endregion
    }
}