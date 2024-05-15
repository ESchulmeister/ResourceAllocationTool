using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using ResourceAllocationTool.Models;
using ResourceAllocationTool.Services;
using System;
using System.Threading.Tasks;

namespace ResourceAllocationTool
{
    public static class Extensions
    {
        #region Methods

        /// <summary>
        /// Get JSON Value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sJson"></param>
        /// <param name="sKey"></param>
        /// <param name="oDefault"></param>
        /// <param name="oValue"></param>
        /// <returns></returns>
        public static bool GetJsonValue<T>(this string sJson, string sKey, T oDefault, out T oValue)
        {
            oValue = oDefault;

            JObject oJObject = JObject.Parse(sJson);
            JToken oValueJToken = oJObject.SelectToken(sKey);
            if (oValueJToken == null)
            {
                return false;
            }

            oValue = oValueJToken.Value<T>();

            return true;
        }

        /// <summary>
        /// Get Users Identity - full name
        /// </summary>
        /// <param name="oHttpContextAccessor"></param>
        /// <returns></returns>
        public static string GetIdentity(this IHttpContextAccessor oHttpContextAccessor)
        {
            var currUser = oHttpContextAccessor.HttpContext.User;

            return (currUser.Identity == null) ? string.Empty : currUser.Identity.Name;
        }

        /// <summary>
        /// Save Hour Association Record
        /// </summary>
        /// <param name="oProjectRepository"></param>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static async Task<bool> SaveHourAssociation(this IProjectRepository oProjectRepository, string key, string values)

        {
            string[] aKeyParts = key.Split(ProjectAllocationModel.KeyDelimiter, StringSplitOptions.RemoveEmptyEntries);
            if (aKeyParts.Length != 2)
            {
                return false;
            }

            int iProjectUserID;
            int.TryParse(aKeyParts[0], out iProjectUserID);

            int iPeriodID;
            int.TryParse(aKeyParts[1], out iPeriodID);
            if (iProjectUserID == 0 || iPeriodID == 0)
            {
                return false;
            }

            double? dEstimatedHours, dActualHours;

            bool bEstimatedChanged = values.GetJsonValue<double?>("EstimatedHours", 0d, out dEstimatedHours);

            bool bActualChanged = values.GetJsonValue<double?>("ActualHours", 0d, out dActualHours);

            if (bEstimatedChanged && !dEstimatedHours.HasValue)
            {
                dEstimatedHours = 0;
            }
            if (bActualChanged && !dActualHours.HasValue)
            {
                dActualHours = 0;
            }

            await oProjectRepository.SaveHourAllocationAsync
                (
                    iProjectUserID, iPeriodID,
                    bEstimatedChanged ? dEstimatedHours.Value : null,
                    bActualChanged ? dActualHours.Value : null
                );

            return true;
        }

        #endregion
    }
}
