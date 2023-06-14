import React, { useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { Formik, Field, Form as FormikForm, ErrorMessage } from "formik";
import teamsFormSchema from "../../schemas/teamsFormSchema";
import { Button, Card, Col, Row, Container, Form } from "react-bootstrap";
import teamService from "../../services/teamService";
import organizationServices from "../../services/organizationService";
import Swal from "sweetalert2";
import { useEffect } from "react";
import debug from "sabio-debug";
import PropTypes from "prop-types";

function TeamsForm(props) {
  const _logger = debug.extend("TEAMS:Form");
  _logger(props, "props");

  const orgId = props.currentUser.organizationId;
  _logger(orgId, "users orgId");

  const userId = props.currentUser.id;
  _logger(userId, "users userId");

  const navigate = useNavigate();
  const { state } = useLocation();

  const [pageData, setPageData] = useState({
    initialValues: {
      organizationId: orgId,
      name: "",
      description: "",
    },
    isUpdate: false,
    orgName: "",
  });

  useEffect(() => {
    if (state?.type === "TEAMS_UPDATE" && state.payload.data) {
      setPageData((prevState) => {
        let update = { ...prevState };
        update.initialValues.organizationId = orgId;
        update.initialValues.name = state.payload.data.name;
        update.initialValues.description = state.payload.data.description;
        update.isUpdate = true;
        return update;
      });
    }
  }, []);

  useEffect(() => {
    organizationServices
      .getUsersOrgMembership(userId)
      .then(getUserOrgSuccess)
      .catch(getUserOrgError);
  }, [userId]);

  function getUserOrgSuccess(response) {
    _logger(response, "getUserOrg response");
    const organizationName = response.item.name;
    _logger(organizationName, " organizationName");
    setPageData((prevState) => ({
      ...prevState,
      orgName: organizationName,
    }));
  }
  function getUserOrgError(error) {
    _logger(error, "getUserOrgError");
  }

  const onSubmit = (values) => {
    if (!pageData.isUpdate) {
      _logger("team add running!");
      teamService.add(values).then(onCreateSuccess).catch(onCreateError);
    } else {
      teamService
        .update(state.payload.id, values)
        .then(onUpdateSuccess)
        .catch(onUpdateError);
    }
  };

  const onCreateSuccess = () => {
    Swal.fire({
      icon: "success",
      title: "You have successfully created a Team!",
    }).then(() => navigate("/teams"));
  };
  const onCreateError = () => {
    Swal.fire({
      icon: "error",
      title: "There was an error, Please check input fields and Try again",
    });
  };

  const onUpdateSuccess = () => {
    Swal.fire({
      icon: "success",
      title: "You have successfully updated the team!",
    }).then(() => navigate("/teams"));
  };
  const onUpdateError = () => {
    Swal.fire({
      icon: "error",
      title: "There was an error, Please check input fields and Try again",
    });
  };

  return (
    <Container className="mt-3">
      <Row className="align-items-center justify-content-center">
        <Col lg={8} md={8} className="py-8 py-xl-0">
          <Card>
            <Card.Header>
              <Card.Title>
                <h1 className="text-primary">
                  {pageData.isUpdate
                    ? `Update Team: ${state.payload.data.name}`
                    : `Create Team For : ${pageData.orgName}`}
                </h1>
              </Card.Title>
            </Card.Header>
            <Card.Body>
              <Formik
                validationSchema={teamsFormSchema}
                onSubmit={onSubmit}
                initialValues={pageData.initialValues}
                enableReinitialize={true}
              >
                <FormikForm>
                  <Row className="mb-4">
                    <Form.Group
                      as={Col}
                      md={12}
                      controlId="name"
                      className="mb-3"
                    >
                      <Form.Label>Team Name:</Form.Label>
                      <Field
                        type="text"
                        name="name"
                        className="form-control"
                        placeholder="Ex. Social Media Marketing Team"
                      />
                      <ErrorMessage
                        name="name"
                        component="div"
                        className="text-danger"
                      />
                    </Form.Group>
                    <Form.Group
                      as={Col}
                      md={12}
                      controlId="description"
                      className="mx-auto"
                    >
                      <Form.Label>Team Description:</Form.Label>
                      <Field
                        as="textarea"
                        type="text"
                        name="description"
                        className="form-control"
                        placeholder="Ex. Social Media Marketing Team is responsible for outreach via social platforms with aims to..."
                      />
                      <ErrorMessage
                        name="description"
                        component="div"
                        className="text-danger"
                      />
                    </Form.Group>
                  </Row>
                  <Button type="submit" className="mx-auto">
                    {pageData.isUpdate ? "Update" : "Create"}
                  </Button>
                  <Link className="p-2" to="/teams">
                    {"View All Teams"}
                  </Link>
                </FormikForm>
              </Formik>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Container>
  );
}

TeamsForm.propTypes = {
  currentUser: PropTypes.shape({
    email: PropTypes.string.isRequired,
    id: PropTypes.number.isRequired,
    isLoggedIn: PropTypes.bool.isRequired,
    organizationId: PropTypes.number.isRequired,
    roles: PropTypes.arrayOf(PropTypes.string).isRequired,
  }),
};

export default TeamsForm;
