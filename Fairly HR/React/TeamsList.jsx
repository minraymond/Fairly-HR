import React, { useState, useEffect } from "react";
import organizationServices from "../../services/organizationService";
import teamService from "../../services/teamService";
import TeamsRecords from "./TeamsRecords";
import { Link, useNavigate } from "react-router-dom";
import { Row, Col, Container, Breadcrumb } from "react-bootstrap";
import toastr from "toastr";
import debug from "sabio-debug";
import "rc-pagination/assets/index.css";
import locale from "rc-pagination/lib/locale/en_US";
import Pagination from "rc-pagination";
import { Helmet } from "react-helmet-async";
import Swal from "sweetalert2";
import PropTypes from "prop-types";
import TeamMembersSideView from "../teams/TeamMembersSideView";

const _loggerPage = debug.extend("TEAMS:List");

function TeamsList(props) {
  const userId = props.currentUser.id;
  const userInfo = props.currentUser;
  _loggerPage(props, "props");
  _loggerPage(userInfo, "userInfo");

  const [pageData, setPageData] = useState({
    arrayOfTeams: [],
    arrayOfOrgs: [],
    mappedTeamComponents: [],
    resultsFound: true,
    totalCount: 0,
    mappedOrganizations: [],
    pageIndex: 0,
    pageSize: 5,
    nameQuery: "",
    selectedOrgTypeId: 0,
    isOpen: false,
    orgName: "",
    selectedTeam: null,
    isSideViewOpen: false,
  });

  useEffect(() => {
    teamService
      .getByOrgId(
        pageData.pageIndex,
        pageData.pageSize,
        props.currentUser.organizationId
      )
      .then(onGetTeamsSuccess)
      .catch(onGetTeamsError);
  }, [pageData.pageIndex, onDeleteSuccess]);

  useEffect(() => {
    organizationServices
      .getUsersOrgMembership(userId)
      .then(getUserOrgSuccess)
      .catch(getUserOrgError);
  }, [userId]);

  function getUserOrgSuccess(response) {
    _loggerPage(response, "getUserOrg response");
    const organizationName = response.item.name;
    _loggerPage(organizationName, " organizationName");
    setPageData((prevState) => ({
      ...prevState,
      orgName: organizationName,
    }));
  }
  function getUserOrgError(error) {
    _loggerPage(error, "getUserOrgError");
  }

  const onChange = (page) => {
    setPageData((prevState) => {
      let newPageData = { ...prevState };
      newPageData.pageIndex = page - 1;
      return newPageData;
    });
  };

  function onGetTeamsSuccess(response) {
    let teamArray = response.item.pagedItems;

    setPageData((prevState) => {
      let newPageData = { ...prevState };
      newPageData.resultsFound = true;
      newPageData.totalCount = response.item.totalCount;
      newPageData.arrayOfTeams = teamArray;
      newPageData.mappedTeamComponents =
        newPageData.arrayOfTeams?.map(mapTeams);
      return newPageData;
    });
  }

  function onGetTeamsError(err) {
    setPageData((prevState) => {
      let newPageData = { ...prevState };
      newPageData.resultsFound = false;
      return newPageData;
    });
    if (err.response.status === 404) {
      toastr.info("No results");
    } else {
      toastr.error(
        "You are not in a team. Please contact the site administrator if you think this is an error.",
        err.response.status
      );
    }
  }

  const navigate = useNavigate();

  function BreadCrumbElement() {
    return (
      <Breadcrumb>
        <Breadcrumb.Item onClick={() => navigate("/")}>Home</Breadcrumb.Item>
        <Breadcrumb.Item active>Teams</Breadcrumb.Item>
      </Breadcrumb>
    );
  }

  const setSelectedTeam = (team) => {
    setPageData((prevState) => ({
      ...prevState,
      selectedTeam: team,
    }));
    _loggerPage(team, "setSelectedTeam");
  };

  const handleSideViewToggle = () => {
    setPageData((prevState) => ({
      ...prevState,
      isSideViewOpen: true,
    }));
  };

  const mapTeams = function (team) {
    _loggerPage("team", team);
    return (
      <TeamsRecords
        team={team}
        key={team.id}
        teamEdit={onEditClicked}
        teamDelete={onDeleteClicked}
        userInfo={userInfo}
        isSelected={team === pageData.selectedTeam}
        setSelectedTeam={setSelectedTeam}
        setIsSideViewOpen={handleSideViewToggle}
      />
    );
  };

  const onEditClicked = (team) => {
    navigate("/teams/add", {
      state: {
        type: "TEAMS_UPDATE",
        payload: {
          id: team.id,
          data: {
            organizationId: team.organizationId,
            name: team.teamName,
            description: team.description,
          },
        },
      },
    });
  };

  const onDeleteClicked = (team) => {
    _loggerPage(team, "TEAM");
    const onDeleteTrue = (response) => {
      let payload = { id: team.id, isDeleted: true };
      if (response.isConfirmed) {
        let handle = onDeleteSuccess(team);
        teamService.deleteById(payload).then(handle).catch(onDeleteError);
      }
    };
    Swal.fire({
      icon: "question",
      title: "Are you sure want to Delete",
      text: `${team.teamName}?`,
      showCancelButton: true,
      confirmButtonText: "Delete",
    }).then(onDeleteTrue);
  };

  const onDeleteSuccess = (team) => {
    teamDeleted(team.id);
    Swal.fire("Success", "Team Deleted", "success");
    setPageData((prevState) => ({
      ...prevState,
      isSideViewOpen: true,
    }));
  };

  const onDeleteError = () => {
    Swal.fire("Something went Wrong", "Please Try Again", "error");
  };

  const teamDeleted = (id) => {
    setPageData((prevState) => {
      let update = { ...prevState };
      const index = update.arrayOfTeams.findIndex((item) => item.id === id);
      if (index !== -1) {
        update.arrayOfTeams.splice(index, 1);
        update.mappedTeamComponents = update.arrayOfTeams.map(mapTeams);
      }
      return update;
    });
  };

  return (
    <React.Fragment>
      <Helmet title="Organizations" />
      <Row>
        <BreadCrumbElement />
      </Row>
      <Col>
        <h1>Teams For {pageData.orgName}:</h1>

        <Link to="/teams/add">
          <button className="btn-success btn">Create a Team</button>
        </Link>
      </Col>
      <Row>
        <Row className="text-center mt-3 mb-3">
          <Col sm={6}>
            <Pagination
              onChange={onChange}
              current={pageData.pageIndex + 1}
              total={pageData.totalCount}
              pageSize={pageData.pageSize}
              locale={locale}
            ></Pagination>
          </Col>
        </Row>
      </Row>
      <Container fluid className="p-0">
        <Row>
          <Col sm={6} className="ml-auto">
            {pageData.resultsFound && (
              <div>{pageData.mappedTeamComponents} </div>
            )}
          </Col>
          <Col sm={6} className="mr-auto">
            {pageData.isSideViewOpen ? (
              <div className="side-view">
                <TeamMembersSideView
                  team={pageData.selectedTeam}
                  currentUser={props.currentUser}
                />
              </div>
            ) : (
              <div name="large-view" className="col text-center">
                <div>
                  <h1 className="font-weight-bold mt-6">
                    Click on a team on the left to display here.
                  </h1>
                  <p className="font-weight-bold mt-5">
                    If no teams show up and you are in an organization, please
                    contact the site administrator.
                  </p>
                </div>
              </div>
            )}
          </Col>
        </Row>
      </Container>
    </React.Fragment>
  );
}

TeamsList.propTypes = {
  currentUser: PropTypes.shape({
    email: PropTypes.string.isRequired,
    id: PropTypes.number.isRequired,
    isLoggedIn: PropTypes.bool.isRequired,
    organizationId: PropTypes.number.isRequired,
    roles: PropTypes.arrayOf(PropTypes.string).isRequired,
  }),
};
export default TeamsList;
