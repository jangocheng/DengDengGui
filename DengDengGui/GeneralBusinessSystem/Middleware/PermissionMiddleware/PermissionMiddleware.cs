﻿
using GeneralBusinessRepository;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GeneralBusinessSystem.Middleware
{
    /// <summary>
    /// 权限中间件
    /// </summary>
    public class PermissionMiddleware
    {
        /// <summary>
        /// 管道代理对象
        /// </summary>
        private readonly RequestDelegate _next;
        /// <summary>
        /// 权限中间件的配置选项
        /// </summary>
        private readonly PermissionMiddlewareOption _option;

        /// <summary>
        /// 权限仓储对象
        /// </summary>
        IBusinessRepository _businessRepository;

        /// <summary>
        /// 权限中间件构造
        /// </summary>
        /// <param name="next">管道代理对象</param>
        /// <param name="permissionResitory">权限仓储对象</param>
        /// <param name="option">权限中间件配置选项</param>
        public PermissionMiddleware(RequestDelegate next, IBusinessRepository businessRepository, PermissionMiddlewareOption option)
        {
            _option = option;
            _businessRepository = businessRepository;
            _next = next;
        }
        /// <summary>
        /// 调用管道
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Invoke(HttpContext context)
        {
            //过滤客户端文件和无权限页面
            if (!Path.HasExtension(context.Request.Path.Value) && context.Request.Path.Value != _option.NoPermissionAction && context.Request.Path.Value != @"/ws")
            {
                //todo 这里的验证有待优化，可以为每浏览分配一个号
                var cookie = context.Request.Cookies["browseweb"];
                if (cookie == null)
                {
                    if (context.Request.Path.Value == _option.LoginAction)
                    {
                        if (context.Request.Method == "POST")
                        {
                            var userName = context.Request.Form["username"];
                            var password = context.Request.Form["password"];
                            //验证用户名密码
                            var userDic = _businessRepository.Login(userName, password);

                            if (userDic != null && userDic.Count > 0)
                            {
                                //添加cookie
                                var guid = Guid.NewGuid().ToString().Replace("-", "");
                                context.Response.Cookies.Append("browseweb", guid, new CookieOptions() { Path = "/", HttpOnly = true });
                                context.Session.SetString(guid, userName);
                            }
                            else
                            {
                                context.Response.Redirect(_option.LoginAction);
                            }
                        }
                    }
                    else
                    {
                        context.Response.Redirect(_option.LoginAction);
                    }
                }
                else
                {
                    //验证权限
                    var permissions = _businessRepository.GetPermissionByUserID(context.Session.GetString(cookie));
                    var actions = new List<string>();
                    foreach (var dic in permissions)
                    {
                        actions.Add(dic["Action"].ToString());
                    }
                    if (!actions.Contains(context.Request.Path.Value))
                    {
                        context.Response.Redirect(_option.NoPermissionAction);
                    }
                }
            }
            return this._next(context);
        }
    }
}
