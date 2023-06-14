using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Blogs;
using Sabio.Models.Domain.Organizations;
using Sabio.Models.Domain.Skills;
using Sabio.Models.Domain.StripeProducts;
using Sabio.Models.Domain.Teams;
using Sabio.Models.Domain.Users;
using Sabio.Models.Requests.Skills;
using Sabio.Models.Requests.Teams;
using Sabio.Services;
using Sabio.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Sabio.Services
{
    public class TeamService : ITeamService
    {
        IDataProvider _data = null;
        IBaseUserMapper _userMapper = null;
        ILookUpService _lookUpService = null;
        public TeamService(IDataProvider data, IBaseUserMapper userMapper, ILookUpService lookUp)
        {
            _data = data;
            _userMapper = userMapper;
            _lookUpService = lookUp;
        }

        public void DelTeamMembers(int userId, int teamId, int createdBy)
        {
            string procName = "[dbo].[Teams_DelTeamMembers]";
             _data.ExecuteNonQuery(procName,
            inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@UserId", userId);
                col.AddWithValue("TeamId", teamId);
                col.AddWithValue("@CreatedBy", createdBy);
            },
         returnParameters: null);
        }

        public void AddTeamMembers(TeamMembersAddRequest model, int userId)
        {
            string procName = "[dbo].[Teams_InsertTeamMembers]";

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                TeamMembersParams(model, col);
                col.AddWithValue("@CreatedBy", userId);
            },
            returnParameters: null);
        }

        public int Add(TeamAddRequest model, int userId)
        {
            int id = 0;
            string procName = "[dbo].[Teams_Insert]";

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                AddCommonParams(model, col);
                col.AddWithValue("@CreatedBy", userId);
                SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;
                col.Add(idOut);
            },
            returnParameters: delegate (SqlParameterCollection returnCollection)
            {
                object oldId = returnCollection["@Id"].Value;
                int.TryParse(oldId.ToString(), out id);

            });
            return id;

        }

        public Team Get(int id)
        {
            string procName = "[dbo].[Teams_SelectById]";
            Team aTeam = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Id", id);
            }, delegate (IDataReader reader, short set)
            {
                int index = 0;
                aTeam = MapSingleTeam(reader, ref index);
            }
            );
            return aTeam;
        }

        public void Update(TeamUpdateRequest model)
        {
            string procName = "[dbo].[Teams_Update]";
            _data.ExecuteNonQuery(procName,
            inputParamMapper: delegate (SqlParameterCollection col)
            {
                AddCommonParams(model, col);
                col.AddWithValue("@Id", model.Id);
                

            },
         returnParameters: null);
        }
        
        public void Delete(int id)
        {
            string procName = "[dbo].[Teams_Delete]";
            _data.ExecuteNonQuery(procName,
            inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Id", id);
            },
         returnParameters: null);
        }

        public Paged<Team> GetOrgId(int orgId, int pageIndex, int pageSize)
        {
            Paged<Team> pagedList = null;
            List<Team> list = null;
            int totalCount = 0;

            _data.ExecuteCmd("dbo.Teams_SelectByOrgIdV2",
                (param) =>
                {
                    param.AddWithValue("@OrganizationId", orgId);
                    param.AddWithValue("@PageIndex", pageIndex);
                    param.AddWithValue("@PageSize", pageSize);
                },
                (reader, recordSetIndex) =>
                {
                    int idx = 0;
                    Team aTeam = MapSingleTeam(reader, ref idx);
                    if (list == null)
                    {
                        list = new List<Team>();
                    }
                    list.Add(aTeam);
                    totalCount = reader.GetSafeInt32(idx++);
                }
                );
            if (list != null)
            {
                pagedList = new Paged<Team>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }

        public List<LookUp> GetTeamNamesByOrganizationId(int orgId)
        {
            List<LookUp> list = null;
            string procName = "[dbo].[Teams_SelectAll]";

           _data.ExecuteCmd(procName, delegate (SqlParameterCollection col)
           {
               col.AddWithValue("@OrganizationId", orgId);
           },
             singleRecordMapper: delegate (IDataReader reader, short set)
             {
                 int startingindex = 0;
                 LookUp aTeam = _lookUpService.MapSingleLookUp(reader, ref startingindex);

                 if (list == null)
                 {
                     list = new List<LookUp>();
                 }
                 list.Add(aTeam);
             });
            return list;
        } 

        private static void TeamMembersParams(TeamMembersAddRequest model, SqlParameterCollection col)
        {
            col.AddWithValue("@UserId", model.UserId);
            col.AddWithValue("@TeamId", model.TeamId);
        }

        private static void AddCommonParams(TeamAddRequest model, SqlParameterCollection col)
        {
            col.AddWithValue("@OrganizationId", model.OrganizationId);
            col.AddWithValue("@Name", model.Name);
            col.AddWithValue("@Description", model.Description);
        }

        private static Team MapSingleTeam(IDataReader reader, ref int index)
        {
            Team team = new Team();
            BaseOrganization organization = new BaseOrganization();

            team.Id = reader.GetSafeInt32(index++); 
            organization.Id = reader.GetSafeInt32(index++);
            organization.Name = reader.GetSafeString(index++);
            organization.Headline = reader.GetSafeString(index++);
            organization.Description = reader.GetSafeString(index++);
            organization.Logo = reader.GetSafeString(index++);
            organization.Phone = reader.GetSafeString(index++);
            organization.SiteUrl = reader.GetSafeString(index++);
            team.Organization = organization;
            team.TeamName = reader.GetSafeString(index++);
            team.Description = reader.GetSafeString(index++);
            team.DateCreated = reader.GetSafeDateTime(index++);
            team.DateModified = reader.GetSafeDateTime(index++);
            team.TeamMember = reader.DeserializeObject<List<TeamMembers>>(index++);
            return team;
        }
        }
    }

    
