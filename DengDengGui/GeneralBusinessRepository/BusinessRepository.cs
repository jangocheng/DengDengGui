﻿using GeneralBusinessData;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GeneralBusinessRepository.SqlServer
{
    /// <summary>
    /// 通用业务平台仓储处理类（For MS SqlServer)
    /// </summary>
    public class BusinessRepository : IBusinessRepository
    {
        /// <summary>
        /// 数据库操作类
        /// </summary>
        ISqlHelper _sqlHelper;
        /// <summary>
        /// 实例化sqlserver的仓储对象
        /// </summary>
        /// <param name="sqlHelper"></param>
        public BusinessRepository(ISqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        #region 用户管理
        /// <summary>
        /// 查询全部用户
        /// </summary>
        /// <returns></returns>
        public List<Dictionary<string, dynamic>> GetUsers()
        {
            var sql = "select * from users";
            return _sqlHelper.QueryList(sql);
        }

        /// <summary>
        /// 按用户名或名称查询用户
        /// </summary>
        /// <returns></returns>
        public List<Dictionary<string, dynamic>> GetUsers(string queryName)
        {
            var sql = "select * from users where username like @queryname or name like @queryname";
            var nameParameter = new SqlParameter() { Value ="%"+ queryName+"%", ParameterName = "@queryname" };
            return _sqlHelper.QueryList(sql);
        }
        
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public int AddUser(string userName, string password,string name)
        {
            var sql = $@"insert into users(username,password) values(@username,@password)";
            var userNameParameter = new SqlParameter() { Value = userName, ParameterName = "@username" };
            var passwordParameter = new SqlParameter() { Value = password, ParameterName = "@password" };
            var nameParameter = new SqlParameter() { Value = name, ParameterName = "@name" };
            return _sqlHelper.ChangeData(sql, userNameParameter, passwordParameter,nameParameter);
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="id">编号</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public int ModifyUser(int id, string userName, string password,string name)
        {
            var sql = $@"update  users set username=@username,password) =@password,[name]=@name where id=@id";
            var userNameParameter = new SqlParameter() { Value = userName, ParameterName = "@username" };
            var passwordParameter = new SqlParameter() { Value = password, ParameterName = "@password" };
            var idParameter = new SqlParameter() { Value = id, ParameterName = "@id" };
            var nameParameter = new SqlParameter() { Value = name, ParameterName = "@name" };
            return _sqlHelper.ChangeData(sql, userNameParameter, passwordParameter, idParameter, nameParameter);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public int RemoveUser(int id)
        {
            var sql = $@"delete  users s where id=@id";
            var idParameter = new SqlParameter() { Value = id, ParameterName = "@id" };
            return _sqlHelper.ChangeData(sql, idParameter);
        }
        /// <summary>
        /// 登录，并返回该用户合部信息
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public Dictionary<string, dynamic> Login(string userName, string password)
        {
            var sql = "select * from users where username=@username and password=@password";
            var userNameParameter = new SqlParameter() { Value = userName, ParameterName = "@username" };
            var passwordParameter = new SqlParameter() { Value = password, ParameterName = "@password" };
            var result = _sqlHelper.QueryList(sql, userNameParameter, passwordParameter);
            if (result != null && result.Count > 0)
            {
                return result[0];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="newPassword">新密码</param>
        /// <returns></returns>
        public bool ModifyPassword(string userName, string newPassword)
        {
            var sql = "update  users set password=@newpassword where username=@username ";
            var userNameParameter = new SqlParameter() { Value = userName, ParameterName = "@username" };
            var newPasswordParameter = new SqlParameter() { Value = newPassword, ParameterName = "@newpassword" };
            var result = _sqlHelper.ChangeData(sql, userNameParameter, newPasswordParameter);
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 查用户名查询权限中的action
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns></returns>
        public List<Dictionary<string, dynamic>> GetPermissionByUserID(string username)
        {
            var sql = $@"SELECT  [Permissions].[Action]
FROM    dbo.Users
        JOIN dbo.UserRoles ON Users.ID = UserRoles.UserID
        JOIN dbo.Roles ON Roles.ID = UserRoles.RoleID
        JOIN RolePermissions ON Roles.ID = RolePermissions.RoleID
        JOIN [Permissions] ON [Permissions].ID = RolePermissions.PermissionID
WHERE   Users.UserName = @username";
            var userNameParameter = new SqlParameter() { Value = username, ParameterName = "@username" };
            return _sqlHelper.QueryList(sql, userNameParameter);
        }
        /// <summary>
        /// 获取全部用户和全部权限
        /// </summary>
        /// <returns></returns>
        public List<Dictionary<string,dynamic>> GetUserPermissions()
        {
            var sql = @"SELECT  [Permissions].[Action],[Users].[UserName],[Users].[ID] AS UserID
FROM    dbo.Users
        JOIN dbo.UserRoles ON Users.ID = UserRoles.UserID
        JOIN dbo.Roles ON Roles.ID = UserRoles.RoleID
        JOIN RolePermissions ON Roles.ID = RolePermissions.RoleID
        JOIN [Permissions] ON [Permissions].ID = RolePermissions.PermissionID";
            return _sqlHelper.QueryList(sql);
        }

        #endregion

        #region 角色管理
        /// <summary>
        /// 查询全部角色
        /// </summary>
        /// <returns></returns>
        public List<Dictionary<string, dynamic>> GetRoles()
        {
            var sql = "select id,name from roles";
            return _sqlHelper.QueryList(sql);
        }
        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="roleName">角色名</param>
        /// <returns></returns>
        public int AddRole(string roleName)
        {
            var sql = $@"insert into roles(name) values(@name)";
            var roleNameParameter = new SqlParameter() { Value = roleName, ParameterName = "@name" };
            return _sqlHelper.ChangeData(sql, roleNameParameter);
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="id">编号</param>
        /// <param name="roleName">角色名</param>
        /// <returns></returns>
        public int ModifyRole(int id, string roleName)
        {
            var sql = $@"update  roles set name=@name,password) =@password where id=@id";
            var userNameParameter = new SqlParameter() { Value = roleName, ParameterName = "@name" };
            var idParameter = new SqlParameter() { Value = id, ParameterName = "@id" };
            return _sqlHelper.ChangeData(sql, userNameParameter, idParameter);
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public int RemoveRole(int id)
        {
            var sql = $@"delete roles where id=@id";
            var idParameter = new SqlParameter() { Value = id, ParameterName = "@id" };
            return _sqlHelper.ChangeData(sql, idParameter);
        }
        #endregion

        #region 菜单管理
        /// <summary>
        /// 按用户名查询菜单
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        public List<Dictionary<string, dynamic>> GetMenusByUserName(string userName)
        {
            var sql = $@"SELECT rolemodules.menuid ,menus.name AS menuname,rolemodules.moduleid,modules.name AS modulename
FROM    users
        JOIN userroles ON users.id = userroles.userid
        JOIN roles ON userroles.roleid = roles.id
        JOIN rolemodules ON roles.id = rolemodules.roleid
		JOIN dbo.Menus ON Menus.id=rolemodules.menuid
        JOIN ( SELECT   id ,
                        name ,
                        menuid
               FROM     billmodules
               UNION ALL
               SELECT   id ,
                        name ,
                        menuid
               FROM     QueryModules
               UNION ALL
               SELECT   id ,
                        name ,
                        menuid
               FROM     ChartModules
             ) AS modules ON modules.menuid = rolemodules.menuid
                             AND modules.id = rolemodules.moduleid
							 WHERE users.username=@username";
            var userNameParameter = new SqlParameter() { Value = userName, ParameterName = "@username" };
            return _sqlHelper.QueryList(sql, userNameParameter);

        }



        #endregion


    }
}
