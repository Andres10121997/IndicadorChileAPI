using IndicadorChileAPI.App.DTO;
using IndicadorChileAPI.App.DTO.HTML;
using IndicadorChileAPI.Utils;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IndicadorChileAPI.App.Context.Tool
{
    internal static class Extract<TCurrency>
        where TCurrency : struct, IFloatingPointConstants<TCurrency>
    {
        internal static async Task<Result<Dictionary<byte, TCurrency[]>, ResultErrorDto>> ValuesAsync(HtmlDto Html)
        {
            #region Collections
            Dictionary<byte, TCurrency[]> data;
            #endregion

            #region Objects
            Result<MatchCollection, ResultErrorDto> rowsResult;
            #endregion

            rowsResult = Table.GetRows(Html: Html);

            if (!rowsResult.IsSuccess)
            {
                return Result<Dictionary<byte, TCurrency[]>, ResultErrorDto>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Extract<TCurrency>),
                        MethodName = nameof(ValuesAsync),
                        VariableName = nameof(rowsResult.IsSuccess),
                        Description = $"La variable {nameof(rowsResult.IsSuccess)} no puede ser {false}."
                    }
                );
            }

            if (rowsResult.Value == null)
            {
                return Result<Dictionary<byte, TCurrency[]>, ResultErrorDto>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Extract<TCurrency>),
                        MethodName = nameof(ValuesAsync),
                        VariableName = nameof(rowsResult.Value),
                        Description = $"La variable {nameof(rowsResult.Value)} no puede ser {null}."
                    }
                );
            }

            data = await Data<TCurrency>.GetAsync(RowMatches: rowsResult.Value);

            if (data == null)
            {
                return Result<Dictionary<byte, TCurrency[]>, ResultErrorDto>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Extract<TCurrency>),
                        MethodName = nameof(ValuesAsync),
                        VariableName = nameof(data),
                        Description = $"La variable {nameof(data)} no puede ser {null}."
                    }
                );
            }

            if (data.Count.Equals(obj: 0))
            {
                return Result<Dictionary<byte, TCurrency[]>, ResultErrorDto>.Failure(
                    Error: new ResultErrorDto()
                    {
                        ClassName = nameof(Extract<TCurrency>),
                        MethodName = nameof(ValuesAsync),
                        VariableName = nameof(data.Count),
                        Description = $"La cantidad de datos de la variable {nameof(data.Count)} no puede ser 0."
                    }
                );
            }

            return Result<Dictionary<byte, TCurrency[]>, ResultErrorDto>.Success(Value: data);
        }
    }
}