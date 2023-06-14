import * as Yup from 'yup'

const teamsFormSchema = Yup.object().shape({
  organizationId: Yup.string().required("Organization Id is required!"),
  name: Yup.string().required("Team Name is required").min(1, "Minimum of 1 character is required").max(100, "Maximum characters allowed is 100"),
  description: Yup.string().required("Description is required").min(5, "Minimum of 5 characters required").max(500, "Maximum characters allowed is 500. Please shorten description."),
 });
 

  export default teamsFormSchema