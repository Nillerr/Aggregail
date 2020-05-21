import React, {useState} from "react";
import {Button, Form, FormGroup, Input, Label} from "reactstrap";
import {Link} from "react-router-dom";

const NewUserPage = () => {

  const [username, setUsername] = useState('');
  const [fullName, setFullName] = useState('');
  const [password, setPassword] = useState('');
  const [confirmedPassword, setConfirmedPassword] = useState('');

  const isFormInvalid =
    username.trim() === '' ||
    fullName.trim() === '' ||
    password.trim() === '' ||
    confirmedPassword.trim() === '' ||
    password.trim() !== confirmedPassword.trim();

  return (
    <React.Fragment>
      <div className="d-flex mt-2 mb-2">
        <h5 className="mr-auto">New User</h5>
        <div className="btn-group" role="group">
          <Link className="btn btn-outline-secondary" to="/users">Back</Link>
        </div>
      </div>
      <Form>
        <div className="form-row">
          <FormGroup className="col-md-6">
            <Label for="username">Username</Label>
            <Input type="email" name="username" id="username" value={username}
                   onChange={(e) => setUsername(e.target.value)}/>
          </FormGroup>
          <FormGroup className="col-md-6">
            <Label for="full_name">Full Name</Label>
            <Input type="email" name="full_name" id="full_name" value={fullName}
                   onChange={(e) => setFullName(e.target.value)}/>
          </FormGroup>
        </div>
        <div className="form-row">
          <FormGroup className="col-md-6">
            <Label for="password">Password</Label>
            <Input type="email" name="password" id="password" value={password}
                   onChange={(e) => setPassword(e.target.value)}/>
          </FormGroup>
          <FormGroup className="col-md-6">
            <Label for="confirm_password">Confirm Password</Label>
            <Input type="email" name="confirm_password" id="confirm_password" value={confirmedPassword}
                   onChange={(e) => setConfirmedPassword(e.target.value)}/>
          </FormGroup>
        </div>
        <Button type="submit" color="primary" disabled={isFormInvalid}>Create</Button>
      </Form>
    </React.Fragment>
  );
};

export default NewUserPage;