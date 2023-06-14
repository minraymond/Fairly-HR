import React from "react";
import PropTypes from "prop-types";
import { Trash2, Edit } from "react-feather";
import "./teams.css";
import debug from "sabio-debug";

function TeamsRecords({
  team,
  teamEdit,
  teamDelete,
  userInfo,
  isSelected,
  setSelectedTeam,
  setIsSideViewOpen,
}) {
  const _loggerPage = debug.extend("TEAMS:Records");

  _loggerPage(team, "team)");

  _loggerPage(userInfo, "user");

  const userRole = userInfo.roles;
  _loggerPage(userRole, " userROle line 13");

  const onEditClicked = () => {
    teamEdit(team);
  };

  const onDeleteClicked = () => {
    teamDelete(team);
  };

  function checkUserRole(userRole) {
    if (userRole.includes("OrgAdmin")) {
      return (
        <div className="buttonsIcons col">
          <button onClick={onEditClicked} className="btn">
            <Edit className="edit-icon" />
          </button>
          <button onClick={onDeleteClicked} className="btn">
            <Trash2 className="edit-icon" />
          </button>
        </div>
      );
    }
  }

  const onSelectedCard = () => {
    setSelectedTeam(team);
    _loggerPage(team, "selectedcard function");
    setIsSideViewOpen(true);
    _loggerPage(setIsSideViewOpen, "setisviewopen");
  };

  return (
    <div className="card text-dark bg-light-subtle border card-shadow card-transform">
      <div
        className={`card-body ${isSelected && "selected"}`}
        onClick={onSelectedCard}
      >
        <div className="row">
          <div className="col-9 col-sm-9 col-md-9 col-lg-9 left-panel">
            <div name="logo-title-company-name">
              <div className="d-flex">
                <div className="flex-shrink-0">
                  <img
                    src={team.organization.logo}
                    className="team-card-logo-size-sm"
                    alt="company-logo"
                  />
                </div>
                <div className="ms-3" id={team.id}>
                  <h3 className="flex-grow-1 card-title ml-3 fw-bold m-auto mt-3">
                    {team.teamName}
                  </h3>
                  <h5 className="card-company-name fw-bolder m-auto mt-1">
                    {team.organization.name}
                  </h5>
                </div>
              </div>
            </div>
          </div>
          <div name="show-large-view" className="row">
            <p
              name="card-job-description"
              className="card-text fst-normal mt-3"
            >
              {team.description}
            </p>
          </div>
          <div>{checkUserRole(userRole)}</div>
        </div>
      </div>
    </div>
  );
}

TeamsRecords.propTypes = {
  setSelectedTeam: PropTypes.func.isRequired,
  setIsSideViewOpen: PropTypes.func.isRequired,
  isSelected: PropTypes.bool.isRequired,
  userInfo: PropTypes.shape({
    email: PropTypes.string.isRequired,
    id: PropTypes.number.isRequired,
    isLoggedIn: PropTypes.bool.isRequired,
    organizationId: PropTypes.number.isRequired,
    roles: PropTypes.arrayOf(PropTypes.string.isRequired).isRequired,
  }),
  teamDelete: PropTypes.func.isRequired,
  teamEdit: PropTypes.func.isRequired,
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

export default TeamsRecords;
