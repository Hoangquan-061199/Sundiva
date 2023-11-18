using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Website.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using ADCOnline.Simple.Json;
using ADCOnline.DA.Dapper;

namespace Website.Infrastructure
{
    public class BlockIP {

        private readonly RequestDelegate _request;

        public BlockIP(RequestDelegate request) {
            _request = request;
        }

        public async Task Invoke(HttpContext context) {
            //DapperDA dapper = new(WebConfig.ConnectionString, host: context.Request.Host.ToString());
            //var connect = dapper.CheckOpenConnection();
            //if (connect != true)
            //{
            //    context.Response.StatusCode = StatusCodes.Status403Forbidden;
            //    return;
            //}
            //else
            //{
            //    byte[] clientIP = context.Connection.RemoteIpAddress.GetAddressBytes();
            //    List<string> _bannedlist = JsonConvert.DeserializeObject<List<string>>(Common.ReadFile("BlockIP.Json", "DataJson"));
            //    if (_bannedlist != null)
            //    {
            //        foreach (string ip in _bannedlist)
            //        {
            //            byte[] bannedIP = IPAddress.Parse(ip).GetAddressBytes();
            //            if (bannedIP.SequenceEqual(clientIP))
            //            {
            //                context.Response.StatusCode = StatusCodes.Status403Forbidden;
            //                return;
            //            }
            //        }
            //    }
            //}
            byte[] clientIP = context.Connection.RemoteIpAddress.GetAddressBytes();
            List<string> _bannedlist = JsonConvert.DeserializeObject<List<string>>(Common.ReadFile("BlockIP.Json", "DataJson"));
            if (_bannedlist != null)
            {
                foreach (string ip in _bannedlist)
                {
                    byte[] bannedIP = IPAddress.Parse(ip).GetAddressBytes();
                    if (bannedIP.SequenceEqual(clientIP))
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return;
                    }
                }
            }
            await _request.Invoke(context);
        }
    }
}