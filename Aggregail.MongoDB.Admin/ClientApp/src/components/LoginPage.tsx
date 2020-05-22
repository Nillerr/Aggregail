import React, {useState} from "react";
import {Alert, Button, Form, Input} from "reactstrap";
import logo from '../assets/logo.svg';
import Axios from "axios";
import ThemeSelector from "./ThemeSelector";
import {useFormPostAction} from "../hooks";

const LoginPage = (props: { onSignIn: () => void }) => {

  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string>();

  const signInAction = useFormPostAction('/api/auth/login', props.onSignIn, reason => {
    if (reason.response && reason.response.status === 401) {
      setError('Username or password was incorrect');
    }
  }, [setError]);

  const signIn = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    
    if (isFormInvalid) {
      return;
    }
    
    setError(undefined);
    signInAction({username, password});
  };
  
  const isFormInvalid = username.trim() === '' || password.trim() === '';

  return (
    <div className="d-flex flex-column justify-content-center align-items-center h-100">
      <Form
        className="shadow rounded bg-info text-center"
        style={{width: '22rem', padding: '2.25rem'}}
        onSubmit={signIn}
      >
        <img className="mb-4" src={logo} alt="Placeholder"/>
        <ThemeSelector fill={true}/>
        <Input
          type="text"
          placeholder="Username"
          className="mt-4"
          bsSize="sm"
          value={username}
          onChange={e => setUsername(e.target.value)}
        />
        <Input
          type="password"
          className="mt-3"
          placeholder="Password"
          bsSize="sm"
          value={password}
          onChange={e => setPassword(e.target.value)}
        />
        {error ? <Alert className="mt-4 mb-0" color="danger">{error}</Alert> : null}
        <Button color="primary" className="border-0 mt-4 w-100" disabled={isFormInvalid}>Sign in</Button>
      </Form>
    </div>
  );
};

export default LoginPage;