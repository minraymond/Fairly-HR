import React, { useState } from "react";
import PropTypes from "prop-types";
import debug from "sabio-debug";
import { Table } from "react-bootstrap";
import teamService from "../../services/teamService";

function TeamMembersSideView(props) {
  const _logger = debug.extend("TEAMS:SideView");
  _logger(props, "props from teamsideview");
  const team = props.team;
  const teamMembersArray = team.teamMember;
  _logger(teamMembersArray, " teammembersarray");
  _logger(team, "memberssideview team");
  const userId = props.currentUser.id;
  _logger(userId, "userId");

  const mapTeamMembersArray = (teamMembersArray) => {
    return teamMembersArray.map((member, index) => (
      <tr key={index}>
        <td>
          <img
            src={member.avatarUrl}
            alt={`${member.firstName} ${member.lastName}`}
            className="rounded-circle userPFP"
          />
        </td>
        <td>{`${member.firstName} ${member.lastName}`}</td>
        <td>{member.email}</td>
      </tr>
    ));
  };

  const formatDate = (dateString) => {
    const date = new Date(dateString);
    const month = date.toLocaleString("en-US", { month: "long" });
    const day = date.getDate();
    const year = date.getFullYear();
    return `${month} ${day}, ${year}`;
  };

  function addTeamMember(userId, teamId) {
    const payload = { userId, teamId };
    teamService
      .addTeamMembers(payload)
      .then(addTeamMemberSuccess)
      .catch(addTeamMemberError);
  }

  function delTeamMember(userId, team) {
    teamService
      .delTeamMembers(userId, team)
      .then(delTeamMemberSuccess)
      .catch(delTeamMemberError);
  }

  const addTeamMemberSuccess = (response) => {
    _logger(response, "addTeamMemberSuccess Success");
    setIsJoined(true);
  };
  const addTeamMemberError = (error) => {
    _logger(error, "addTeamMemberError Error");
  };

  const delTeamMemberSuccess = (response) => {
    _logger(response, "delTeamMemberSuccess success");
    setIsJoined(false);
  };
  const delTeamMemberError = (error) => {
    _logger(error, "delTeamMemberError Error");
  };

  const checkEmptyTeamMembers = () => {
    if (teamMembersArray && teamMembersArray.length > 0) {
      return (
        <Table size="sm" className="my-2">
          <thead>
            <tr>
              <th>Avatar</th>
              <th>Name</th>
              <th>Email</th>
            </tr>
          </thead>
          <tbody>{mapTeamMembersArray(teamMembersArray)}</tbody>
        </Table>
      );
    } else {
      return <div>This team has no team members.</div>;
    }
  };

  const onClickJoinTeam = () => {
    _logger(userId, "userIdlclickteam");
    addTeamMember(userId, team.id);
  };

  const onClickLeaveTeam = () => {
    _logger(userId, team.id, "clicklteavteam");
    delTeamMember(userId, team.id);
  };

  const [isJoined, setIsJoined] = useState(
    teamMembersArray.some((member) => member.email === props.currentUser.email)
  );

  return (
    <div name="large-view" className="col">
      <div className="d-flex">
        <div className="d-sm-block flex-shrink-0">
          <img
            src={team.organization.logo}
            className="team-large-view-logo-size-md"
            alt="company-logo"
          />
        </div>
        <div className="flex-grow-1 ms-3">
          <h1 className="card-title ml-3 m-auto fs-1">{team.teamName}</h1>
          <div name="card-company-name" className="fs-3">
            {team.organization.name}
          </div>
          <div className="d-flex">
            {props.currentUser.email && (
              <>
                {isJoined ? (
                  <button
                    onClick={onClickLeaveTeam}
                    className="btn btn-danger btn-sm rounded ml-3 mb-2"
                  >
                    Leave Team
                  </button>
                ) : (
                  <button
                    onClick={onClickJoinTeam}
                    className="btn btn-success btn-sm rounded mr-3 mb-2"
                  >
                    Join Team
                  </button>
                )}
              </>
            )}
          </div>
        </div>
      </div>
      <hr />
      <div name="description-box">
        <h3>Description</h3>
        <p>{team.description}</p>
      </div>
      <hr />
      <div name="teamMembers-box">
        <h3>Team Members:</h3>
      </div>

      {checkEmptyTeamMembers()}
      <hr />

      <div className="mt-3 mb-0">
        <small>
          <p>Last Modified: {formatDate(team.dateModified)}</p>
        </small>
      </div>
    </div>
  );
}

TeamMembersSideView.propTypes = {
  currentUser: PropTypes.shape({
    email: PropTypes.string.isRequired,
    id: PropTypes.number.isRequired,
    isLoggedIn: PropTypes.bool.isRequired,
    organizationId: PropTypes.number.isRequired,
    roles: PropTypes.arrayOf(PropTypes.string.isRequired).isRequired,
  }),
  team: PropTypes.shape({
    id: PropTypes.number.isRequired,
    organization: PropTypes.shape({
      id: PropTypes.number.isRequired,
      logo: PropTypes.string.isRequired,
      name: PropTypes.string.isRequired,
      description: PropTypes.string.isRequired,
      headline: PropTypes.string.isRequired,
      phone: PropTypes.string.isRequired,
      siteUrl: PropTypes.string.isRequired,
    }),
    teamName: PropTypes.string.isRequired,
    description: PropTypes.string.isRequired,
    dateCreated: PropTypes.string.isRequired,
    dateModified: PropTypes.string.isRequired,
    teamMember: PropTypes.arrayOf(
      PropTypes.shape({
        firstName: PropTypes.string.isRequired,
        map: PropTypes.func,
      })
    ).isRequired,
  }),
};

export default TeamMembersSideView;
