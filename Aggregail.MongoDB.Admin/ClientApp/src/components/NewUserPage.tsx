import React, {useState} from "react";
import {Button, Form, FormGroup, Input, Label} from "reactstrap";
import {Link} from "react-router-dom";
import {useFormPostAction} from "../hooks";
import {CreateUserData} from "../model";

const NewUserForm = (props: { onSubmit: (data: CreateUserData) => void }) => {
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

  const onSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    props.onSubmit({username, fullName, password, confirmedPassword});
  }

  return <React.Fragment>
    <div className="d-flex mt-2 mb-2">
      <h5 className="mr-auto">New User</h5>
      <div className="btn-group" role="group">
        <Link className="btn btn-outline-secondary" to="/users">Back</Link>
      </div>
    </div>
    <Form onSubmit={e => onSubmit(e)}>
      <div className="form-row">
        <FormGroup className="col-md-6">
          <Label for="username">Username</Label>
          <Input
            type="text"
            name="username"
            id="username"
            value={username}
            onChange={e => setUsername(e.target.value)}
          />
        </FormGroup>
        <FormGroup className="col-md-6">
          <Label for="full_name">Full Name</Label>
          <Input
            type="text"
            name="full_name"
            id="full_name"
            value={fullName}
            onChange={e => setFullName(e.target.value)}
          />
        </FormGroup>
      </div>
      <div className="form-row">
        <FormGroup className="col-md-6">
          <Label for="password">Password</Label>
          <Input
            type="password"
            name="password"
            id="password"
            value={password}
            onChange={e => setPassword(e.target.value)}
          />
        </FormGroup>
        <FormGroup className="col-md-6">
          <Label for="confirm_password">Confirm Password</Label>
          <Input
            type="password"
            name="confirm_password"
            id="confirm_password"
            value={confirmedPassword}
            onChange={e => setConfirmedPassword(e.target.value)}
          />
        </FormGroup>
      </div>
      <Button type="submit" color="primary" disabled={isFormInvalid}>Create</Button>
    </Form>
  </React.Fragment>;
}

const NewUserPage = (props: { onCreate: () => void }) => {
  const onSubmit = useFormPostAction<CreateUserData>('/api/users', props.onCreate);
  return <NewUserForm onSubmit={onSubmit}/>;
};

export default NewUserPage;