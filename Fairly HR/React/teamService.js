import axios from "axios";
import * as helper from "../services/serviceHelpers";

const teamsUrl = {
  endpoint: `${helper.API_HOST_PREFIX}/api/teams`,
};

const add = (team) => {
  const config = {
    method: "POST",
    url: teamsUrl.endpoint,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
    data: team
  };
  return axios(config).then(helper.onGlobalSuccess).catch(helper.onGlobalError);
};
     
const getById = (id) => {
  const config = {
    method: "GET",
    url: teamsUrl.endpoint + `/${id}`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config).then(helper.onGlobalSuccess).catch(helper.onGlobalError);
};

const update = (id, values) => {
  const config = {
    method: "PUT",
    data: values,
    url: `${teamsUrl.endpoint}/${id}`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  }
  return axios(config).then(helper.onGlobalSuccess).catch(helper.onGlobalError);
}

const deleteById = (payload) => {
    const config = {
      method: "DELETE",
      url: teamsUrl.endpoint + `/${payload.id}`,
      withCredentials: true,
      crossdomain: true,
      headers: { "Content-Type": "application/json" },
    };
    return axios(config).then(helper.onGlobalSuccess).catch(helper.onGlobalError);
  };

  const getByOrgId = (pageIndex, pageSize, orgId) => {
    const config = {
      method: "GET",
      url:
      teamsUrl.endpoint +
      `/paginate/?pageIndex=${pageIndex}&pageSize=${pageSize}&orgId=${orgId}`,
      withCredentials: true,
      crossdomain: true,
      headers: { "Content-Type": "application/json" },
    };
    return axios(config).then(helper.onGlobalSuccess).catch(helper.onGlobalError);
  };

  const getTeamNamesByOrganizationId = (orgId) => {
    const config = {
      method: "GET",
      url:
      teamsUrl.endpoint + `/org/${orgId}`,
      withCredentials: true,
      crossdomain: true,
      headers: { "Content-Type": "application/json" },
    };
    return axios(config)
  };

  const addTeamMembers = (payload) => {
    const config = {
      method: "POST",
      url: teamsUrl.endpoint + '/members/new',
      withCredentials: true,
      crossdomain: true,
      headers: { "Content-Type": "application/json" },
      data: payload
    };
    return axios(config).then(helper.onGlobalSuccess).catch(helper.onGlobalError);
  };

  const delTeamMembers = (userId, teamId) => {
    const config = {
      method: "DELETE",
      url: teamsUrl.endpoint + "/members/del" + `/${userId}` + `/${teamId}`,
      withCredentials: true,
      crossdomain: true,
      headers: { "Content-Type": "application/json" },
    };
    return axios(config).then(helper.onGlobalSuccess).catch(helper.onGlobalError);
  };

const teamService = {
    add,
    getById,
    update,
    deleteById,
    getByOrgId,
    getTeamNamesByOrganizationId,
    addTeamMembers,
    delTeamMembers
}

export default teamService;